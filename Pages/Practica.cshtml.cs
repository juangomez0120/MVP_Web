using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MVP_Web.Pages
{
    public class PracticaModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("SessionUsername") == null || HttpContext.Session.GetString("SessionUsername") == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            return Page();
        }
    }
}
