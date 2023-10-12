using LisTOMania.Common.Interfaces.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.item
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IItemManager manager;
        private readonly IRechteManager rechteManager;

        public DeleteModel(IItemManager manager, IRechteManager rechteManager)
        {
            this.manager = manager;
            this.rechteManager = rechteManager;
        }

        public async Task<IActionResult> OnGet(Guid itemId)
        {
            var item = await this.manager.Get(itemId);
            if (item is null)
            {
                return new RedirectResult("/NotFound");
            }

            if (!await rechteManager.CanEdit(item.ContainingListId.Value, User))
            {
                return Forbid();
            }

            await this.manager.Remove(itemId);
            return new NoContentResult();
        }
    }
}