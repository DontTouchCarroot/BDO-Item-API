using ItemBase.Core.Models;
using ItemBase.Core.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZstdSharp.Unsafe;
using ItemBase.Core.Services.Cache;
using Microsoft.AspNetCore.Mvc;

namespace ItemBase.Core.Repositories
{
    public sealed class MongoRepository<TLanguage> : IItemRepository<TLanguage>
        where TLanguage : Language
    {
        private IMongoCollection<ItemModel> _itemCollection;


        private readonly Language _language = Language.Create(typeof(TLanguage));
        private readonly ICacheService _cache;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        };

        public long Count => _countDocuments;

        private readonly long _countDocuments;
        string IItemRepository.Language => _language.Prefix;

        public MongoRepository(IOptions<MongoConnection> settings,
            ICacheService cache)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);

            var mongoDb = mongoClient
                .GetDatabase(settings.Value.DatabaseName);


            var collectionName = $"items_{_language.Prefix}";

            _itemCollection = mongoDb
                .GetCollection<ItemModel>(collectionName);


            _countDocuments = _itemCollection.CountDocuments(new BsonDocument());

            _cache = cache;
        }


        public async Task AddAsync(ItemModel itemModel, CancellationToken cancellationToken = default)
        {
            await _itemCollection
                .InsertOneAsync(itemModel);
        }

        public async Task AddRangeAsync(IReadOnlyCollection<ItemModel> itemModels, CancellationToken cancellationToken = default)
        {
            await _itemCollection
                .InsertManyAsync(itemModels);
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_all_items";

            return await _cache.GetAsync<List<ItemModel>>(
                key,
                async () =>
                {
                    return await _itemCollection
                    .Find(_ => true)
                    .ToListAsync(cancellationToken);
                },
                _cacheEntryOptions,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetByGradeAsync(int grade, CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_grade_{grade}";

            return await _cache.GetAsync<List<ItemModel>>(
                key,
                async () =>
                {
                    return await _itemCollection
                    .Find( x => x.Grade == grade)
                    .ToListAsync(cancellationToken);
                },
                _cacheEntryOptions,
                cancellationToken);
        }

        public async Task<ItemModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_{id}";

            return await _cache.GetAsync<ItemModel>(
                key,
                async () =>
                {
                    return await _itemCollection
                    .Find(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
                },
                _cacheEntryOptions,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_{name}";
            return await _cache.GetAsync<List<ItemModel>>(
                key,
                async () =>
                {
                    return await _itemCollection
                    .Find(x => x.Name.Contains(name))
                    .ToListAsync(cancellationToken);
                },
                _cacheEntryOptions,
                cancellationToken);
        }


        public async Task<IReadOnlyCollection<ItemModel>> GetByRangeGradeAsync(int startGrade, int endGrade, CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_{startGrade}-{endGrade}";
            return await _cache.GetAsync<List<ItemModel>>(
                key,
                async () =>
                {
                    return await _itemCollection
                    .Find(x => x.Grade>= startGrade && x.Grade<= endGrade)
                    .ToListAsync(cancellationToken);
                },
                _cacheEntryOptions,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
        {
            return await _itemCollection
                .Find(x=> ids.Contains(x.Id))
                .ToListAsync(cancellationToken);
                
                
        }



        public async Task UpdateAsync(ItemModel item, CancellationToken cancellationToken = default)
        {

            await _itemCollection
                .FindOneAndReplaceAsync(x => x.Id == item.Id, item, null, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _itemCollection
                .FindOneAndDeleteAsync(x => x.Id == id, null, cancellationToken);
        }

        public async Task<bool> ContainsItemAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await GetByIdAsync(id, cancellationToken);

            return result != null;
        }

        public async Task UpdateIconPathAsync(string path, CancellationToken cancellationToken = default)
        {
            var items = await GetAllAsync(cancellationToken);

            foreach(var item in items)
            {
                var newPath = $"{path}/{item.Id}.png";
                var update = Builders<ItemModel>.Update.Set(x => x.Icon, newPath);

                await _itemCollection.UpdateOneAsync(x => x.Id == item.Id, update);

            } 

        }

        public async Task<IReadOnlyCollection<ItemModel>> GetByNameAndGrade(string name, int grade, CancellationToken cancellationToken = default)
        {
            var key = $"{_language.Prefix}_{name}_{grade}";

            return await _cache.GetAsync<List<ItemModel>>(key,
                async () =>
                {
                    return await _itemCollection
                    .Find(x => x.Grade == grade && x.Name.Contains(name))
                    .ToListAsync(cancellationToken);
                }, 
                _cacheEntryOptions
                , cancellationToken);
        }
    }

     
    
}
