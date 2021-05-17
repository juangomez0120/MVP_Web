using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MVP_Web.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MVP_Web.Pages
{
    public class DatosUsuarioModel : PageModel
    {
        [BindProperty]
        public string NombreUsuario { get; set; }
        [BindProperty]
        public string ApellidoUsuario { get; set; }

        //Agarrar primera letra de NombreUsuario y ApellidoUsuario y mandarlas a pagina de Layout

        //Mandar NombreUsuario y ApellidoUsuario a página de perfil

        private static string PrimeraLetraMayuscula(string str) { // REVISAR (nombres/apellidos con más de 1 palabra)
            return char.ToUpper(str[0]) + str[1..];
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            Login Usuario = new Login();
            string value = HttpContext.Session.GetString("SessionUsuario");

            if (value != null)
            {
                Usuario = JsonConvert.DeserializeObject<Login>(value);
            }

            Usuario.nombre = NombreUsuario;
            Usuario.apellido = ApellidoUsuario;

            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            var query1 = "INSERT INTO Usuario(Id, Nombre, Apellido) VALUES(@id, @nombre, @apellido)";
            var query2 = "INSERT INTO Bitacora(Usuario_Id, Fecha) VALUES(@id, @fecha)";

            var cmd = new MySqlCommand(query1, conexion);

            cmd.Parameters.Add("@id",MySqlDbType.VarChar).Value = Usuario.username;
            cmd.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = PrimeraLetraMayuscula(Usuario.nombre);
            cmd.Parameters.Add("@apellido", MySqlDbType.VarChar).Value = PrimeraLetraMayuscula(Usuario.apellido);
            cmd.ExecuteNonQuery();

            cmd.CommandText = query2;
            cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.ExecuteNonQuery();

            HttpContext.Session.SetString("SessionUsuario", JsonConvert.SerializeObject(Usuario));

            return RedirectToPage("menu");
        }
    }
}