using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using System.Security.Claims;

namespace LisTOMania.Common.Interfaces.Business
{
    public interface IRechteManager
    {
        Task<bool> CanEdit(Guid listId, ClaimsPrincipal user);

        Task<bool> CanRead(Guid listId, ClaimsPrincipal user);

        string[] GetAll();

        Task<bool> HasPermit(string name, string permissionType, Guid? listId);
    }
}