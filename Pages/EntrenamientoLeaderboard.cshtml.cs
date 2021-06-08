using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MVP_Web.Model;
using Microsoft.AspNetCore.Http;

namespace Sprint3.Pages
{
    public class EntrenamientoLeaderboardModel : PageModel
    {
        public IList<LeaderboardEntrenamiento> ListaEntrenamiento { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("SessionUsername") == null || HttpContext.Session.GetString("SessionUsername") == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            ListaEntrenamiento = new List<LeaderboardEntrenamiento>();

            //Base de Datos
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT Usuario.Nombre, Usuario.Apellido, SUM(Entrenamiento.Puntaje) AS PuntajeTotal FROM Usuario INNER JOIN Entrenamiento ON Usuario.Id = Entrenamiento.Usuario_Id GROUP BY Usuario.Id ORDER BY PuntajeTotal DESC LIMIT 10;";

            LeaderboardEntrenamiento en1 = new LeaderboardEntrenamiento();
            ListaEntrenamiento = new List<LeaderboardEntrenamiento>();
            int pos = 0;

            //Agarra información de Base de Datos y se lo pone a las variables
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    en1 = new LeaderboardEntrenamiento();
                    en1.posicion = ++pos;
                    en1.username = reader["Nombre"].ToString() + " " + reader["Apellido"].ToString();
                    en1.score = Convert.ToInt32(reader["PuntajeTotal"]);
                    ListaEntrenamiento.Add(en1);
                }
            }

            pos++;
            cmd.CommandText = "SELECT Usuario.Nombre, Usuario.Apellido FROM Usuario WHERE NOT EXISTS (SELECT * FROM Entrenamiento WHERE Usuario.Id = Entrenamiento.Usuario_Id);";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    en1 = new LeaderboardEntrenamiento();
                    en1.posicion = pos;
                    en1.username = reader["Nombre"].ToString() + " " + reader["Apellido"].ToString();
                    en1.score = 0;
                    ListaEntrenamiento.Add(en1);
                }
            }

            conexion.Dispose();

            return Page();
        }
    }
}
