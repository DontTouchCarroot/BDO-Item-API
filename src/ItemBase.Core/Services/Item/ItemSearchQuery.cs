using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Services.Item
{
    public record class ItemSearchQuery
    {
        [Range(-1, 4)]
        public int Grade { get; init; } = -1;

        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; init; } 
    }
}
