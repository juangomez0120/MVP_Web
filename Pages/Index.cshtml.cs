using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MVP_Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string UsernameUsuario { get; set; }
        [BindProperty]
        public string PasswordUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string MensajeError { get; set; }

        public void OnGet()
        {
            MensajeError = "";
        }

        public async Task<IActionResult> OnPost() // Antes: public IActionResult OnPost()
        {
            if (UsernameUsuario == null || PasswordUsuario == null)
            {
                MensajeError = "Credenciales Inválidas";
                return Page();
            }
            else
            {
                string responseContent = "Credenciales Inválidas";

                Uri baseURL = new Uri("https://chatarrap-api.herokuapp.com/users/login");

                HttpClient client = new HttpClient();

                JObject joPeticion = new JObject();
                joPeticion.Add("username", UsernameUsuario);
                joPeticion.Add("password", PasswordUsuario);

                var stringContent = new StringContent(joPeticion.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(baseURL.ToString(), stringContent);

                if (response.IsSuccessStatusCode)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("SessionKey", responseContent);

                    // BASE DE DATOS
                    
                    string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
                    MySqlConnection conexion = new MySqlConnection(connectionString);
                    conexion.Open();

                    var query1 = "SELECT Nombre, Apellido FROM Usuario WHERE Id = @id;";

                    var cmd = new MySqlCommand(query1, conexion);
                    cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = UsernameUsuario;

                    HttpContext.Session.SetString("SessionUsername", UsernameUsuario);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        NombreUsuario = reader.GetString(0);
                        ApellidoUsuario = reader.GetString(1);
                        reader.Close();
                        cmd.CommandText = "INSERT INTO Bitacora(Usuario_Id, Fecha) VALUES(@id, @fecha)";
                        cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.ExecuteNonQuery();

                        HttpContext.Session.SetString("SessionNombre", NombreUsuario);
                        HttpContext.Session.SetString("SessionApellido", ApellidoUsuario);

                        return RedirectToPage("menu");
                    }

                    reader.Close();

                    return RedirectToPage("DatosUsuario");
                }
                   
                MensajeError = responseContent;

                return Page();
            }
        }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

    }
}
