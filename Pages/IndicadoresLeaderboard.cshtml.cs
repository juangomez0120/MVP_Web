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
    public class IndicadoresLeaderboardModel : PageModel
    {
        public IList<Indicadores> ListaIndicadores { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("SessionUsername") == null || HttpContext.Session.GetString("SessionUsername") == "")
            {
                return RedirectToPage("ExpiracionSesion");
            }

            ListaIndicadores = new List<Indicadores>();
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select imagen.Nombre, categoriaimg.NombreCategoria, count(*) from usuario_imagen inner join imagen on usuario_imagen.Imagen_Id = imagen.Id inner join categoriaimg on imagen.CategoriaImg_Id = categoriaimg.Id group by Imagen_Id, Estatus having(count(*)> 1 and Estatus = 0) or Estatus = 0 order by count(*) desc limit 10;";

            Indicadores en1 = new Indicadores();
            int pos = 1;

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    en1 = new Indicadores();
                    en1.posicion = pos;
                    en1.nombre = reader["Nombre"].ToString();
                    en1.tipo = reader["NombreCategoria"].ToString();
                    en1.intentos = Convert.ToInt32(reader["count(*)"]);
                    ListaIndicadores.Add(en1);
                    pos++;
                }
            }
            conexion.Dispose();

            return Page();
        }
    }
}
