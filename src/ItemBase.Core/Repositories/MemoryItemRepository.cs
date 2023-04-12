using ItemBase.Core.Models;
using ItemBase.Core.Services;
using ItemBase.Core.Settings;
using ItemBase.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ItemBase.Core.Repositories
{
    public class MemoryItemRepository<TLanuage> : IItemRepository<TLanuage>
        where TLanuage : Language 
    {
      
        private readonly ConcurrentDictionary<int, ItemModel> _items = new();
        private readonly Language _language;

        private DateTime _lastSave;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly ResourcesManager _resourcesManager;

        public long Count => _items.Count;

        string IItemRepository.Language => _language.Prefix;

        public MemoryItemRepository(ResourcesManager resourcesManager)
        {


            _language = Language.Create(typeof(TLanuage));
            _resourcesManager = resourcesManager;
        }

        public MemoryItemRepository()
        {
            _language = Language.Create(typeof(TLanuage));


            var items = _resourcesManager.LoadLocalizationAsync(_language.Prefix)
                .GetAwaiter()
                .GetResult()
                .Select(x => new KeyValuePair<int, ItemModel>(x.Id, x)); 

            _items = new ConcurrentDictionary<int, ItemModel>(items);



        }
        private async Task SaveAsync(CancellationToken cancellationToken = default)
        {

            if(_lastSave.AddMinutes(1) < DateTime.Now)
            {
                return;
            }


            await _semaphore.WaitAsync();
            try
            {
                var valuesToSave = _items.Values.ToList();
                await _resourcesManager.SaveLocalizationAsync(_language.Prefix, valuesToSave);
            }
            finally
            {
                _lastSave = DateTime.Now;
                _semaphore.Release();
            }
            


        }
        public async Task AddAsync(ItemModel itemModel, CancellationToken cancellationToken = default)
        {
            _items.TryAdd(itemModel.Id, itemModel);
#if !DEBUG
            await SaveAsync();
#endif
        }

        public async Task AddRangeAsync(IReadOnlyCollection<ItemModel> itemModels, CancellationToken cancellationToken = default)
        {
            int counter = 0;

            await _semaphore.WaitAsync();
            try
            {
                foreach (var itemModel in itemModels)
                {
                    if (itemModel is null)
                    {
                        continue;
                    }
                    if (_items.ContainsKey(itemModel.Id))
                    {
                        continue;
                    }

                    _items.TryAdd(itemModel.Id, itemModel);
                    counter++;
                }
            }finally
            {
                _semaphore.Release();
            }
            if (counter > 0)
            {
#if !DEBUG
                await SaveAsync();
#endif
            }
        }

        public Task<ItemModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _items.TryGetValue(id, out var item);


            return Task.FromResult(item);
        }

        public Task<IReadOnlyCollection<ItemModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ItemModel> result = _items
                .Values
                .ToList();

            return Task.FromResult(result);

        }

        public Task<IReadOnlyCollection<ItemModel>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ItemModel> result = _items.Values
                .Where(x=> x.Name == name).ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<ItemModel>> GetByGradeAsync(int grade, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ItemModel> result = _items
                .Values
                .Where(x => x.Grade == grade).ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<ItemModel>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
        {

            var containsId = ids.
                Where(x => _items.ContainsKey(x));

            var result = _items
                .Where(x => containsId.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();

            
            return Task.FromResult<IReadOnlyCollection<ItemModel>>(result);


        }

        public Task<IReadOnlyCollection<ItemModel>> GetByRangeGradeAsync(int startGrade, int endGrade, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ItemModel>? result = _items
               .Values
               .Where(x => x.Grade >= startGrade && x.Grade<=endGrade).ToList();

            return Task.FromResult(result);
        }



        public Task UpdateAsync(ItemModel item, CancellationToken cancellationToken = default)
        {
            _items.AddOrUpdate(item.Id, item, (id, item) =>
            {
                return item;
            });

#if !DEBUG
             await SaveAsync();
#endif

            return Task.CompletedTask;
        }
        public Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _items.TryRemove(id, out _);
#if !DEBUG
            await SaveAsync();
#endif
            return Task.CompletedTask;
        }

        public Task<bool> ContainsItemAsync(int id, CancellationToken cancellationToken = default)
        {

            var result = _items.ContainsKey(id);

            return Task.FromResult(result);
        }

        public async Task UpdateIconPathAsync(string path, CancellationToken cancellationToken = default)
        {
            foreach(var item in _items.Values)
            {
                var newPath = $"{path}/{item.Id}.png";

                item.Icon = newPath;
            }
#if !DEBUG
            await SaveAsync();
#endif
      
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetByNameAndGrade(string name, int grade, CancellationToken cancellationToken = default)
        {
            var result =  _items.Values
                .Where(x => x.Name.Contains(name) && x.Grade == grade)
                .ToList();

            return await Task.FromResult(result);
        }
    }

      
    
}
