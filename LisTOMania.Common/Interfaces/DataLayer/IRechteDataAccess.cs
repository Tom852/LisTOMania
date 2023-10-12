using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.DataLayer
{
    public interface IRechteDataAccess
    {
        Task<bool> CanEdit(Guid listId, string userName);

        Task<bool> CanRead(Guid listId, string userName);

        Task<bool> HasPermit(string name, string permissionType, Guid value);
    }
}