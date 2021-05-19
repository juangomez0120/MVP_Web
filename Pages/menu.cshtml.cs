using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MVP_Web.Pages
{
    public class Index1Model : PageModel
    {
        public void OnGet()
        {
            string usernameUsuario = HttpContext.Session.GetString("SessionUsuario");
        }
    }
}
