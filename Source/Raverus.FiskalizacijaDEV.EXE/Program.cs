//Copyright (c) 2012. Raverus d.o.o.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Raverus.FiskalizacijaDEV.EXE
{
    class Program
    {
        private readonly static string loggingDatoteka = Path.Combine(Environment.CurrentDirectory, "FiskalizacijaDEV.log");
        private readonly static string jirDatoteka = Path.Combine(Environment.CurrentDirectory, "JIR.txt");
        private readonly static string uuidDatoteka = Path.Combine(Environment.CurrentDirectory, "UUID.txt");
        private readonly static string zkiDatoteka = Path.Combine(Environment.CurrentDirectory, "ZKI.txt");
        private readonly static string echoDatoteka = Path.Combine(Environment.CurrentDirectory, "ECHO.txt");
        private readonly static string statusDatoteka = Path.Combine(Environment.CurrentDirectory, "Status.txt");
        private readonly static string greskaDatoteka = Path.Combine(Environment.CurrentDirectory, "Greska.txt");


        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0].ToUpper() == "GENERIRAJUUID" || args[0].ToUpper() == "GU")
                    WriteToFile(uuidDatoteka, Guid.NewGuid().ToString());
                else if (args[0].ToUpper() == "DOHVATISTATUS" || args[0].ToUpper() == "DS") 
                {
                    Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum status = PopratneFunkcije.ServiceStatusEnum.unknown;

                    try
                    {
                        status = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiStatusCisServisa();
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }

                    WriteToFile(statusDatoteka, status.ToString());
                }
                else
                    Upotreba();
            }
            else if (args.Length > 1)
            {
                switch (args[0].ToUpper())
                {
                    case "RACUNZAHTJEVPOTPISI":
                    case "RACUNZAHTJEVPOSALJI":
                    case "POSLOVNIPROSTORZAHTJEVPOTPISI":
                    case "POSLOVNIPROSTORZAHTJEVPOSALJI":
                    case "RACUNZAHTJEV":
                    case "POSLOVNIPROSTORZAHTJEV":
                    case "RZPOT":
                    case "RZPOS":
                    case "PPZPOT":
                    case "PPZPOS":
                    case "RZ":
                    case "PPZ":
                        if (args.Length == 7)
                        {
                            string cert = args[6];
                            if (string.IsNullOrEmpty(cert))
                                cert = Properties.Settings.Default.NazivCertifikata;

                            X509Certificate2 certifikat = CertificateStore(cert);

                            if (certifikat != null)
                                DoWork(args, certifikat);
                            else
                                Console.WriteLine("Certifikat nije pronađen.");
                        }
                        else if (args.Length == 8)
                        {
                            X509Certificate2 certifikat = CertificateFile(args[6], args[7]);

                            if (certifikat != null)
                                DoWork(args, certifikat);
                            else
                                Console.WriteLine("Certifikat nije pronađen.");
                        }
                        else
                            Upotreba();
                        break;
                    case "GENERIRAJZKI":
                    case "GZ":
                        if (args.Length == 8)
                        {
                            string cert = args[7];
                            if (string.IsNullOrEmpty(cert))
                                cert = Properties.Settings.Default.NazivCertifikata;

                            X509Certificate2 certifikat = CertificateStore(cert);

                            if (certifikat != null)
                                Zki(args, certifikat);
                            else
                                Console.WriteLine("Certifikat nije pronađen.");
                        }
                        else if (args.Length == 9)
                        {
                            X509Certificate2 certifikat = CertificateFile(args[7], args[8]);

                            if (certifikat != null)
                                Zki(args, certifikat);
                            else
                                Console.WriteLine("Certifikat nije pronađen.");
                        }
                        else
                            Upotreba();
                        break;
                    case "ECHO":
                    case "E":
                        if (args.Length == 2)
                        {
                            SendEcho(args);
                        }
                        break;
                    default:
                        Upotreba();
                        break;
                }
            }
            else
                Upotreba();
        }

        private static void Zki(string[] args, X509Certificate2 certifikat)
        {
            string zki = "";
            string oibObveznika;
            string datumVrijemeIzdavanjaRacuna;
            string brojcanaOznakaRacuna;
            string oznakaPoslovnogProstora;
            string oznakaNaplatnogUredaja;
            string ukupniIznosRacuna;


            oibObveznika = args[1];
            datumVrijemeIzdavanjaRacuna = args[2];
            brojcanaOznakaRacuna = args[3];
            oznakaPoslovnogProstora = args[4];
            oznakaNaplatnogUredaja = args[5];
            ukupniIznosRacuna = args[6];


            if (string.IsNullOrEmpty(oibObveznika))
            {
                Console.WriteLine("OIB mora biti postavljen.");
                return;
            }

            if (string.IsNullOrEmpty(datumVrijemeIzdavanjaRacuna))
            {
                Console.WriteLine("Datum i vrijeme izdavanja računa mora biti postavljeno.");
                return;
            }

            if (string.IsNullOrEmpty(brojcanaOznakaRacuna))
            {
                Console.WriteLine("Brojčana oznaka računa mora biti postavljena.");
                return;
            }

            if (string.IsNullOrEmpty(oznakaPoslovnogProstora))
            {
                Console.WriteLine("Oznaka poslovnog prostora mora biti postavljena.");
                return;
            }

            if (string.IsNullOrEmpty(oznakaNaplatnogUredaja))
            {
                Console.WriteLine("Oznaka naplatnog uređaja mora biti postavljena.");
                return;
            }

            if (string.IsNullOrEmpty(ukupniIznosRacuna))
            {
                Console.WriteLine("Ukupni iznos računa mora biti postavljen.");
                return;
            }


            try
            {
                zki = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(certifikat, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Greška kod generiranja ZKI: {0}", ex.Message));
            }



            if (!string.IsNullOrEmpty(zki))
                WriteToFile(zkiDatoteka, zki);
            else
            {
                Console.WriteLine("Greška kod generiranja ZKI.");
                WriteToFile(zkiDatoteka, "");
            }
        }

        private static void SendEcho(string[] args)
        {
            CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav() { CisUrl = Properties.Settings.Default.CisUrl, TimeOut = Properties.Settings.Default.TimeOut };

            try
            {
                XmlDocument doc = cis.PosaljiEcho(args[1]);
                if (doc != null)
                    Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(doc, echoDatoteka);
                else
                {
                    Console.WriteLine("Greška kod slanja ECHO poruke.");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Greška kod obrade i slanja dokumenta: {0}", ex.Message));
                if (cis.OdgovorGreskaStatus.HasValue)
                    HandleResponseError(cis.OdgovorGreskaStatus.Value.ToString(), cis.OdgovorGreska);
            }
        }

        private static void HandleResponseError(string status, XmlDocument odgovor)
        {
            if (odgovor != null && !string.IsNullOrEmpty(odgovor.InnerXml))
            {
                try
                {
                    string greska = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiGreskuRezultataZahtjeva(odgovor);
                    WriteToFile(greskaDatoteka, String.Format("Status greške: {0}{1}Greška: {2}", status, Environment.NewLine, greska));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("Greška kod obrade poruke greške vraćene iz CIS-a: {0}", ex.Message));
                }
            }
            else
            {
                try
                {
                    WriteToFile(greskaDatoteka, String.Format("Status greške: {0}", status));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("Greška kod obrade poruke greške vraćene iz CIS-a: {0}", ex.Message));
                }
            }
        }

        private static X509Certificate2 CertificateStore(string certificateSubject)
        {
            X509Certificate2 certifikat = null;

            if (string.IsNullOrEmpty(certificateSubject))
                Console.WriteLine("Naziv (subjekt) certifikata mora biti postavljen.");


            try
            {
                certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Greška kod dohvata certifikata: {0}", ex.Message));
            }


            return certifikat;
        }

        private static X509Certificate2 CertificateFile(string certifikatDatoteka, string zaporka)
        {
            X509Certificate2 certifikat = null;

            if (string.IsNullOrEmpty(certifikatDatoteka))
                Console.WriteLine("Datoteka sa certifikatom mora biti postavljena.");

            if (string.IsNullOrEmpty(zaporka))
                Console.WriteLine("Zaporka za certifikat mora biti postavljena.");


            try
            {
                certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Greška kod dohvata certifikata: {0}", ex.Message));
            }


            return certifikat;
        }

        private static void DoWork(string[] args, X509Certificate2 certifikat)
        {
            Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;
            string tipDokumenta;
            string datoteka;
            string odgovorDatoteka;
            string logging;
            string url;
            bool bLogging;
            string snimanje;
            bool bSnimanje;
            bool bNemojPoslati = false;
            bool bNemojPotpisati = false;


            tipDokumenta = args[0];
            url = args[1];
            datoteka = args[2];
            odgovorDatoteka = args[3];
            logging = args[4];
            snimanje = args[5];


            if (string.IsNullOrEmpty(tipDokumenta))
            {
                Console.WriteLine("Tip dokumenta mora biti postavljen.");
                return;
            }

            if (string.IsNullOrEmpty(datoteka))
            {
                switch (tipDokumenta.ToUpper())
                {
                    case "RACUNZAHTJEV":
                    case "RZ":
                        datoteka = Properties.Settings.Default.RacunZahtjevDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.RacunOdgovorDatoteka;
                        break;
                    case "POSLOVNIPROSTORZAHTJEV":
                    case "PPZ":
                        datoteka = Properties.Settings.Default.PoslovniProstorZahtjevDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.PoslovniProstorOdgovorDatoteka;
                        break;
                    case "RACUNZAHTJEVPOTPISI":
                    case "RZPOT":
                        datoteka = Properties.Settings.Default.RacunZahtjevPotpisiDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.RacunOdgovorPotpisiDatoteka;
                        break;
                    case "RACUNZAHTJEVPOSALJI":
                    case "RZPOS":
                        datoteka = Properties.Settings.Default.RacunZahtjevPosaljiDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.RacunOdgovorPosaljiDatoteka;
                        break;
                    case "POSLOVNIPROSTORZAHTJEVPOTPISI":
                    case "PPZPOT":
                        datoteka = Properties.Settings.Default.PoslovniProstorZahtjevPotpisiDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.PoslovniProstorOdgovorPotpisiDatoteka;
                        break;
                    case "POSLOVNIPROSTORZAHTJEVPOSALJI":
                    case "PPZPOS":
                        datoteka = Properties.Settings.Default.PoslovniProstorZahtjevPosaljiDatoteka;
                        odgovorDatoteka = Properties.Settings.Default.PoslovniProstorOdgovorPosaljiDatoteka;
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(datoteka))
            {
                Console.WriteLine("Datoteka mora biti postavljena.");
                return;
            }

            if (string.IsNullOrEmpty(odgovorDatoteka))
            {
                Console.WriteLine("Datoteka za odgovor mora biti postavljena.");
                return;
            }

            if (string.IsNullOrEmpty(logging))
            {
                Console.WriteLine("Opcija za logging mora biti postavljena.");
                return;
            }

            bool bTest = Boolean.TryParse(logging, out bLogging);
            if (!bTest)
            {
                Console.WriteLine("Vrijednost za logging mora biti postavljena na 'true' ili 'false'.");
                return;
            }

            if (string.IsNullOrEmpty(snimanje))
            {
                Console.WriteLine("Opcija za snimanje mora biti postavljena.");
                return;
            }

            bTest = Boolean.TryParse(snimanje, out bSnimanje);
            if (!bTest)
            {
                Console.WriteLine("Vrijednost za snimanje mora biti postavljena na 'true' ili 'false'.");
                return;
            }



            switch (tipDokumenta.ToUpper())
            {
                case "RACUNZAHTJEV":
                case "RZ":
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunOdgovor;
                    break;
                case "POSLOVNIPROSTORZAHTJEV":
                case "PPZ":
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor;
                    break;
                case "RACUNZAHTJEVPOTPISI":
                case "RZPOT":
                    bNemojPoslati = true;
                    tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;
                    break;
                case "RACUNZAHTJEVPOSALJI":
                case "RZPOS":
                    bNemojPotpisati = true;
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunOdgovor;
                    break;
                case "POSLOVNIPROSTORZAHTJEVPOTPISI":
                case "PPZPOT":
                    bNemojPoslati = true;
                    tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;
                    break;
                case "POSLOVNIPROSTORZAHTJEVPOSALJI":
                case "PPZPOS":
                    bNemojPotpisati = true;
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor;
                    break;
                default:
                    break;
            }


            FileInfo fi = new FileInfo(datoteka);
            if (!fi.Exists)
            {
                Console.WriteLine("Datoteka ne postoji.");
                if (bLogging)
                    DoLog(String.Format("Datoteka '{0}' ne postoji.", datoteka));
                return;
            }

            XmlDocument dokument = null;
            try
            {
                if (bLogging)
                    DoLog(String.Format("Učitavanje XML datoteke '{0}'.", datoteka));

                dokument = new XmlDocument();
                dokument.Load(datoteka);

                if (bLogging)
                    DoLog(String.Format("XML datoteka '{0}' uspješno učitana.", datoteka));
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Greška kod učitavanja XML datoteke: {0}", ex.Message));
                if (bLogging)
                    DoLog(String.Format("Greška kod učitavanja XML datoteke: {0}", ex.Message));
                return;
            }

            if (dokument != null)
            {
                XmlDocument odgovor = null;
                Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav() { TimeOut = Properties.Settings.Default.TimeOut };
                if (!string.IsNullOrEmpty(url))
                    cis.CisUrl = url;
                else
                    cis.CisUrl = Properties.Settings.Default.CisUrl;

                try
                {


                    if (bSnimanje)
                    {
                        cis.NazivAutoGeneriranje = true;
                        cis.NazivMapeZahtjev = Path.Combine(Environment.CurrentDirectory, "Zahtjev");
                        cis.NazivMapeOdgovor = Path.Combine(Environment.CurrentDirectory, "Odgovor");
                    }

                    if (!bNemojPotpisati)
                    {
                        if (bLogging)
                            DoLog(String.Format("Certifikat: {0}", certifikat.Subject));

                        if (bLogging)
                            DoLog(String.Format("Potpisujem dokument: {0}", dokument.InnerXml));

                        PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

                        if (bLogging)
                            DoLog(String.Format("Potpisani dokument: {0}", dokument.InnerXml));

                        PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);
                    }




                    if (bLogging)
                        DoLog(String.Format("Šaljem dokument: {0}", dokument.InnerXml));


                    if (!bNemojPoslati)
                        odgovor = cis.PosaljiSoapPoruku(dokument);
                    else
                        odgovor = dokument;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("Greška kod obrade i slanja dokumenta: {0}", ex.Message));
                    if (bLogging)
                        DoLog(String.Format("Greška kod obrade i slanja dokumenta: {0}", ex.Message));

                    if (cis.OdgovorGreskaStatus.HasValue)
                        HandleResponseError(cis.OdgovorGreskaStatus.Value.ToString(), cis.OdgovorGreska);
                }


                if (odgovor != null)
                {
                    try
                    {
                        if (bLogging)
                            DoLog(String.Format("Odgovor: {0}", odgovor.InnerXml));

                        if (bLogging)
                            DoLog(String.Format("Spremanje u XML datoteku '{0}'.", odgovorDatoteka));


                        Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(odgovor, odgovorDatoteka);

                        if (tip == PopratneFunkcije.TipDokumentaEnum.RacunOdgovor)
                        {
                            string uuid = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiUuid(odgovor, tip);
                            string jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(odgovor);
                            WriteToFile(jirDatoteka, String.Format("{0};{1}", uuid, jir));
                        }

                        if (bLogging)
                            DoLog(String.Format("Odgovor je spremljen u XML datoteku '{0}'.", odgovorDatoteka));

                        Console.WriteLine("Status OK");

                        if (bLogging)
                            DoLog("Status OK");
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(jirDatoteka, "");
                        Console.WriteLine(String.Format("Greška kod snimanja odgovora u datoteku: {0}", ex.Message));
                        if (bLogging)
                            DoLog(String.Format("Greška kod snimanja odgovora u datoteku: {0}", ex.Message));
                    }
                }
            }
        }

        private static void WriteToFile(string filePath, string valueToWrite)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(valueToWrite);
            sw.Close();
            fs.Close();
        }

        private static void DoLog(string valueToLog)
        {
            FileStream aFile = new FileStream(loggingDatoteka, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(aFile);
            sw.WriteLine(String.Format("{0}: {1}", Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), valueToLog));
            sw.Close();
            aFile.Close();
        }

        private static void Upotreba()
        {
            Console.WriteLine("Upotreba:");
            Console.WriteLine("Ovaj se program može pozivati na dva načina, u ovisnosti da li se certifikat nalazi u Certificate Store-u ili u datoteci");
            Console.WriteLine("Primjer 1: certifikat se nalazi u Certificate Store-u");
            Console.WriteLine(@"Raverus.FiskalizacijaDEV.EXE.exe NazivOperacije ""CIS URL"" ""XML datoteka sa zahtjevom"" ""XML datoteka u koju se sprema odgovor"" Logging Snimanje ""Naziv (subject) certifikata""");
            Console.WriteLine(@"Raverus.FiskalizacijaDEV.EXE.exe RacunZahtjev ""D:\Users\Nino\Desktop\RacunZahtjev.xml"" ""D:\Users\Nino\Desktop\RacunOdgovor.xml"" false true ""FISKAL 1""");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Primjer 2: certifikat se nalazi u datoteci");
            Console.WriteLine(@"Raverus.FiskalizacijaDEV.EXE.exe NazivOperacije ""CIS URL"" ""XML datoteka sa zahtjevom"" ""XML datoteka u koju se sprema odgovor"" Logging Snimanje ""Datoteka sa certifikatom"" ""Zaporka""");
            Console.WriteLine(@"Raverus.FiskalizacijaDEV.EXE.exe RacunZahtjev ""D:\Users\Nino\Desktop\RacunZahtjev.xml"" ""D:\Users\Nino\Desktop\RacunOdgovor.xml"" false true ""D:\Users\Nino\Desktop\MojCert.pfx"" ""PASSWORD""");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Više na: http://fiskalizacija.codeplex.com/wikipage?title=Potpisivanje%20i%20slanje%20XML%20dokumenata%20pozivanjem%20vanjskog%20programa%20%28EXE%29");
        }
    }
}

// GenerirajUuid
// echo "test poruka"
// RacunZahtjev "" "D:\Users\Nino\Desktop\Fiskalizacija\RacunZahtjev.xml" "D:\Users\Nino\Desktop\Fiskalizacija\RacunOdgovor.xml" true true "FISKAL 1" 
// PoslovniProstorZahtjev "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest" "D:\Users\Nino\Desktop\Fiskalizacija\PPZahtjev.xml" "D:\Users\Nino\Desktop\Fiskalizacija\PPOdgovor.xml" true true "FISKAL 1" 
// RacunZahtjev "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest" "D:\Users\Nino\Desktop\Fiskalizacija\RacunZahtjev.xml" "D:\Users\Nino\Desktop\Fiskalizacija\RacunOdgovor.xml" true true "D:\Users\Nino\Desktop\Fiskalizacija\MojCert.pfx" "PASSWORD" 
// PoslovniProstorZahtjev "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest" "D:\Users\Nino\Desktop\Fiskalizacija\PPZahtjev.xml" "D:\Users\Nino\Desktop\Fiskalizacija\PPOdgovor.xml" true true "D:\Users\Nino\Desktop\Fiskalizacija\MojCert.pfx" "PASSWORD" 
// RacunZahtjevPotpisi "" "D:\Users\Nino\Desktop\Fiskalizacija\NepotpisaniXmlRacun.xml" "D:\Users\Nino\Desktop\Fiskalizacija\PotpisaniXmlRacun.xml" true true "FISKAL 1" 
// RacunZahtjevPosalji "" "D:\Users\Nino\Desktop\Fiskalizacija\PotpisaniXmlRacun.xml" "D:\Users\Nino\Desktop\Fiskalizacija\RacunOdgovor.xml" true true "FISKAL 1" 
// PoslovniProstorZahtjevPotpisi "" "D:\Users\Nino\Desktop\Fiskalizacija\NepotpisaniXmlPoslovniProstor.xml" "D:\Users\Nino\Desktop\Fiskalizacija\PotpisaniXmlPoslovniProstor.xml" true true "FISKAL 1" 
// PoslovniProstorZahtjevPosalji "" "D:\Users\Nino\Desktop\Fiskalizacija\PotpisaniXmlPoslovniProstor.xml" "D:\Users\Nino\Desktop\Fiskalizacija\PPOdgovor.xml" true true "FISKAL 1" 
// GenerirajZki "44718633471" "22.10.2012 18:04:26" "1" "11" "12" "125.25" "FISKAL 1" 
// GenerirajZki "44718633471" "22.10.2012 18:04:26" "1" "11" "12" "125.25" "D:\Users\Nino\Desktop\Fiskalizacija\MojCert.pfx" "PASSWORD"
// DohvatiStatus
// RacunZahtjev "" "" "" true true "" 