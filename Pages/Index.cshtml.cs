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
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string correoUsuario { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=TerniumDB;Uid=root;password=password;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            var sql = "INSERT INTO Bitacora(Usuario_Id, Fecha) VALUES(@id, @fecha)";
            using var cmd = new MySqlCommand(sql, conexion);

            cmd.Parameters.AddWithValue("@id", correoUsuario);
            cmd.Parameters.AddWithValue("@fecha", DateTime.Now.Date);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }

    }
}
