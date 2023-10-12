using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ListTOMania.Pages
{
    public class AboutModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public AboutModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}