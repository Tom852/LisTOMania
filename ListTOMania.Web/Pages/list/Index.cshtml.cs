using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.list
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private IListManager manager;

        [BindProperty]
        public string? TextFilter { get; set; } = "";

        public IndexModel(IListManager listManager)
        {
            this.manager = listManager;
        }

        public IEnumerable<ListDto> AllLists { get; private set; }

        public async Task OnGet()
        {
            var data = await manager.GetAllByUser(User);
            this.AllLists = data.OrderByDescending(list => list.LastAccess);
        }

        public async Task<IActionResult> OnPostFilter(string textFilter)
        {
            var data = await this.manager.GetAllByUser(User);
            if (data is null)
            {
                return new RedirectResult("/NotFound");
            }

            this.TextFilter = textFilter;
            data = data.Where(d => SearchPredicate(textFilter, d));
            this.AllLists = data.OrderByDescending(list => list.LastAccess);
            return Page();
        }

        private static bool SearchPredicate(string textFilter, ListDto i)
        {
            if (string.IsNullOrWhiteSpace(textFilter))
            {
                return true;
            }

            var searchTerms = textFilter.Split().Select(e => e.Trim());

            return searchTerms.All(term => ItemMatchesTerm(term, i));
        }

        private static bool ItemMatchesTerm(string term, ListDto i)
            => i.Designation.ToLower().Contains(term.ToLower());
    }
}