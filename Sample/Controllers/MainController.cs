using CacheManager.Redis.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Sample.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class MainController(IRedisCacheManager<Book> cacheManager) : Controller
{
    [HttpGet("async")]
    public async Task<ActionResult<Book>> GetBookAsync(CancellationToken cancellationToken)
    {
        if (cacheManager.TryGet("book-key", out var cachedBook) && cachedBook is not null)
            return Ok(cachedBook);

        var newBook = new Book
        {
            Id = 1,
            Name = "Redis Cache Manager"
        };
        await cacheManager.SetAsync("book-key", newBook,new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromDays(1)
        }, cancellationToken);
        return Ok(newBook);
    }

    [HttpGet("sync")]
    public ActionResult<Book> GetBookSync()
    {
        if (cacheManager.TryGet("book-sync-key", out var cachedBook) && cachedBook is not null)
        {
            return Ok(cachedBook);
        }

        var book = new Book
        {
            Id = 2,
            Name = "Synchronous Book"
        };

        cacheManager.Set("book-sync-key", book);
        return Ok(book);
    }

    [HttpDelete("{key}")]
    public IActionResult Remove(string key)
    {
        if (cacheManager.TryRemove(key))
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpPost("refresh/{key}")]
    public IActionResult Refresh(string key)
    {
        var refreshed = cacheManager.TryRefresh(key);
        return refreshed ? Ok() : NotFound();
    }
}