using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Interfaces
{
    public interface IRedisCacheManager<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets an entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="response"></param>
        /// <returns>A boolean which shows if the get was successful and out the located entity or null</returns>
        /// <remarks>
        /// return false if the key is null or whitespace
        /// </remarks>
        bool TryGet(string key, out TEntity? response);

        /// <summary>
        /// Gets an entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located entity or null.</returns>
        /// <remarks>
        /// Returns null if the key is null or whitespace.
        /// </remarks>
        Task<TEntity?> GetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the entity with the given key, or creates it with <paramref name="factory"/> and stores it when it is not cached.
        /// </summary>
        /// <typeparam name="TResult"><typeparamref name="TEntity"/> or <c>TEntity?</c>, inferred from <paramref name="factory"/>.</typeparam>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="factory">Creates the entity on a cache miss. It is not called when a valid cached value exists.</param>
        /// <param name="options">Optional. The cache options for a newly stored entity. When null, the default options configured at registration are used.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled. It is passed to <paramref name="factory"/>.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the cached or newly created entity.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> or <paramref name="factory" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// Throws an <see cref="InvalidOperationException" /> if <typeparamref name="TResult"/> is a subclass of
        /// <typeparamref name="TEntity"/> and the cached value is not that subclass. To cache subclasses, use a manager
        /// for the subclass, or mark <typeparamref name="TEntity"/> with <c>[JsonDerivedType]</c> for each subclass.
        /// A cached value that cannot be deserialized is treated as a miss.
        /// If <paramref name="factory"/> returns null, nothing is stored and null is returned.
        /// <typeparamref name="TResult"/> is inferred from <paramref name="factory"/>: a factory returning
        /// <c>Task&lt;TEntity&gt;</c> gives a non-nullable result, one returning <c>Task&lt;TEntity?&gt;</c> a nullable one.
        /// </remarks>
        Task<TResult> GetOrSetAsync<TResult>(string key, Func<CancellationToken, Task<TResult>> factory,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
            where TResult : TEntity?;

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Set(string key, TEntity entity);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <returns>true if the set was sent to the cache; otherwise false.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        bool TrySet(string key, TEntity entity);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Set(string key, TEntity entity, DistributedCacheEntryOptions options);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options"></param>
        /// <returns>true if the set was sent to the cache; otherwise false.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        bool TrySet(string key, TEntity entity, DistributedCacheEntryOptions options);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Refresh(string key);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <returns>true if the refresh was sent to the cache; otherwise false.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        bool TryRefresh(string key);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if the refresh was sent to the cache.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        Task<bool> TryRefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Remove(string key);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <returns>true if the remove was sent to the cache; otherwise false.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        bool TryRemove(string key);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if the remove was sent to the cache.</returns>
        /// <remarks>
        /// Returns false if the key is null or whitespace, without calling the cache.
        /// </remarks>
        Task<bool> TryRemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}