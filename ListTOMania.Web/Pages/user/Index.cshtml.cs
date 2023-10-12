using LisTOMania.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Web.Pages.user
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserManager userManager;

        public IndexModel(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public IEnumerable<UserDto> Users { get; set; }

        public async Task OnGet()
        {
            Users = await userManager.GetAll();
        }
    }
}