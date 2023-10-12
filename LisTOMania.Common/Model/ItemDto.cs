using LisTOMania.Common.Model.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model
{
    public class ItemDto
    {
        public ItemDto()
        { }

        public ItemDto(N4JItem n4ji)
        {
            this.Id = n4ji.Id;
            this.Designation = n4ji.Designation;
            this.Prio = n4ji.Prio;
            this.IsDone = n4ji.IsDone;
            this.IsRepeatable = n4ji.IsRepeatable;
            this.DoneAt = n4ji.DoneAt;
            this.AdditionalText = n4ji.AdditionalText;
        }

        public Guid? Id { get; set; }

        [MinLength(1)]
        public string Designation { get; set; }

        public string? AdditionalText { get; set; }

        [Range(1, 4)]
        public int Prio { get; set; }

        public bool IsDone { get; set; }

        public bool IsRepeatable { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DoneAt { get; set; }

        public Guid? ContainingListId { get; set; }

        public string? ContainingListDesignation { get; set; }

        public List<string>? Tags { get; set; }
    }
}