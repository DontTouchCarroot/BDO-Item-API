using ItemBase.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;


namespace ItemBase.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string objectName):base($"{objectName} not found")
        {

        }
        public static void ThrowIfNull(object? @object,string objectName)
        {
            if (@object == null)
            {
                throw new NotFoundException(objectName);
            }
        }
        public static void ThrowIfEmpty(IReadOnlyCollection<object> items,string objectName)
        {
            if (!items.Any())
            {
                throw new NotFoundException(objectName);
            }
        }
    }
}
