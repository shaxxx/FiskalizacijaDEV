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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Raverus.FiskalizacijaDEV.PopratneFunkcije
{
    /// <summary>
    /// Koristi se za formatiranje i pripremu XML dokumenata.</summary>
    /// <remarks>
    /// Ovo je klasa čija je namjena pomoć kod kreiranja i formatiranja XML dokumenata, njihova serijalizacija, priprema zahtjeva i SOAP envelop-a i sl.
    /// </remarks>
    public static class XmlDokumenti
    {
        #region Račun
        /// <summary>
        /// Koristi se za serijalizaciju XML-a zahtjeva.</summary>
        /// <param name="racunZahtjev">Objekt tipa Schema.RacunZahtjev koji sadrži RacunZahtjev.</param>
        /// <example>
        ///  Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
        ///  XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML zahtjeva. U slučaju greške vraća null.</returns>
        public static XmlDocument SerijalizirajRacunZahtjev(Schema.RacunZahtjev racunZahtjev)
        {
            string xml = "";

            try
            {
                xml = racunZahtjev.Serialize();
                xml = xml.Replace("<tns:Pdv />", "");
                xml = xml.Replace("<tns:Pnp />", "");
                xml = xml.Replace("<tns:OstaliPor />", "");
                xml = xml.Replace("<tns:Naknade />", "");
                //xml = xml.Replace(@"Id=""signXmlId""", @"Id=""racunId"" xsi:schemaLocation=""http://www.apis-it.hr/fin/2012/types/f73 ../schema/FiskalizacijaSchema.xsd""");
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod serijalizacije zahtjeva za račun: {0}", ex.Message));
                throw;
            }

            XmlDocument doc = UcitajXml(xml);


            return doc;
        }

        /// <summary>
        /// Koristi se za serijalizaciju računa.</summary>
        /// <param name="racun">Objekt koji treba serijalizirati</param>
        /// <example>
        ///  
        ///  
        /// </example>
        /// <returns>
        /// Vraća serijalizirani dokument, kao XML. U slučaju greške vraća null.</returns>
        public static XmlDocument SerijalizirajRacun(Schema.RacunType racun)
        {
            string xml = "";

            try
            {
                xml = racun.Serialize();
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod serijalizacije računa: {0}", ex.Message));
                throw;
            }

            XmlDocument doc = UcitajXml(xml);


            return doc;
        }

        /// <summary>
        /// Kreira RacunZahtjev.</summary>
        /// <remarks>
        /// Namjena je ove metode da doda zaglavlje zahtjevu.
        /// </remarks>
        /// <param name="racun">Racun kojem treba dodati dio vezan uz zahtjev.</param>
        /// <example>
        ///  Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
        /// </example>
        /// <returns>
        /// Vraća RacunZahtjev.</returns>
        public static Schema.RacunZahtjev KreirajRacunZahtjev(Schema.RacunType racun)
        {
            Schema.RacunZahtjev zahtjev = new Schema.RacunZahtjev() { Id = "signXmlId", Racun = racun };

            Schema.ZaglavljeType zaglavlje = new Schema.ZaglavljeType() { DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), IdPoruke = Guid.NewGuid().ToString() };

            zahtjev.Zaglavlje = zaglavlje;

            return zahtjev;
        }

        public static Schema.RacunZahtjev KreirajRacunZahtjev(string ID, Schema.RacunType racun)
        {
            Schema.RacunZahtjev zahtjev = new Schema.RacunZahtjev() { Id = ID, Racun = racun };

            Schema.ZaglavljeType zaglavlje = new Schema.ZaglavljeType() { DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), IdPoruke = Guid.NewGuid().ToString() };

            zahtjev.Zaglavlje = zaglavlje;

            return zahtjev;
        }

        /// <summary>
        /// Vraća JIR iz XML dokumenta koji sadrži odgovor (RacunOdgovor) na RacunZahtjev.</summary>
        /// <param name="dokument">RacunOdgovor koji je vratio CIS.</param>
        /// <example>
        /// string jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
        /// </example>
        /// <returns>
        /// Vraća JIR.</returns>
        public static string DohvatiJir(XmlDocument dokument)
        {
            string jir = "";

            if (dokument != null)
            {
                XmlNamespaceManager nsmgr;
                DodajNamespace(dokument, out nsmgr);
                XmlElement root = dokument.DocumentElement;

                XmlNode node = root.SelectSingleNode("soap:Body/tns:RacunOdgovor/tns:Jir", nsmgr);

                if (node != null)
                    jir = node.InnerText;
            }

            return jir;
        }

        /// <summary>
        /// Vraća UUID iz XML dokumenta.</summary>
        /// <param name="dokument">XML dokument prema XSD schemi zajedno sa SOAP omotnicom.</param>
        /// <param name="tipDokumenta">Tip dokumenta, npr. RacunZahtjev, RacunOdgovor i sl.</param>
        /// <returns>
        /// Vraća UUID.</returns>
        public static string DohvatiUuid(XmlDocument dokument, TipDokumentaEnum tipDokumenta)
        {
            string uuid = "";

            if (dokument != null)
            {
                XmlNamespaceManager nsmgr;
                DodajNamespace(dokument, out nsmgr);
                XmlElement root = dokument.DocumentElement;

                XmlNode node = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Zaglavlje/tns:IdPoruke", tipDokumenta), nsmgr);

                if (node != null)
                    uuid = node.InnerText;
            }

            return uuid;
        }

        /// <summary>
        /// Vraća šifru greške iz XML dokumenta koji sadrži odgovor.</summary>
        /// <param name="dokument">XML dokument primljen kao odgovor iz CIS-a.</param>
        /// <param name="tipDokumenta">Tip dokumenta, npr. RacunOdgovor, PoslovniProstorOdgovor i sl.</param>
        /// <returns>
        /// Vraća šifru greške (Pogledati poglavlje 2.4. Šifrarnik grešaka u Tehničkoj specifikaciji).</returns>
        public static string DohvatiSifruGreske(XmlDocument dokument, TipDokumentaEnum tipDokumenta)
        {
            string jir = "";

            if (dokument != null)
            {
                XmlNamespaceManager nsmgr;
                DodajNamespace(dokument, out nsmgr);
                XmlElement root = dokument.DocumentElement;

                XmlNode node = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Greske/tns:Greska/tns:SifraGreske", tipDokumenta), nsmgr);

                if (node != null)
                    jir = node.InnerText;
            }

            return jir;
        }

        /// <summary>
        /// Vraća poruku greške iz XML dokumenta koji sadrži odgovor.</summary>
        /// <param name="dokument">XML dokument primljen kao odgovor iz CIS-a.</param>
        /// <param name="tipDokumenta">Tip dokumenta, npr. RacunOdgovor, PoslovniProstorOdgovor i sl.</param>
        /// <returns>
        /// Vraća poruku greške (Pogledati poglavlje 2.4. Šifrarnik grešaka u Tehničkoj specifikaciji).</returns>
        public static string DohvatiPorukuGreske(XmlDocument dokument, TipDokumentaEnum tipDokumenta)
        {
            string jir = "";

            if (dokument != null)
            {
                XmlNamespaceManager nsmgr;
                DodajNamespace(dokument, out nsmgr);
                XmlElement root = dokument.DocumentElement;

                XmlNode node = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Greske/tns:Greska/tns:PorukaGreske", tipDokumenta), nsmgr);

                if (node != null)
                    jir = node.InnerText;
            }

            return jir;
        }

        /// <summary>
        /// Vraća sve greške iz XML dokumenta koji sadrži odgovor.</summary>
        /// <param name="dokument">XML dokument primljen kao odgovor iz CIS-a.</param>
        /// <returns>
        /// Vraća poruku greške (Pogledati poglavlje 2.4. Šifrarnik grešaka u Tehničkoj specifikaciji).</returns>
        public static string DohvatiGreskuRezultataZahtjeva(XmlDocument OdgovorGreska)
        {
            // prema sugestiji hebos: http://fiskalizacija.codeplex.com/workitem/672

            XmlNamespaceManager nsmgr;

            TipDokumentaEnum tipDokumenta = XmlDokumenti.OdrediTipDokumenta(OdgovorGreska);
            XmlDokumenti.DodajNamespace(OdgovorGreska, out nsmgr);
            XmlElement root = OdgovorGreska.DocumentElement;
            XmlNode bodyNode = root.SelectSingleNode("soap:Body", nsmgr);
            XmlNode documentNode = null;

            if (bodyNode.HasChildNodes)
            {
                if (tipDokumenta == TipDokumentaEnum.EchoOdgovor)
                {
                    documentNode = bodyNode.SelectSingleNode("tns:EchoResponse", nsmgr);
                }
                else if (tipDokumenta == TipDokumentaEnum.RacunOdgovor)
                {
                    documentNode = bodyNode.SelectSingleNode("tns:RacunOdgovor", nsmgr);
                }
                else if (tipDokumenta == TipDokumentaEnum.PoslovniProstorOdgovor)
                {
                    documentNode = bodyNode.SelectSingleNode("tns:PoslovniProstorOdgovor", nsmgr);
                }
            }

            string ErrorCollection = "";

            if (documentNode != null)
            {
                XmlNodeList ErrorList = documentNode.SelectNodes("tns:Greske", nsmgr);

                foreach (XmlNode NextError in ErrorList)
                {
                    ErrorCollection += String.Format("{0} Šifra greške: {1}\n", NextError.FirstChild.SelectSingleNode("tns:PorukaGreske", nsmgr).InnerText, NextError.FirstChild.SelectSingleNode("tns:SifraGreske", nsmgr).InnerText);
                }

                return ErrorCollection;

            }
            else
            {
                return "Nepoznata greška";
            }

        }

        /// <summary>
        /// Snima XML dokument u datoteku.</summary>
        /// <param name="dokument">XML dokument koji se snima u datoteku.</param>
        /// <param name="nazivDatoteke">Naziv datoteke u koju se XML snima.</param>
        /// <example>
        /// Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(zahtjevXml, @"D:\Users\Nino\Desktop\Zahtjev.xml");
        /// </example>
        public static void SnimiXmlDokumentDatoteka(XmlDocument dokument, string nazivDatoteke)
        {
            if (dokument != null)
            {
                try
                {
                    dokument.Save(nazivDatoteke);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(String.Format("Greška kod snimanja XML dokumenta u datoteku: {0}", ex.Message));
                    throw;
                }

            }
        }
        #endregion


        #region Poslovni prostor
        /// <summary>
        /// Koristi se za serijalizaciju XML-a zahtjeva.</summary>
        /// <param name="PoslovniProstorZahtjev">Objekt tipa Schema.PoslovniProstorZahtjev koji sadrži PoslovniProstorZahtjev.</param>
        /// <example>
        /// Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
        /// XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML zahtjeva. U slučaju greške vraća null.</returns>
        public static XmlDocument SerijalizirajPoslovniProstorZahtjev(Schema.PoslovniProstorZahtjev poslovniProstorZahtjev)
        {
            string xml = "";

            try
            {
                xml = poslovniProstorZahtjev.Serialize();
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod serijalizacije zahtjeva za poslovni prostor: {0}", ex.Message));
                throw;
            }

            XmlDocument doc = UcitajXml(xml);


            return doc;
        }

        /// <summary>
        /// Koristi se za serijalizaciju poslovnog prostora.</summary>
        /// <param name="poslovniProstor">Objekt koji treba serijalizirati</param>
        /// <example>
        ///  
        ///  
        /// </example>
        /// <returns>
        /// Vraća serijalizirani dokument, kao XML. U slučaju greške vraća null.</returns>
        public static XmlDocument SerijalizirajPoslovniProstor(Schema.PoslovniProstorType poslovniProstor)
        {
            string xml = "";

            try
            {
                xml = poslovniProstor.Serialize();
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod serijalizacije poslovnog prostora: {0}", ex.Message));
                throw;
            }

            XmlDocument doc = UcitajXml(xml);


            return doc;
        }

        /// <summary>
        /// Kreira PoslovniProstorZahtjev.</summary>
        /// <remarks>
        /// Namjena je ove metode da doda zaglavlje zahtjevu.
        /// </remarks>
        /// <param name="poslovniProstor">PoslovniProstor kojem treba dodati dio vezan uz zahtjev.</param>
        /// <example>
        ///  Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
        /// </example>
        /// <returns>
        /// Vraća PoslovniProstorZahtjev.</returns>
        public static Schema.PoslovniProstorZahtjev KreirajPoslovniProstorZahtjev(Schema.PoslovniProstorType poslovniProstor)
        {
            Schema.PoslovniProstorZahtjev zahtjev = new Schema.PoslovniProstorZahtjev() { Id = "signXmlId", PoslovniProstor = poslovniProstor };

            Schema.ZaglavljeType zaglavlje = new Schema.ZaglavljeType() { DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), IdPoruke = Guid.NewGuid().ToString() };

            zahtjev.Zaglavlje = zaglavlje;

            return zahtjev;
        }

        public static Schema.PoslovniProstorZahtjev KreirajPoslovniProstorZahtjev(string ID, Schema.PoslovniProstorType poslovniProstor)
        {
            Schema.PoslovniProstorZahtjev zahtjev = new Schema.PoslovniProstorZahtjev() { Id = ID, PoslovniProstor = poslovniProstor };

            Schema.ZaglavljeType zaglavlje = new Schema.ZaglavljeType() { DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(), IdPoruke = Guid.NewGuid().ToString() };

            zahtjev.Zaglavlje = zaglavlje;

            return zahtjev;
        }
        #endregion


        #region Echo
        /// <summary>
        /// Kreira XML poruku zajedno sa SOAP envelop, koja je sprena za slanje ECHO web metodi.</summary>
        /// <param name="poruka">Tekst poruke koja se šalje, na primjer 'test' ili 'test poruka' ili sl. Ukoliko se radi o praznom stringu (""), tada će tekst poruke biti 'echo test'.</param>
        /// <example>
        /// XmlDocument echoZahtjev = PopratneFunkcije.XmlDokumenti.DohvatiPorukuEchoZahtjev(poruka);
        /// </example>
        /// <returns>
        /// Vraća XmlDocument sa XML zahtjeva, u slučaju greške vraća null.</returns>
        public static XmlDocument DohvatiPorukuEchoZahtjev(string poruka)
        {
            XmlDocument xml = null;

            Razno.FormatirajEchoPoruku(ref poruka);

            string soap = String.Format(@"<tns:EchoRequest xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.apisit.hr/fin/2012/types/f73/FiskalizacijaSchema.xsd"">{0}</tns:EchoRequest>", poruka);

            try
            {
                xml = new XmlDocument();
                xml.LoadXml(soap);
                XmlDokumenti.DodajSoapEnvelope(ref xml);
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod kreiranja poruke za ECHO zahtjev: {0}", ex.Message));
                throw;
            }


            return xml;
        }
        #endregion


        #region Ostalo
        /// <summary>
        /// Pomoćna metoda koja učitava XML u XmlDocument.</summary>
        /// <param name="xml">XML koji treba učitati.</param>
        /// <example>
        ///  XmlDocument doc = UcitajXml(xml);
        /// </example>
        /// <returns>
        /// Vraća XmlDocument ukoliko je sve uredu, inače vraća null.</returns>
        public static XmlDocument UcitajXml(string xml)
        {
            XmlDocument doc = null;

            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    doc = new XmlDocument();
                    doc.LoadXml(xml);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(String.Format("Greška kod učitavanja XML dokumenta: {0}", ex.Message));
                    throw;
                }
            }

            return doc;
        }

        /// <summary>
        /// Pomoćna metoda koja dodaje SOAP envelope proslijeđenom XML dokumentu.</summary>
        /// <param name="dokument">XML kojem treba nadodati SOAP envelope.</param>
        /// <example>
        ///  PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);
        /// </example>
        public static void DodajSoapEnvelope(ref XmlDocument dokument)
        {
            if (dokument != null && !string.IsNullOrEmpty(dokument.InnerXml) && dokument.DocumentElement != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchemainstance"">");
                sb.Append("<soapenv:Body>");
                sb.Append(String.Format("<{0}", dokument.DocumentElement.Name));
                for (int i = 0; i < dokument.DocumentElement.Attributes.Count; i++)
                {
                    sb.Append(String.Format(@" {0}=""{1}""", dokument.DocumentElement.Attributes[i].Name, dokument.DocumentElement.Attributes[i].Value));
                }
                sb.Append(">");
                sb.Append(dokument.DocumentElement.InnerXml);
                sb.Append(String.Format("</{0}>", dokument.DocumentElement.Name));
                sb.Append("</soapenv:Body>");
                sb.Append("</soapenv:Envelope>");

                string xml = sb.ToString();

                xml = xml.Replace(@"<tns:Zaglavlje xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"">", "<tns:Zaglavlje>");
                xml = xml.Replace(@"<tns:Racun xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"">", "<tns:Racun>");

                try
                {
                    dokument.LoadXml(xml);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(String.Format("Greška kod dodavanja SOAP envelop-a: {0}", ex.Message));
                    throw;
                }
            }
        }

        /// <summary>
        /// Pomoćna metoda koja dodaje odgovarajući Namespace XML dokumentu
        /// </summary>
        /// <param name="dokument">XML dokument, zajedno sa SOAP omotnicom, na primjer RacunZahtjev ili RacunOdgovor</param>
        /// <param name="nsmgr">XmlNamespaceManager</param>
        public static void DodajNamespace(XmlDocument dokument, out XmlNamespaceManager nsmgr)
        {
            nsmgr = new XmlNamespaceManager(dokument.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73");
        }
        /// <summary>
        /// Pomoćna metoda koja određuje tip XML dokumenta
        /// </summary>
        /// <param name="dokument">XML dokument prema XSD schemi, zajedno sa SOAP omotnicom</param>
        /// <returns>Tip dokumenta</returns>
        public static TipDokumentaEnum OdrediTipDokumenta(XmlDocument dokument)
        {
            TipDokumentaEnum tipDokumenta = TipDokumentaEnum.Nepoznato;

            if (dokument != null)
            {
                XmlNamespaceManager nsmgr;
                XmlDokumenti.DodajNamespace(dokument, out nsmgr);

                if (dokument.DocumentElement != null)
                {
                    XmlElement root = dokument.DocumentElement;
                    XmlNode node = root.SelectSingleNode("soap:Body", nsmgr);

                    if (node != null && node.HasChildNodes)
                    {
                        switch (node.FirstChild.Name)
                        {
                            case "tns:EchoRequest":
                                tipDokumenta = TipDokumentaEnum.EchoZahtjev;
                                break;
                            case "tns:EchoResponse":
                                tipDokumenta = TipDokumentaEnum.EchoOdgovor;
                                break;
                            case "tns:RacunZahtjev":
                                tipDokumenta = TipDokumentaEnum.RacunZahtjev;
                                break;
                            case "tns:RacunOdgovor":
                                tipDokumenta = TipDokumentaEnum.RacunOdgovor;
                                break;
                            case "tns:PoslovniProstorZahtjev":
                                tipDokumenta = TipDokumentaEnum.PoslovniProstorZahtjev;
                                break;
                            case "tns:PoslovniProstorOdgovor":
                                tipDokumenta = TipDokumentaEnum.PoslovniProstorOdgovor;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return tipDokumenta;
        }
        #endregion


    }
}
