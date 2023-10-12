using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using System.Security.Claims;

namespace LisTOMania.Common.Interfaces.Business
{
    public interface IListManager
    {
        Task ClearAll(Guid listId, bool onlyDone);

        Task Create(ListDto list);

        Task Edit(ListDto list);

        Task<ListDto?> Get(Guid id);

        Task<IEnumerable<ListDto>> GetAll();

        Task<IEnumerable<ListDto>> GetAllByUser(ClaimsPrincipal user);

        Task MarkAllAs(Guid listId, bool isDone);

        Task Remove(Guid id);
    }
}