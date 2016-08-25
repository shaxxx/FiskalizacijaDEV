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
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Raverus.FiskalizacijaDEV.PopratneFunkcije
{
    /// <summary>
    /// Koristi se za rad sa certifikatom i potpisivanje XML poruke.</summary>
    /// <remarks>
    /// Ovo je klasa čija je namjena pomoć kod rada sa certifikatom, dohvat certifikata iz Certificate Store-a ili iz datoteke, potpisivanje XML poruka i sl.
    /// </remarks>
    public class Potpisivanje
    {
        #region Certifikat
        /// <summary>
        /// Dohvaća certifikat iz Certificate Store-a na osnovu naziva.</summary>
        /// <remarks>
        /// Certifikat se dohvaća iz StoreLocation.CurrentUser, StoreName.My.
        /// </remarks>
        /// <param name="certificateSubject">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <example>
        /// X509Certificate2 certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject);
        /// </example>
        /// <returns>
        /// Vraća dohvaćeni certifikat. U slučaju greške ili ukoliko certifikat nije pronađen, vraća null.</returns>
        public static X509Certificate2 DohvatiCertifikat(string certificateSubject)
        {
            return DohvatiCertifikat(certificateSubject, StoreLocation.CurrentUser, StoreName.My);
        }

        /// <summary>
        /// Dohvaća certifikat iz Certificate Store-a.</summary>
        /// <param name="certificateSubject">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <param name="storeLocation">Lokacija certificate store-a, na primjer StoreLocation.CurrentUser.</param>
        /// <param name="storeName">Naziv certifikate sotre-a, na primjer StoreName.My.</param>
        /// <example>
        /// DohvatiCertifikat(certificateSubject, StoreLocation.CurrentUser, StoreName.My);
        /// </example>
        /// <returns>
        /// Vraća dohvaćeni certifikat. U slučaju greške ili ukoliko certifikat nije pronađen, vraća null.</returns>
        public static X509Certificate2 DohvatiCertifikat(string certificateSubject, StoreLocation storeLocation, StoreName storeName)
        {
            X509Certificate2 certificate = null;

            X509Store certificateStore = new X509Store(storeName, storeLocation);
            certificateStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            foreach (X509Certificate2 item in certificateStore.Certificates)
            {
                if (item.Subject.StartsWith(String.Format("CN={0}", certificateSubject)))
                {
                    certificate = item;
                    break;
                }
            }

            return certificate;
        }

        /// <summary>
        /// Dohvaća certifikat iz PFX datoteke.</summary>
        /// <param name="certifikatDatoteka">Path i naziv (full path) datoteke u kojoj se nalazi certifikat.</param>
        /// <param name="zaporka">Zaporka koja se koristi za pristup certifikatu.</param>
        /// <example>
        /// 
        /// </example>
        /// <returns>
        /// Vraća dohvaćeni certifikat. U slučaju greške ili ukoliko certifikat nije pronađen, vraća null.</returns>
        public static X509Certificate2 DohvatiCertifikat(string certifikatDatoteka, string zaporka)
        {
            X509Certificate2 certificate = null;

            FileInfo fi = new FileInfo(certifikatDatoteka);
            if (fi.Exists)
            {
                try
                {
                    certificate = new X509Certificate2(certifikatDatoteka, zaporka);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(String.Format("Greška kod kreiranja certifikata: {0}", ex.Message));
                    throw;
                }
            }


            return certificate;
        }
        #endregion

        #region Potpisivanje
        /// <summary>
        /// Potpisuje XML dokument.</summary>
        /// <param name="dokument">XML dokument koji treba potpisati.</param>
        /// <param name="certifikat">Certifikat koji se koristi kod potpisivanja.</param>
        /// <example>
        /// PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certificate);
        /// </example>
        /// <returns>
        /// Vraća potpisani XMl dokument.</returns>
        public static XmlDocument PotpisiXmlDokument(XmlDocument dokument, X509Certificate2 certifikat)
        {
            RSACryptoServiceProvider provider = (RSACryptoServiceProvider)certifikat.PrivateKey;

            SignedXml xml = null;
            try
            {
                xml = new SignedXml(dokument);
                xml.SigningKey = provider;
                xml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;


                KeyInfo keyInfo = new KeyInfo();
                KeyInfoX509Data keyInfoData = new KeyInfoX509Data();
                keyInfoData.AddCertificate(certifikat);
                keyInfoData.AddIssuerSerial(certifikat.Issuer, certifikat.GetSerialNumberString());
                keyInfo.AddClause(keyInfoData);

                xml.KeyInfo = keyInfo;


                Reference reference = new Reference("");
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));
                reference.AddTransform(new XmlDsigExcC14NTransform(false));
                reference.Uri = "#signXmlId";
                xml.AddReference(reference);
                xml.ComputeSignature();

                XmlElement element = xml.GetXml();
                dokument.DocumentElement.AppendChild(element);
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod potpisivanja XML dokumenta: {0}", ex.Message));
                throw;
            }

            return dokument;
        }

        public static byte[] PotpisiTekst(string tekst, X509Certificate2 certifikat)
        {
            if (certifikat == null)
                throw new ArgumentNullException();


            byte[] potpisaniTekst = null;

            RSACryptoServiceProvider provider = (RSACryptoServiceProvider)certifikat.PrivateKey;


            try
            {
                byte[] by = Encoding.ASCII.GetBytes(tekst);
                potpisaniTekst = provider.SignData(by, new SHA1CryptoServiceProvider());
            }
            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod potpisivanja teksta: {0}", ex.Message));
                throw;
            }

            return potpisaniTekst;
        }
        #endregion

        #region Provjera potpisa
        /// <summary>
        /// Provjerava digitalni potpis na XML dokumentu vraćenom iz CIS-a.</summary>
        /// <param name="dokument">XML dokument za koji treba provjeriti digitalni potpis.</param>
        /// <example>
        /// PopratneFunkcije.Potpisivanje.ProvjeriPotpis(odgovorXml);
        /// </example>
        /// <returns>
        /// Vraća True ukoliko je potpis ispravan.</returns>
        public static Boolean ProvjeriPotpis(XmlDocument dokument)
        {
            // Prema sugestiji mladenbabic (http://fiskalizacija.codeplex.com/discussions/403288) 

            // Provjeri argument
            if (dokument == null)
                throw new ArgumentNullException();


            // Kreiraj novi SignedXml objekt i dostavi mu xmlDoc
            SignedXml potpisaniXml = new SignedXml(dokument);

            // Pronadji "Signature" nod(ove) i kreiraj XmlNodeList objekt
            XmlNodeList signatureNodeList = dokument.GetElementsByTagName("Signature");

            if (signatureNodeList.Count <= 0)
            {
                Trace.TraceError("Verifikacija nije uspjela: U primljenom dokumentu nije pronadjen digitalni potpis.");
                throw new CryptographicException("Verifikacija nije uspjela: U primljenom dokumentu nije pronadjen digitalni potpis.");
            }

            // Ucitaj nod u SignedXml objekt
            potpisaniXml.LoadXml((XmlElement)signatureNodeList[0]);

            // preuzmi dostavljeni certifikat
            X509Certificate2 certificate = null;
            foreach (KeyInfoClause clause in potpisaniXml.KeyInfo)
            {
                if (clause is KeyInfoX509Data)
                {
                    if (((KeyInfoX509Data)clause).Certificates.Count > 0)
                    {
                        certificate = (X509Certificate2)((KeyInfoX509Data)clause).Certificates[0];
                    }
                }
            }
            if (certificate == null)
            {
                Trace.TraceError("U primljenom XMLu nema certifikata.");
                throw new Exception("U primljenom XMLu nema certifikata.");
            }

            // Provjeri Signature i vrati bool rezultat
            Boolean reza = potpisaniXml.CheckSignature(certificate, true);
            return reza;

        }
        #endregion
    }
}
