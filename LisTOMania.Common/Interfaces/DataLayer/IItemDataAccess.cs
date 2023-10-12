using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.DataLayer
{
    public interface IItemDataAccess
    {
        Task Add(Guid listId, ItemDto itemDto);

        Task Edit(ItemDto itemDto);

        Task<ItemDto?> Get(Guid itemId);

        Task Remove(Guid itemId);
    }
}