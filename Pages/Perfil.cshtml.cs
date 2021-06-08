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

        public async Task<IActionResult> OnGet() // void onGet()
        {
            // Variables de sesión
            string UsernameUsuario = HttpContext.Session.GetString("SessionUsername");
            UsernameUsuario = "ternium1234"; // EJEMPLO Usuario: ternium1234

            int totalExamenes = 0, totalInmunidad = 0, scoresPosicion = -1, scoresPastPosicion = -1;

            if (UsernameUsuario == null || UsernameUsuario == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            // API
            if (ListaExamenes == null)
            {
                string respuestaExamen = "[]";
                string respuestaMedallaInmunidad = "[]";
                string respuestaScores = "[]";
                string respuestaScoresPast = "[]";

                string keyJsonString = HttpContext.Session.GetString("SessionKey");
                Key llave = JsonConvert.DeserializeObject<Key>(keyJsonString);
                llave.user = "5feb85cabdbd4e00176e634c"; // EJEMPLO Usuario: ternium1234

                string uriStringExamen = "https://chatarrap-api.herokuapp.com/users/getScores/" + llave.user;

                Uri baseUrlExamen = new Uri(uriStringExamen);
                Uri baseUrlMedallaInmunidad = new Uri("https://chatarrap-api.herokuapp.com/attempts/");
                Uri baseUrlScores = new Uri("https://chatarrap-api.herokuapp.com/attempts/scores");
                Uri baseUrlScoresPast = new Uri("https://chatarrap-api.herokuapp.com/attempts/scoresPast");

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("auth_key", llave.token);

                HttpResponseMessage responseExamen = await client.GetAsync(baseUrlExamen.ToString());
                HttpResponseMessage responseMedallaInmunidad = await client.GetAsync(baseUrlMedallaInmunidad.ToString());
                HttpResponseMessage responseScores = await client.GetAsync(baseUrlScores.ToString());
                HttpResponseMessage responseScoresPast = await client.GetAsync(baseUrlScoresPast.ToString());

                if (responseExamen.IsSuccessStatusCode)
                {
                    respuestaExamen = await responseExamen.Content.ReadAsStringAsync();

                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(respuestaExamen);
                    Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> diccionarioExamenes = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>>(obj);

                    ListaExamenes = new List<Examen>();
                    int num = 0;

                    foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<int>>>> mesRaw in diccionarioExamenes)
                    {
                        if (diccionarioExamenes[mesRaw.Key].ContainsKey("Chatarra")) {
                            foreach (KeyValuePair<string, List<int>> categoria in diccionarioExamenes[mesRaw.Key]["Chatarra"])
                            {
                                ListaExamenes.Add(new Examen(++num, mesRaw.Key, categoria.Key, (double)categoria.Value[0] / categoria.Value[1] * 100));
                            }
                        }
                    }
                }

                if (responseMedallaInmunidad.IsSuccessStatusCode)
                {
                    respuestaMedallaInmunidad = responseMedallaInmunidad.Content.ReadAsStringAsync().Result;

                    List<Attempt> listaIntentos = new List<Attempt>();

                    JArray jsonCompleto = JArray.Parse(respuestaMedallaInmunidad);
                    foreach (JObject jsonIndividual in jsonCompleto.Children<JObject>())
                    {
                        Attempt intento = new Attempt();
                        intento = JsonConvert.DeserializeObject<Attempt>(jsonIndividual.ToString());
                        listaIntentos.Add(intento);
                    }

                    List<Attempt> listaUsuarioIntentos = listaIntentos.Where(x => x.username == UsernameUsuario).ToList();
                    totalExamenes = listaUsuarioIntentos.Count();

                    List<Attempt> listaUsuarioPerfecto = listaIntentos.Where(x => x.username == UsernameUsuario && x.attempt == 1 && x.score == 100).ToList();
                    totalInmunidad = listaUsuarioPerfecto.Count();
                }

                if (responseScores.IsSuccessStatusCode)
                {
                    respuestaScores = await responseScores.Content.ReadAsStringAsync();
                    List<LeaderboardExamen> listaScores = JsonConvert.DeserializeObject<List<LeaderboardExamen>>(respuestaScores);
                    listaScores.OrderByDescending(x => x.score);

                    scoresPosicion = listaScores.FindIndex(x => x.username == UsernameUsuario);
                }

                if (responseScoresPast.IsSuccessStatusCode)
                {
                    respuestaScoresPast = await responseScoresPast.Content.ReadAsStringAsync();
                    List<LeaderboardExamen> listaScoresPast = JsonConvert.DeserializeObject<List<LeaderboardExamen>>(respuestaScoresPast);
                    listaScoresPast.OrderByDescending(x => x.score);

                    scoresPastPosicion = listaScoresPast.FindIndex(x => x.username == UsernameUsuario);
                }

            }

            historialEntrenamiento(UsernameUsuario);
            actualizaMedallas(UsernameUsuario, totalExamenes, totalInmunidad, scoresPosicion, scoresPastPosicion);
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

        public void actualizaMedallas(string UsernameUsuario, int totalExamenes, int totalInmunidad, int scoresPosicion, int scoresPastPosicion)
        {
            var query = "";

            if(totalExamenes >= 100)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '1') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '1') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '2') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '2') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '3') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '3') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '4') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '4') LIMIT 1;";
            }
            else if(totalExamenes >= 50)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '1') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '1') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '2') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '2') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '3') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '3') LIMIT 1;";
            }
            else if(totalExamenes >= 10)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '1') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '1') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '2') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '2') LIMIT 1;";
            }
            else if (totalExamenes >= 5)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '1') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '1') LIMIT 1;";
            }

            if (totalInmunidad >= 100)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '5') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '5') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '6') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '6') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '7') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '7') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '8') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '8') LIMIT 1;";
            }
            else if (totalInmunidad >= 50)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '5') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '5') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '6') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '6') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '7') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '7') LIMIT 1;";
            }
            else if (totalInmunidad >= 10)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '5') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '5') LIMIT 1;";
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '6') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '6') LIMIT 1;";
            }
            else if (totalInmunidad >= 1)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '5') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '5') LIMIT 1;";
            }

            if (scoresPastPosicion >= 0 && scoresPastPosicion <= 9)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '9') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '9') LIMIT 1;";
            }

            if (scoresPosicion >= 0 && scoresPosicion <= 9 && scoresPastPosicion >= 0 && scoresPastPosicion <= 9)
            {
                query += "INSERT INTO Medalla (Usuario_Id, SubCategoriaMedalla_Id) SELECT * FROM (SELECT @id, '10') AS temp WHERE NOT EXISTS (SELECT Usuario_Id, SubCategoriaMedalla_Id FROM Medalla WHERE Usuario_Id = @id AND SubCategoriaMedalla_Id = '10') LIMIT 1;";
            }

            if (query != "")
            {
                string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
                MySqlConnection conexion = new MySqlConnection(connectionString);
                conexion.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = query;
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = UsernameUsuario;
                cmd.ExecuteNonQuery();
            }
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
