using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace ListTOMania.Web.Pages.list
{
    // löschen erstmal nur der admin
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly IListManager tomListManager;

        public string Name { get; set; }

        public DeleteModel(IListManager tomListManager)
        {
            this.tomListManager = tomListManager;
        }

        public async Task OnGet(Guid listId)
        {
            var list = await this.tomListManager.Get(listId);
            this.Name = list.Designation;
        }

        public async Task<IActionResult> OnPost(Guid listId)
        {
            await this.tomListManager.Remove(listId);
            return RedirectToPage("Index");
        }
    }
}