//Copyright (c) 2012. Raverus d.o.o.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;

namespace Raverus.FiskalizacijaDEV.Sample
{
    public partial class Form1 : Form
    {
        readonly Stopwatch stopWatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            string oib = "44718633471"; // ovdje postavite OIB koji je u FISKAL DEMO certifikatu kojeg koristite, kasnije ćemo implementirati Razno.DohvatiOibIzCertifikata()

            textBox1.Text = oib;
            textBox18.Text = oib;


            //EchoPrimjer();
            //PoslovniProstorPrimjer(oib);
            //RacunPrimjer(oib);
            //CertifikatDatoteka(oib);
            //DohvatiJir(oib);
            //SnimiDokumentUDatoteku(oib);
            //AutomatskoSnimanjeDatoteka(oib);
            //ViseStopaPoreza(oib);
            //XmlPrimjer(oib);
            //ObradaGreske(oib);
            //TimeOutPrimjer(oib);
            //ProvjeraPotpisa(oib);
        }

        private void ProvjeraPotpisa(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");

            bool potpisTest = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.ProvjeriPotpis(doc);
        }

        private void TimeOutPrimjer(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            cis.TimeOut = 10;   // postavljamo timeout na 10 ms

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            try
            {
                XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
                if (doc != null)
                {
                    string jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
                    MessageBox.Show(jir);
                }
            }
            catch (Exception ex)
            {
                if (cis.OdgovorGreskaStatus != null && cis.OdgovorGreskaStatus == System.Net.WebExceptionStatus.Timeout)
                {
                    MessageBox.Show("Timeout");
                }

                if (cis.OdgovorGreska != null)
                {
                    MessageBox.Show(cis.OdgovorGreska.InnerXml);
                    MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiSifruGreske(cis.OdgovorGreska, PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuGreske(cis.OdgovorGreska, PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));

                }
                else
                    MessageBox.Show(String.Format("Greška: {0}", ex.Message));
            }
        }

        private void ObradaGreske(string oib)
        {
            //Greška je u datumu i vremenu slanja, "21.10.201210:10:28" umjesto "21.10.2012T10:10:28"
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?><tns:RacunZahtjev xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Id=""signXmlId"" xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73""><tns:Zaglavlje><tns:IdPoruke>cff93023-850b-403c-ac8b-277619e81dc9</tns:IdPoruke><tns:DatumVrijeme>21.10.201210:10:28</tns:DatumVrijeme></tns:Zaglavlje><tns:Racun><tns:Oib>44718633471</tns:Oib><tns:USustPdv>true</tns:USustPdv><tns:DatVrijeme>21.10.2012T10:10:22</tns:DatVrijeme><tns:OznSlijed>P</tns:OznSlijed><tns:BrRac><tns:BrOznRac>1</tns:BrOznRac><tns:OznPosPr>123</tns:OznPosPr><tns:OznNapUr>1</tns:OznNapUr></tns:BrRac><tns:Pdv><tns:Porez><tns:Stopa>25.00</tns:Stopa><tns:Osnovica>10.00</tns:Osnovica><tns:Iznos>2.50</tns:Iznos></tns:Porez></tns:Pdv><tns:IznosUkupno>12.50</tns:IznosUkupno><tns:NacinPlac>G</tns:NacinPlac><tns:OibOper>12345678900</tns:OibOper><tns:ZastKod>e4d909c290d0fb1ca068ffaddf22cbd0</tns:ZastKod><tns:NakDost>false</tns:NakDost></tns:Racun></tns:RacunZahtjev>";

            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);

            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat("FISKAL 1");
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);

            try
            {
                XmlDocument odgovor = cis.PosaljiSoapPoruku(dokument);
                if (odgovor != null)
                {
                    string jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(odgovor);
                    MessageBox.Show(jir);
                }
            }
            catch (Exception ex)
            {
                if (cis.OdgovorGreska != null)
                {
                    MessageBox.Show(cis.OdgovorGreska.InnerXml);
                    MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiSifruGreske(cis.OdgovorGreska, PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuGreske(cis.OdgovorGreska, PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    MessageBox.Show(Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiGreskuRezultataZahtjeva(cis.OdgovorGreska));

                }
                else
                    MessageBox.Show(String.Format("Greška: {0}", ex.Message));
            }

        }

        private void XmlPrimjer(string oib)
        {
            // radi se o primjeru XML-a, obratite pažnju na UUID, datum i vrijeme, OIB, ...
            string ss = "cff93023-850b-403c-ac8b-277619e81dc9".ToUpper();
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?><tns:RacunZahtjev xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Id=""signXmlId"" xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73""><tns:Zaglavlje><tns:IdPoruke>" + ss + "</tns:IdPoruke><tns:DatumVrijeme>21.10.2012T10:10:28</tns:DatumVrijeme></tns:Zaglavlje><tns:Racun><tns:Oib>44718633471</tns:Oib><tns:USustPdv>true</tns:USustPdv><tns:DatVrijeme>21.10.2012T10:10:22</tns:DatVrijeme><tns:OznSlijed>P</tns:OznSlijed><tns:BrRac><tns:BrOznRac>1</tns:BrOznRac><tns:OznPosPr>123</tns:OznPosPr><tns:OznNapUr>1</tns:OznNapUr></tns:BrRac><tns:Pdv><tns:Porez><tns:Stopa>25.00</tns:Stopa><tns:Osnovica>10.00</tns:Osnovica><tns:Iznos>2.50</tns:Iznos></tns:Porez></tns:Pdv><tns:IznosUkupno>12.50</tns:IznosUkupno><tns:NacinPlac>G</tns:NacinPlac><tns:OibOper>12345678900</tns:OibOper><tns:ZastKod>e4d909c290d0fb1ca068ffaddf22cbd0</tns:ZastKod><tns:NakDost>false</tns:NakDost></tns:Racun></tns:RacunZahtjev>";


            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);

            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat("FISKAL 1");
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);

            XmlDocument odgovor = cis.PosaljiSoapPoruku(dokument);
        }

        private void ViseStopaPoreza(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Schema.RacunType();
            racun.Oib = oib;
            racun.USustPdv = true;
            racun.DatVrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme();
            racun.OznSlijed = Schema.OznakaSlijednostiType.P;

            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType broj = new Schema.BrojRacunaType();
            broj.BrOznRac = "1";
            broj.OznPosPr = "123";
            broj.OznNapUr = "1";
            racun.BrRac = broj;

            Raverus.FiskalizacijaDEV.Schema.PorezType porez25 = new Schema.PorezType();
            porez25.Stopa = "25.00";
            porez25.Osnovica = "10.00";
            porez25.Iznos = "2.50";

            Raverus.FiskalizacijaDEV.Schema.PorezType porez0 = new Schema.PorezType();
            porez0.Stopa = "0.00";
            porez0.Osnovica = "10.00";
            porez0.Iznos = "0.00";

            racun.Pdv.Add(porez25);
            racun.Pdv.Add(porez0);



            racun.IznosUkupno = "22.50";
            racun.NacinPlac = Schema.NacinPlacanjaType.G;
            racun.OibOper = "12345678900";
            racun.ZastKod = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun("FISKAL 1", racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());
            racun.NakDost = false;



            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
        }

        private static Raverus.FiskalizacijaDEV.Schema.RacunType GetRacun(string oib)
        {
            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Schema.RacunType();
            racun.Oib = oib;
            racun.USustPdv = true;
            racun.DatVrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme();
            racun.OznSlijed = Schema.OznakaSlijednostiType.P;

            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType broj = new Schema.BrojRacunaType();
            broj.BrOznRac = "1";
            broj.OznPosPr = "123";
            broj.OznNapUr = "1";
            racun.BrRac = broj;

            Raverus.FiskalizacijaDEV.Schema.PorezType porez = new Schema.PorezType();
            porez.Stopa = "25.00";   
            porez.Osnovica = "10.00";
            porez.Iznos = "2.50";
            racun.Pdv.Add(porez);

            racun.IznosUkupno = "12.50";
            racun.NacinPlac = Schema.NacinPlacanjaType.G;
            racun.OibOper = "12345678900";
            racun.ZastKod = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun("FISKAL 1", racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());
            racun.NakDost = false;
            return racun;
        }

        private void AutomatskoSnimanjeDatoteka(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav() { NazivMapeZahtjev = @"D:\Users\Nino\Desktop\Zahtjevi", NazivMapeOdgovor = @"D:\Users\Nino\Desktop\Odgovori", NazivAutoGeneriranje = true };

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
        }

        private void SnimiDokumentUDatoteku(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);


            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, PopratneFunkcije.Potpisivanje.DohvatiCertifikat("FISKAL 1"));
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(zahtjevXml, @"D:\Users\Nino\Desktop\Zahtjev.xml");

            XmlDocument doc = cis.PosaljiSoapPoruku(zahtjevXml);

            Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(doc, @"D:\Users\Nino\Desktop\Odgovor.xml");
        }

        private void DohvatiJir(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
            if (doc != null)
            {
                string jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
            }
        }

        private void CertifikatDatoteka(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            X509Certificate2 cert = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(@"D:\Users\Nino\Desktop\Fiskalizacija\MojCert.pfx", "PASSWORD");
            if (cert != null)
            {
                // varijanta 1
                XmlDocument odgovor = cis.PosaljiRacun(racun, cert);
                

                // varijanta 2
                Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
                XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

                PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, cert);
                PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                XmlDocument doc = cis.PosaljiSoapPoruku(zahtjevXml);
            }

        }

        private void RacunPrimjer(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.RacunType racun = GetRacun(oib);

            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
        }

        private static void PoslovniProstorPrimjer(string oib)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            Raverus.FiskalizacijaDEV.Schema.PoslovniProstorType poslovniProstor = new Schema.PoslovniProstorType();
            poslovniProstor.Oib = oib;
            poslovniProstor.OznPoslProstora = "123";

            Raverus.FiskalizacijaDEV.Schema.AdresaType adresa = new Schema.AdresaType();
            adresa.Ulica = "Ilica";
            adresa.KucniBroj = "1";
            adresa.KucniBrojDodatak = "c";
            adresa.BrojPoste = "10000";
            adresa.Naselje = "Zagreb";
            adresa.Opcina = "Centar";
            Raverus.FiskalizacijaDEV.Schema.AdresniPodatakType adresniPodatak = new Schema.AdresniPodatakType();
            adresniPodatak.Item = adresa;
            poslovniProstor.AdresniPodatak = adresniPodatak;

            poslovniProstor.RadnoVrijeme = "od 8 do 8";
            poslovniProstor.DatumPocetkaPrimjene = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatum(DateTime.Now.Date);

            XmlDocument doc = cis.PosaljiPoslovniProstor(poslovniProstor, "FISKAL 1");
        }

        private static void EchoPrimjer()
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

            bool bTest = cis.Echo();
            bTest = cis.Echo("moj test");

            XmlDocument doc = cis.PosaljiEcho("");
            doc = cis.PosaljiEcho("moja poruka");

            doc = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuEchoZahtjev("moj prvi test");
            doc = cis.PosaljiSoapPoruku(doc);
        }



        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            Raverus.FiskalizacijaDEV.Schema.PoslovniProstorType poslovniProstor = new Schema.PoslovniProstorType() { Oib = textBox1.Text, OznPoslProstora = textBox2.Text };
            Raverus.FiskalizacijaDEV.Schema.AdresaType adresa = new Schema.AdresaType() { Ulica = textBox3.Text, KucniBroj = textBox5.Text, KucniBrojDodatak = textBox6.Text, BrojPoste = textBox7.Text, Naselje = textBox4.Text, Opcina = textBox8.Text };
            Raverus.FiskalizacijaDEV.Schema.AdresniPodatakType adresniPodatak = new Schema.AdresniPodatakType() { Item = adresa };
            poslovniProstor.AdresniPodatak = adresniPodatak;
            poslovniProstor.RadnoVrijeme = textBox9.Text;
            poslovniProstor.DatumPocetkaPrimjene = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatum(dateTimePicker1.Value.Date);
            poslovniProstor.SpecNamj = textBox16.Text;

            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
            cis.SoapMessageSending += cis_SoapMessageSending;
            cis.SoapMessageSent += cis_SoapMessageSent;

            XmlDocument doc = cis.PosaljiPoslovniProstor(poslovniProstor, "FISKAL 1");
            if (doc != null)
                textBox14.Text = doc.InnerXml;
            else
            {
                textBox14.Text = "GREŠKA";
                stopWatch.Stop();
                pictureBox1.Visible = false;
                button2.Enabled = true;
            }
        }

        void cis_SoapMessageSent(object sender, EventArgs e)
        {
            stopWatch.Stop();
            pictureBox1.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            label26.Text = String.Format("Vrijeme: {0} s", stopWatch.Elapsed.TotalSeconds);
            Application.DoEvents();
            stopWatch.Reset();
            Cursor.Current = Cursors.Default;
        }

        void cis_SoapMessageSending(object sender, CentralniInformacijskiSustavEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            stopWatch.Start();
            pictureBox1.Visible = true;
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("hr-HR");

            button1.Enabled = false;
            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Schema.RacunType() { Oib = textBox18.Text, USustPdv = checkBox1.Checked, DatVrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), OznSlijed = Schema.OznakaSlijednostiType.P };
            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType br = new Schema.BrojRacunaType() { BrOznRac = textBox13.Text, OznPosPr = textBox12.Text, OznNapUr = textBox15.Text };
            racun.BrRac = br;
            Raverus.FiskalizacijaDEV.Schema.PdvType pdv = new Schema.PdvType();
            Raverus.FiskalizacijaDEV.Schema.PorezType porez = new Schema.PorezType() { Stopa = textBox11.Text, Osnovica = textBox10.Text, Iznos = textBox19.Text };
            pdv.Porez.Add(porez);
            racun.Pdv.Add(porez);
            racun.IznosUkupno = textBox20.Text;
            racun.NacinPlac = Schema.NacinPlacanjaType.G;
            racun.OibOper = textBox22.Text;

            textBox23.Text = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun("FISKAL 1", racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());

            racun.ZastKod = textBox23.Text;
            racun.NakDost = checkBox2.Checked;

            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
            cis.SoapMessageSending += cis_SoapMessageSending;
            cis.SoapMessageSent += cis_SoapMessageSent;

            XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
            if (doc != null)
            {
                textBox14.Text = doc.InnerXml;
                bool potpisTest = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.ProvjeriPotpis(doc);
                if (potpisTest)
                    label27.Text = "Potpis na odgovoru je OK";
                else
                    label27.Text = "Greška kod provjere potpisa na odgovoru.";
            }
            else
            {
                textBox14.Text = "GREŠKA";
                stopWatch.Stop();
                pictureBox1.Visible = false;
                button1.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
            cis.SoapMessageSending += cis_SoapMessageSending;
            cis.SoapMessageSent += cis_SoapMessageSent;

            XmlDocument doc = cis.PosaljiEcho("");
            if (doc != null)
                textBox14.Text = doc.InnerXml;
            else
            {
                textBox14.Text = "GREŠKA";
                stopWatch.Stop();
                pictureBox1.Visible = false;
                button3.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pictureBox2.Image = null;
            Application.DoEvents();
            stopWatch.Start();


            Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum status = PopratneFunkcije.ServiceStatusEnum.unknown;
            try
            {
                status = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiStatusCisServisa();
            }
            catch (Exception)
            {
                // do nothing
            }
            stopWatch.Stop();
            label26.Text = String.Format("Vrijeme: {0} s", stopWatch.Elapsed.TotalSeconds);
            Application.DoEvents();
            stopWatch.Reset();
            Cursor.Current = Cursors.Default;

            switch (status)
            {
                case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.green:
                    pictureBox2.Image = Properties.Resources.Symbol_Check;
                    break;
                case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.yellow:
                    MessageBox.Show(status.ToString());
                    break;
                case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.red:
                    pictureBox2.Image = Properties.Resources.Symbol_Error;
                    break;
                case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.unknown:
                    MessageBox.Show(status.ToString());
                    break;
                default:
                    break;
            }



        }

    }
}
