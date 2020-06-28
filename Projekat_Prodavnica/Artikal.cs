using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_Prodavnica
{
    public class Artikal
    {
        private int id_artikla;
        private string naziv;
        private double cena;
        private int popust;

        public Artikal(int idArtikla, string naziv, double cena, int popust)
        {
            id_artikla = idArtikla;
            this.naziv = naziv;
            this.cena = cena;
            this.popust = popust;
        }
        public Artikal(string naziv, double cena, int popust)
        {
            this.id_artikla = -1;
            this.naziv = naziv;
            this.cena = cena;
            this.popust = popust;
        }
        public Artikal()
        {
            id_artikla = 0;
            this.naziv ="";
            this.cena = 0;
            this.popust =0;
        }

        public int IdArtikla
        {
            get { return id_artikla; }
            set { id_artikla = value; }
        }

        public string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        public double Cena
        {
            get { return cena; }
            set { cena = value; }
        }

        public int Popust
        {
            get { return popust; }
            set { popust = value; }
        }

        public override string ToString()
        {
            return "" + this.naziv + " Cena:" + this.cena + " RSD %" + this.popust;
        }
    }
}
