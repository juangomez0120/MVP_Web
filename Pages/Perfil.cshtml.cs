using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Web.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Sprint3.Pages
{
    public class PerfilModel : PageModel
    {
        public Login Usuario { get; set; }

        public void OnGet()
        {
            string value = HttpContext.Session.GetString("SessionUsuario");

            if (value != null)
            {
                Usuario = JsonConvert.DeserializeObject<Login>(value);
            }
        }
    }
}
