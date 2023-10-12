using LisTOMania.Common.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model.DataBase
{
    public class N4JList
    {
        public Guid Id { get; set; }

        public string Designation { get; set; }

        public DateTime LastAccess { get; set; }

        public IEnumerable<N4JList> SAIO { get; set; }
    }
}