using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Business
{
    public class TagManager : ITagManager
    {
        public IEnumerable<string> GetAll() => new List<string>() { "Tag1", "Tag2", "Tag3" };
    }
}