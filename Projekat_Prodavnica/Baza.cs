using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Projekat_Prodavnica.ProdavnicaDataSetTableAdapters;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Projekat_Prodavnica
{

   

    class Baza
    {

        private ProdavnicaDataSet ds;
        private ProdavnicaDataSetTableAdapters.RacunTableAdapter daRacun;
       private OleDbConnection conn;

        public Baza()
        {
            conn = new OleDbConnection();
            conn.ConnectionString =
                @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='Prodavnica.accdb'";
            ds = new ProdavnicaDataSet();
            daRacun = new ProdavnicaDataSetTableAdapters.RacunTableAdapter();

        }

        public void OtvoriKonekciju()
        {
            if (conn != null)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Baza ne postoji");
                }
               
                
               
            }

        }
        public void ZatvoriKonekciju()
        {
            if (conn != null)
            {
                conn.Close();
            }

        }

        public OleDbConnection DbConnection
        {
            get { return conn;}
            set { conn = value;}
        }

        /// <summary>
        /// Cita sve artikle iz tabele Artikal
        /// </summary>
        /// <returns></returns>
        public List<Artikal> CitajArtikle()
        {
            
            OtvoriKonekciju();
            List<Artikal> art = new List<Artikal>();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM Artikal WHERE NOT id_artikla = 1 ";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Artikal artikal = new Artikal();
                    artikal.IdArtikla = int.Parse(reader["id_artikla"].ToString());
                    artikal.Naziv = reader["naziv"].ToString();
                    artikal.Cena = int.Parse(reader["cena"].ToString());
                    artikal.Popust = int.Parse(reader["popust"].ToString());
                    art.Add(artikal);
                }
                reader.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
               ZatvoriKonekciju();
            }
            return art;
        }
        /// <summary>
        /// Cita grupu sa navedenim ID-em i vraca grupu sa artiklima. Ova metoda, je namenja da se koristi unutar drugih metoda, zato nema nigde otvaranje i zatvaranje konekcije.
        /// </summary>
        /// <param name="idGrupa"></param>
        /// <returns></returns>
        public List<Artikal> CitajArtikleGrupe(int idGrupa)
        {

            List<Artikal> art = new List<Artikal>();

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText =
                "SELECT * FROM Artikal WHERE id_artikla in (SELECT id_artikla FROM Grupa WHERE id_grupa = " + idGrupa + " )";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Artikal artikal = new Artikal();
                    artikal.IdArtikla = int.Parse(reader["id_artikla"].ToString());
                    artikal.Naziv = reader["naziv"].ToString();
                    artikal.Cena = double.Parse(reader["cena"].ToString());
                    artikal.Popust = int.Parse(reader["popust"].ToString());
                    art.Add(artikal);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            return art;
        }

        /// <summary>
        /// Isto sto i CitajArtikleGrupe samo sto ova moze se koristi van baze tj Otvara i zatvara Konekciju
        /// </summary>
        public List<Artikal> CitajArtikleGrupeVan(int idGrupa)
        {
            OtvoriKonekciju();
            List<Artikal> art = new List<Artikal>();

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText =
                "SELECT * FROM Artikal WHERE id_artikla in (SELECT id_artikla FROM Grupa WHERE id_grupa = " + idGrupa + " )";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Artikal artikal = new Artikal();
                    artikal.IdArtikla = int.Parse(reader["id_artikla"].ToString());
                    artikal.Naziv = reader["naziv"].ToString();
                    artikal.Cena = double.Parse(reader["cena"].ToString());
                    artikal.Popust = int.Parse(reader["popust"].ToString());
                    art.Add(artikal);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            ZatvoriKonekciju();
            return art;
        }
        /// <summary>
        /// Cita artikle kojima nisu dodeljene grupe
        /// </summary>
        /// <returns></returns>
        public List<Artikal> CitajArtikleBezGrupa()
        {
            OtvoriKonekciju();
            List<Artikal> artikli = new List<Artikal>();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText =
                "SELECT * FROM Artikal WHERE id_artikla not in(SELECT id_artikla FROM Grupa) AND NOT id_artikla = 1";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Artikal art = new Artikal();
                    art.IdArtikla = int.Parse(reader["id_artikla"].ToString());
                    art.Naziv = reader["naziv"].ToString();
                    art.Cena = int.Parse(reader["cena"].ToString());
                    art.Popust = int.Parse(reader["popust"].ToString());
                    artikli.Add(art);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ZatvoriKonekciju();
            return artikli;
        }
        
        /// <summary>
        /// Cita grupe i artikle iz tabele Grupa 
        /// </summary>
        /// <returns> Vraca grupu sa artiklima</returns>
        public List<Grupa> CitajGrupe()
        {
            Grupa grp;
            List<Grupa> grupe = new List<Grupa>();
            OleDbCommand cmd = new OleDbCommand();
            OtvoriKonekciju();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT naziv, id_grupa FROM Grupa GROUP BY naziv, id_grupa ORDER BY id_grupa";
            OleDbDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    grp = new Grupa();
                    grp.IdGrupa = int.Parse(reader["id_grupa"].ToString());
                    grp.Naziv = reader["naziv"].ToString();
                    grp.IdArtikla = CitajArtikleGrupe(int.Parse(reader["id_grupa"].ToString()));
                    grupe.Add(grp);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ZatvoriKonekciju();
            }
            return grupe;
        }

        /// <summary>
        /// Dodaje racun u tabelu Racun
        /// </summary>
        /// <param name="racun"></param>
        public void UnesiRacun(Racun racun)
        {
            
            
            OtvoriKonekciju();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Racun(cena,datum,vreme) VALUES(@cena,@datum,@vreme)";
            cmd.Parameters.AddWithValue("cena", racun.Cena);
            cmd.Parameters.AddWithValue("datum", racun.Datum.Date);
            cmd.Parameters.AddWithValue("vreme", racun.Vreme.TimeOfDay);
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Dodaje artikal u tabelu Artikal
        /// </summary>
        /// <param name="artikal"></param>
        public  void DodajArtikal(Artikal artikal)
        {
            
           
            OtvoriKonekciju();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Artikal(naziv,cena,popust) VALUES(@naziv,@cena,@popust)";
            cmd.Parameters.AddWithValue("naziv", artikal.Naziv);
            cmd.Parameters.AddWithValue("cena", artikal.Cena);
            cmd.Parameters.AddWithValue("popust", artikal.Popust);
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }

        /// <summary>
        /// Dodaje artikal u tabeli Grupa u bazi
        /// </summary>
        public void DodajArtikaluGrupu(Grupa grp,int idArtikla)
        {

           
            
            List<Artikal> artikli = CitajArtikle();
            OtvoriKonekciju();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Grupa(naziv,id_grupa,id_artikla) VALUES(@naziv,@id_grupa,@id_artikla)";
            cmd.Parameters.AddWithValue("naziv", grp.Naziv);
            cmd.Parameters.AddWithValue("id_grupa", grp.IdGrupa);
            cmd.Parameters.AddWithValue("id_artikla", idArtikla);
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Prvo obrise idArtikla iz Grupe pa ga doda u postavljenu Grupu
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="idArtikla"></param>
        public void AzurirajArtikaluGrupu(Grupa grp, int idArtikla)
        {

            ObrisiArtikaluGrupi(idArtikla);
            OtvoriKonekciju();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Grupa(naziv,id_grupa,id_artikla) VALUES(@naziv,@id_grupa,@id_artikla)";
            cmd.Parameters.AddWithValue("naziv", grp.Naziv);
            cmd.Parameters.AddWithValue("id_grupa", grp.IdGrupa);
            cmd.Parameters.AddWithValue("id_artikla", idArtikla);
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Dodaj grupu u tabelu Grupa
        /// </summary>
        /// <param name="naziv"></param>
        /// <param name="idArtikla"></param>
        public void DodajGrupu(string naziv,int idArtikla)
        {
           
            List<Grupa> grp = CitajGrupe();
            OtvoriKonekciju();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO Grupa(id_grupa,id_artikla,naziv) VALUES(@id_grupa,@id_artikla,@naziv)";
            cmd.Parameters.AddWithValue("id_grupa", grp.Last().IdGrupa+1);
            cmd.Parameters.AddWithValue("id_artikla",idArtikla);
            cmd.Parameters.AddWithValue("naziv", naziv);
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Brise grupu iz tabele grupa
        /// </summary>
        /// <param name="idGrupe"></param>
        public void ObrisiGrupu(string idGrupe)
        {
           
            OleDbCommand cmd = new OleDbCommand();
            OtvoriKonekciju();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM Grupa WHERE id_grupa = " + idGrupe;
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Brise artikal iz grupe u tabeli Grupa
        /// </summary>
        /// <param name="idArtikla"></param>
        public void ObrisiArtikaluGrupi(int idArtikla)
        {

            OleDbCommand cmd = new OleDbCommand();
            OtvoriKonekciju();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM Grupa WHERE id_artikla = " + idArtikla;
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Brise prvo artikal iz tabele Grupa pa onda iz tabele Artikal
        /// </summary>
        /// <param name="idArtikla"></param>
        public void ObrisiArtikal(int idArtikla)
        {
            
            OleDbCommand cmd = new OleDbCommand();
            OtvoriKonekciju();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM Grupa WHERE id_artikla = " + idArtikla;
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DELETE FROM Artikal WHERE id_artikla = " + idArtikla;
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// IDArtikla sluzi da nade artikal u tabeli Artikla u bazi i postavlja navedene vrednosti
        /// </summary>
        public void AzurirajArtikal(int idArtikla,string naziv, double cena, int popust)
        {
            OtvoriKonekciju();
            int tu = 0;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE Artikal SET naziv = '" +naziv+"' ,cena = "+cena+" ,popust = "+popust+" WHERE id_artikla = " + idArtikla;
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Menja naziv grupe u tabeli Grupa gde se idGrupe poklapa sa grupom
        /// </summary>
        /// <param name="naziv"></param>
        /// <param name="idGrupe"></param>
        public void AzurirajGrupu(string naziv, int idGrupe)
        {
            OtvoriKonekciju();
            int tu = 0;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE Grupa SET naziv = '"+ naziv +"' WHERE id_grupa = " + idGrupe;
            cmd.ExecuteNonQuery();
            ZatvoriKonekciju();
        }
        /// <summary>
        /// Cita racun zadat u datom opsegu uz pomoc DataSet i vraca DataTable namanje za rad sa DataGridView
        /// </summary>
        /// <param name="Od"></param>
        /// <param name="Do"></param>
        /// <returns></returns>
        public DataTable CitajRacun(DateTime Od, DateTime Do)
        {
            daRacun.Fill(ds.Racun);
            var linq = from x in ds.Racun
                       where x.datum.Date > Od && x.datum.Date < Do
                    select x;
            try
            {
               return linq.CopyToDataTable();
            }
            catch (Exception e)
            {
                MessageBox.Show("Nema racuna u tom opsegu");
            }

            return null;
        }

    }
}
