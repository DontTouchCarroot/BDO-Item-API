using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ItemBase.Core.Services.Cache
{
    public sealed class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
        };
        private readonly ConcurrentDictionary<string, bool> _keys = new ConcurrentDictionary<string, bool>();

        public CacheService(IDistributedCache cache)
        {
            ArgumentNullException.ThrowIfNull(cache, nameof(cache));

            _cache = cache;


        }



        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {

            string? cachedValue = await _cache.GetStringAsync(
                key,
                cancellationToken);

            if (cachedValue is null)
            {
                return default;
            }
            T? value = JsonSerializer.Deserialize<T>(cachedValue, jsonSerializerOptions);

            return value;
        }

        public async Task<T?> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        {

            T? cachedValue = await GetAsync<T>(key,
                cancellationToken);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            if (cachedValue is null)
            {
                return default;
            }

            await SetAsync(key,
                cachedValue,
                cancellationToken);

            return cachedValue;
        }

        public async Task<T?> GetAsync<T>(string key, Func<ValueTask<T>> factory, CancellationToken cancellationToken = default)
        {
            T? cachedValue = await GetAsync<T>(key,
                cancellationToken);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            if (cachedValue is null)
            {
                return default;
            }

            await SetAsync(key,
                cachedValue,
                cancellationToken);

            return cachedValue;
        }

        public async Task<T?> GetAsync<T>(string key, Func<ValueTask<T>> factory, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (options is null)
            {
                return await GetAsync(key,
                    factory,
                    cancellationToken);
            }

            T? cachedValue = await GetAsync<T>(key,
               cancellationToken);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            if (cachedValue is null)
            {
                return default;
            }

            await SetAsync(key,
                cachedValue,
                options,
                cancellationToken);

            return cachedValue;

        }



        public async Task<byte[]?> GetBytesAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _cache.GetAsync(key, cancellationToken);
        }

        public async Task<byte[]?> GetBytesAsync(string key, Func<ValueTask<byte[]>> factory, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        {
            if(options is null)
            {
                await GetBytesAsync(key,factory, cancellationToken);
            }

            var value =  await _cache.GetAsync(key, cancellationToken);

            if(value is not null)
            {
                return value;
            }

            value = await factory();

            await _cache.SetAsync(key, value, options, cancellationToken);

            return value;
            


        }

        public async Task<byte[]?> GetBytesAsync(string key, Func<ValueTask<byte[]>> factory, CancellationToken cancellationToken = default)
        {
            var value = await _cache.GetAsync(key, cancellationToken);

            if (value is not null)
            {
                return value;
            }
            value = await factory();

            await _cache.SetAsync(key, value,  cancellationToken);

            return value;

        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key,
                cancellationToken);

            _keys.TryRemove(key, out _);

        }




        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            string cacheValue = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, cacheValue);
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        {
            if(options is null)
            {
                await SetAsync(key, value, cancellationToken);
            }

            string cachedValue = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, cachedValue, options, cancellationToken);

        }

        public async Task SetBytesAsync(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            await _cache.SetAsync(key, value);
        }

        public async Task SetBytesAsync(string key, byte[] value, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
        {


            await _cache.SetAsync(key,value,options,cancellationToken); 
        }

        public async Task UpdateAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RefreshAsync(key, cancellationToken);
        }
    }
    public interface ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        public Task<T?> GetAsync<T>(string key, Func<ValueTask<T>> factory, CancellationToken cancellationToken = default);

        public Task<T?> GetAsync<T>(string key, Func<ValueTask<T>> factory, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);


        public Task UpdateAsync(string key, CancellationToken cancellationToken = default);


        public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);


        public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default);


        public Task<byte[]?> GetBytesAsync(string key, CancellationToken cancellationToken = default);

        public Task<byte[]> GetBytesAsync(string key, Func<ValueTask<byte[]>> factory, CancellationToken cancellationToken = default);
        public Task<byte[]> GetBytesAsync(string key, Func<ValueTask<byte[]>> factory, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);

        public Task SetBytesAsync(string key, byte[] value, CancellationToken cancellationToken = default);
        public Task SetBytesAsync(string key, byte[] value, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);






    }
}
