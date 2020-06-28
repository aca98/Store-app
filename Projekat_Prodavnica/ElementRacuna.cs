using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Projekat_Prodavnica
{
    public class ElementRacuna
    {
        private Label labelArtikla;
        private KolicinaArtikla artikl;
        private Button btnPlus;
        private Button btnMinus;
        private Button btnUkloni;


        public ElementRacuna(KolicinaArtikla artikl, Button btnPlus, Button btnMinus, Button btnUkloni)
        {
            
            this.artikl = artikl;
            this.btnPlus = btnPlus;
            this.btnMinus = btnMinus;
            this.btnUkloni = btnUkloni;
            this.labelArtikla = new Label
            {
                Text = artikl.ToString(),
                Height = 20
            };
        }
        public ElementRacuna()
        {
            this.artikl = null;
            this.btnPlus = null;
            this.btnMinus = null;
            this.btnUkloni = null;
            this.labelArtikla = null;
        }
        public static ElementRacuna NadiElementPoDugmetu(List<ElementRacuna> racun, Button btn)
        {
            for (int i = 0; i < racun.Count; i++)
            {
                if (racun[i].BtnUkloni == btn)
                {
                    return racun[i];
                }
            }

            return null;
        }
        public static ElementRacuna NadiElementOdIndeks(List<ElementRacuna> racun, ElementRacuna element)
        {
            for (int i = 0; i < racun.Count; i++)
            {
                if (racun[i] == element)
                {
                    return racun[i];
                }
            }

            return null;
        }
        public static int NadiElementPoLabeli(List<ElementRacuna> racun, Label labela)
        {
            for (int i = 0; i < racun.Count; i++)
            {
                if (racun[i].labelArtikla == labela)
                {
                    return i;
                }
            }

            return 0;
        }
        public static int NadiElementPoArtiklu(List<ElementRacuna> racun, Artikal artikal)
        {
            for (int i = 0; i < racun.Count; i++)
            {
                if (racun[i].Artikl.Artikal == artikal)
                {
                    return i;
                }
            }

            return 0;
        }

        public static void DodajNaRacun()
        {

        }


        public KolicinaArtikla Artikl
        {
            get { return artikl; }
            set { artikl = value; }
        }

        public Button BtnPlus
        {
            get { return btnPlus; }
            set { btnPlus = value; }
        }

        public Button BtnMinus
        {
            get { return btnMinus; }
            set { btnMinus = value; }
        }

        public Button BtnUkloni
        {
            get { return btnUkloni; }
            set { btnUkloni = value; }
        }

        public Label LabelArtikla
        {
            get { return labelArtikla; }
            set { labelArtikla = value; }
        }

        public override string ToString()
        {
            return $"Artikl: {artikl}, BtnPlus: {btnPlus}, BtnMinus: {btnMinus}, BtnUkloni: {btnUkloni}";
        }
    }
}
