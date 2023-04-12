using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Settings
{
    public class LanguageSettings
    {

        public readonly static LanguageSettings Default = new LanguageSettings() { 
            DefaultLanuage= "Ru",
            Languges = new Dictionary<string, bool>
            {
                { "Ru",true },
                { "Jp",true },
                { "Kr",true },
                { "Es",true },
                { "Pt",true },
                { "Cn",true },
                { "Tw",true },
                { "Tr",true },              
                { "Id",true },
                { "De",true }
            }
        };

        public string DefaultLanuage { get; init; }

        public Dictionary<string,bool> Languges { get; init; }


    }
}
