//Copyright (c) 2012. Raverus d.o.o.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Raverus.FiskalizacijaDEV.PopratneFunkcije
{
    /// <summary>
    /// Klasa koja sadrži pomoćne stvari.</summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class Razno
    {
        public static string FormatirajDatumVrijeme(DateTime datumVrijeme)
        {
            return String.Format("{0:dd.MM.yyyy}T{1}", datumVrijeme, datumVrijeme.ToString("HH:mm:ss"));
        }

        public static string FormatirajDatum(DateTime datum)
        {
            return String.Format("{0:dd.MM.yyyy}", datum);
        }

        public static string DohvatiFormatiranoTrenutnoDatumVrijeme()
        {
            return FormatirajDatumVrijeme(DateTime.Now);
        }

        public static void FormatirajEchoPoruku(ref string poruka)
        {
            if (string.IsNullOrEmpty(poruka))
                poruka = "echo test";
        }

        public static string ZastitniKodIzracun(X509Certificate2 certifikat, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            if (certifikat == null || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja))
                throw new ArgumentNullException();


            return ZKI(certifikat, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
        }

        public static string ZastitniKodIzracun(string certificateSubject, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            if (string.IsNullOrEmpty(certificateSubject) || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja))
                throw new ArgumentNullException();


            X509Certificate2 certificate = Potpisivanje.DohvatiCertifikat(certificateSubject);

            return ZKI(certificate, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
        }

        public static string ZastitniKodIzracun(string certifikatDatoteka, string zaporka, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            if (string.IsNullOrEmpty(certifikatDatoteka) || string.IsNullOrEmpty(zaporka) || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja))
                throw new ArgumentNullException();

            
            X509Certificate2 certificate = Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);

            return ZKI(certificate, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
        }

        public static DirectoryInfo GenerirajProvjeriMapu(string mapa)
        {
            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(mapa);
                if (!di.Exists)
                    di.Create();
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod kreiranja mape:{0}", ex.Message));
                throw;
            }

            return di;
        }

        public static ServiceStatusEnum DohvatiStatusCisServisa()
        {
            ServiceStatusEnum status = ServiceStatusEnum.unknown;

            try
            {
                StatusService.GetStatus ss = new StatusService.GetStatus();
                string result = ss.Status();

                switch (result)
                {
                    case "green":
                        status = ServiceStatusEnum.green;
                        break;
                    case "yellow":
                        status = ServiceStatusEnum.yellow;
                        break;
                    case "red":
                        status = ServiceStatusEnum.red;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod dohvata statusa:{0}", ex.Message));
                throw;
            }

            return status;
        }

        #region Private
        // Preuzeto sa http://www.codeproject.com/Articles/21312/Generating-MD5-Hash-out-of-C-Objects
        private static string ComputeHash(byte[] objectAsBytes)
        {
            //MD5 md5 =  new MD5CryptoServiceProvider();
            MD5 md5 = MD5.Create();
            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("x2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                //If something occurred during serialization, 
                //this method is called with a null argument. 
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

        private static string ZKI(X509Certificate2 certifikat, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            string zastitniKod;

            StringBuilder sb = new StringBuilder();
            sb.Append(oibObveznika);
            sb.Append(datumVrijemeIzdavanjaRacuna);
            sb.Append(brojcanaOznakaRacuna);
            sb.Append(oznakaPoslovnogProstora);
            sb.Append(oznakaNaplatnogUredaja);
            sb.Append(ukupniIznosRacuna.Replace(',', '.'));

            byte[] by = Potpisivanje.PotpisiTekst(sb.ToString(), certifikat);
            if (by != null)
                zastitniKod = ComputeHash(by);
            else
                zastitniKod = "";


            return zastitniKod;
        }
        #endregion
    }
}
