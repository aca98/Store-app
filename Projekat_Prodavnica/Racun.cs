using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_Prodavnica
{
    class Racun
    {
        private int id_racun;
        private double cena;
        private DateTime datum;
        private DateTime vreme;

        public Racun(int idRacun, double cena, DateTime datum, DateTime vreme)
        {
            id_racun = idRacun;
            this.cena = cena;
            this.datum = datum;
            this.vreme = vreme;
        }
        public Racun(double cena, DateTime datum, DateTime vreme)
        {
            id_racun = 0;
            this.cena = cena;
            this.datum = datum;
            this.vreme = vreme;
        }

        public int IdRacun
        {
            get { return id_racun; }
            set { id_racun = value; }
        }

        public double Cena
        {
            get { return cena; }
            set { cena = value; }
        }

        public DateTime Datum
        {
            get { return datum; }
            set { datum = value; }
        }

        public DateTime Vreme
        {
            get { return vreme; }
            set { vreme = value; }
        }

        public override string ToString()
        {
            return "Cena: " + cena + ", Datum:" + datum + " " + vreme;
        }
    }
}
