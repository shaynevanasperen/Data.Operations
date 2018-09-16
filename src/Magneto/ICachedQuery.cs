using System.Threading;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto
{
	/// <summary>
	/// A query operation that executes synchronously, potentially returning a cached result.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public interface ISyncCachedQuery<in TContext, out TCacheEntryOptions, out TResult> : ISyncCachedQuery<TCacheEntryOptions>
	{
		/// <summary>
		/// <para>Queries the <paramref name="cacheStore"/> and in the case of a cache miss, executes the query, caches and returns the result. Otherwise the cached result is returned.</para>
		/// <para>If <paramref name="cacheOption"/> is <see cref="CacheOption.Refresh"/>, the <paramref name="cacheStore"/> is not queried, but will still be written to.</para>
		/// </summary>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		/// <param name="cacheOption">Optional. An option designating whether or not the cache should be read when executing the query.</param>
		/// <returns>The result of query execution in the case of a cache miss or if <paramref name="cacheOption"/> is <see cref="CacheOption.Refresh"/>, otherwise the cached result.</returns>
		TResult Execute(TContext context, ISyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default);
	}

	/// <summary>
	/// A query operation that executes asynchronously, potentially returning a cached result.
	/// </summary>
	/// <typeparam name="TContext">The type of context with which the query is executed.</typeparam>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	/// <typeparam name="TResult">The type of the query result.</typeparam>
	public interface IAsyncCachedQuery<in TContext, out TCacheEntryOptions, TResult> : IAsyncCachedQuery<TCacheEntryOptions>
	{
		/// <summary>
		/// <para>Queries the <paramref name="cacheStore"/> and in the case of a cache miss, executes the query, caches and returns the result. Otherwise the cached result is returned.</para>
		/// <para>If <paramref name="cacheOption"/> is <see cref="CacheOption.Refresh"/>, the <paramref name="cacheStore"/> is not queried, but will still be written to.</para>
		/// </summary>
		/// <param name="context">The context with which to execute the query.</param>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		/// <param name="cacheOption">Optional. An option designating whether or not the cache should be read when executing the query.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>The result of query execution in the case of a cache miss or if <paramref name="cacheOption"/> is <see cref="CacheOption.Refresh"/>, otherwise the cached result.</returns>
		Task<TResult> ExecuteAsync(TContext context, IAsyncCacheStore<TCacheEntryOptions> cacheStore, CacheOption cacheOption = CacheOption.Default, CancellationToken cancellationToken = default);
	}

	/// <summary>
	/// A query operation that executes synchronously, potentially returning a cached result.
	/// </summary>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	public interface ISyncCachedQuery<out TCacheEntryOptions>
	{
		/// <summary>
		/// Evicts any prior cached result from previous execution of the query.
		/// </summary>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		void EvictCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore);

		/// <summary>
		/// Updates the prior cached result from previous execution of the query. Useful when the underlying cache store is not in memory.
		/// </summary>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		void UpdateCachedResult(ISyncCacheStore<TCacheEntryOptions> cacheStore);
	}

	/// <summary>
	/// A query operation that executes asynchronously, potentially returning a cached result.
	/// </summary>
	/// <typeparam name="TCacheEntryOptions">The type of cache entry options configured by the query.</typeparam>
	public interface IAsyncCachedQuery<out TCacheEntryOptions>
	{
		/// <summary>
		/// Evicts any prior cached result from previous execution of the query.
		/// </summary>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>A task representing the eviction of the cached result.</returns>
		Task EvictCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore, CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates the prior cached result from previous execution of the query. Useful when the underlying cache store is not in memory.
		/// </summary>
		/// <param name="cacheStore">An object used for storing and retrieving cached values.</param>
		/// <param name="cancellationToken">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
		/// <returns>A task representing the update of the cached result.</returns>
		Task UpdateCachedResultAsync(IAsyncCacheStore<TCacheEntryOptions> cacheStore, CancellationToken cancellationToken = default);
	}
}