call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"

SvcUtil /out:%~dp0/CertTransaction.cs /tcv:Version35 /noconfig /n:*,Vantiv.ElementExpress.CertTransactionUtil https://certtransaction.elementexpress.com/express.asmx?WSDL

csc /t:library /optimize /out:%~dp0/Vantiv.ElementExpress.CertTransactionUtil.dll %~dp0/CertTransaction.cs

SvcUtil /out:%~dp0/CertReporting.cs /tcv:Version35 /noconfig /n:*,Vantiv.ElementExpress.CertReportingUtil https://certreporting.elementexpress.com/express.asmx?WSDL

csc /t:library /optimize /out:%~dp0/Vantiv.ElementExpress.CertReportingUtil.dll %~dp0/CertReporting.cs

SvcUtil /out:%~dp0/CertServices.cs /tcv:Version35 /noconfig /n:*,Vantiv.ElementExpress.CertServicesUtil https://certservices.elementexpress.com/express.asmx?WSDL

csc /t:library /optimize /out:%~dp0/Vantiv.ElementExpress.CertServicesUtil.dll %~dp0/CertServices.cs

del "%~dp0/*.cs"