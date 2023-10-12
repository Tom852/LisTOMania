using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.DataLayer
{
    public interface IListDataAccess
    {
        Task ClearAll(Guid listId, bool onlyDone);

        Task Create(ListDto list);

        Task Edit(ListDto list);

        Task<ListDto?> Get(Guid id);

        Task<IEnumerable<ListDto>> GetAll();

        Task<IEnumerable<ListDto>> GetAllForUser(string user);

        Task RefreshLastAccess(Guid listid);

        //Task MarkAllAs(Guid listId, bool isDone);
        Task Remove(Guid id);
    }
}