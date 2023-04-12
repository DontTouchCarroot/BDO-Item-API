using ItemBase.Core;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ItemBase.Presentation.Controllers;
using ItemBase.Core.Settings;
using ItemBase.Presentation.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ItemBase.Resources;
using Microsoft.Extensions.Configuration;
using ItemBase.Presentation.Middleware;

namespace ItemBase.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddItemBasePresentation(this IServiceCollection services, IConfiguration configuration)
        {
            var languageSettings = new LanguageSettings();
            configuration.GetSection("AppSettings:LanguageSettings")
                .Bind(languageSettings);

            ArgumentNullException.ThrowIfNull(languageSettings, nameof(languageSettings));

            LanguageList languages = LanguageList.Create(languageSettings);


            services.AddTransient<ExceptionMiddleware>();

            return services.AddItemContollers(languages);
        }
        private static IServiceCollection AddItemContollers(this IServiceCollection services
            ,LanguageList languages)
        {

            var types = languages.Select(x => x.GetType());

            services.AddMvc(options =>
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
