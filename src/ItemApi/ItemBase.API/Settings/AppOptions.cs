using ItemBase.Core.Settings;

namespace ItemBase.API.Settings
{
    public class AppOptions
    {

        public const string File = "appsettings.json";
        public const string Position = nameof(AppOptions);

        public static AppOptions Default = new AppOptions() {
            ControllerOptions = new ControllerOptions(),
            CoreOptions = CoreOptions.Default,
            LanguageOptions = LanguageOptions.Default,

        };
        public LanguageOptions LanguageOptions { get; init; }

        public  ControllerOptions ControllerOptions { get; init; }

        public  CoreOptions CoreOptions { get; init; }

        

    }

}
