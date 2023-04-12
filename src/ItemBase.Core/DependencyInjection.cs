using ItemBase.Core.Background;
using ItemBase.Core.Repositories;
using ItemBase.Core.Services.Cache;
using ItemBase.Core.Services.Icon;
using ItemBase.Core.Services.Item;
using ItemBase.Core.Settings;
using ItemBase.Resources;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Configuration;


namespace ItemBase.Core
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddItemBaseCore(this IServiceCollection services, IConfiguration configuration)
        {
            var coreSettings = new CoreSettings();
            configuration.GetSection("AppSettings:CoreSettings")
                .Bind(coreSettings);


            services.AddSettings(configuration);

            return services.AddItemBaseCore(coreSettings);


        }

        private static IServiceCollection AddItemBaseCore(this IServiceCollection services,CoreSettings coreOptions)
        {

            ArgumentNullException.ThrowIfNull(coreOptions, nameof(coreOptions));


            Type repositoryType;

            services.AddSingleton<ResourcesManager>();

            if (coreOptions.Database == Database.Mongo)
            {
                var mongoConnection = coreOptions.MongoConnection;

                ArgumentNullException.ThrowIfNullOrEmpty(mongoConnection.ConnectionString);


                repositoryType = typeof(Repositories.MongoRepository<>);

                services.AddItemRepository(repositoryType);

                services.AddHostedService<ItemDbInitializer>();           

            }
            else
            {

                repositoryType = typeof(MemoryItemRepository<>);

                services.AddItemRepository(repositoryType);
            }


            services.AddCache(coreOptions.CacheSettings);

            services.AddItemService();

            services.AddIconRepository();

            services.AddIconService();


            return services;


        }

        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<LanguageSettings>(configuration.GetSection("AppSettings:LanguageSettings"));
            services.Configure<MongoConnection>(configuration.GetSection("AppSettings:CoreSettings:MongoConnection"));

            services.Configure<ResourcesSettings>(configuration.GetSection("AppSettings:CoreSettings:ResourcesSettings"));

            return services;
        }

        private static IServiceCollection AddIconRepository(this IServiceCollection services)
        {
            return services.AddSingleton<IIconRepository,IconRepository>();
        }
        private static IServiceCollection AddIconService(this IServiceCollection services)
        {
            return services.AddSingleton<IIconService,IconService>();   
        }

        private static IServiceCollection AddItemService(this IServiceCollection services)
        {

            var serviceType = typeof(IItemService<>);

            var implementationType = typeof(ItemService<>);

            services.TryAddSingleton(serviceType, implementationType);
            return services;

        }

        private static IServiceCollection AddItemRepository(this IServiceCollection services,Type implementationType)
        {

            var serviceType = typeof(IItemRepository<>);

            services.TryAddSingleton(serviceType, implementationType);

            return services;

        }

        private static IServiceCollection AddCache(this IServiceCollection services,
            CacheSettings cacheSettings)
        {
            if(cacheSettings is null)
            {
                return services.AddDefaultMemoryCache();
            }


            RedisCacheOptions? redisCacheOptions = cacheSettings.RedisCacheOptions;
            
            if (redisCacheOptions is not null && !string.IsNullOrEmpty(redisCacheOptions.Configuration))
            {
               

                services.AddStackExchangeRedisCache(option =>
                {
                    option.InstanceName = cacheSettings.RedisCacheOptions.InstanceName;
                    option.Configuration = cacheSettings.RedisCacheOptions.Configuration;
                });
                services.TryAddSingleton<ICacheService, CacheService>();

                return services;
            }

            MemoryCacheOptions? memoryCacheOptions = cacheSettings.MemoryDistributedCacheOptions;

            if (memoryCacheOptions is not null)
            {          
                services.AddDistributedMemoryCache(option =>
                {
                    option.SizeLimit = memoryCacheOptions.SizeLimit;
                    option.ExpirationScanFrequency = memoryCacheOptions.ExpirationScanFrequency;
                    

                });
                services.TryAddSingleton<ICacheService, CacheService>();
                return services;
            }

            return services.AddDefaultMemoryCache();



   
        }
        private static IServiceCollection AddDefaultMemoryCache(this IServiceCollection services)
        {
            var memoryCacheOptions = CacheSettings.Default.MemoryDistributedCacheOptions;

            services.AddDistributedMemoryCache(option =>
            {
                option.SizeLimit = memoryCacheOptions.SizeLimit;
                option.ExpirationScanFrequency = memoryCacheOptions.ExpirationScanFrequency;


            });
            services.TryAddSingleton<ICacheService, CacheService>();

            return services;
        }
  
    
   
    }


}
