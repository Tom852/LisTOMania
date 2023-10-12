using LisTOMania.Common.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Common.Model
{
    public class UserDto
    {
        public UserDto()
        {
        }

        public UserDto(N4JUser u)
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