using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model.DataBase
{
    public class N4JUser
    {
        public N4JUser()
        {
        }

        public N4JUser(UserDto u)
        {
            this.Id = u.Id;
            this.Name = u.Name;
            this.Password = u.Password;
            this.IsAdmin = u.IsAdmin;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}