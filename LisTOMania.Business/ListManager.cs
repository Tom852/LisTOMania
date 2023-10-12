using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using System.Security.Claims;

namespace LisTOMania.Business
{
    public class ListManager : IListManager
    {
        private readonly IListDataAccess listDataAccess;
        private readonly IItemManager itemManager;
        private readonly IRechteManager rechteManager;

        public ListManager(IListDataAccess listDataAccess, IItemManager itemManager, IRechteManager rechteManager)
        {
            this.listDataAccess = listDataAccess;
            this.itemManager = itemManager;
            this.rechteManager = rechteManager;
        }

        public Task ClearAll(Guid listId, bool onlyDone)
            => listDataAccess.ClearAll(listId, onlyDone);

        public async Task Create(ListDto listdto)
        {
            listdto.LastAccess = DateTime.Now;
            await listDataAccess.Create(listdto);
        }

        public async Task Edit(ListDto listdto)
        {
            listdto.LastAccess = DateTime.Now;
            await listDataAccess.Edit(listdto);
        }

        public async Task<ListDto?> Get(Guid id)
        {
            var list = await listDataAccess.Get(id);
            if (list is null) { return null; }
            await this.listDataAccess.RefreshLastAccess(id);

            list.Items = list.Items.OrderBy(l => l.IsDone).ThenBy(l => l.Prio).ThenBy(l => l.DoneAt).ThenBy(l => l.Designation).ToList();
            list.Items.ForEach(i => i.Tags = i.Tags.OrderBy(t => t).ToList());
            return list;
        }

        public Task<IEnumerable<ListDto>> GetAll()
            => listDataAccess.GetAll();

        public async Task MarkAllAs(Guid listId, bool isDone)
        {
            var list = await this.Get(listId);
            foreach (var item in list.Items)
            {
                item.IsDone = isDone;
                await this.itemManager.Edit(item);
            }
        }

        public Task Remove(Guid id)
            => listDataAccess.Remove(id);

        public async Task<IEnumerable<ListDto>> GetAllByUser(ClaimsPrincipal user)
        {
            if (user.IsInRole("admin"))
            {
                return await this.GetAll();
            }

            return await this.listDataAccess.GetAllForUser(user.Identity.Name);
        }
    }
}