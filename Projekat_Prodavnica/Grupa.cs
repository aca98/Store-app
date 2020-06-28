using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_Prodavnica
{
    public class Grupa
    {
        private string naziv;
        private int id_grupa;
        private List<Artikal> id_artikla;

        public Grupa(string naziv, int idGrupa, List<Artikal> idArtikla)
        {
            this.naziv = naziv;
            id_grupa = idGrupa;
            id_artikla = idArtikla;
        }

        public Grupa(string naziv, int idGrupa)
        {
            this.naziv = naziv;
            id_grupa = idGrupa;
            id_artikla = new List<Artikal>();
        }
        public Grupa()
        {
            this.naziv ="";
            id_grupa = 0;
            id_artikla = new List<Artikal>();
        }

        public void DodajArtikal(Artikal artikal)
        {
            id_artikla.Add(artikal);
        }

        public string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        public int IdGrupa
        {
            get { return id_grupa; }
            set { id_grupa = value; }
        }

        public List<Artikal> IdArtikla
        {
            get { return id_artikla; }
            set { id_artikla = value; }
        }

        public override string ToString()
        {
            string s = "";
            foreach (Artikal artikal in id_artikla)
            {
                s += artikal + Environment.NewLine;
            }
                

            return "" + naziv + " " + id_grupa + " " + s + "";
        }
    }
}
