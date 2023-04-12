using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItemBase.Core.Models
{
    public record class ItemModel
    {

        public int Id { get; set; }

        [Range(0, 4)]
        public int Grade { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        [Required]
        [StringLength(50)]

        public string Icon { get; set; }


    }
   
}
