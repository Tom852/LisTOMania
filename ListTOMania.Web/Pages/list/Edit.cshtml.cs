using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using ListTOMania.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace ListTOMania.Web.Pages.list
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IListManager tomListManager;
        private readonly IUserManager userManager;
        private readonly IRechteManager rechteManager;

        public bool IsCreate { get; set; }

        public ListDto Data { get; set; }

        public IEnumerable<SelectListItem> Available_SAIO { get; set; }

        [BindProperty]
        public IEnumerable<string> Selected_SAIO { get; set; } = new List<string>();

        [BindProperty]
        public List<UserDto?> AllUsers { get; set; }

        public string[] AllRechte { get; set; }

        [BindProperty]
        [DisplayName("Rechte:")]
        public RechteCheckbox[][] RechteCheckboxen { get; set; }

        public class RechteCheckbox
        {
            public string UserName { get; set; }
            public string RechtName { get; set; }
            public bool IsSelected { get; set; }
        }

        public EditModel(IListManager tomListManager, IUserManager userManager, IRechteManager rechteManager)
        {
            this.tomListManager = tomListManager;
            this.userManager = userManager;
            this.rechteManager = rechteManager;
        }

        public async Task<IActionResult> OnGet(Guid? listId)
        {
            ListDto? element = null;
            if (listId is null)
            {
                this.IsCreate = true;
                this.Data = new()
                {
                    Items = new(),
                };
            }
            else
            {
                element = await this.tomListManager.Get(listId.Value);
                this.Data = element;
            }

            if (IsCreate)
            {
                if (!User.IsInRole("admin"))
                {
                    // Hier könnt man das ERstellen für non-admins verbieten.
                    // return Forbid();
                }
            }
            else
            {
                if (!await this.rechteManager.CanEdit(this.Data.Id.Value, User))
                {
                    return Forbid();
                }
            }

            this.Available_SAIO = (await this.tomListManager.GetAllByUser(User)).Where(l => l.Id != element?.Id).Select(l => new SelectListItem(l.Designation, l.Id.ToString()));
            this.Selected_SAIO = element?.ShowsAlsoItemsOf.Select(saio => saio.Id.Value.ToString()) ?? new List<string>();

            this.AllUsers = (await this.userManager.GetAll()).ToList();

            this.AllRechte = this.rechteManager.GetAll();

            this.RechteCheckboxen = AllUsers.Select(user =>
                    AllRechte.Select(permissionType => new RechteCheckbox
                    {
                        UserName = user.Name,
                        RechtName = permissionType,
                        IsSelected = this.rechteManager.HasPermit(user.Name, permissionType, listId).Result, // da wir das heir so machen,w as eig recht ineffizient aber schön ist, könnte man beim list dto die echte entfernen und dann iwei unten den rechtemanager callen (setRight(permissionType, listId, user)) dann sit das auch nicht so hardcoded property pro recht
                    }).ToArray()
                ).ToArray();

            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPost(string? listId, ListDto data)
        {
            if (listId is not null && data.Id.HasValue)
            {
                if (new Guid(listId) != data.Id.Value)
                {
                    return BadRequest("ID Mismatch");
                }
            }

            if (listId is null && data.Id.HasValue)
            {
                return BadRequest("list id is null but data has id");
            }

            if (listId is not null && !data.Id.HasValue)
            {
                return BadRequest("list id is not null but data has no id");
            }

            if (!ModelState.IsValid)
            {
                this.AllUsers = (await this.userManager.GetAll()).ToList();
                this.AllRechte = this.rechteManager.GetAll();
                return Page();
            }

            if (listId is null)
            {
                if (!User.IsInRole("admin"))
                {
                    // Hier könnt man das ERstellen für non-admins verbieten.
                    // return Forbid();
                }
            }
            else
            {
                if (!await this.rechteManager.CanEdit(data.Id.Value, User))
                {
                    return Forbid();
                }
            }

            data.ShowsAlsoItemsOf = Selected_SAIO.Select(s =>   // TODO: Ob hjier man nicht ienfach mit strings arbeiten will?
                new ListDto()
                {
                    Id = Guid.Parse(s),
                }
            ).ToList();

            data.CanRead = this.RechteCheckboxen.SelectMany(o => o)
                .Where(r => r.RechtName == "CanRead")
                .Where(r => r.IsSelected)
                .Select(r => new UserDto() { Name = r.UserName });

            data.CanEdit = this.RechteCheckboxen.SelectMany(o => o)
                .Where(r => r.RechtName == "CanEdit")
                .Where(r => r.IsSelected)
                .Select(r => new UserDto() { Name = r.UserName });

            if (listId is null)
            {
                await this.tomListManager.Create(data);
            }
            else
            {
                await this.tomListManager.Edit(data);
            }
            return RedirectToPage("/List/Index");
        }
    }
}