using CacheManager.Redis.Attributes;
using CacheManager.Redis.Enums;
using CacheManager.Redis.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Sample.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class MainController : Controller
{
    private readonly IRedisCacheManager<Book> _cacheManager;

    public MainController(IRedisCacheManager<Book> cacheManager)
        => _cacheManager = cacheManager;

    [HttpGet]
    public async Task<ActionResult<Book>> GetBookAsync(CancellationToken cancellationToken)
    {
        if (_cacheManager.TryGet("book-key", out var cachedBook) && cachedBook is not null)
            return Ok(cachedBook);

        var newBook = new Book
        {
            Id = 1,
            Name = "Redis Cache Manager"
        };
        await _cacheManager.SetAsync("book-key", newBook,new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromDays(1)
        }, cancellationToken);
        return Ok(newBook);
    }

    [HttpGet]
    [Cacheable(typeof(Book))]
    public Task<ActionResult<Book>> GetBookWithCacheableAttributeWithMethodNameAsKey(CancellationToken cancellationToken)
    {
        var newBook = new Book
        {
            Id = 1,
            Name = "Redis Cache Manager"
        };

        return Task.FromResult<ActionResult<Book>>(Ok(newBook));
    }

    /// <summary>
    /// if the "book-key" exist in cache, this method will be short-circuiting |
    /// if the response is 200 the content will be persisted in cache with the specified key
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Cacheable(typeof(Book), CacheableKeyType.FromProvidedValue, "book-key")]
    public Task<ActionResult<Book>> GetBookWithCacheableAttributeWithProvidedKey(CancellationToken cancellationToken)
    {
        var newBook = new Book
        {
            Id = 1,
            Name = "Redis Cache Manager"
        };

        return Task.FromResult<ActionResult<Book>>(Ok(newBook));
    }

    [HttpGet("[action]/{bookId}")]
    [Cacheable(typeof(Book), CacheableKeyType.FromRouteOrQuery, "bookId")]
    public Task<ActionResult<Book>> GetBookWithCacheableAttributeWithKeyFromRoute(int bookId, CancellationToken cancellationToken)
    {
        var newBook = new Book
        {
            Id = bookId,
            Name = "Sample Book"
        };

        return Task.FromResult<ActionResult<Book>>(Ok(newBook));
    }

    [Cacheable(typeof(Book), CacheableKeyType.FromModel, "book.id")]
    public Task<ActionResult<Book>> GetBookWithCacheableAttributeWithKeyFromModel(Book book, CancellationToken cancellationToken)
    {
        var newBook = new Book
        {
            Id = book.Id,
            Name = "Sample Book"
        };

        return Task.FromResult<ActionResult<Book>>(Ok(newBook));
    }
}