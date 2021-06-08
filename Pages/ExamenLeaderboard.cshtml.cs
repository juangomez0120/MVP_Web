using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MVP_Web.Model;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Sprint3.Pages
{
    public class ExamenLeaderboardModel : PageModel
    {
        public List<LeaderboardExamen> ListaExamenes { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("SessionUsername") == null || HttpContext.Session.GetString("SessionUsername") == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            if (ListaExamenes == null)
            {
                string respuesta = "[]";

                string jsonString = HttpContext.Session.GetString("SessionKey");

                Key llave = JsonConvert.DeserializeObject<Key>(jsonString);

                Uri baseUrl = new Uri("https://chatarrap-api.herokuapp.com/attempts/scores");

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("auth_key", llave.token);

                HttpResponseMessage response = await client.GetAsync(baseUrl.ToString());

                if (response.IsSuccessStatusCode)
                {
                    respuesta = await response.Content.ReadAsStringAsync();
                    ListaExamenes = JsonConvert.DeserializeObject<List<LeaderboardExamen>>(respuesta);
                    ListaExamenes.OrderByDescending(x => x.score);
                }
            }

            return Page();
        }
    }
}

