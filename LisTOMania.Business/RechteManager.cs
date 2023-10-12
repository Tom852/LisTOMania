using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using System.Collections.Generic;
using System.Security.Claims;

namespace LisTOMania.Business
{
    public class RechteManager : IRechteManager
    {
        private readonly IRechteDataAccess dataAccess;

        public RechteManager(IRechteDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> CanEdit(Guid listId, ClaimsPrincipal user)
        {
            if (user.IsInRole("admin"))
            {
                return true;
            }
            return await dataAccess.CanEdit(listId, user.Identity.Name);
        }

        public async Task<bool> CanRead(Guid listId, ClaimsPrincipal user)
        {
            if (user.IsInRole("admin"))
            {
                return true;
            }

            if (await this.CanEdit(listId, user))
            {
                return true;
            }

            return await dataAccess.CanRead(listId, user.Identity.Name);
        }

        public string[] GetAll()
            => new string[] { "CanRead", "CanEdit" };

        public async Task<bool> HasPermit(string name, string permissionType, Guid? listId)
        {
            if (!listId.HasValue)
            {
                return false;
            }

            return await dataAccess.HasPermit(name, permissionType, listId.Value);
        }
    }
}