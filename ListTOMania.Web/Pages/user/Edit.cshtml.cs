using LisTOMania.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.user
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly IUserManager userManager;

        public EditModel(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public UserDto UserDto { get; set; }

        public bool IsCreate { get; set; }

        public async Task<IActionResult> OnGet(Guid? userId)
        {
            if (userId.HasValue)
            {
                var usre = await this.userManager.Get(userId.Value);
                if (usre is null)
                {
                    return new RedirectResult("/NotFound");
                }
                usre.Password = string.Empty;
                this.UserDto = usre;
            }
            else
            {
                this.UserDto = new UserDto();
                IsCreate = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (this.UserDto.Id == default)
                {
                    await userManager.Create(UserDto);
                }
                else
                {
                    await userManager.Edit(UserDto);
                }
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}