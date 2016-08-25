using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Raverus.FiskalizacijaDEV.ConnectionTest
{
    public partial class frmMain : Form
    {
        const int iterations = 50;
        readonly Stopwatch stopWatch = new Stopwatch();
        X509Certificate2 certifikat = null;
        double echo = 0;
        double potpis = 0;
        double slanje = 0;
        bool testFinished;

        public frmMain()
        {
            InitializeComponent();
            Shown += frmMain_Shown;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            radioButton2.CheckedChanged += radioButton1_CheckedChanged;
        }

        void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                txtDatoteka.Enabled = false;
                txtZaporka.Enabled = false;
                btnDatoteka.Enabled = false;
                txtCertifikat.Enabled = true;
            }
            else
            {
                txtDatoteka.Enabled = true;
                txtZaporka.Enabled = true;
                btnDatoteka.Enabled = true;
                txtCertifikat.Enabled = false;
            }
        }

        void frmMain_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            ConnectionTest.ConnectionTest ct = new ConnectionTest.ConnectionTest();
            ct.DohvatiListuMjestaCompleted += ct_DohvatiListuMjestaCompleted;
            ct.DohvatiListuMjestaAsync();

            Application.DoEvents();
            ConnectionTest.ConnectionTest ct1 = new ConnectionTest.ConnectionTest();
            ct1.DohvatiListuInternetProvideraCompleted += ct_DohvatiListuInternetProvideraCompleted;
            ct1.DohvatiListuInternetProvideraAsync();

            Application.DoEvents();
            ConnectionTest.ConnectionTest ct2 = new ConnectionTest.ConnectionTest();
            ct2.DohvatiListuOsCompleted += ct2_DohvatiListuOsCompleted;
            ct2.DohvatiListuOsAsync();

            Application.DoEvents();
            ConnectionTest.ConnectionTest ct3 = new ConnectionTest.ConnectionTest();
            ct3.DohvatiListuTipVezeCompleted += ct3_DohvatiListuTipVezeCompleted;
            ct3.DohvatiListuTipVezeAsync();



        }

        void ct3_DohvatiListuTipVezeCompleted(object sender, ConnectionTest.DohvatiListuTipVezeCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                DataSet ds = (DataSet)e.Result;
                DataBind(comTipVeze, ds, pictureBox3);

            }
        }

        void ct2_DohvatiListuOsCompleted(object sender, ConnectionTest.DohvatiListuOsCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                DataSet ds = (DataSet)e.Result;
                DataBind(comOs, ds, pictureBox4);
            }
        }

        void ct_DohvatiListuInternetProvideraCompleted(object sender, ConnectionTest.DohvatiListuInternetProvideraCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                DataSet ds = (DataSet)e.Result;
                DataBind(comInternetProvideri, ds, pictureBox2);
            }
        }

        void ct_DohvatiListuMjestaCompleted(object sender, ConnectionTest.DohvatiListuMjestaCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                DataSet ds = (DataSet)e.Result;
                DataBind(comMjesto, ds, pictureBox1);
            }
        }

        static void DataBind(ComboBox combo, DataSet ds, PictureBox pb)
        {
            if (ds != null && ds.Tables.Count == 1)
            {
                DataTable dt = ds.Tables[0];
                combo.DataSource = dt;
                combo.ValueMember = "guid";
                combo.DisplayMember = "Naziv";
                pb.Visible = false;
                Application.DoEvents();
                combo.SelectedValue = DBNull.Value;
            }
        }

        private void btnDatoteka_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "Certifikat|*.pfx" };
            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
                txtDatoteka.Text = dialog.FileName;
            else
                txtDatoteka.Text = "";
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
            if (CheckCertificate())
            {
                if (CheckOib())
                {
                    if (CheckSsl())
                    {
                        Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(certifikat);
                        if (racun != null)
                        {
                            progressBar1.Maximum = iterations * 3;
                            btnStartTest.Enabled = false;
                            stopWatch.Reset();
                            RefreshLog("ECHO test počinje...");
                            stopWatch.Start();
                            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
                            for (int i = 0; i < iterations; i++)
                            {
                                cis.Echo();
                                Application.DoEvents();
                                progressBar1.PerformStep();
                            }
                            stopWatch.Stop();
                            echo = stopWatch.Elapsed.TotalSeconds;
                            RefreshLog(String.Format("ECHO test završen. Vrijeme: {0:n4}. Prosjek: {1:n4}", echo, echo / iterations));
                            RefreshLog("");

                            RefreshLog("Račun test počinje...");
                            RefreshLog("Kreiranje i potpisivanje računa...");
                            SortedList<int, XmlDocument> sl = new SortedList<int, XmlDocument>();
                            stopWatch.Reset();
                            stopWatch.Start();
                            for (int i = 0; i < iterations; i++)
                            {
                                Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
                                XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);
                                PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
                                PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                                sl.Add(i, zahtjevXml);

                                Application.DoEvents();
                                progressBar1.PerformStep();
                            }
                            stopWatch.Stop();
                            potpis = stopWatch.Elapsed.TotalSeconds;
                            RefreshLog(String.Format("Kreiranje i potpisivanje završeno. Vrijeme: {0:n4}. Prosjek: {1:n4}", potpis, potpis / iterations));
                            RefreshLog("");

                            RefreshLog("Slanje računa...");
                            stopWatch.Reset();
                            stopWatch.Start();
                            foreach (XmlDocument item in sl.Values)
                            {
                                cis.PosaljiSoapPoruku(item);
                                Application.DoEvents();
                                progressBar1.PerformStep();
                            }
                            stopWatch.Stop();
                            slanje = stopWatch.Elapsed.TotalSeconds;
                            RefreshLog(String.Format("Slanje računa završeno. Vrijeme: {0:n4}. Prosjek: {1:n4}", slanje, slanje / iterations));
                            RefreshLog("");

                            RefreshLog(String.Format("Račun test završen. Vrijeme: {0:n4}. Prosjek: {1:n4}", potpis + slanje, (potpis + slanje) / iterations));

                            btnStartTest.Enabled = true;
                            testFinished = true;
                        }
                        else
                            MessageBox.Show("Greška kod kreiranja test računa.");
                    }
                    else
                    {
                        MessageBox.Show("Nije moguće uspostaviti SSL vezu prema CIS-u. Provjerite da li su potrebni certifikati uredno instalirani.");
                        DialogResult dire = MessageBox.Show("Želite li provjeriti status CIS-a?", "Status CIS-a", MessageBoxButtons.YesNo);
                        if (dire == DialogResult.Yes)
                            MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiStatusCisServisa().ToString());
                    }
                }
                else
                    MessageBox.Show("Greška kod dohvata OIB-a iz certifikata.");
            }
        }

        private void RefreshLog(string message)
        {
            string s = txtLog.Text;

            if (string.IsNullOrEmpty(s))
                s = message;
            else
                s = s + Environment.NewLine + message;

            txtLog.Text = s;
        }

        private bool CheckOib()
        {
            bool bOut;

            string oib = DohvatiOibIzCertifikata(certifikat);
            if (!string.IsNullOrEmpty(oib) && oib.Length == 11)
                bOut = true;
            else
                bOut = false;

            return bOut;
        }

        private static Raverus.FiskalizacijaDEV.Schema.RacunType GetRacun(X509Certificate2 cert)
        {
            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Schema.RacunType() { Oib = DohvatiOibIzCertifikata(cert), USustPdv = true, DatVrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), OznSlijed = Schema.OznakaSlijednostiType.P };

            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType broj = new Schema.BrojRacunaType() { BrOznRac = "0", OznPosPr = "0", OznNapUr = "0" };
            racun.BrRac = broj;

            Raverus.FiskalizacijaDEV.Schema.PorezType porez = new Schema.PorezType() { Stopa = "0.00", Osnovica = "0.00", Iznos = "0.00" };
            racun.Pdv.Add(porez);

            racun.IznosUkupno = "0.00";
            racun.NacinPlac = Schema.NacinPlacanjaType.G;
            racun.OibOper = "00000000000";
            racun.ZastKod = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(cert, racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());
            racun.NakDost = false;
            return racun;
        }

        private bool CheckCertificate()
        {
            bool bOut = false;
            X509Certificate2 certificate = null;

            try
            {
                if (radioButton1.Checked)
                    certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(txtCertifikat.Text);
                else
                    certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(txtDatoteka.Text, txtZaporka.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Greška kod dohvata certifikata: {0}", ex.Message));
            }

            if (certificate != null)
            {
                certifikat = certificate;
                bOut = true;
            }


            return bOut;
        }

        static bool CheckSsl()
        {
            bool bOut = false;


            try
            {
                Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
                bOut = cis.Echo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Nije moguće uspostaviti SSL vezu prema CIS-u. Greška: {0}. Provjerite da li su potrebni certifikati uredno instalirani.", ex.Message));
            }


            return bOut;
        }

        private static string DohvatiOibIzCertifikata(X509Certificate2 cert)
        {
            try
            {
                string oib = "";

                if (cert != null)
                {
                    X500DistinguishedName dName = new X500DistinguishedName(cert.SubjectName);
                    string[] dn = dName.Name.Split(',');
                    foreach (string s in dn)
                    {

                        if (s.Trim().StartsWith("O="))
                        {
                            oib = s.Substring(s.Length - 11);
                        }

                    }
                }
                return oib;
            }
            catch
            {
                return null;
            }
        }

        private void btnPosalji_Click(object sender, EventArgs e)
        {
            if (comMjesto.SelectedValue == DBNull.Value || comMjesto.SelectedValue == null)
            {
                MessageBox.Show("Molim, odaberite mjesto.");
                return;
            }

            if (comInternetProvideri.SelectedValue == DBNull.Value || comInternetProvideri.SelectedValue == null)
            {
                MessageBox.Show("Molim, odaberite Internet providera.");
                return;
            }

            if (comTipVeze.SelectedValue == DBNull.Value || comTipVeze.SelectedValue == null)
            {
                MessageBox.Show("Molim, odaberite tip veze.");
                return;
            }

            if (comOs.SelectedValue == DBNull.Value || comOs.SelectedValue == null)
            {
                MessageBox.Show("Molim, odaberite OS.");
                return;
            }

            if (!testFinished)
            {
                MessageBox.Show("Pokrenite test prije pozivanja ove opcije.");
                return;
            }

            try
            {
                ConnectionTest.ConnectionTest ct = new ConnectionTest.ConnectionTest();

                DataSet ds = new DataSet();
                AppData.dsData.CT_RezultatiDataTable dt = new AppData.dsData.CT_RezultatiDataTable();
                AppData.dsData.CT_RezultatiRow row = dt.NewCT_RezultatiRow();
                row.guid = Guid.NewGuid();
                row.mjesto_guid = (Guid)comMjesto.SelectedValue;
                row.internetprovider_guid = (Guid)comInternetProvideri.SelectedValue;
                row.tipveze_guid = (Guid)comTipVeze.SelectedValue;
                row.os_guid = (Guid)comOs.SelectedValue;
                row.echo = Convert.ToDecimal(echo / iterations);
                row.potpis = Convert.ToDecimal(potpis / iterations);
                row.racun = Convert.ToDecimal(slanje / iterations);
                row.TimeStamp = new DateTime(1900, 1, 1);
                dt.Rows.Add(row);
                ds.Tables.Add(dt);

                bool test = ct.Posalji(ds);
                if (test)
                    MessageBox.Show("Podaci su uspješno poslani.");
                else
                    MessageBox.Show("Greška kod slanja.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Greška kod slanja: {0}", ex.Message));
            }
        }



        private void btnPogledaj_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.fdev.hr/ConnectionTest/Default.aspx");
        }


    }
}
