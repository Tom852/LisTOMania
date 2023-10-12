using LisTOMania.Common.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.user
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly IUserManager userManager;

        public UserDto UserDto { get; set; }

        public DeleteModel(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGet(Guid userId)
        {
            var user = await this.userManager.Get(userId);
            if (user is null)
            {
                return new RedirectResult("/NotFound");
            }
            this.UserDto = user;
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid userId)
        {
            await this.userManager.Delete(userId);
            return RedirectToPage("Index");
        }
    }
}