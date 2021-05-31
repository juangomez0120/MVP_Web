using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using MVP_Web.Model;

namespace Sprint3.Pages
{
    public class PerfilModel : PageModel
    {
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public IList<Entrenamiento> ListaEntrenamiento { get; set; }

        public void OnGet()
        {
            // Variables de sesión
            NombreUsuario = HttpContext.Session.GetString("SessionNombre");
            ApellidoUsuario = HttpContext.Session.GetString("SessionApellido");
            string UsernameUsuario = HttpContext.Session.GetString("SessionUsername");

            ListaEntrenamiento = new List<Entrenamiento>();

            //Base de Datos
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT * FROM Entrenamiento WHERE Usuario_id = @id;";
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = UsernameUsuario;

            Entrenamiento en1 = new Entrenamiento();
            ListaEntrenamiento = new List<Entrenamiento>();
            int pos = 0;

            //Agarra información de Base de Datos y se lo pone a las variables
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    en1 = new Entrenamiento();
                    en1.numero = ++pos;
                    en1.puntaje = Convert.ToInt32(reader["Puntaje"]);
                    en1.racha = Convert.ToInt32(reader["StreakMax"]);
                    en1.fecha = DateTime.Parse(reader["Fecha"].ToString()).ToShortDateString();
                    ListaEntrenamiento.Add(en1);
                }
            }
            conexion.Dispose();
        }
    }
}
