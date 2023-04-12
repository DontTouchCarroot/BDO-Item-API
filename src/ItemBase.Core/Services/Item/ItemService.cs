using ItemBase.Core.Exceptions;
using ItemBase.Core.Models;
using ItemBase.Core.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading;



namespace ItemBase.Core.Services.Item
{
    public sealed class ItemService<TLanguage> : IItemService<TLanguage>
        where TLanguage : Language
    {
        private readonly IItemRepository<TLanguage> _itemsRepository;
        private readonly ILogger<ItemService<TLanguage>> _logger;
        private readonly Language _language = Language.Create(typeof(TLanguage));
        private readonly ResourcesManager _resourcesManager;
        public ItemService(IItemRepository<TLanguage> items,
            ILogger<ItemService<TLanguage>> logger,
            ResourcesManager resourcesManager)
        {
            _itemsRepository = items;
            _logger = logger;
            _resourcesManager = resourcesManager;
        }
        public ItemService(IItemRepository<TLanguage> items)
        {
            _itemsRepository = items;
        }

        public async Task AddItemAsync(ItemModel item, CancellationToken cancellationToken = default)
        {
            var result = await _itemsRepository.GetByIdAsync(item.Id);

            AlreadyExsisttException.ThrowIfNotNull(result, nameof(ItemModel));


            _logger.LogInformation($"Added [{item}] item in [{_language.Prefix}]DataBase");

            await _itemsRepository.AddAsync(item);
        }

        public async Task AddRangeItemsAsync(IReadOnlyCollection<ItemModel> items, CancellationToken cancellationToken = default)
        {
            var ids = items
                .Select(x => x.Id)
                .ToList();

            var containsItems = await _itemsRepository.GetByIdsAsync(ids,
                cancellationToken);


            if (containsItems is not null)
            {


                var newItems = items
                    .Where(x => !containsItems.Contains(x))
                    .ToList();

                if (!newItems.Any())
                {
                    throw new AlreadyExsisttException(nameof(ItemModel));
                }

                await _itemsRepository.AddRangeAsync(newItems, cancellationToken);

                _logger.LogInformation($"Added {newItems.Count} items in [{_language.Prefix}]DataBase");

                return;
            }

            await _itemsRepository.AddRangeAsync(items, cancellationToken);

            _logger.LogInformation($"Added {items.Count} items in DataBase");

        }

        public async Task<IReadOnlyCollection<ItemModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _itemsRepository.GetAllAsync(cancellationToken);
        }

        public async Task<ItemModel> GetItemAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _itemsRepository.GetByIdAsync(id, cancellationToken);

            NotFoundException.ThrowIfNull(result, nameof(ItemModel));

            return result;
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetItemByGradeAsync(int grade, CancellationToken cancellationToken = default)
        {
            var result = await _itemsRepository.GetByGradeAsync(grade, cancellationToken);


            return result;

        }

        public async Task<IReadOnlyCollection<ItemModel>> GetItemsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var result = await _itemsRepository.GetByNameAsync(name, cancellationToken);


            NotFoundException.ThrowIfEmpty(result, nameof(ItemModel));

            return result;

        }


        public async Task DeleteItemAsync(int id, CancellationToken cancellationToken = default)
        {
            await GetItemAsync(id, cancellationToken);

            await _itemsRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task UpdateItemAsync(ItemModel item, CancellationToken cancellationToken = default)
        {
            await GetItemAsync(item.Id, cancellationToken);

            await _itemsRepository.UpdateAsync(item, cancellationToken);
        }

        public async Task UpdateIconPath(string path, CancellationToken cancellationToken = default)
        {
            var items = await _itemsRepository.GetAllAsync();

            var itemsChanedPath = items.Select(item=> new ItemModel()
            {
                Grade= item.Grade,
                Icon = $"{path}/{item.Id}.png",
                Name= item.Name,
                Id= item.Id,
            });

            IEnumerable<Task> tasks = new List<Task>()
            {
                _itemsRepository.UpdateIconPathAsync(path,cancellationToken),
                _resourcesManager.SaveLocalizationAsync(_language.Prefix,items,cancellationToken)
            };

            await Task.WhenAll(tasks);
            
            
        }

        public async Task<IReadOnlyCollection<ItemModel>> GetItemByQuery(ItemSearchQuery query, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ItemModel> items = Enumerable.Empty<ItemModel>()
                .ToList();

            if(query.Grade !=-1 && !string.IsNullOrEmpty(query.Name))
            {
                items = await _itemsRepository.GetByNameAndGrade(query.Name, query.Grade); 
            }
            else if (!string.IsNullOrEmpty(query.Name))
            {
                items = await _itemsRepository.GetByNameAsync(query.Name);
            }
            else if (query.Grade != -1)
            {
                items = await _itemsRepository.GetByGradeAsync(query.Grade);
            }

            NotFoundException.ThrowIfEmpty(items,nameof(ItemModel));

            return items;
        }
        
    }

}
