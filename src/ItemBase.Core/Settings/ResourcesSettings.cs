using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Resources
{
    public class ResourcesSettings
    {

       
        public static ResourcesSettings Default = new ResourcesSettings()
        {
            IconPath = "",
            LocalizationPath = "",

        };

       

        public string LocalizationPath {  get; init; }

        public string IconPath { get; init; }
    }
}
