using ItemBase.Core.Settings;

namespace ItemBase.Presentation.Settings
{
    public class AppSettings
    {

        public const string File = "appsettings.json";
        public const string Position = nameof(AppSettings);

        public static AppSettings Default = new AppSettings() {
            CoreSettings = CoreSettings.Default,
            LanguageSettings = LanguageSettings.Default,

        };
        public LanguageSettings LanguageSettings { get; init; }

        public  CoreSettings CoreSettings { get; init; }

        

    }

}
