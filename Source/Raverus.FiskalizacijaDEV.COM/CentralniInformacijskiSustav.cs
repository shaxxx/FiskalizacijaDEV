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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace Raverus.FiskalizacijaDEV.COM
{
    [Guid("0FE931F7-D185-478D-9E1A-DBC655CF415F"), ComVisible(true)]
    public class CentralniInformacijskiSustav
    {
        public string TestCOM(string test)
        {
            return test;
        }

        public string PosaljiEcho(string poruka)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new FiskalizacijaDEV.CentralniInformacijskiSustav() { CisUrl = this.CisUrl, NazivAutoGeneriranje = this.NazivAutoGeneriranje, NazivMapeOdgovor = this.NazivMapeOdgovor, NazivMapeZahtjev = this.NazivMapeZahtjev, TimeOut = this.TimeOut };
            return cis.PosaljiEcho(poruka).InnerXml;
        }

        public string PosaljiSoapPoruku(string xml, string certificateSubject)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new FiskalizacijaDEV.CentralniInformacijskiSustav() { CisUrl = this.CisUrl, NazivAutoGeneriranje = this.NazivAutoGeneriranje, NazivMapeOdgovor = this.NazivMapeOdgovor, NazivMapeZahtjev = this.NazivMapeZahtjev, TimeOut = this.TimeOut };
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);


            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject);
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);

            try
            {
                XmlDocument odgovor = cis.PosaljiSoapPoruku(dokument);
                if (odgovor != null)
                    return odgovor.InnerXml;
                else
                    return "";
            }
            catch (Exception ex)
            {
                if (cis.OdgovorGreska != null)
                {
                    this.OdgovorGreska = cis.OdgovorGreska.InnerXml;
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();
                }
                else
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();

                throw;
            }
        }

        public string PosaljiSoapPorukuCertifikatDatoteka(string xml, string certifikatDatoteka, string zaporka)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new FiskalizacijaDEV.CentralniInformacijskiSustav() { CisUrl = this.CisUrl, NazivAutoGeneriranje = this.NazivAutoGeneriranje, NazivMapeOdgovor = this.NazivMapeOdgovor, NazivMapeZahtjev = this.NazivMapeZahtjev, TimeOut = this.TimeOut };
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);


            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);

            try
            {
                XmlDocument odgovor = cis.PosaljiSoapPoruku(dokument);
                if (odgovor != null)
                    return odgovor.InnerXml;
                else
                    return "";
            }
            catch (Exception ex)
            {
                if (cis.OdgovorGreska != null)
                {
                    this.OdgovorGreska = cis.OdgovorGreska.InnerXml;
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();
                }
                else
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();

                throw;
            }
        }

        public string DohvatiJir(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                return Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
            }
            else
                return "";
        }

        public string PosaljiPotpisanuSoapXmlPoruku(string xml)
        {
            Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new FiskalizacijaDEV.CentralniInformacijskiSustav() { CisUrl = this.CisUrl, NazivAutoGeneriranje = this.NazivAutoGeneriranje, NazivMapeOdgovor = this.NazivMapeOdgovor, NazivMapeZahtjev = this.NazivMapeZahtjev, TimeOut = this.TimeOut };
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);

            try
            {
                XmlDocument odgovor = cis.PosaljiSoapPoruku(dokument);
                if (odgovor != null)
                    return odgovor.InnerXml;
                else
                    return "";
            }
            catch (Exception ex)
            {
                if (cis.OdgovorGreska != null)
                {
                    this.OdgovorGreska = cis.OdgovorGreska.InnerXml;
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();
                }
                else
                    this.OdgovorGreskaStatus = cis.OdgovorGreskaStatus.ToString();

                throw;
            }
        }

        public string DodajSoapEnvelope(string xml)
        {
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);

            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref dokument);

            return dokument.InnerXml;
        }

        public string PotpisiXmlDokument(string xml, string certificateSubject)
        {
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);


            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject);
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            return dokument.InnerXml;
        }

        public string PotpisiXmlDokumentCertifikatDatoteka(string xml, string certifikatDatoteka, string zaporka)
        {
            XmlDocument dokument = new XmlDocument();
            dokument.LoadXml(xml);


            X509Certificate2 certifikat = Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);
            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(dokument, certifikat);

            return dokument.InnerXml;
        }

        public string ZastitniKodIzracun(string certificateSubject, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            return Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(certificateSubject, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
        }

        public string ZastitniKodIzracunCertifikatDatoteka(string certifikatDatoteka, string zaporka, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            return Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(certifikatDatoteka, zaporka, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
        }

        public string DohvatiStatusCisServisa()
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

            return status.ToString();
        }

        public string DohvatiUuid(string xml, string tipDokumenta)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum tip;

                if (tipDokumenta == "RacunOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunOdgovor;
                else if (tipDokumenta == "PoslovniProstorOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor;
                else if (tipDokumenta == "RacunZahtjev")
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunZahtjev;
                else if (tipDokumenta == "PoslovniProstorZahtjev")
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorZahtjev;
                else
                    tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;

                return Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiUuid(doc, tip);
            }
            else
                return "";
        }

        public string GenerirajUuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string DohvatiSifruGreske(string xml, string tipDokumenta)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum tip;

                if (tipDokumenta == "RacunOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunOdgovor;
                else if (tipDokumenta == "PoslovniProstorOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor;
                else
                    tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;

                return Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiSifruGreske(doc, tip);
            }
            else
                return "";
        }

        public string DohvatiPorukuGreske(string xml, string tipDokumenta)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum tip;

                if (tipDokumenta == "RacunOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.RacunOdgovor;
                else if (tipDokumenta == "PoslovniProstorOdgovor")
                    tip = PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor;
                else
                    tip = PopratneFunkcije.TipDokumentaEnum.Nepoznato;

                return Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuGreske(doc, tip);
            }
            else
                return "";
        }

        public string DohvatiGresku(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                return Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiGreskuRezultataZahtjeva(doc);
            }
            else
                return "";
        }

        public void SnimiXmlDokumentDatoteka(string xml, string nazivDatoteke)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(doc, nazivDatoteke);
            }
        }

        #region Properties
        /// <summary>
        /// Naziv mape (foldera) u koji će se spremati XML dokumenti za zahtjeve. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
        /// </summary>
        public string NazivMapeZahtjev { get; set; }

        /// <summary>
        /// Naziv mape (foldera) u koji će se spremati XML dokumenti za odgovore. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
        /// </summary>
        public string NazivMapeOdgovor { get; set; }

        /// <summary>
        /// Određuje da li se naziv generira automatski koristeći UUID ili datoteka uvijek ima isti naziv
        /// </summary>
        /// <remarks>
        /// Ako je vrijednost true, naziv datoteke će biti određen koristeći naziv tipa dokumenta i UUID-a, ako je false naziv datoteke će biti uvijek isti i biti će određen tipom dokumenta.
        /// Ne koristi se ukoliko NazivMapeZahtjev odnosno NazivMapeOdgovor nisu postavljeni na odgovarajuću vrijednost.
        /// Nema smisla postavljati na TRUE za ECHO.
        /// </remarks>
        public bool NazivAutoGeneriranje { get; set; }

        /// <summary>
        /// Ukoliko je CIS vratio odgovor i ukoliko taj odgovor sadrži tehničkom specifikacijom propisanu grešku, tada OdgovorGreska sadži vraćenu XML poruku. U suprotnom, vrijednost je NULL.
        /// </summary>
        public string OdgovorGreska { get; set; }

        /// <summary>
        /// Vraća WebExceptionStatus greške (http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx). U suprotnom, vrijednost je NULL.
        /// </summary>
        public string OdgovorGreskaStatus { get; set; }

        /// <summary>
        /// Vrijednost, u milisekundama, za HttpWebRequest.TimeOut, odnosno, za TimeOut kod komunikacije sa CIS web servisom.
        /// Ako je vrijednost 0 (nula), property se ignorira (ne postavlja se vrijednost za HttpWebRequest.TimeOut).
        /// http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.NET.HTTPWEBREQUEST.TIMEOUT)&rd=true
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Adresa CIS web servisa; ako vrijednost nije postavljena, koristi se trenutna adresa koja je u službenoj upotrebi.
        /// </summary>
        public string CisUrl { get; set; }
        #endregion
    }
}


//D:\CodePlex\FiskalizacijaDEV\Main\Source\FiskalizacijaDEV\Source\Raverus.FiskalizacijaDEV.COM\bin\Debug>C:\Windows\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe Raverus.FiskalizacijaDEV.COM.dll /codebase


//C:\Windows\SysWOW64\cscript.exe test.vbs