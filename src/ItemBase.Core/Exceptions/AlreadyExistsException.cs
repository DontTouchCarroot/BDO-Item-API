using ItemBase.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Exceptions
{
    public class AlreadyExsisttException : Exception
    {

        public AlreadyExsisttException(string objectName):base($"This {objectName} already exists")
        {

        }
        public static void ThrowIfNotNull(object? @object,string name)
        {
            if (@object is not null)
            {
                throw new AlreadyExsisttException(name);
            }
        }
    }
}
