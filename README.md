# ASP.NET Core Redis Cache Manager

A lightweight abstraction over `StackExchange.Redis` that offers strongly-typed caching, consistent serialization via `System.Text.Json`, and ergonomic registration helpers for ASP.NET Core apps.

> ⭐ If this project saves you time, please consider giving it a star.

---

## Table of Contents
1. [Installation](#installation)
2. [Registering the Cache Manager](#registering-the-cache-manager)
3. [Core API Usage](#core-api-usage)
4. [Configuring Options](#configuring-options)
5. [Sample Application](#sample-application)
6. [Testing the Library](#testing-the-library)
7. [Performance & Design Notes](#performance--design-notes)
8. [Roadmap](#roadmap)
9. [Dependencies](#dependencies)

---

## Installation

Install the package from NuGet:

```bash
  dotnet add package CacheManager.Redis
```

Target frameworks: `net6.0`, `net8.0`.

---

## Registering the Cache Manager

Add the manager during service registration (typically in `Program.cs`):

```csharp
using CacheManager.Redis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisCacheManager(
    builder.Configuration.GetConnectionString("Redis"),
    options =>
    {
        options.InstanceName = "sample:"; // optional key prefix
        options.SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        options.DefaultCacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
        // options.CustomImplementation = typeof(CustomCacheManager<>);
    });
```

This call will:

- Register `StackExchange.Redis` using your connection string.
- Expose `IRedisDistributedCache` as a singleton wrapper that carries serializer/options metadata.
- Register `IRedisCacheManager<T>` as scoped (or a custom implementation if provided).

> **Tip:** The connection string should embed SSL/password settings as needed, e.g. `mycache.redis.cache.windows.net:6380,password=...,ssl=True`.

---

## Core API Usage

Inject `IRedisCacheManager<T>` wherever you need typed access to Redis:

```csharp
public sealed class MainController : ControllerBase
{
    private readonly IRedisCacheManager<Book> _cacheManager;

    public MainController(IRedisCacheManager<Book> cacheManager)
        => _cacheManager = cacheManager;

    [HttpGet("async")]
    public async Task<ActionResult<Book>> GetBookAsync(CancellationToken cancellationToken)
    {
        if (_cacheManager.TryGet("book-key", out var cachedBook) && cachedBook is not null)
        {
            return Ok(cachedBook);
        }

        var book = new Book { Id = 1, Name = "Redis Cache Manager" };
        await _cacheManager.SetAsync(
            "book-key",
            book,
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) },
            cancellationToken);

        return Ok(book);
    }
}
```

Available operations:

- `TryGet` / `TryGetAsync` – read-through access returning `bool` or nullable values.
- `Set` / `SetAsync` – write-through storage with optional `DistributedCacheEntryOptions`.
- `TrySet` – best-effort write that returns `false` for invalid keys without throwing.
- `Refresh` / `RefreshAsync` – reset sliding expiration.
- `Remove` / `RemoveAsync` – evict cache entries.

Serialization is handled through `System.Text.Json`; you can override serializer settings globally via registration options.

---

## Configuring Options

`RedisCacheMangerOptions` (note the historical spelling) allows you to tailor behaviour:

| Option | Description |
| --- | --- |
| `InstanceName` | Prefix automatically added to every key stored via this manager. Helpful for multi-tenant or shared Redis deployments. |
| `SerializerOptions` | Supply a `JsonSerializerOptions` instance for consistent naming policies, converters, etc. |
| `DefaultCacheOptions` | Provide default `DistributedCacheEntryOptions` so that `Set`/`SetAsync` calls without explicit options still expire. |
| `CustomImplementation` | Replace the default `RedisCacheManager<T>` with your own implementation/decorator. Must implement `IRedisCacheManager<T>`. |

### Custom decorator example

```csharp
public sealed class LoggingCacheManager<T> : IRedisCacheManager<T> where T : class
{
    private readonly IRedisCacheManager<T> _inner;
    private readonly ILogger<LoggingCacheManager<T>> _logger;

    public LoggingCacheManager(IRedisCacheManager<T> inner, ILogger<LoggingCacheManager<T>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public bool TryGet(string key, out T? response)
    {
        var hit = _inner.TryGet(key, out response);
        _logger.LogDebug("Cache {Result} for {Key}", hit ? "hit" : "miss", key);
        return hit;
    }

    // delegate remaining members ...
}
```

Register by setting `options.CustomImplementation = typeof(LoggingCacheManager<>)`.

---

## Sample Application

The `Sample/` folder contains a minimal ASP.NET Core API that exercises the package.

Key endpoints in `Sample/Controllers/MainController.cs`:

- `GET /main/async` – asynchronous read/write with custom expiration.
- `GET /main/sync` – synchronous access using default cache options.
- `POST /main/refresh/{key}` – refresh existing entries.
- `DELETE /main/{key}` – remove entries safely via `TryRemove`.

The `Sample.http` scratch file contains ready-to-run HTTP requests for these endpoints. Update `Sample/appsettings.json` with a valid Redis connection string before running:

```bash
dotnet run --project Sample/Sample.csproj
```

---

## Testing the Library

Unit tests live under `test/CacheManager.Redis.Tests` and target both `net6.0` and `net8.0`. They cover:

- Serialization helpers (`SerializeExtensions`, `Helpers.HasValue`).
- Dependency injection setup (`SetupTests`).
- `RedisCacheManager` behaviours (sync/async, error handling, refresh/remove).

Run the suite with:

```bash
dotnet test CacheManager.Redis.sln
```

> Need integration coverage? Spin up a disposable Redis instance (Docker, Azure Cache for Redis emulator, etc.) and add tests under a separate project referencing this library.

---

## Performance & Design Notes

- **Binary serialization** – the cache manager now uses `JsonSerializer.SerializeToUtf8Bytes` / `JsonSerializer.Deserialize` directly, avoiding intermediate `string` allocations.
- **Reusable configuration** – `AddRedisCacheManager` bootstraps `AddStackExchangeRedisCache`, ensuring a single `IConnectionMultiplexer` instance via the built-in provider and registering the `IRedisDistributedCache` wrapper as a singleton.
- **Null safety** – extension helpers guard against null input and no longer throw when `Type.BaseType` is null.
- **Extensibility** – provide custom cache managers or decorators for logging, metrics, encryption, etc., using the built-in options hook.
- **Observability** – collect cache hit/miss metrics and failures by plugging in a decorator. See [`docs/performance-and-observability.md`](docs/performance-and-observability.md) for additional guidance.

---

## Roadmap

- Automatic cache invalidation hooks (e.g., Redis pub/sub or queue listeners).
- Additional backing stores behind the same abstraction (memcached, in-memory fallbacks).
- Optional middleware/filter packages for transparent caching in MVC or minimal APIs.
- Built-in instrumentation helpers (logging, metrics) with opt-in decorators.

Contributions and feature requests are welcome—open an issue to discuss ideas.

---

## Dependencies

- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
- [System.Text.Json](https://learn.microsoft.com/dotnet/api/system.text.json)
- [Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses)

---

Happy caching! Feel free to reach out or file issues if you run into problems. Contributions of documentation, tests, and new samples are especially appreciated. 🚀
