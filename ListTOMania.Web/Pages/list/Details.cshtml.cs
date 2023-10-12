using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using ListTOMania.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;

namespace ListTOMania.Web.Pages.list
{
    [Authorize]
    public class GetModel : PageModel
    {
        private readonly IListManager tomListManager;
        private readonly IItemManager itemManager;
        private readonly IRechteManager rechteManager;
        private readonly ITagManager tagManager;

        [BindProperty(SupportsGet = true)]
        public Guid ListId { get; set; } // eig unnötig weil in list...

        public ItemDto NewItem { get; set; }

        //public IEnumerable<SelectListItem> Available_Tags { get; set; }
        //[BindProperty]
        //public IEnumerable<string> Selected_Tags { get; set; }

        [BindProperty]
        public string? Tags { get; set; } = "";

        [BindProperty]
        public string? TextFilter { get; set; } = "";

        public bool ReadDonly { get; set; }

        public ListDto Data { get; set; }

        public GetModel(IListManager tomListManager, ITagManager tagManager, IItemManager itemManager, IRechteManager rechteManager)
        {
            this.tomListManager = tomListManager;
            this.tagManager = tagManager;
            this.itemManager = itemManager;
            this.rechteManager = rechteManager;

            //this.Available_Tags = this.tagManager.GetAll().Select(t => new SelectListItem(t, t));
        }

        public async Task<IActionResult> OnGet()
        {
            var element = await this.tomListManager.Get(this.ListId);
            if (element is null)
            {
                return new RedirectResult("/NotFound");
            }

            if (!await this.rechteManager.CanRead(element.Id.Value, User))
            {
                return Forbid();
            }

            if (!await this.rechteManager.CanEdit(element.Id.Value, User))
            {
                this.ReadDonly = true;
            }

            this.Data = element;
            return Page();
        }

        public async Task<IActionResult> OnPost(ItemDto newItem)
        {
            if (!ModelState.IsValid)
            {
                this.Data = await this.tomListManager.Get(this.ListId);
                return Page();
            }

            if (!await this.rechteManager.CanEdit(this.ListId, User))
            {
                return Forbid();
            }

            newItem.Tags = TagConverter.GetStringListForBackend(this.Tags);

            await this.itemManager.Add(this.ListId, newItem);

            var element = await this.tomListManager.Get(this.ListId);
            this.Data = element;

            ModelState.Clear();
            this.Tags = string.Empty;
            return Page();
        }

        public async Task<IActionResult> OnPostFilter(string textFilter)
        {
            var element = await this.tomListManager.Get(this.ListId);
            if (element is null)
            {
                return new RedirectResult("/NotFound");
            }

            element.Items = element.Items.Where(i => SearchPredicate(textFilter, i)).ToList();


            if (!await this.rechteManager.CanEdit(element.Id.Value, User))
            {
                this.ReadDonly = true;
            }

            this.TextFilter = textFilter;
            this.Data = element;
            this.ModelState.Clear();
            return Page();
        }

        private static bool SearchPredicate(string textFilter, ItemDto i)
        {
            if (string.IsNullOrWhiteSpace(textFilter))
            {
                return true;
            }

            var searchTerms = textFilter.Split().Where(s => !string.IsNullOrWhiteSpace(s)).Select(e => e.Trim().ToLower());

            return searchTerms.All(term => ItemMatchesTerm(term, i));
        }

        private static bool ItemMatchesTerm(string term, ItemDto i)
            => i.Designation.ToLower().Contains(term) || i.Tags.Any(t => t.ToLower().Contains(term));

        public async Task<IActionResult> OnPostMarkAllHandler()
        {
            if (!await this.rechteManager.CanEdit(this.ListId, User))
            {
                return Forbid();
            }

            await this.tomListManager.MarkAllAs(this.ListId, true);
            this.Data = await this.tomListManager.Get(this.ListId);

            return Page();
        }

        public async Task<IActionResult> OnPostUnmarkAllHandler()
        {
            if (!await this.rechteManager.CanEdit(this.ListId, User))
            {
                return Forbid();
            }

            await this.tomListManager.MarkAllAs(this.ListId, false);
            this.Data = await this.tomListManager.Get(this.ListId);
            return Page();
        }

        public async Task<IActionResult> OnPostClearMarkedHandler()
        {
            if (!await this.rechteManager.CanEdit(this.ListId, User))
            {
                return Forbid();
            }

            await this.tomListManager.ClearAll(this.ListId, true);
            this.Data = await this.tomListManager.Get(this.ListId);
            return Page();
        }

        public async Task<IActionResult> OnPostClearAllHandler()
        {
            if (!await this.rechteManager.CanEdit(this.ListId, User))
            {
                return Forbid();
            }

            await this.tomListManager.ClearAll(this.ListId, false);
            this.Data = await this.tomListManager.Get(this.ListId);
            return Page();
        }
    }
}