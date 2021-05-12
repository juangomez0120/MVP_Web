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
using System.Text;
using MVP_Web.Model;
using Microsoft.AspNetCore.Http;

namespace MVP_Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public Login Usuario { get; set; }

        public async Task<IActionResult> OnPost() // Antes: public IActionResult OnPost()
        {
            string responseContent = "Credenciales Inválidas";

            //Buscamos el recurso
            Uri baseURL = new Uri("https://chatarrap-api.herokuapp.com/users/login");

            //Creamos el cliente para que haga nuestra peticion
            HttpClient client = new HttpClient();

            // Armamos nuestra peticion
            JObject joPeticion = new JObject();
            joPeticion.Add("username", Usuario.username);
            joPeticion.Add("password", Usuario.password);

            var stringContent = new StringContent(joPeticion.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(baseURL.ToString(), stringContent);

            if (response.IsSuccessStatusCode)
            {
                // Convertir responseContent en variable de sesión (llave)
                responseContent = await response.Content.ReadAsStringAsync();

                // BASE DE DATOS

                /*
               
                // Crear variable de sesión "Username" (checar si puedo usar el tipo de dato "Login")
                HttpContext.Session.SetString("username", Usuario.username);

                string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
                MySqlConnection conexion = new MySqlConnection(connectionString);
                conexion.Open();

                var query1 = "SELECT * FROM Usuario WHERE Id = @id;";

                var cmd = new MySqlCommand(query1, conexion);
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = Usuario.username;

                MySqlDataReader reader = cmd.ExecuteReader();


                if (reader.Read())
                {
                    
                    // Query para leer y crear las variables de sesión "Nombre" y "Apellido"

                    reader.Close();
                    cmd.CommandText = "INSERT INTO Bitacora(Usuario_Id, Fecha) VALUES(@id, @fecha)";
                    cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();

                    return RedirectToPage("menu");
                }
                else
                {
                    reader.Close();
                    return RedirectToPage("DatosUsuario");
                }
                
                */
            }

            return RedirectToPage("RespuestaLogin", new { result = responseContent });
        }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
       
        }

    }
}
