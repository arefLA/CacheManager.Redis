# ASP.NET Core Redis Cache Manager

A project for supporting caching with redis in ASP.NET Core web applications.

# Send a Star! ⭐

If you find this project helpful for learning or solving issues in your solution, please consider giving it a star. Thank you!

# Contents
[1. Motivation](#1-motivation)

[2. Introducing ASP.NET Core Redis Cache Manager](#2-introducing-aspnet-core-redis-cache-manager)

[3. Getting Started](#3-getting-started)

[4. Options](#4-options)

[5. Roadmap](#5-roadmap)

[6. Related Articles](#6-realted-articles)

[7. Dependencies](#7-dependencies)

# 1. Motivation
I was working on a project using the microservices pattern, which included many services. We decided to utilize Redis as our cache database. 
To implement Redis, I attempted to use [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/), which offers a straightforward configuration process. 

However, I found myself repeatedly executing the same methods for each microservice, encountering minor differences such as serialization and deserialization. 
Eventually, With a look at [DRY](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself) I developed a project capable of handling these tasks with a generic pattern, requiring minimal preparation code for public use.

I've shared it here so anyone can benefit; it could save you at least a week!

# 2. Introducing ASP.NET Core Redis Cache Manager
**ASP.NET Core Redis Cache Manager** is essentially a genric cache manager using [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) and [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json). this package provide an interface and base implementation to do the convertion from bytes to the desired type.

# 3. Getting Started
1. Add the [CacheManager.Redis](https://www.nuget.org/packages/CacheManager.Redis/) to your ASP.NET Core web project.
```
  dotnet add package CacheManager.Redis
```
*Either command or Package Manager will download and install CacheManager.Redis and all required dependencies.*

2. Register CacheManager to you ASP.NET Core web project.

for .NET 6 or higher add this code to you `Program.cs` before `Build()`
```
  builder.Services.AddRedisCacheManager("redis connection string");
```

fon .NET 5 or before add this code to your `Startup.cs` in `ConfigureServices` method
```
  services.AddRedisCacheManager("redis connection string");
```
This registers:
- `IRedisDistributedCache` as singleton
- `IRedisCacheManager` as scope
  
This behaviour can change by [options](#4-options)

3. Basic Usage

Let's assume you have an object named `Book.cs` and you want to store it in cache and retrieve it.
```
public class Book
{
  public int Id { get; set; }
  public string Name { get; set; }
}
```

First you need to inject the IRedisCacheManager interface with the book class
```
public class MainController : Controller
{
  public readonly IRedisCacheManager<Book> _cacheManager;

  public MainController(IRedisCacheManager<Book> cacheManager) => _cacheManager = cacheManger;
}
```

4. Methods

- TryGet
```
  var exist = _cacheManager.TryGet("key", out var cachedBook);
```

- TryGetAsync
```
  var cachedBook = await _cacheManager.TryGetAsync("key");
```

- Set
```
  var book = new Book { Id = 1, Name = "Redis Cache" };

  _cacheManager.Set("key", newBook);

  var customOptions = new DistributedCacheEntryOptions {
    SlidingExpiration = TimeSpan.FromDays(1)
  }
  _cacheManager.Set("key", newBook, customOptions);
```

- TrySet
```
  var book = new Book { Id = 1, Name = "Redis Cache" };

  var result = _cacheManager.TrySet("key", newBook);
```

- SetAsync
```
  var book = new Book { Id = 1, Name = "Redis Cache" };

  await _cacheManager.SetAsync("key", newBook);

  var customOptions = new DistributedCacheEntryOptions {
    SlidingExpiration = TimeSpan.FromDays(1)
  }

  await _cacheManager.SetAsync("key", newBook, customOptions);
```
* You can use other expiration types in the set method options. notice that this options only will affect the current call. if you want to have a default expiration rule see [4. Options](4-options).*

- Refresh
```
  _cacheManager.Refresh("key");
```

- TryRefresh
```
  var result = _cacheManager.TryRefresh("key");
```

- RefreshAsync
```
  await _cacheManager.RefreshAsync("key");
```

- Remove
```
  _cacheManager.Remove("key");
```

- TryRemove
```
  var result = _cacheManager.TryRefresh("key");
```

- RemoveAsync
```
  await _cacheManager.RemoveAsync("key");
```

**All the async endpoints accept cancellation token as an optional parameter**

# 4. Options

While registering ASP.NET Core Redis Cahce Manger you can use these options to more customize it
```
  AddRedisCacheManager("redis connection string", options =>
  {
      options.InstanceName = "Library:";  // add this at start of every key in the database
      options.SerializerOptions = new JsonSerializerOptions  // use this to customize the serializer used in object convert
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =  
        {
            new JsonStringEnumConverter()    // custom json converters
        }
      },
      options.DefaultCacheOptions = new DistributedCacheEntryOptions  // configure default cache expiration model. it can be rewrite by the set method at the runtime
      {
          SlidingExpiration = TimeSpan.FromDays(1)
      };
      options.CustomImplemntation = typeof(CustomRedisCacheManagerImplementation)  // any custom cache manager implementation, notice that it should implement IRedisCacheManager interface
  });
```
*all options are optional*

# 5. Roadmap

i have some interesting features in my mind. i'd like to addd them to the project/package.

**Auto Update Cache By Subscribing to queue or an event source**

**Make it more generic: using other data store services like memcache and etc.**

**Develop a middleware or filter attribute to shortcircuit the process if the cache exist**

**Develop functionality for encrypting and decrypting stored caches.**

*Feel free to suggest anything*

# 6. Related Articles

# 7. Dependencies

- **[StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)**

- **[System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json)**

- **[Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses)**
