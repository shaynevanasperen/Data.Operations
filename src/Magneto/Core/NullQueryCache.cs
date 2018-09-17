﻿using System;
using System.Threading.Tasks;
using Magneto.Configuration;

namespace Magneto.Core
{
	public class NullQueryCache<TCacheEntryOptions> : IQueryCache<TCacheEntryOptions>
	{
		public T Get<T>(Func<T> executeQuery, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions) => executeQuery();

		public Task<T> GetAsync<T>(Func<Task<T>> executeQueryAsync, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions) => executeQueryAsync();

		public void Set<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions) { }

		public Task SetAsync<T>(T queryResult, ICacheInfo cacheInfo, Func<TCacheEntryOptions> getCacheEntryOptions)
		{
			return Task.CompletedTask;
		}

		public void Evict(string cacheKey) { }

		public Task EvictAsync(string cacheKey)
		{
			return Task.CompletedTask;
		}
	}
}
