using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using System.Collections.Generic;

namespace LisTOMania.Business
{
    public class ItemManager : IItemManager
    {
        private IItemDataAccess dataAccess;

        public ItemManager(IItemDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Task Add(Guid listId, ItemDto itemdto)
            => this.dataAccess.Add(listId, itemdto);

        public async Task Edit(ItemDto item)
        {
            if (item.IsRepeatable && item.IsDone)
            {
                item.IsDone = false;
                if (!string.IsNullOrWhiteSpace(item.AdditionalText))
                {
                    item.AdditionalText += "\n";
                }
                item.AdditionalText += $"Repeatable item was marked done at: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";
            }

            if (item.IsDone)
            {
                item.DoneAt = DateTime.Now;
            }
            await this.dataAccess.Edit(item);
        }

        public Task<ItemDto?> Get(Guid itemId)
            => this.dataAccess.Get(itemId);

        public Task Remove(Guid itemId)
            => this.dataAccess.Remove(itemId);
    }
}