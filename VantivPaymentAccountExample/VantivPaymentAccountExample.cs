using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vantiv.ElementExpress.CertServicesUtil;

namespace VantivPaymentAccountExample
{
    [TestFixture]
    public class VantivPaymentAccountExample
    {
        private ExpressSoapClient _proxy;
        private string _acceptorId;
        private string _accountId;
        private string _accountToken;
        private string _applicationId;
        private string _applicationName;
        private string _applicationVersion;

        [SetUp]
        public void Setup()
        {
            var binding = new System.ServiceModel.BasicHttpBinding();
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
            binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;

            var endpoint = new System.ServiceModel.EndpointAddress(@"https://certservices.elementexpress.com/express.asmx");

            _proxy = new ExpressSoapClient(binding, endpoint);

            //BPC UAT integration
            //creds
            _acceptorId = "3928907";
            _accountId = "1009580";
            _accountToken = "";
            //app
            _applicationId = "1331";
            _applicationName = "HME Bill Pay";
            _applicationVersion = "2.0.0";
        }

        Credentials GetCreds()
        {
            return new Credentials
            {
                AcceptorID = _acceptorId,
                AccountID = _accountId,
                AccountToken = _accountToken
            };
        }

        Application GetApplication()
        {
            return new Application
            {
                ApplicationID = _applicationId,
                ApplicationName = _applicationName,
                ApplicationVersion = _applicationVersion
            };
        }

        [TestCase]
        public void MakePaymentAccountAndLookItUp()
        {
            var ptAcctNo = 1234;

            var address = new Address
            {
                BillingName = "somebody guy",
                BillingZipcode = "33913"
            };

            var paymentAccount = new PaymentAccount
            {
                PaymentAccountReferenceNumber = $"EV{ptAcctNo}_{DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")}"
            };

            var acctCreateResp = _proxy.PaymentAccountCreate(
               GetCreds(),
               GetApplication(),
               paymentAccount,
               new Card
               {
                   CardNumber = "4895281000000006",
                   CVV = "123",
                   CardholderName = "somebody guy",
                   ExpirationMonth = "12",
                   ExpirationYear = "25"
               },
               new DemandDepositAccount { },
               address,
               new ExtendedParameters[] { }
               );

            Assert.That(acctCreateResp.ExpressResponseCode, Is.EqualTo("0"));
            Assert.That(acctCreateResp.ExpressResponseMessage, Is.EqualTo("PaymentAccount created"));

            var queryResp = _proxy.PaymentAccountQuery(
                GetCreds(),
                GetApplication(),
                new PaymentAccountParameters
                {
                    PaymentAccountID =
                        acctCreateResp.PaymentAccount.PaymentAccountID
                },
                new ExtendedParameters[] { }
                );

            var queryData = XDocument.Parse(queryResp.QueryData);

            Assert.That(queryData.Descendants("BillingName").FirstOrDefault()?.Value, Is.EqualTo(address.BillingName));
            Assert.That(queryData.Descendants("PaymentAccountReferenceNumber").FirstOrDefault()?.Value, Is.EqualTo(paymentAccount.PaymentAccountReferenceNumber));
        }
    }
}
