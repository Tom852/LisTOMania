using LisTOMania.Common.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model.DataBase
{
    public class N4JItem : ITaggable
    {
        public Guid Id { get; set; }

        public string Designation { get; set; } = "";
        
        public string AdditionalText { get; set; } = "";

        public int Prio { get; set; }

        public bool IsDone { get; set; }

        public bool IsRepeatable { get; set; }

        public DateTime? DoneAt { get; set; }

        [NotMapped]
        public string Neo4JLabel => "Item";
    }
}