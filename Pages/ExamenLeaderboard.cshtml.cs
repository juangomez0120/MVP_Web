using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MVP_Web.Model;

namespace Sprint3.Pages
{
    public class ExamenLeaderboardModel : PageModel
    {
        public IList<Leaderboard> ListaEntrenamiento { get; set; }

        public void OnGet()
        {
            ListaEntrenamiento = new List<Leaderboard>();

            //Base de Datos
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT Usuario.Nombre, Usuario.Apellido, SUM(Entrenamiento.Puntaje) AS PuntajeTotal FROM Usuario INNER JOIN Entrenamiento ON Usuario.Id = Entrenamiento.Usuario_Id GROUP BY Usuario.Id ORDER BY PuntajeTotal DESC LIMIT 10;";

            Leaderboard en1 = new Leaderboard();
            ListaEntrenamiento = new List<Leaderboard>();
            int pos = 0;

            //Agarra información de Base de Datos y se lo pone a las variables
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    en1 = new Leaderboard();
                    en1.posicion = ++pos;
                    en1.nombre = reader["Nombre"].ToString() + " " + reader["Apellido"].ToString();
                    en1.score = Convert.ToInt32(reader["PuntajeTotal"]);
                    ListaEntrenamiento.Add(en1);
                }
            }
            conexion.Dispose();
        }
    }
}

