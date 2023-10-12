using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.Business
{
    public interface IItemManager
    {
        Task Add(Guid listId, ItemDto itemDto);

        Task Edit(ItemDto item);

        Task<ItemDto?> Get(Guid itemId);

        Task Remove(Guid itemId);
    }
}