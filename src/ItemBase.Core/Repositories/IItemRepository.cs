using ItemBase.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ItemBase.Core.Repositories
{

    public interface IItemRepository
    {
       
        public long Count { get; }
        public string Language { get; }


        public Task AddRangeAsync(IReadOnlyCollection<ItemModel> itemModels, CancellationToken cancellationToken = default);
        public Task AddAsync(ItemModel item, CancellationToken cancellationToken = default);

        public Task UpdateAsync(ItemModel item, CancellationToken cancellationToken = default);

        public Task UpdateIconPathAsync(string path, CancellationToken cancellationToken = default);

        public Task DeleteAsync(int id,CancellationToken cancellationToken);

        public Task<ItemModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<IReadOnlyCollection<ItemModel>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        public Task<IReadOnlyCollection<ItemModel>> GetByGradeAsync(int grade, CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetByIdsAsync(IReadOnlyCollection<int> ids,CancellationToken cancellationToken = default);

        public Task<IReadOnlyCollection<ItemModel>> GetByNameAndGrade(string name, int grade, CancellationToken cancellationToken = default);


    }
    public interface IItemRepository<TLanuage> : IItemRepository
        where TLanuage : Language
    {

       

    }

}
