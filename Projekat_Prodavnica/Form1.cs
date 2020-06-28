using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Threading;
using Projekat_Prodavnica.ProdavnicaDataSetTableAdapters;
using Button = System.Windows.Forms.Button;

namespace Projekat_Prodavnica
{
    public partial class Form1 : Form
    {
        private Baza baza;
        private List<Button> dugmicGrupe;
        private List<Grupa> grupe;
        private List<ElementRacuna> racun;
        private ElementRacuna rEelement;
        private Thread t;
        private Button FokusiranoDugme;

        public delegate void OsveziBazu();
        public Form1()
        {
           
            baza = new Baza();
            InitializeComponent();
            rEelement = null;
            dugmicGrupe = new List<Button>();
            grupe = new List<Grupa>();
            racun = new List<ElementRacuna>();
            lblUkupanIznos.TextChanged += txtUplata_TextChanged;
            t = new Thread(OsvezavanjeProizvoda);
            t.IsBackground = true;
            t.Start();
            NapraviDugmice();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lbxGrupa.DisplayMember = "naziv";
            cbxGrupa.DisplayMember = "naziv";
            cbxGrupa.DisplayMember = "naziv";
            cbxGrupaArtikla.DisplayMember = "naziv";



        }
        public void OsvezavanjeProizvoda()
        {
            while (true)
            {
                Podaci();
                if (FokusiranoDugme != null)
                {
                    NadiGrupu(FokusiranoDugme.Name, lstBoxProizvodi);
                }
                this.Invoke(new OsveziBazu(NapraviDugmice));
                Thread.Sleep(120000);
            }
        }

        public void Podaci()
        {
            grupe = baza.CitajGrupe();
            lbxGrupa.DataSource = baza.CitajGrupe();
            PunicbxGroup(baza.CitajGrupe());
            lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
            if (cbxGrupa.SelectedItem != null)
            {
                lbxArtikli.DataSource = baza.CitajArtikleGrupeVan(((Grupa)cbxGrupa.SelectedItem).IdGrupa);
            }
        }

        public void DodajNaRacun(KolicinaArtikla kol)
        {
            ElementRacuna element;
            bool ElementNaden = false;
            int left = panel2.ClientRectangle.Left;
            int top = panel2.ClientRectangle.Top;
            int br = 0;
            if (racun.Count != 0)
            {
                foreach (ElementRacuna ele in racun)
                {
                    top += ele.LabelArtikla.Height;
                }
            }
            Label lbl = new Label
            {
                Text = kol.ToString(),
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 10),
                Left = left,
                Top = top,
                Width = panel2.ClientSize.Width - 45,
                Height = 20
            };
            lbl.Click += LabelaClick;
            lbl.DoubleClick += LabelaDoubleClick;
            top += 20;
            Button btnUkloni = new Button
            {
                Width = 15,
                Height = lbl.Height,
                Text = "X",
                Left = panel2.ClientRectangle.Right - 45,
                Top = lbl.Top
            };
            btnUkloni.Click += LblBtnUkloniKlik;
            Button btnPlus = new Button
            {
                Width = 15,
                Height = lbl.Height,
                Text = "+",
                Left = btnUkloni.Left + btnUkloni.Width,
                Top = lbl.Top
            };
            btnPlus.Click += LblBtnPlusiKlik;
            Button btnMinus = new Button
            {
                Width = 15,
                Height = lbl.Height,
                Text = "-",
                Left = btnPlus.Left + btnPlus.Width,
                Top = lbl.Top
            };
            btnMinus.Click += LblBtnMinusKlik;

            element = new ElementRacuna(kol, btnPlus, btnMinus, btnUkloni);
            element.LabelArtikla = lbl;


            if (racun.Count != 0)
            {
                foreach (ElementRacuna elementRacuna in racun)
                {
                    if (elementRacuna.Artikl.Artikal.IdArtikla == element.Artikl.Artikal.IdArtikla)
                    {
                        elementRacuna.Artikl.Kolicina++;
                        elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                        SelekcijaRacuna(elementRacuna.LabelArtikla);
                        ElementNaden = true;
                        break;
                    }
                }
            }

            if (!ElementNaden)
            {
                panel2.Controls.Add(element.BtnMinus);
                panel2.Controls.Add(element.BtnPlus);
                panel2.Controls.Add(element.BtnUkloni);
                panel2.Controls.Add(lbl);
                racun.Add(element);
                SelekcijaRacuna(racun[racun.Count - 1].LabelArtikla);
            }
            UkupanIznos();
        }

        public void AzuriranjeRacuna()
        {
            List<Artikal> art = baza.CitajArtikle();
            foreach (Artikal artikal in art)
            {
                foreach (ElementRacuna elementRacuna in racun)
                {
                    if (elementRacuna.Artikl.Artikal.IdArtikla == artikal.IdArtikla)
                    {
                        elementRacuna.Artikl.Artikal = artikal;
                        elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                       
                    }
                }
            }
            OsveziRacun(racun);
            UkupanIznos();
        }

        

        public void ObrisiDugmiceGrupe()
        {
            foreach (Button button in dugmicGrupe)
            {
                panel1.Controls.Remove(button);
            }
            dugmicGrupe.Clear();
        }

        public void NapraviDugmice()
        {
            ObrisiDugmiceGrupe();
            int left = panel1.ClientRectangle.Left;
            int top = panel1.ClientRectangle.Top;
            int br = 0;

            foreach (Grupa grupa in grupe)
            {
                br++;
                Button btn = new Button
                {
                    Width = (panel1.ClientSize.Width / 2) - 10,
                    Font = new Font(new FontFamily("Microsoft Sans Serif"), 10),
                    Height = 100,
                    BackColor = Color.LightGray,
                    Text = grupa.Naziv,
                    Left = left,
                    Top = top,
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                    Name = grupa.IdGrupa.ToString()
                };
                btn.Click += NaKlik;
                if (br == 2)
                {
                    left = panel1.ClientRectangle.Left;
                    top += btn.Height;
                    br = 0;
                }
                else
                {
                    left += btn.Width;
                }
                dugmicGrupe.Add(btn);
                panel1.Controls.Add(btn);
            }
        }
        public void PretrazeniDugmici(List<Grupa> grupe)
        {
            ObrisiDugmiceGrupe();
            int left = panel1.ClientRectangle.Left;
            int top = panel1.ClientRectangle.Top;
            int br = 0;

            foreach (Grupa grupa in grupe)
            {
                br++;
                Button btn = new Button
                {
                    Width = (panel1.ClientSize.Width / 2) - 10,
                    Font = new Font(new FontFamily("Microsoft Sans Serif"), 10),
                    Height = 100,
                    BackColor = Color.LightGray,
                    Text = grupa.Naziv,
                    Left = left,
                    Top = top,
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                    Name = grupa.IdGrupa.ToString()
                };
                btn.Click += NaKlik;
                if (br == 2)
                {
                    left = panel1.ClientRectangle.Left;
                    top += btn.Height;
                    br = 0;
                }
                else
                {
                    left += btn.Width;
                }
                dugmicGrupe.Add(btn);
                panel1.Controls.Add(btn);
            }
        }


        public void obrisiDugmiceRacuna()
        {
            foreach (ElementRacuna element in racun)
            {
                panel2.Controls.Remove(element.BtnPlus);
                panel2.Controls.Remove(element.BtnMinus);
                panel2.Controls.Remove(element.BtnUkloni);
                panel2.Controls.Remove(element.LabelArtikla);
            }
        }

        public void UkupanIznos()
        {
            int suma = 0;
            int cenaPopust = 0;
            foreach (ElementRacuna elementRacuna in racun)
            {
                if (elementRacuna.Artikl.Artikal.Popust != 0)
                {
                    cenaPopust = (int)elementRacuna.Artikl.Artikal.Cena - (int)(elementRacuna.Artikl.Artikal.Cena / (100 / elementRacuna.Artikl.Artikal.Popust));
                }
                else
                {
                    cenaPopust = (int)elementRacuna.Artikl.Artikal.Cena;
                }
                suma += cenaPopust * elementRacuna.Artikl.Kolicina;
            }
            lblUkupanIznos.Text = "" + suma + " RSD";
        }

        public void NaKlik(object sender, EventArgs e)
        {
            NadiGrupu(((Button)sender).Name, lstBoxProizvodi);
            FokusiranoDugme = ((Button)sender);
        }

        public void LblBtnUkloniKlik(object sender, EventArgs e)
        {
            int left = panel2.ClientRectangle.Left;
            int top = panel2.ClientRectangle.Top;

            ElementRacuna element = ElementRacuna.NadiElementPoDugmetu(racun, (Button)sender);
            panel2.Controls.Remove(element.BtnPlus);
            panel2.Controls.Remove(element.BtnMinus);
            panel2.Controls.Remove(element.BtnUkloni);
            panel2.Controls.Remove(element.LabelArtikla);
            racun.Remove(element);

            foreach (ElementRacuna elementRacuna in racun)
            {
                elementRacuna.LabelArtikla.Top = top;
                elementRacuna.BtnMinus.Top = top;
                elementRacuna.BtnPlus.Top = top;
                elementRacuna.BtnUkloni.Top = top;
                panel2.Controls.Add(elementRacuna.BtnPlus);
                panel2.Controls.Add(elementRacuna.BtnMinus);
                panel2.Controls.Add(elementRacuna.BtnUkloni);
                panel2.Controls.Add(elementRacuna.LabelArtikla);
                top += elementRacuna.LabelArtikla.Height;
            }
            UkupanIznos();
        }
        public void LblBtnMinusKlik(object sender, EventArgs e)
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                if (elementRacuna.BtnMinus.Equals((Button)sender))
                {
                    if (elementRacuna.Artikl.Kolicina != 1)
                    {
                        elementRacuna.Artikl.Kolicina--;
                        elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                        break;
                    }
                }
            }
            UkupanIznos();
        }
        public void LblBtnPlusiKlik(object sender, EventArgs e)
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                if (elementRacuna.BtnPlus == (Button)sender)
                {
                    elementRacuna.Artikl.Kolicina++;
                    elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                    break;
                }
            }
            UkupanIznos();
        }
        public void OsveziRacun(List<ElementRacuna> racun)
        {
            OcistiRacun(racun);
            foreach (ElementRacuna elementRacuna in racun)
            {
                panel2.Controls.Add(elementRacuna.BtnPlus);
                panel2.Controls.Add(elementRacuna.BtnMinus);
                panel2.Controls.Add(elementRacuna.BtnUkloni);
                panel2.Controls.Add(elementRacuna.LabelArtikla);
            }

        }

        public void OcistiRacun(List<ElementRacuna> racun)
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                panel2.Controls.Remove(elementRacuna.BtnPlus);
                panel2.Controls.Remove(elementRacuna.BtnMinus);
                panel2.Controls.Remove(elementRacuna.BtnUkloni);
                panel2.Controls.Remove(elementRacuna.LabelArtikla);
            }
        }
        public void ResetujRacun()
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                panel2.Controls.Remove(elementRacuna.BtnPlus);
                panel2.Controls.Remove(elementRacuna.BtnMinus);
                panel2.Controls.Remove(elementRacuna.BtnUkloni);
                panel2.Controls.Remove(elementRacuna.LabelArtikla);
            }
            racun.Clear();
        }

        public void NadiGrupu(string idgrupe, ListBox lbx)
        {
            //this.Invoke(new OsveziBazu(lstBoxProizvodi.Items.Clear));
            foreach (Grupa grupa in grupe)
            {
                if (grupa.IdGrupa == int.Parse(idgrupe))
                {
                    lbx.DataSource = grupa.IdArtikla;
                    //foreach (Artikal artikal in grupa.IdArtikla)
                    //{
                    //    if (!lstBoxProizvodi.Items.Contains(artikal))
                    //    {
                    //        lstBoxProizvodi.Items.Add(artikal);
                    //    }
                    //}

                }
            }
        }

        public void LabelaDoubleClick(object sender, EventArgs e)
        {
            int left = panel2.ClientRectangle.Left;
            int top = panel2.ClientRectangle.Top;

            panel2.Controls.Remove((Label)sender);
            for (int i = 0; i < racun.Count; i++)
            {
                if (racun[i].LabelArtikla == (Label)sender)
                {
                    panel2.Controls.Remove(racun[i].BtnPlus);
                    panel2.Controls.Remove(racun[i].BtnMinus);
                    panel2.Controls.Remove(racun[i].BtnUkloni);

                    racun.Remove(racun[i]);
                    break;
                }
            }
            foreach (ElementRacuna elementRacuna in racun)
            {
                elementRacuna.LabelArtikla.Top = top;
                elementRacuna.BtnMinus.Top = top;
                elementRacuna.BtnPlus.Top = top;
                elementRacuna.BtnUkloni.Top = top;
                panel2.Controls.Add(elementRacuna.BtnPlus);
                panel2.Controls.Add(elementRacuna.BtnMinus);
                panel2.Controls.Add(elementRacuna.BtnUkloni);
                panel2.Controls.Add(elementRacuna.LabelArtikla);
                top += elementRacuna.LabelArtikla.Height;
            }
            UkupanIznos();
        }


        public void SelekcijaRacuna(object objekat)
        {
            foreach (ElementRacuna element in racun)
            {
                element.LabelArtikla.BackColor = Color.WhiteSmoke;
            }

            ((Label)objekat).BackColor = ColorTranslator.FromHtml("#85d2fb");
            rEelement = racun[ElementRacuna.NadiElementPoLabeli(racun, (Label)objekat)];
        }

        public void LabelaClick(object sender, EventArgs e)
        {
            SelekcijaRacuna(sender);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int kolicina = 0;
            if (lstBoxProizvodi.SelectedItem != null)
            {
                if (txtBoxKolicina.Text == "")
                {
                    DodajNaRacun(new KolicinaArtikla((Artikal)lstBoxProizvodi.SelectedItem, 1));
                }
                else if (int.TryParse(txtBoxKolicina.Text, out kolicina))
                {
                    DodajNaRacun(new KolicinaArtikla((Artikal)lstBoxProizvodi.SelectedItem, kolicina));
                }
                else
                {
                    MessageBox.Show("NIje unet broj u Dodaj Kolicinu");
                }
            }
            else
            {
                MessageBox.Show("Niste selektovali proizvod");
            }
            UkupanIznos();
        }

        private void lstBoxProizvodi_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            DodajNaRacun(new KolicinaArtikla((Artikal)lstBoxProizvodi.SelectedItem, 1));

        }

        private void lstBoxRacun_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            obrisiDugmiceRacuna();
        }
        private void btnOduzmi_Click(object sender, EventArgs e)
        {
            int kolicina = 0;
            int i = racun.IndexOf(rEelement);
            if (lstBoxProizvodi.SelectedItem != null)
            {
                if (txtBoxKolicina.Text == "")
                {
                    if (!((racun[i].Artikl.Kolicina - 1) < 1))
                    {
                        racun[i].Artikl.Kolicina -= 1;
                        racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                    }
                    else
                    {
                        racun[i].Artikl.Kolicina = 1;
                        racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                    }
                }
                else if (int.TryParse(txtBoxKolicina.Text, out kolicina))
                {
                    if (!((racun[i].Artikl.Kolicina - kolicina) < 1))
                    {
                        racun[i].Artikl.Kolicina -= kolicina;
                        racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                    }
                    else
                    {
                        racun[i].Artikl.Kolicina = 1;
                        racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                    }
                }
                else
                {
                    MessageBox.Show("NIje unet broj u Dodaj Kolicinu");
                }
            }
            else
            {
                MessageBox.Show("Niste selektovali proizvod");
            }

            UkupanIznos();
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            int kolicina = 0;
            int i = racun.IndexOf(rEelement);
            if (lstBoxProizvodi.SelectedItem != null)
            {
                if (txtBoxKolicina.Text == "")
                {
                    racun[i].Artikl.Kolicina++;
                    racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                }
                else if (int.TryParse(txtBoxKolicina.Text, out kolicina))
                {
                    racun[i].Artikl.Kolicina += kolicina;
                    racun[i].LabelArtikla.Text = racun[i].Artikl.ToString();
                }
                else
                {
                    MessageBox.Show("NIje unet broj u Dodaj Kolicinu");
                }
            }
            else
            {
                MessageBox.Show("Niste selektovali proizvod");
            }

            UkupanIznos();
        }

        public bool ProveraUplate()
        {
            int uplata = 0;
            int ukupanIznos = int.Parse(lblUkupanIznos.Text.Replace("RSD", "").Trim());
            if (int.TryParse(txtUplata.Text, out uplata))
            {
                if (uplata >= ukupanIznos)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Uneli ste slovo umesto broja");
                return false;
            }
        }

        public void OduzmiSve(int broj)
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                if (!((elementRacuna.Artikl.Kolicina - broj) < 1))
                {
                    elementRacuna.Artikl.Kolicina -= broj;
                    elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                }
                else
                {
                    elementRacuna.Artikl.Kolicina = 1;
                    elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
                }
            }
            UkupanIznos();
        }
        public void PovecajSve(int broj)
        {
            foreach (ElementRacuna elementRacuna in racun)
            {
                elementRacuna.Artikl.Kolicina += broj;
                elementRacuna.LabelArtikla.Text = elementRacuna.Artikl.ToString();
            }
            UkupanIznos();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (racun.Count != 0)
            {
                DialogResult rez = MessageBox.Show("Dali ste sigurni da zelite da obrisete sve sa racuna", "Upozorenje",
                    MessageBoxButtons.YesNo);
                if (rez == DialogResult.Yes)
                {
                    obrisiDugmiceRacuna();
                    racun.Clear();
                    UkupanIznos();
                }
            }
            else
            {
                MessageBox.Show("Racun je prazan");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PovecajSve(1);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            PovecajSve(5);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OduzmiSve(1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OduzmiSve(5);
        }

        public string IzvestajRacuna()
        {
            string izvestaj = "" + Environment.NewLine;
            foreach (ElementRacuna elementRacuna in racun)
            {
                izvestaj += elementRacuna.LabelArtikla.Text + Environment.NewLine;
            }

            return izvestaj;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (racun.Count != 0)
            {
                if (txtUplata.Text != "")
                {
                    if (ProveraUplate())
                    {
                        baza.UnesiRacun(new Racun(int.Parse(lblUkupanIznos.Text.Replace("RSD", "").Trim()),
                            DateTime.Now.Date,
                            DateTime.Now));
                        MessageBox.Show("Racun uspesno izdat" + "\n KUSUR:" + txtKusur.Text + " RSD\n Racun:" +
                                        IzvestajRacuna());
                        ResetujRacun();
                        lblUkupanIznos.Text = "0 RSD";
                        txtUplata.Text = "";
                        txtKusur.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Nedovjno novca");
                    }
                }
                else
                {
                    MessageBox.Show("Polje za uplatu je prazno");
                }
            }
            else
            {
                MessageBox.Show("Niste dodali nista na racun");
            }
        }

        private void txtUplata_TextChanged(object sender, EventArgs e)
        {
            int uplata = 0;

            int ukupanIznos = int.Parse(lblUkupanIznos.Text.Replace("RSD", "").Trim());
            if (int.TryParse(txtUplata.Text, out uplata))
            {
                if (uplata > ukupanIznos)
                {
                    txtKusur.Text = "" + (uplata - ukupanIznos);
                }
                else if (uplata == ukupanIznos)
                {
                    txtKusur.Text = "" + 0;
                }
                else
                {
                    txtKusur.Text = "X";
                }
            }
        }

        private void lbxGrupa_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNazivGrupe.Text = ((Grupa)lbxGrupa.SelectedItem).Naziv;
        }

        public bool ProveraArtikla(string naziv)
        {
            List<Artikal> artikali = baza.CitajArtikle();
            foreach (Artikal artikal in artikali)
            {
                try
                {
                    if (artikal.Naziv.ToLower().Equals(naziv.ToLower().Trim()) && artikal.Cena == double.Parse(txtCenaArtikla.Text.Trim()) && artikal.Popust == int.Parse(txtPopustArtikla.Text.Trim()))
                    {
                        MessageBox.Show("Artikal vec postoji");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Nije unet broj u polje za cenu ili popust");
                }
            }

            return true;
        }

        public bool ProveraGrupa()
        {
            List<Grupa> grupe = baza.CitajGrupe();
            foreach (Grupa grupa in grupe)
            {
                if (grupa.Naziv.ToLower().Equals(txtNazivGrupe.Text.Trim().ToLower()))
                {
                    MessageBox.Show("Grupa vec postoji");
                    return false;

                }
            }

            return true;
        }
        public bool ProveraGrupaArtikal(Grupa grp, Artikal artikal)
        {
            List<Grupa> grupe = baza.CitajGrupe();
            foreach (Grupa grupa in grupe)
            {
                if (grp.Naziv.Equals(grupa.Naziv))
                {
                    foreach (Artikal artikal1 in grupa.IdArtikla)
                    {
                        if (artikal1.IdArtikla == artikal.IdArtikla)
                        {
                            MessageBox.Show("Artikal se vec nalazi u grupi");
                            return false;
                        }

                    }
                }
            }
            return true;
        }
        public bool ProveraDaliPripadaGrupi(Artikal artikal)
        {
            List<Grupa> grupe = baza.CitajGrupe();
            foreach (Grupa grupa in grupe)
            {
                foreach (Artikal artikal1 in grupa.IdArtikla)
                {
                    if (artikal1.IdArtikla == artikal.IdArtikla)
                    {
                        return false;
                    }

                }
            }
            return true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult rez;
            if (lbxArtikli.SelectedItem != null)
            {
                rez = MessageBox.Show("Dali ste sigurni da zelite da obrisete artikal", "Obavestenje",
                    MessageBoxButtons.YesNo);
                if (rez == DialogResult.Yes)
                {
                    baza.ObrisiArtikal(((Artikal)lbxArtikli.SelectedItem).IdArtikla);
                    AkoJeArtiliBezGrupe();
                    lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                    grupe = baza.CitajGrupe();
                    lblGrupa.Text = ((Grupa)cbxGrupa.SelectedItem).Naziv;
                    FokusiranoDugmeVrednost();

                }

            }
        }

        public void AkoJeArtiliBezGrupe()
        {
            List<Grupa> grp = baza.CitajGrupe();
            foreach (Grupa gru in grp)
            {
                if (((Grupa)cbxGrupa.SelectedItem).IdGrupa == gru.IdGrupa)
                {
                    lbxArtikli.DataSource = gru.IdArtikla;
                    lblGrupa.Text = gru.Naziv;
                }
            }
        }

        public void CbxGrupaDodela()
        {
            cbxGrupaArtikla.DataSource = baza.CitajGrupe();
            cbxGrupaArtikla.Items.Add("Artikli bez grupe");
        }

        public void FokusiranoDugmeVrednost()
        {
            if (FokusiranoDugme != null)
            {
                lstBoxProizvodi.DataSource = baza.CitajArtikleGrupeVan(int.Parse(FokusiranoDugme.Name.Trim()));
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (lbxArtikli.SelectedItem != null)
            {
                if (cbxGrupaArtikla.SelectedItem != null)
                {
                    if (ProveraGrupaArtikal(((Grupa)cbxGrupaArtikla.SelectedItem), ((Artikal)lbxArtikli.SelectedItem)))
                    {
                        baza.AzurirajArtikaluGrupu(((Grupa)cbxGrupaArtikla.SelectedItem),
                            ((Artikal)lbxArtikli.SelectedItem).IdArtikla);

                        grupe = baza.CitajGrupe();
                        AkoJeArtiliBezGrupe();
                        cbxGrupa.SelectedItem = "" + cbxGrupaArtikla.Text;
                        lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                        FokusiranoDugmeVrednost();
                        lblGrupa.Text = cbxGrupaArtikla.Text;

                    }
                }
                else
                {
                    MessageBox.Show("Niste selektovali grupu");
                }
            }
            else
            {
                MessageBox.Show("Niste selektovali artikal");
            }
        }

        private void lbxArtikli_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNazivArtikla.Text = ((Artikal)lbxArtikli.SelectedItem).Naziv;
            txtCenaArtikla.Text = ((Artikal)lbxArtikli.SelectedItem).Cena.ToString();
            txtPopustArtikla.Text = ((Artikal)lbxArtikli.SelectedItem).Popust.ToString();

            cbxGrupaArtikla.DataSource = baza.CitajGrupe();

            foreach (Grupa grupa in cbxGrupaArtikla.Items)
            {
                foreach (Artikal artikal in grupa.IdArtikla)
                {
                    if (artikal.IdArtikla == ((Artikal)lbxArtikli.SelectedItem).IdArtikla)
                    {
                        cbxGrupaArtikla.SelectedIndex = cbxGrupaArtikla.Items.IndexOf(grupa);
                        break;

                    }
                }
            }
        }



        private void btnDodajDugme_Click(object sender, EventArgs e)
        {
            if (txtNazivGrupe.Text != "")
            {

                if (lbxArtBez.SelectedItem != null)
                {
                    if (!cbxDodajUGrupu.Checked)
                    {
                        if (ProveraGrupa())
                        {
                            baza.DodajGrupu(txtNazivGrupe.Text, ((Artikal)lbxArtBez.SelectedItem).IdArtikla);
                            OsvezavanjeDUgmicaGrupe();
                            

                        }
                    }
                    else
                    {
                        if (lbxGrupa.SelectedItem != null)
                        {
                            if (ProveraGrupaArtikal(((Grupa)lbxGrupa.SelectedItem),
                                ((Artikal)lbxArtBez.SelectedItem)))
                            {
                                baza.AzurirajArtikaluGrupu(((Grupa)lbxGrupa.SelectedItem),
                                    ((Artikal)lbxArtBez.SelectedItem).IdArtikla);

                                lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                                cbxGrupaArtikla.DataSource = baza.CitajGrupe();
                                grupe = baza.CitajGrupe();
                                ObrisiDugmiceGrupe();
                                NapraviDugmice();

                                OsvezavanjeDodajArtikalGrupu();
                                AkoJeArtiliBezGrupe();

                                FokusiranoDugmeVrednost();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Niste selektovali grupu");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Niste selektovali artikal");
                }
            }
            else
            {
                MessageBox.Show("Polje za naziv grupe je prazno");
            }

        }

        public void OsvezavanjeDUgmicaGrupe()
        {

            lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
            cbxGrupaArtikla.DataSource = baza.CitajGrupe();
            PunicbxGroup(baza.CitajGrupe());
            lbxGrupa.DataSource = baza.CitajGrupe();
            grupe = baza.CitajGrupe();
            ObrisiDugmiceGrupe();
            NapraviDugmice();
        }
        public void OsvezavanjeDodajArtikalGrupu()
        {

            lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
            cbxGrupaArtikla.DataSource = baza.CitajGrupe();
            grupe = baza.CitajGrupe();
            ObrisiDugmiceGrupe();
            NapraviDugmice();
        }
        private void btnAzurirajGrupu_Click(object sender, EventArgs e)
        {
            if (txtNazivGrupe.Text != "")
            {
                if (ProveraGrupa())
                {
                    baza.AzurirajGrupu(txtNazivGrupe.Text, ((Grupa)lbxGrupa.SelectedItem).IdGrupa);
                    OsvezavanjeDUgmicaGrupe();
                }
            }
            else
            {
                MessageBox.Show("Polje za naziv grupe je prazno");
            }
        }

        private void btnObrisiGrupu_Click(object sender, EventArgs e)
        {
            if (lbxGrupa.SelectedItem != null)
            {
                DialogResult rez = MessageBox.Show("Dali zelite da obrises sve artikle u ovoj grupi", "Obavestenje", MessageBoxButtons.YesNoCancel);
                if (rez == DialogResult.Yes)
                {
                    foreach (Grupa grupa in grupe)
                    {
                        if (grupa.IdGrupa == ((Grupa)lbxGrupa.SelectedItem).IdGrupa)
                        {
                            foreach (Artikal artikal in grupa.IdArtikla)
                            {
                                baza.ObrisiArtikal(artikal.IdArtikla);
                            }
                        }
                    }
                    baza.ObrisiGrupu(((Grupa)lbxGrupa.SelectedItem).IdGrupa.ToString());
                    OsvezavanjeDUgmicaGrupe();
                }

                if (rez == DialogResult.No)
                {

                    if (((Grupa)cbxGrupa.SelectedItem).IdGrupa == ((Grupa)lbxGrupa.SelectedItem).IdGrupa)
                    {
                        List<Artikal> prazan = new List<Artikal>();
                        cbxGrupa.Text = "";
                        lbxArtikli.DataSource = prazan;
                    }
                    baza.ObrisiGrupu(((Grupa)lbxGrupa.SelectedItem).IdGrupa.ToString());
                    OsvezavanjeDUgmicaGrupe();
                }
            }
        }
        private void cbxGrupa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxGrupa.SelectedItem != null)
            {
                if (cbxGrupa.Text == "Artikli bez grupe")
                {
                    lbxArtikli.DataSource = baza.CitajArtikleBezGrupa();
                    lblGrupa.Text = "Artikli bez grupe";
                }
                else
                {
                    lbxArtikli.DataSource = baza.CitajArtikleGrupeVan(((Grupa)cbxGrupa.SelectedItem).IdGrupa);
                    lblGrupa.Text = ((Grupa)cbxGrupa.SelectedItem).Naziv;
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            lbxArtikli.DataSource = baza.CitajArtikleBezGrupa();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (txtNazivArtikla.Text != "" && txtCenaArtikla.Text != "" && txtPopustArtikla.Text != "")
            {
                try
                {
                    if (ProveraArtikla(txtNazivArtikla.Text))
                    {
                        baza.DodajArtikal(new Artikal(txtNazivArtikla.Text, double.Parse(txtCenaArtikla.Text),
                          int.Parse(txtPopustArtikla.Text)));
                        grupe = baza.CitajGrupe();
                        FokusiranoDugmeVrednost();
                        AkoJeArtiliBezGrupe();
                        lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                       
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Niste uneli broj u polje za cenu ili popust");
                }

            }
            else
            {
                MessageBox.Show("Prazna polja");
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (lbxArtikli.SelectedItem != null)
            {
                if (txtNazivArtikla.Text != "" && txtCenaArtikla.Text != "" && txtPopustArtikla.Text != "")
                {
                    if (ProveraArtikla(txtNazivArtikla.Text))
                    {
                        try
                        {
                            baza.AzurirajArtikal(((Artikal)lbxArtikli.SelectedItem).IdArtikla, txtNazivArtikla.Text, double.Parse(txtCenaArtikla.Text), int.Parse(txtPopustArtikla.Text));
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("Niste uneli broj u polje cena ili popust");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Neka od polja za unos artikla su prazna ");
                }
               
                lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                grupe = baza.CitajGrupe();
                OsvezavanjeDodajArtikalGrupu();
                AkoJeArtiliBezGrupe();
                FokusiranoDugmeVrednost();
                AzuriranjeRacuna();
            }
            else
            {
                MessageBox.Show("Niste selektovali artikal");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (!ProveraDaliPripadaGrupi(((Artikal)lbxArtikli.SelectedItem)))
            {
                DialogResult rez = MessageBox.Show("Dali ste sigurni da zelite da obrise artikal iz grupe",
                    "Upozorenje",
                    MessageBoxButtons.YesNo);
                if (rez == DialogResult.Yes)
                {
                    baza.ObrisiArtikaluGrupi(((Artikal)lbxArtikli.SelectedItem).IdArtikla);
                   
                    grupe = baza.CitajGrupe();
                    lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                    lbxArtikli.DataSource = baza.CitajArtikleGrupeVan(((Grupa) cbxGrupa.SelectedItem).IdGrupa);
                    FokusiranoDugmeVrednost();
                }
            }
            else
            {
                MessageBox.Show("Artikal ne pripada ni jednoj grupi");
            }
        }

        public void PunicbxGroup(List<Grupa> grp)
        {

            cbxGrupa.DataSource = baza.CitajGrupe();

        }
        private void button8_Click_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = baza.CitajRacun(Od.Value, Do.Value);
            try
            {
                dataGridView1.Columns[3].DefaultCellStyle.Format = "HH:mm:ss";
            }
            catch (Exception exception)
            {
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<Grupa> grp = new List<Grupa>();
            NapraviDugmice();
            foreach (Grupa button in grupe)
            {
                if (button.Naziv.ToLower().Contains(textBox1.Text.ToLower()))
                {
                    grp.Add(button);
                }

            }
            PretrazeniDugmici(grp);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            List<Artikal> art = new List<Artikal>();
            if (lstBoxProizvodi.Items != null)
            {
                if (textBox2.Text != "")
                {
                    NadiGrupu(FokusiranoDugme.Name, lstBoxProizvodi);
                    for (int i = 0; i < lstBoxProizvodi.Items.Count; i++)
                    {
                        if (((Artikal)lstBoxProizvodi.Items[i]).Naziv.ToLower().Contains(textBox2.Text.ToLower()))
                        {
                            art.Add((Artikal)lstBoxProizvodi.Items[i]);
                        }
                    }

                    lstBoxProizvodi.DataSource = art;
                }
                else
                {
                    NadiGrupu(FokusiranoDugme.Name, lstBoxProizvodi);
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int left = panel2.ClientRectangle.Left;
            int top = panel2.ClientRectangle.Top;
            int width = panel2.ClientSize.Width;
            if (textBox3.Text != "" || textBox3.Text != " ")
            {
                OsveziRacun(racun);
                foreach (ElementRacuna elementRacuna in racun)
                {
                    if (!elementRacuna.LabelArtikla.Text.ToLower().Contains(textBox3.Text.ToLower()))
                    {
                        panel2.Controls.Remove(elementRacuna.BtnPlus);
                        panel2.Controls.Remove(elementRacuna.BtnMinus);
                        panel2.Controls.Remove(elementRacuna.BtnUkloni);
                        panel2.Controls.Remove(elementRacuna.LabelArtikla);
                    }
                    else
                    {
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.LabelArtikla)].Left = left;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.LabelArtikla)].Top = top;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnMinus)].Left = width - 15;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnMinus)].Top = top;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnPlus)].Left = width - 30;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnPlus)].Top = top;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnUkloni)].Left = width - 45;
                        panel2.Controls[panel2.Controls.IndexOf(elementRacuna.BtnUkloni)].Top = top;

                        top += elementRacuna.LabelArtikla.Height;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    OsveziRacun(racun);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblVreme.Text = "Datum: " + DateTime.Now.Date.ToString("d") + "  " + DateTime.Now.TimeOfDay.Hours + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
        }

        private void cbxDodajUGrupu_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxDodajUGrupu.Checked)
            {
                btnDodajGrupu.Text = "Dodaj Artikal u Grupu";
                txtNazivGrupe.ReadOnly = true;
                button14.Enabled = false;
            }
            else
            {
                btnDodajGrupu.Text = "Dodaj Grupu";
                txtNazivGrupe.ReadOnly = false;
                button14.Enabled = true;
            }
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            List<Artikal> art = new List<Artikal>();
            if (lbxArtBez.Items != null)
            {
                if (textBox4.Text != "")
                {
                    lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                    for (int i = 0; i < lbxArtBez.Items.Count; i++)
                    {
                        if (((Artikal)lbxArtBez.Items[i]).Naziv.ToLower().Contains(textBox4.Text.ToLower()))
                        {
                            art.Add((Artikal)lbxArtBez.Items[i]);
                        }
                    }
                    lbxArtBez.DataSource = art;
                }
                else
                {
                    lbxArtBez.DataSource = baza.CitajArtikleBezGrupa();
                }
            }
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            List<Grupa> grp = new List<Grupa>();
            if (lbxGrupa.Items != null)
            {
                if (textBox5.Text != "")
                {
                    lbxGrupa.DataSource = baza.CitajGrupe();
                    for (int i = 0; i < lbxGrupa.Items.Count; i++)
                    {
                        if(((Grupa)lbxGrupa.Items[i]).Naziv.ToLower().Contains(textBox5.Text.ToLower()))
                        {
                            grp.Add((Grupa)lbxGrupa.Items[i]);
                        }
                    }

                    lbxGrupa.DataSource = grp;
                }
                else
                {
                    lbxGrupa.DataSource = baza.CitajGrupe();
                }
            }
        }
    }
}
