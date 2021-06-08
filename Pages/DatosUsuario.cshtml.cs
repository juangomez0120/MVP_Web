using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
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
        public string MensajeError { get; set; }

        //Agarrar primera letra de NombreUsuario y ApellidoUsuario y mandarlas a pagina de Layout

        //Mandar NombreUsuario y ApellidoUsuario a página de perfil

        private static string PrimeraLetraMayuscula(string str) {
            return char.ToUpper(str[0]) + str[1..];
        }

        public IActionResult OnGet()
        {
            MensajeError = "";

            if(HttpContext.Session.GetString("SessionUsername") == null || HttpContext.Session.GetString("SessionUsername") == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string usernameUsuario = HttpContext.Session.GetString("SessionUsername");

            if(usernameUsuario == null || usernameUsuario == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            if (NombreUsuario == null || ApellidoUsuario == null)
            {
                MensajeError = "Credenciales Inválidas";
                return Page();
            }

            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            var query1 = "INSERT INTO Usuario(Id, Nombre, Apellido) VALUES(@id, @nombre, @apellido)";
            var query2 = "INSERT INTO Bitacora(Usuario_Id, Fecha) VALUES(@id, @fecha)";

            var cmd = new MySqlCommand(query1, conexion);

            string nombreCaps = PrimeraLetraMayuscula(NombreUsuario);
            string apellidoCaps = PrimeraLetraMayuscula(ApellidoUsuario);

            cmd.Parameters.Add("@id",MySqlDbType.VarChar).Value = usernameUsuario;
            cmd.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombreCaps;
            cmd.Parameters.Add("@apellido", MySqlDbType.VarChar).Value = apellidoCaps;
            cmd.ExecuteNonQuery();

            cmd.CommandText = query2;
            cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.ExecuteNonQuery();

            HttpContext.Session.SetString("SessionNombre", nombreCaps);
            HttpContext.Session.SetString("SessionApellido", apellidoCaps);

            return RedirectToPage("menu");
        }
    }
}