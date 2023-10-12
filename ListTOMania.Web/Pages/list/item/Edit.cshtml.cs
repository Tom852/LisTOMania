using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using ListTOMania.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.list.item
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IItemManager manager;
        private readonly IRechteManager rechteManager;

        public EditModel(IItemManager manager, IRechteManager rechteManager)
        {
            this.manager = manager;
            this.rechteManager = rechteManager;
        }

        [BindProperty]
        public ItemDto Item { get; set; }

        [BindProperty]
        public string Tags { get; set; }

        public Guid ListId { get; set; }

        public async Task<IActionResult> OnGet(Guid listId, Guid itemId)
        {
            var item = await this.manager.Get(itemId);
            if (item is null)
            {
                return new RedirectResult("/NotFound");
            }

            if (!await this.rechteManager.CanRead(listId, User))
            {
                return Forbid();
            }

            this.Item = item;
            this.ListId = listId;
            this.Tags = TagConverter.GetSingleStringForFrontend(item.Tags);
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid listId, Guid itemId)
        {
            // Die ListId ist ein query param und eig easy fälschable und entspricht der 'childlist'. Aber so wär es gleich per listId hochvererbt.
            if (!await this.rechteManager.CanEdit(listId, User))
            {
                return Forbid();
            }

            this.Item.Tags = TagConverter.GetStringListForBackend(this.Tags);
            await this.manager.Edit(this.Item);
            return new RedirectResult($"/list/details/{listId}");
        }
    }
}