using ItemBase.Core;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ItemBase.API.Controllers;
using ItemBase.Core.Settings;
using ItemBase.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ItemBase.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddItemApi(this IServiceCollection services, IConfiguration configuration)
        {
            var appOptions = new AppOptions();
            configuration.GetSection("AppOptions")
                .Bind(appOptions);


            var options = Options.Create(appOptions.LanguageOptions);

            services.TryAddSingleton(options);
                

            return AddItemApi(services, appOptions);
        }
        public static IServiceCollection AddItemApi(this IServiceCollection services,AppOptions appSettings)
        {
            ArgumentNullException.ThrowIfNull(appSettings, nameof(appSettings));

            var languageOptions = appSettings.LanguageOptions;


            LanguageList languages = LanguageList.Create(languageOptions);


            
            services.AddItemBaseCore(appSettings.CoreOptions);

            services.AddItemContoller(languages);


            return services;
        }
        public static IServiceCollection AddItemApi(this IServiceCollection services,Func<AppOptions> factory)
            =>  AddItemApi(services,factory());
 

        private static IServiceCollection AddItemContoller(this IServiceCollection services
            ,LanguageList languages)
        {

            var types = languages.Select(x => x.GetType());

            services.AddControllers(options =>
            {
                options.Conventions.Add(new LanguageControllerModelConvention());
            }).ConfigureApplicationPartManager(c =>
            {
                c.FeatureProviders.Add(new LanguageControllersFeatureProvider(types));
            });

          
            return services;

        }

    }
}
