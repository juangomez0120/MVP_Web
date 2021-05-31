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
    public class IndicadoresLeaderboardModel : PageModel
    {
        public IList<Indicadores> ListaIndicadores { get; set; }
        public void OnGet()
        {
            ListaIndicadores = new List<Indicadores>();
            string connectionString = "Server=127.0.0.1;Port=3306;Database=DB_Gran_Escape;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select imagen.Nombre, imagen.CategoriaImg_Id, count(*) from usuario_imagen inner join imagen on usuario_imagen.Imagen_Id = imagen.Id group by Imagen_Id, Estatus having count(*)>1 and Estatus = 0;";

            Indicadores en1 = new Indicadores();
            int pos = 0;

            using (var reader = cmd.ExecuteReader())
            {
                en1 = new Indicadores();
                en1.nombre = reader["Nombre"].ToString();
                en1.tipo = Convert.ToInt32(reader["CategoriaImg_Id"]);
                en1.intentos = Convert.ToInt32(reader["count(*)"]);
                ListaIndicadores.Add(en1);
            }
            conexion.Dispose();
        }
    }
}
