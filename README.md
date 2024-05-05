# ASP.NET Core Redis Cache Manager

A project for supporting caching with redis in ASP.NET Core wehb applications.

# Send a Star! ⭐

If you find this project helpful for learning or solving issues in your solution, please consider giving it a star. Thank you!

# Contents
[1. Motivation](#1.-motivation)

[2. Introducing ASP.NET Core Redis Cache Manager](#2.-introducing-asp.net-core-redis-cache-manager)

[3. Getting Started](#3.-getting-started)

[4. Options](#4.-options)

[5. Roadmap](#5.-roadmap)

[6. Related Articles](#6.-realted-articles)

[7. Dependencies](#7.-dependencies)

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
  
This behaviour can change by [options](#4.-options)

3. sample 
```

```
# 4. Options

# 5. Roadmap

# 6. Related Articles

# 7. Dependencies
