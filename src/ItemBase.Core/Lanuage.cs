using ItemBase.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ItemBase.Core
{


    public class LanguageList : IEnumerable<Language>
    {
        private readonly IReadOnlyCollection<Language> _languages;

        protected LanguageList(IEnumerable<Language> languages)
        {
            _languages = languages.ToList();
        }
        public static LanguageList Create(LanguageSettings languageOptions)
        {
            var configuretedLanguages = languageOptions.Languges
                .Where(x => x.Value)
                .Select(x => Language.Create(x.Key));

            LanguageList lanuages;

            if (!configuretedLanguages.Any())
            {
                var defaultLanuage = languageOptions.DefaultLanuage;

                ArgumentNullException.ThrowIfNullOrEmpty(defaultLanuage);

                configuretedLanguages = new List<Language>()
                {
                    Language.Create(defaultLanuage)
                };


                lanuages = new LanguageList(configuretedLanguages);
            }
            else
            {
                lanuages = new LanguageList(configuretedLanguages);
            }
            return lanuages;
        }
      
        public IEnumerator<Language> GetEnumerator()
        {
            return _languages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _languages.GetEnumerator();
        }
    }
    public abstract class Language
    {
        public readonly string Prefix;

        protected Language(string prefix) =>
            Prefix = prefix;
        internal static Language Create(Type type) => Create(type.Name);

        internal static Language Create(string prefix)
        {
            switch (prefix.ToLower())
            {
                case "ru":
                    return new Ru();

                case "fr":
                    return new Fr();

                case "es":
                    return new Es();

                case "pt":
                    return new Pt();

                case "jp":
                    return new Jp();

                case "kr":
                    return new Kr();

                case "cn":
                    return new Cn();

                case "tw":
                    return new Tw();

                case "th":
                    return new Th();

                case "tr":
                    return new Tr();

                case "id":
                    return new Id();

                case "de":
                    return new De();

                default:
                    throw new ArgumentException($"Invalid prefix: {prefix}");
            }
        }
    }
    public sealed class Ru : Language
    {
        internal protected Ru() : base("ru")
        {

        }

    }
    public sealed class Us : Language
    {
        internal protected Us() : base("us")
        {

        }

    }
    public sealed class Fr : Language
    {
        internal protected Fr() : base("fr")
        {

        }

    }

    public sealed class Es : Language
    {
        internal protected Es() : base("es")
        {

        }
    }

    public sealed class Pt : Language
    {
        internal protected Pt() : base("pt")
        {

        }
    }

    public sealed class Jp : Language
    {
        internal protected Jp() : base("jp")
        {

        }
    }

    public sealed class Kr : Language
    {
        internal protected Kr() : base("kr")
        {

        }
    }

    public sealed class Cn : Language
    {
        internal protected Cn() : base("cn")
        {

        }
    }

    public sealed class Tw : Language
    {
        internal protected Tw() : base("tw")
        {

        }
    }

    public sealed class Th : Language
    {
        internal protected Th() : base("tr")
        {

        }
    }

    public sealed class Tr : Language
    {
        internal protected Tr() : base("tr")
        {

        }
    }

    public sealed class Id : Language
    {
        internal protected Id() : base("Id")
        {

        }

    }
    public sealed class De : Language
    {
        internal protected De() : base("de")
        {

        }

    }


}
