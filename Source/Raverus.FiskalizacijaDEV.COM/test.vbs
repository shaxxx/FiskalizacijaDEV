Dim o

Set o = CreateObject("Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav")
msgbox o.PosaljiEcho("")


Dim xml

xml="<?xml version=""1.0"" encoding=""utf-8""?><tns:RacunZahtjev xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Id=""signXmlId"" xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73""><tns:Zaglavlje><tns:IdPoruke>cff93023-850b-403c-ac8b-277619e81dc9</tns:IdPoruke><tns:DatumVrijeme>21.10.2012T10:10:28</tns:DatumVrijeme></tns:Zaglavlje><tns:Racun><tns:Oib>44718633471</tns:Oib><tns:USustPdv>true</tns:USustPdv><tns:DatVrijeme>21.10.2012T10:10:22</tns:DatVrijeme><tns:OznSlijed>P</tns:OznSlijed><tns:BrRac><tns:BrOznRac>1</tns:BrOznRac><tns:OznPosPr>123</tns:OznPosPr><tns:OznNapUr>1</tns:OznNapUr></tns:BrRac><tns:Pdv><tns:Porez><tns:Stopa>25.00</tns:Stopa><tns:Osnovica>10.00</tns:Osnovica><tns:Iznos>2.50</tns:Iznos></tns:Porez></tns:Pdv><tns:IznosUkupno>12.50</tns:IznosUkupno><tns:NacinPlac>G</tns:NacinPlac><tns:OibOper>12345678900</tns:OibOper><tns:ZastKod>e4d909c290d0fb1ca068ffaddf22cbd0</tns:ZastKod><tns:NakDost>false</tns:NakDost></tns:Racun></tns:RacunZahtjev>"


Dim odgovor

odgovor = o.PosaljiSoapPoruku(xml,"FISKAL 1")
msgbox odgovor


dim jir

jir = o.DohvatiJir(odgovor)
msgbox jir



odgovor = o.PosaljiSoapPorukuCertifikatDatoteka(xml,"D:\Users\Nino\Desktop\Fiskalizacija\MojCert.pfx","PASSWORD")
msgbox odgovor


jir = o.DohvatiJir(odgovor)
msgbox jir



Dim potpisaniXml

potpisaniXml=o.PotpisiXmlDokument(xml,"FISKAL 1")
msgbox potpisaniXml



Dim soap

soap=o.DodajSoapEnvelope(potpisaniXml)
msgbox soap

odgovor=o.PosaljiPotpisanuSoapXmlPoruku(soap)
msgbox odgovor
msgbox o.DohvatiUuid(odgovor,"RacunOdgovor")


Dim zki

zki=o.ZastitniKodIzracun("FISKAL 1","12345678901","21.10.2012 10:10:28","1","1","1","125.00")
msgbox zki


msgbox "Status: " + o.DohvatiStatusCisServisa()


msgbox "UUID: " + o.GenerirajUuid()




Dim xmlError

xmlError="<?xml version=""1.0"" encoding=""utf-8""?><tns:RacunZahtjev xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Id=""signXmlId"" xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73""><tns:Zaglavlje><tns:IdPoruke>cff93023-850b-403c-ac8b-277619e81dc9</tns:IdPoruke><tns:DatumVrijeme>21.10.2012T10:10:28</tns:DatumVrijeme></tns:Zaglavlje><tns:Racun><tns:Oib>44718633471</tns:Oib><tns:USustPdv>true_ERROR_JE_OVDJE</tns:USustPdv><tns:DatVrijeme>21.10.2012T10:10:22</tns:DatVrijeme><tns:OznSlijed>P</tns:OznSlijed><tns:BrRac><tns:BrOznRac>1</tns:BrOznRac><tns:OznPosPr>123</tns:OznPosPr><tns:OznNapUr>1</tns:OznNapUr></tns:BrRac><tns:Pdv><tns:Porez><tns:Stopa>25.00</tns:Stopa><tns:Osnovica>10.00</tns:Osnovica><tns:Iznos>2.50</tns:Iznos></tns:Porez></tns:Pdv><tns:IznosUkupno>12.50</tns:IznosUkupno><tns:NacinPlac>G</tns:NacinPlac><tns:OibOper>12345678900</tns:OibOper><tns:ZastKod>e4d909c290d0fb1ca068ffaddf22cbd0</tns:ZastKod><tns:NakDost>false</tns:NakDost></tns:Racun></tns:RacunZahtjev>"


Dim odgovorErr

On Error Resume Next
Err.Clear
odgovorErr = o.PosaljiSoapPoruku(xmlError,"FISKAL 1")
If Err.Number <> 0 Then
	msgbox Err.Number
	msgbox odgovorErr
	msgbox o.OdgovorGreska
	msgbox o.OdgovorGreskaStatus
	msgbox o.DohvatiGresku(o.OdgovorGreska)
End If
On Error Goto 0	


Err.Clear
On Error Resume Next


o.TimeOut=10
odgovor=o.PosaljiSoapPoruku(xml,"FISKAL 1")
If Err.Number <> 0 Then
	msgbox Err.Number
	msgbox o.OdgovorGreskaStatus

End If
On Error Goto 0

