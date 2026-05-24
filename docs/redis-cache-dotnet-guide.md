# Redis Cache in .NET: A Practical Guide for Modern Backends

**Meta description:** Redis cache in .NET: a practical guide to distributed caching with StackExchange.Redis, IDistributedCache trade-offs, cache-aside patterns, and production best practices in .NET 8.

---

## Why Caching Matters in Modern .NET Applications

Database and external API calls are often the slowest parts of a request. Repeated reads of the same data—product catalogs, user sessions, computed aggregates—multiply latency and load. Caching moves frequently accessed data closer to the application, reducing round-trips and giving users faster responses.

In .NET, caching is usually implemented as an in-process cache (`IMemoryCache`) for single-instance apps or a **distributed cache** when you run multiple instances behind a load balancer. For the latter, Redis is one of the most common choices: it’s fast, supports TTLs and eviction, and is widely available in cloud and on-prem environments. This guide focuses on **Redis cache in .NET**—how to integrate it, what to watch out for, and how to use it effectively.

---

## What Redis Is and When to Use It

Redis is an in-memory data store that can act as a cache, session store, or message broker. For caching, you typically use key-value operations with optional expiration (TTL). Benefits include:

- **Sub-millisecond reads** for hot data
- **Sliding and absolute expiration** so entries can expire after idle time or at a fixed point
- **Shared state** across app instances, so cache is consistent in multi-node deployments
- **Persistence options** if you need durability (less common for pure cache use cases)

Use Redis when you have multiple .NET instances and need a **distributed cache** that all of them share. Use in-memory cache when you have a single instance and don’t need cross-process consistency.

**Redis cache best practices** start with choosing the right tool: Redis for shared, multi-instance caching; in-memory for single-node speed without operational overhead.

---

## Common Redis Integration Approaches in .NET

### Raw StackExchange.Redis

[StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) is the standard .NET client for Redis. You get direct access to the full Redis API:

```csharp
var conn = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
var db = conn.GetDatabase();
await db.StringSetAsync("user:42", JsonSerializer.Serialize(user), TimeSpan.FromMinutes(30));
var raw = await db.StringGetAsync("user:42");
var user = JsonSerializer.Deserialize<User>(raw!);
```

You must manage serialization, key naming, TTLs, and connection lifecycle yourself. For a few keys this is fine; at scale it becomes repetitive and error-prone.

### IDistributedCache and Its Limitations

ASP.NET Core’s `IDistributedCache` abstracts the storage backend. The built-in Redis implementation (`AddStackExchangeRedisCache`) gives you a key–value store with `byte[]` only:

```csharp
await cache.SetAsync("user:42", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user)), options);
var bytes = await cache.GetAsync("user:42");
var user = JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(bytes));
```

**IDistributedCache vs Redis** in practice means: you still write serialization and deserialization at every call site. There is no strong typing, no shared serializer configuration, and no built-in convention for cache keys or default TTLs. You end up with boilerplate and inconsistent patterns across the codebase. Many teams adopt a small wrapper or helper library to centralize serialization and key naming; the next sections describe the kinds of issues that motivate that and one concrete option.

---

## Problems Developers Usually Face

When integrating **Redis caching .NET** applications, a few pain points show up repeatedly:

- **Serialization** – Deciding between JSON, MessagePack, or other formats, and applying the same options (naming, enums, null handling) everywhere.
- **TTLs** – Mixing sliding vs absolute expiration and forgetting to set defaults, so some entries never expire.
- **Cache keys** – No standard structure (e.g. `{instance}:{entity}:{id}`), leading to collisions or unmaintainable key strings.
- **Boilerplate** – Repeated try-get, serialize, set, and error-handling code in every service.
- **Testing** – Swapping Redis for an in-memory or fake implementation is possible but not trivial when logic is tied to raw `IDistributedCache` or `IDatabase`.

A thin abstraction that keeps Redis and StackExchange.Redis under the hood but gives you typed get/set, consistent serialization, and configurable defaults can address most of these. You still rely on the same battle-tested client and ASP.NET Core integration; you just write less glue code and get a consistent **distributed cache .NET** style API. The next section describes one such option.

---

## Introducing CacheManager.Redis

[CacheManager.Redis](https://www.nuget.org/packages/CacheManager.Redis) is an opinionated wrapper over `StackExchange.Redis` and the ASP.NET Core Redis distributed cache. It adds:

- **Strongly-typed API** – `IRedisCacheManager<T>` so you work with your domain types instead of `byte[]`.
- **Consistent serialization** – `System.Text.Json` with configurable options (camelCase, enums, etc.) applied globally.
- **Default expiration** – Optional default `DistributedCacheEntryOptions` so every set can have a TTL without repeating options.
- **Key prefixing** – Instance name support for multi-tenant or shared Redis instances.
- **Async-first** – Async methods for all I/O, with sync variants where needed.

It does not replace Redis or StackExchange.Redis; it sits on top of the standard stack and reduces boilerplate. Source and issues: [GitHub – arefLA/CacheManager.Redis](https://github.com/arefLA/CacheManager.Redis).

---

## Installation

Add the package from NuGet (targets .NET 6 and .NET 8):

```bash
dotnet add package CacheManager.Redis
```

Ensure Redis is running and you have a connection string (host, port, password, SSL as required). For local development, something like `localhost:6379` is enough; for Azure Cache for Redis you might use `yourcache.redis.cache.windows.net:6380,password=...,ssl=True`.

---

## Basic Usage

Register the cache manager in `Program.cs`:

```csharp
using CacheManager.Redis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisCacheManager(
    builder.Configuration.GetConnectionString("Redis")!,
    options =>
    {
        options.InstanceName = "myapp:";
        options.SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        options.DefaultCacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
    });
```

You get one `IRedisDistributedCache` (singleton) and one `IRedisCacheManager<T>` per entity type (scoped). Then inject `IRedisCacheManager<T>` and use it with your type:

```csharp
public sealed class ProductService
{
    private readonly IRedisCacheManager<Product> _cache;

    public ProductService(IRedisCacheManager<Product> cache) => _cache = cache;

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = $"product:{id}";
        var cached = await _cache.GetAsync(key, cancellationToken);
        if (cached is not null)
            return cached;

        var product = await LoadFromDbAsync(id, cancellationToken);
        if (product is not null)
            await _cache.SetAsync(key, product, cancellationToken);

        return product;
    }
}
```

`GetAsync` returns `null` on miss or invalid key. `SetAsync` uses your default cache options (e.g. 30-minute sliding expiration) unless you pass explicit `DistributedCacheEntryOptions`. No manual serialization or byte handling is required. For sync code you can use `TryGet(key, out var value)` and `Set` or `TrySet`; the same serializer and options apply. The library also exposes `TryRemove` and `TryRefresh` so you can avoid exceptions when keys are missing or invalid.

---

## Advanced Scenarios

### Cache-Aside Pattern

Cache-aside means: read from cache first; on miss, load from the source of truth (database, API), then write to cache. The snippet above is already cache-aside. A slightly more explicit version with per-call options:

```csharp
public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
{
    var key = $"order:{orderId}";
    var order = await _cache.GetAsync(key, cancellationToken);
    if (order is not null)
        return order;

    order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
    if (order is null)
        return null;

    await _cache.SetAsync(key, order, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    }, cancellationToken);

    return order;
}
```

### Sliding vs Absolute Expiration

- **Sliding** – Expiration timer resets on each read. Good for “recently used” data (e.g. session, recently viewed items). Use `SlidingExpiration` in `DistributedCacheEntryOptions`.
- **Absolute** – Entry expires at a fixed time or after a fixed duration from now. Good for data that must be refreshed periodically (e.g. daily config). Use `AbsoluteExpiration` or `AbsoluteExpirationRelativeToNow`.

You can combine both: the entry goes away when either the sliding window has passed without access or the absolute time is reached.

```csharp
await _cache.SetAsync(key, entity, new DistributedCacheEntryOptions
{
    SlidingExpiration = TimeSpan.FromMinutes(15),
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
}, cancellationToken);
```

### Async Usage

Prefer async for all cache I/O to avoid blocking threads:

```csharp
var value = await _cache.GetAsync(key, cancellationToken);
await _cache.SetAsync(key, entity, options, cancellationToken);
await _cache.RefreshAsync(key, cancellationToken);
await _cache.RemoveAsync(key, cancellationToken);
```

Use `TryGet` / `Set` / `TryRemove` / `TryRefresh` only when you’re in a sync context and cannot use async.

### Cache Invalidation

When the source data changes, remove or refresh the cached entry so the next read gets fresh data:

```csharp
public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
{
    await _productRepository.UpdateAsync(product, cancellationToken);
    await _cache.RemoveAsync($"product:{product.Id}", cancellationToken);
}
```

For “refresh TTL but keep value,” use `RefreshAsync` so sliding expiration is reset without re-fetching from the database. If you need to invalidate by pattern (e.g. all keys under `product:*`), you’d need to either maintain a set of keys in Redis or use a separate mechanism (e.g. pub/sub or a tag/metadata layer); the current API is key-based only, which keeps the abstraction simple and matches typical cache-aside usage.

---

## Testing and Local Development

- **Local Redis** – Run Redis in Docker (`docker run -d -p 6379:6379 redis`) or use a cloud/dev instance and point your connection string to it.
- **Unit tests** – `IRedisCacheManager<T>` is interface-based. Use a fake or in-memory implementation in tests so you don’t depend on a real Redis server. The library supports registering a custom implementation via `options.CustomImplementation = typeof(YourFakeCacheManager<>);`.
- **Integration tests** – Use a real Redis instance (e.g. Testcontainers) and the real `RedisCacheManager` to validate serialization and options. The project’s test suite uses this approach; see the [GitHub repo](https://github.com/arefLA/CacheManager.Redis) for examples.

Avoid hardcoding connection strings; use configuration and environment-specific settings (e.g. `appsettings.Development.json`).

---

## Performance and Production Best Practices

- **Connection** – CacheManager.Redis uses the same `IConnectionMultiplexer` as `AddStackExchangeRedisCache`; keep a single connection per app and reuse it.
- **Serialization** – The library uses `System.Text.Json` and `SerializeToUtf8Bytes` to limit allocations. Reuse one `JsonSerializerOptions` via registration instead of creating new ones per call.
- **Key design** – Use a clear, consistent key scheme (e.g. `{instance}:{entity}:{id}`) and the `InstanceName` option so multiple apps or environments don’t clash.
- **TTLs** – Always set expiration so Redis doesn’t fill up; use defaults in registration plus overrides where needed.
- **Observability** – Add a decorator (via `CustomImplementation`) to log cache hits/misses or record metrics. See the library’s [performance and observability notes](https://github.com/arefLA/CacheManager.Redis) if documented in the repo.

These practices apply to any **distributed cache .NET** setup; CacheManager.Redis helps you apply them with less boilerplate. If you need custom behaviour (e.g. logging, metrics, or encryption), you can implement `IRedisCacheManager<T>` yourself and register it via `options.CustomImplementation = typeof(YourCacheManager<>);` so the DI container uses your implementation instead of the default one.

---

## When Not to Use Redis

- **Single instance, no shared state** – `IMemoryCache` is simpler and has no network hop.
- **Very large values** – Redis is better for small to medium values; consider chunking or a different store for huge payloads.
- **Strong consistency requirements** – Cache is a performance optimization; use the database as the source of truth and invalidate on write.
- **No ops for Redis** – If you can’t run, monitor, or patch Redis, use a managed service or stick to in-memory cache.

---

## Conclusion

Using **Redis cache in .NET** reduces load on your database and improves response times when you have shared state across instances. Raw StackExchange.Redis is powerful but pushes serialization, key design, and TTL handling onto you. The built-in `IDistributedCache` with Redis standardizes the backend but still leaves you with byte arrays and repeated serialization code.

An abstraction like CacheManager.Redis keeps the familiar StackExchange.Redis and ASP.NET Core stack while giving you a typed API, consistent JSON serialization, and sensible defaults. Install from [NuGet](https://www.nuget.org/packages/CacheManager.Redis), register with your connection string and options, inject `IRedisCacheManager<T>`, and implement cache-aside with sliding or absolute expiration as needed. Combine that with clear cache keys, explicit invalidation, and observability, and you have a solid **Redis caching .NET** setup that scales with your application. For source code, samples, and issue tracking, see the [GitHub repository](https://github.com/arefLA/CacheManager.Redis).
