using ItemBase.Core.Models;
using ItemBase.Core.Settings;
using ItemBase.Core.Repositories;
using ItemBase.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemBase.Resources;

namespace ItemBase.Core.Background
{
    internal class ItemDbInitializer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ItemDbInitializer> _logger;
        private readonly IOptions<LanguageSettings> _languageSettings;
        private readonly ResourcesManager _resourcesManager;

        public ItemDbInitializer(IServiceProvider serviceProvider,
            ILogger<ItemDbInitializer> logger,
            IOptions<LanguageSettings> languageSettings,
            ResourcesManager resourcesManager)
        {
            _serviceProvider = serviceProvider;

            _languageSettings = languageSettings;

            _logger = logger;
            _resourcesManager = resourcesManager;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            var type = typeof(IItemRepository<>);

            var initResposotories = GetItemRepositories();

            if (!initResposotories.Any())
            {
                return;
            }

            _logger.LogInformation($"[{DateTime.Now}]Start Db Initialize");

            IEnumerable<Task> tasks = initResposotories
                .Select(x => InitializeAsync(x, stoppingToken));

            await Task.WhenAll(tasks);

            _logger.LogInformation($"[{DateTime.Now}]End Db Initialize");
        }

        private IEnumerable<IItemRepository> GetItemRepositories()
        {
            var lanuageTypes = LanguageList.Create(_languageSettings.Value)
                .Select(x => x.GetType());

            foreach (var type in lanuageTypes)
            {
                var resitoryType = typeof(IItemRepository<>).MakeGenericType(type);

                var repository = _serviceProvider.GetService(resitoryType) as IItemRepository;

                if(repository is not null)
                {
                    yield return repository;
                }
            }

            


        }
        private async Task InitializeAsync(IItemRepository repository,CancellationToken cancellationToken = default)
        {
            
            if(repository.Count != 0)
            {
                return;
            }

            var itemsFromJson = await _resourcesManager.LoadLocalizationAsync(repository.Language);


            _logger.LogInformation($"{DateTime.Now}Start initialize items_{repository.Language} collection");


            await repository.AddRangeAsync(itemsFromJson);

            _logger.LogInformation($"[{DateTime.Now}]End initialize items_{repository.Language} collection");

        }
    }
}
