using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace ListTOMania.Web.Pages.login
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IUserManager userManager;

        public LoginModel(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/list");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await this.userManager.GetUserIfCredentialsCorrect(Username, Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Username),
                        // Add additional claims as needed
                    };

                    if (user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Set to true to create a persistent cookie
                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1) // Set cookie expiration [commented cause set in startup]
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToPage("/list/Index");
                }

                ModelState.AddModelError("Credentials", "Invalid credentials.");
            }

            return Page();
        }
    }
}