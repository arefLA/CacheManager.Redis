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
}