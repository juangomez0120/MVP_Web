﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sprint3.Pages
{
    public class PerfilModel : PageModel
    {
        public void OnGet()
        {
<<<<<<< Updated upstream
=======
            Usuario = new Login();
            string value = HttpContext.Session.GetString("SessionUsuario");

            if (value != null)
            {
                Usuario = JsonConvert.DeserializeObject<Login>(value);
            }
>>>>>>> Stashed changes
        }
    }
}
