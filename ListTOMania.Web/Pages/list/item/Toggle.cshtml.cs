using LisTOMania.Business;
using LisTOMania.Common.Interfaces.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.item
{
    [Authorize]
    public class ToggleModel : PageModel
    {
        private readonly IItemManager manager;
        private readonly IRechteManager rechteManager;

        public ToggleModel(IItemManager manager, IRechteManager rechteManager)
        {
            this.manager = manager;
            this.rechteManager = rechteManager;
        }

        public async Task<IActionResult> OnGet(Guid itemId, bool setAs)
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
            item.IsDone = setAs;
            await this.manager.Edit(item);
            return new NoContentResult();
        }
    }
}