using ItemBase.Core;
using ItemBase.Core.Exceptions;
using ItemBase.Core.Models;
using ItemBase.Core.Settings;
using ItemBase.Core.Repositories;
using ItemBase.Core.Services.Icon;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemBase.Core.Services.Cache;
using ItemBase.Core.Services.Item;
using ItemBase.Resources;

namespace ItemBase.Tests
{
    public class ItemServiceTests
    {
        private readonly IItemRepository<Ru> _repository;

        private readonly IItemService<Ru> _itemService;

        private readonly ResourcesManager _resourcesManager; 
        public ItemServiceTests()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions()
            {
                SizeLimit = 1024,
                ExpirationScanFrequency = TimeSpan.FromMinutes(5),
            });

            var cache = new MemoryDistributedCache(cacheOptions);

            var mongoOptions = Options.Create<MongoConnection>(new MongoConnection()
            {
                DatabaseName = "TestDb",
                ConnectionString = "mongodb://localhost:27017",
            });


            var cacheService = new CacheService(cache);


            _repository = new MongoRepository<Ru>(mongoOptions, cacheService);

            _itemService = new ItemService<Ru>(_repository);


            if (_repository.Count == 0)
            {
                var resSettings = Options.Create(new ResourcesSettings());

                _resourcesManager = new ResourcesManager(resSettings);


                var items = _resourcesManager.LoadLocalizationAsync("ru").GetAwaiter().GetResult();

                _repository.AddRangeAsync(items).GetAwaiter().GetResult();
            }

            




        }

        #region GetTests
        [Fact]
        public async Task GetItemsById()
        {

            int id = 734025;


            var actual = await _itemService.GetItemAsync(id);

            Assert.Equal(id, actual.Id);
        }
        [Fact]
        public async Task GetItemByName()
        {
            string name = "Щит Черной звезды";

            var actual = await _itemService.GetItemsByNameAsync(name);

            var expected = 735001;

            Assert.Equal(expected, actual.First().Id);

        }
        [Fact]
        public async Task GetByGrade()
        {
            int grade = 4;

            var expecetedCount = 22687;

            var actual = await _itemService.GetItemByGradeAsync(grade);


            Assert.True( actual.Any() && actual.Count == expecetedCount);
        }
        [Fact]
        public async Task GetByFullQuery()
        {
            ItemSearchQuery itemSearchQuery = new ItemSearchQuery()
            {
                Grade = 4,
                Name = "Черной",
            };

            var actual = await _itemService.GetItemByQuery(itemSearchQuery);

            Assert.True(actual.Any());
        }
        [Fact]
        public async Task GetByQueryByName()
        {
            ItemSearchQuery itemSearchQuery = new ItemSearchQuery()
            {
                Name = "Черной",
            };

            var actual = await _itemService.GetItemByQuery(itemSearchQuery);

            Assert.True(actual.Any());
        }
        [Fact]
        public async Task GetByQueryByGrade()
        {
            ItemSearchQuery itemSearchQuery = new ItemSearchQuery()
            {
                Grade = 4  
            };

            var actual = await _itemService.GetItemByQuery(itemSearchQuery);

            Assert.True(actual.Any());
        }


 


        #endregion

        #region Exception tests

        [Fact]
        public async Task AddItemAsync_Should_Throw_ItemAlreadyExsist()
        {
            var item = new ItemModel
            {
                Id = 830062,
                Icon = "cdn.arsha.io/icons/830063.png",
                Grade = 4,
                Name = "[Tamer] Moonlight Faerie Gloves (7 Hari)"
            };

            await Assert.ThrowsAsync<AlreadyExsisttException>(() => _itemService.AddItemAsync(item));


        }
        [Fact]
        public async Task GetItemAsync_Should_Throw_ItemNotFoud()
        {
            int id = -500;
            await Assert.ThrowsAsync<NotFoundException>(() => _itemService.GetItemAsync(id));

        }
        [Fact]

        public async Task GetItemsByQuery_Should_Throw_NotFound()
        {
            ItemSearchQuery query = new ItemSearchQuery()
            {
                Grade = 4,
                Name = "45645"
            };

            await Assert.ThrowsAsync<NotFoundException>(() => _itemService.GetItemByQuery(query));

        }
        [Fact]
        public async Task GetItemsByEmptyQuery_Should_Throw_NotFound()
        {
            ItemSearchQuery query = new ItemSearchQuery()
            {

            };

            await Assert.ThrowsAsync<NotFoundException>(() => _itemService.GetItemByQuery(query));

        }

        [Fact]

        public async Task GetItemsByName_Should_Throw_NotFound()
        {
            string name = "Empty name";

            await Assert.ThrowsAsync<NotFoundException>(() => _itemService.GetItemsByNameAsync(name));

        }
        #endregion
    }
}
