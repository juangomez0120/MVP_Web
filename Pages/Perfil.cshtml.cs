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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Sprint3.Pages
{
    public class PerfilModel : PageModel
    {
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public IList<Entrenamiento> ListaEntrenamiento { get; set; }
        public IList<Examen> ListaExamenes { get; set; }
        public List<List<string>> ListaMedallas { get; set; }
        public bool Estatus { get; set; }

        public async Task<IActionResult> OnGet() // void onGet()
        {
            // Variables de sesión
            string UsernameUsuario = HttpContext.Session.GetString("SessionUsername");

            // API
            if (ListaExamenes == null)
            {
                Estatus = false;

                string respuesta = "[]";

                string jsonString = HttpContext.Session.GetString("SessionKey");

                Key llave = JsonConvert.DeserializeObject<Key>(jsonString);

                Uri baseUrl = new Uri("https://chatarrap-api.herokuapp.com/users/getScores/605285198d2862d0df655e3d"); //getScores/{llave.user}

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("auth_key", llave.token);

                HttpResponseMessage response = await client.GetAsync(baseUrl.ToString());

                if (response.IsSuccessStatusCode)
                {
                    respuesta = await response.Content.ReadAsStringAsync();

                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(respuesta);
                    Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> diccionarioExamenes = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>>(obj);

                    ListaExamenes = new List<Examen>();
                    int num = 0;

                    foreach(KeyValuePair<string, Dictionary<string, Dictionary<string, List<int>>>> mesRaw in diccionarioExamenes)
                    {
                        foreach(KeyValuePair<string, Dictionary<string, List<int>>> tipo in diccionarioExamenes[mesRaw.Key])
                        {
                            foreach(KeyValuePair<string, List<int>> categoria in diccionarioExamenes[mesRaw.Key][tipo.Key])
                            {
                                ListaExamenes.Add(new Examen(++num, mesRaw.Key, tipo.Key + " - " + categoria.Key, (double)categoria.Value[0] / categoria.Value[1] * 100));

                            }
                        }
                    }

                    Estatus = true;
                }
            }
            else
            {
                Estatus = true;
            }

            historialEntrenamiento(UsernameUsuario);
            medallas(UsernameUsuario);

             return Page();
        }

        public void historialEntrenamiento(string UsernameUsuario) {
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

        public void medallas(string UsernameUsuario)
        {
            ListaMedallas = new List<List<string>>();
            for(int i = 1; i <= 3; i++)
            {
                ListaMedallas.Add(new List<string>());
            }

            //Base de Datos
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT Medalla.SubCategoriaMedalla_Id, SubCategoriaMedalla.Nombre, SubCategoriaMedalla.CategoriaMedalla_Id FROM Medalla INNER JOIN SubCategoriaMedalla ON Medalla.SubCategoriaMedalla_Id = SubCategoriaMedalla.Id WHERE Medalla.Usuario_Id = @id;";
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = UsernameUsuario;

            int categoria;
            string nombre;

            //Agarra información de Base de Datos y se lo pone a las variables
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categoria = Convert.ToInt32(reader["CategoriaMedalla_Id"]);
                    nombre = reader["Nombre"].ToString();
                    ListaMedallas[categoria - 1].Add(nombre);
                }
            }
            conexion.Dispose();

        }
    }
}
