using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model
{
    public class ListDto
    {
        public Guid? Id { get; set; }

        [Required, MinLength(3)]
        public string Designation { get; set; } = string.Empty;

        public List<ListDto>? ShowsAlsoItemsOf { get; set; } = new();

        // public List<string>? Tags { get; set; }

        public List<ItemDto>? Items { get; set; } = new();

        public DateTime LastAccess { get; set; } = DateTime.MinValue;

        public IEnumerable<UserDto>? CanRead { get; set; }

        public IEnumerable<UserDto>? CanEdit { get; set; }
    }
}