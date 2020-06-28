using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_Prodavnica
{
    public class KolicinaArtikla
    {
        private Artikal artikal;
        private int kolicina;

        public KolicinaArtikla(Artikal artikal, int kolicina)
        {
            this.artikal = artikal;
            this.kolicina = kolicina;
        }
        public KolicinaArtikla()
        {
            this.artikal = null;
            this.kolicina = 0;
        }

        public Artikal Artikal
        {
            get { return artikal; }
            set { artikal = value; }
        }

        public int Kolicina
        {
            get { return kolicina; }
            set { kolicina = value; }
        }

        public override string ToString()
        {
            return $"{artikal} X{kolicina}";
        }
    }
}

