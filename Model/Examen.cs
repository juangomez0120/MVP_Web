using System;
using System.Collections.Generic;

namespace MVP_Web.Model
{
    public class Examen
    {
        public int num { get; set; }
        public string mes { get; set; }
        public string examen { get; set; }
        public int score { get; set; }

        public Examen(int num, string mes, string examen, double score)
        {
            this.num = num;
            this.mes = convierteMes(mes);
            this.examen = examen;
            this.score = (int)score;
        }

        private string convierteMes(string mes)
        {
            Dictionary<string, string> meses = new Dictionary<string, string>();
            meses.Add("Jan", "Enero");
            meses.Add("Feb", "Febrero");
            meses.Add("Mar", "Marzo");
            meses.Add("Apr", "Abril");
            meses.Add("May", "Mayo");
            meses.Add("Jun", "Junio");
            meses.Add("Jul", "Julio");
            meses.Add("Aug", "Agosto");
            meses.Add("Sep", "Septiembre");
            meses.Add("Oct", "Octubre");
            meses.Add("Nov", "Noviembre");
            meses.Add("Dic", "Diciembre");

            return meses[mes[..3]] + " " + mes[3..];
        }
    }
}
