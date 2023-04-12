using ItemBase.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Services.Item
{
    public interface IItemService<TLangauge>
        where TLangauge : Language
    {
        public Task<ItemModel> GetItemAsync(int id, CancellationToken cancellationToken = default);

        public Task DeleteItemAsync(int id, CancellationToken cancellationToken = default);

        public Task UpdateItemAsync(ItemModel item, CancellationToken cancellationToken = default);

        public Task UpdateIconPath(string path,CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetAllAsync(CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetItemByGradeAsync(int grade, CancellationToken cancellationToken = default);


        public Task<IReadOnlyCollection<ItemModel>> GetItemByQuery(ItemSearchQuery query, CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetItemsByNameAsync(string name, CancellationToken cancellationToken = default);

        public Task AddItemAsync(ItemModel item, CancellationToken cancellationToken = default);
        public Task AddRangeItemsAsync(IReadOnlyCollection<ItemModel> items, CancellationToken cancellationToken = default);




    }
}
