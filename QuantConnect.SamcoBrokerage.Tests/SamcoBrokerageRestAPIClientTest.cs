using NUnit.Framework;
using QuantConnect.Brokerages;
using QuantConnect.Brokerages.Samco;
using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Securities;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuantConnect.Tests.Brokerages.Samco
{
    [TestFixture]
    public class SamcoBrokerageRestAPIClientTest
    {
        SamcoBrokerageRestAPIClient _client;
        [OneTimeSetUp]
        public void connectAndAuthenticate()
        {
            _client = new SamcoBrokerageRestAPIClient();
            var apiSecret = Config.Get("samco-client-password");
            var apiKey = Config.Get("samco-client-id");
            var yob = Config.Get("samco-year-of-birth");
            _client.Authorize(apiKey, apiSecret, yob);

        }

        [OneTimeTearDown]
        public void logout()
        {
            _client.logout();
        }

       
        [Ignore("Testcase only used during development"),Test]
        public void testLogin()
        {
            _client = new SamcoBrokerageRestAPIClient();
            var apiSecret = Config.Get("samco-client-password");
            var apiKey = Config.Get("samco-client-id");
            var yob = Config.Get("samco-year-of-birth");
            _client.Authorize(apiKey, apiSecret, yob);
        }
        

        //[TestCase("NIFTY")]
        //[TestCase("nifty")]
        //[TestCase("nifty50")]
        [TestCase("nifty 50")]
        [TestCase("NIFTY 50")]
        public void indexQuote(string indexName)
        {
            // get quote
            var resp = _client.GetIndexQuote(indexName);
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");
                
            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }


        [TestCase("SBIN")]
        public void equityQuote(string symbolName, string exchange="NSE")
        {
            // get quote
            var resp = _client.GetQuote(symbolName, exchange);
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");
                
            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }

        
        [TestCase("NIFTY")]
        [TestCase("BANKNIFTY")]
        [TestCase("TCS")]
        public void getOptionChain(string underlying )
        {
            var resp = _client.getOptionChain(underlying);
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");
                
            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }

        [TestCase("HDFC")]
        [TestCase("NIFTY")]
        [TestCase("NIFTY BANK")]
        [TestCase("TCS")]
        [TestCase("BAJFINANCE")]
        public void searchEquityDerivScrips(string searchSymbolName)
        {
            var resp = _client.searchEquityDerivScrips(searchSymbolName);
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");

            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }

        
        [Test]
        public void getUserLimit()
        {
            var resp = _client.GetUserLimits();
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");

            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }
        
        [Test]
        public void getHoldings()
        {
            var resp = _client.GetHoldings();
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");

            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }

        [Test]
        public void getTradeBookDetails()
        {
            var resp = _client.getTradebook();
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("search request executed successfully");

            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }


        [Test]
        public void getPositions()
        {
            var resp = _client.GetPositions();
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.status == "Success");
            if (resp.status == "Success")
            {
                Console.WriteLine("get Positions executed successfully");

            }
            Console.WriteLine(JsonConvert.SerializeObject(resp, Formatting.Indented));
            System.Threading.Thread.Sleep(1000);
        }

        

    }
}
