using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Magneto.Configuration;
using Magneto.Core;

namespace Magneto
{
	/// <summary>
	/// If using an IoC container, it's highly recommended that this be registered as a scoped service
	/// so that the injected <see cref="IServiceProvider"/> is scoped appropriately.
	/// </summary>
	public class Mediary : IMediary
	{
		readonly ConcurrentDictionary<Type, object> _nullQueryCaches = new ConcurrentDictionary<Type, object>();

		public Mediary(IServiceProvider serviceProvider = null, IDecorator decorator = null)
		{
			ServiceProvider = serviceProvider;
			Decorator = decorator ?? (IDecorator)serviceProvider?.GetService(typeof(IDecorator)) ?? NullDecorator.Instance;
		}

		protected IServiceProvider ServiceProvider { get; }

		protected IDecorator Decorator { get; }

		protected virtual IQueryCache<TCacheEntryOptions> GetQueryCache<TCacheEntryOptions>() =>
			(IQueryCache<TCacheEntryOptions>)(ServiceProvider?.GetService(typeof(IQueryCache<TCacheEntryOptions>)) ?? _nullQueryCaches.GetOrAdd(typeof(TCacheEntryOptions), x => new NullQueryCache<TCacheEntryOptions>()));

		protected virtual ISyncQueryCache<TCacheEntryOptions> GetSyncQueryCache<TCacheEntryOptions>() => GetQueryCache<TCacheEntryOptions>();

		protected virtual IAsyncQueryCache<TCacheEntryOptions> GetAsyncQueryCache<TCacheEntryOptions>() => GetQueryCache<TCacheEntryOptions>();

		/// <inheritdoc cref="ISyncQueryMediary.Query{TContext,TResult}"/>
		public virtual TResult Query<TContext, TResult>(ISyncQuery<TContext, TResult> query, TContext context)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(query, context, x => query.Execute(x));
		}

		/// <inheritdoc cref="IAsyncQueryMediary.QueryAsync{TContext,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TResult>(IAsyncQuery<TContext, TResult> query, TContext context)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(query, context, query.ExecuteAsync);
		}

		/// <inheritdoc cref="ISyncQueryMediary.Query{TContext,TCacheEntryOptions,TResult}"/>
		public virtual TResult Query<TContext, TCacheEntryOptions, TResult>(ISyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return query.Execute(context, Decorator, GetSyncQueryCache<TCacheEntryOptions>(), cacheOption);
		}

		/// <inheritdoc cref="IAsyncQueryMediary.QueryAsync{TContext,TCacheEntryOptions,TResult}"/>
		public virtual Task<TResult> QueryAsync<TContext, TCacheEntryOptions, TResult>(IAsyncCachedQuery<TContext, TCacheEntryOptions, TResult> query, TContext context, CacheOption cacheOption = CacheOption.Default)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return query.ExecuteAsync(context, Decorator, GetAsyncQueryCache<TCacheEntryOptions>(), cacheOption);
		}

		/// <inheritdoc cref="ISyncCacheManager.EvictCachedResult{TCacheEntryOptions}"/>
		public virtual void EvictCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			query.EvictCachedResult(GetSyncQueryCache<TCacheEntryOptions>());
		}

		/// <inheritdoc cref="IAsyncCacheManager.EvictCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task EvictCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			return query.EvictCachedResultAsync(GetAsyncQueryCache<TCacheEntryOptions>());
		}

		/// <inheritdoc cref="ISyncCacheManager.UpdateCachedResult{TCacheEntryOptions}"/>
		public virtual void UpdateCachedResult<TCacheEntryOptions>(ISyncCachedQuery<TCacheEntryOptions> executedQuery)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			executedQuery.UpdateCachedResult(GetSyncQueryCache<TCacheEntryOptions>());
		}

		/// <inheritdoc cref="IAsyncCacheManager.UpdateCachedResultAsync{TCacheEntryOptions}"/>
		public virtual Task UpdateCachedResultAsync<TCacheEntryOptions>(IAsyncCachedQuery<TCacheEntryOptions> executedQuery)
		{
			if (executedQuery == null) throw new ArgumentNullException(nameof(executedQuery));

			return executedQuery.UpdateCachedResultAsync(GetAsyncQueryCache<TCacheEntryOptions>());
		}

		/// <inheritdoc cref="ISyncCommandMediary.Command{TContext}"/>
		public virtual void Command<TContext>(ISyncCommand<TContext> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			Decorator.Decorate(command, context, x => command.Execute(x));
		}

		/// <inheritdoc cref="IAsyncCommandMediary.CommandAsync{TContext}"/>
		public virtual Task CommandAsync<TContext>(IAsyncCommand<TContext> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, context, command.ExecuteAsync);
		}

		/// <inheritdoc cref="ISyncCommandMediary.Command{TContext,TResult}"/>
		public virtual TResult Command<TContext, TResult>(ISyncCommand<TContext, TResult> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, context, x => command.Execute(x));
		}

		/// <inheritdoc cref="IAsyncCommandMediary.CommandAsync{TContext,TResult}"/>
		public virtual Task<TResult> CommandAsync<TContext, TResult>(IAsyncCommand<TContext, TResult> command, TContext context)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return Decorator.Decorate(command, context, command.ExecuteAsync);
		}
	}
}