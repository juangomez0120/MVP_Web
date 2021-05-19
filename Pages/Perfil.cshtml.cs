using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Sprint3.Pages
{
    public class PerfilModel : PageModel
    {
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }

        public void OnGet()
        {
            NombreUsuario = HttpContext.Session.GetString("SessionNombre");
            ApellidoUsuario = HttpContext.Session.GetString("SessionApellido");
        }
    }
}
