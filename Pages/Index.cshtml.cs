using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MVP_Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string UsernameUsuario { get; set; }

        public IActionResult OnPost()
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            var query1 = "SELECT * FROM Usuario WHERE Id = @id;";

            var cmd = new MySqlCommand(query1, conexion);
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = UsernameUsuario;

            MySqlDataReader reader = cmd.ExecuteReader();
            

            if (reader.Read())
            {
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
