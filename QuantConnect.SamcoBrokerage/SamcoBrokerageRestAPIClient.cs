
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuantConnect.Brokerages.Samco.SamcoMessages;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Securities;
using QuantConnect.Util;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;

namespace QuantConnect.Brokerages.Samco
{
    public partial class SamcoBrokerageRestAPIClient : IDisposable
    {
        private readonly RateGate _restRateLimiter = new RateGate(10, TimeSpan.FromSeconds(1));
        private readonly string _tokenHeader = "x-session-token";
        private string _token = "";
        private SamcoSymbolMapper _symbolMapper;
        private ISecurityProvider _securityProvider;

        /// <summary>
        /// Constructor for Samco API
        /// </summary>
        public SamcoBrokerageRestAPIClient()

        {
            _symbolMapper = new SamcoSymbolMapper();
            
            RestClient = new RestClient("https://api.stocknote.com");
        }

        /// <summary>
        /// Gets the RestClient.
        /// </summary>
        /// <value>An instance of the RestClient</value>
        public IRestClient RestClient { get; }

        /// <summary>
        /// Samco API Token
        /// </summary>
        /// <returns>A Samco API Token</returns>
        public string SamcoToken => _token;

        private void SignRequest(IRestRequest request)
        {
            request.AddHeader(_tokenHeader, _token);
            request.AddHeader("Accept", "application/json");
        }

        /// <summary>
        /// If an IP address exceeds a certain number of requests per minute the 429 status code and
        /// JSON response {"error": "ERR_RATE_LIMIT"} will be returned
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IRestResponse ExecuteRestRequest(IRestRequest request)
        {
            const int maxAttempts = 10;
            var attempts = 0;

            IRestResponse response;
            SignRequest(request);
            do
            {
                if (!_restRateLimiter.WaitToProceed(TimeSpan.Zero))
                {
                    Log.Trace("Brokerage.OnMessage(): " + new BrokerageMessageEvent(BrokerageMessageType.Warning, "RateLimit",
                        "The API request has been rate limited. To avoid this message, please reduce the frequency of API calls."));

                    _restRateLimiter.WaitToProceed();
                }

                response = RestClient.Execute(request);
                // 429 status code: Too Many Requests
            } while (++attempts < maxAttempts && (int)response.StatusCode == 429);
            return response;
        }


        /// <summary>
        /// Authenticate yourself to Samco API.
        /// </summary>
        /// <param name="login">Your Samco User ID</param>
        /// <param name="password">Your Samco login Password</param>
        /// <param name="yob">Birth year as registered with Samco</param>
        public loginResponse Authorize(string userId, string password, string yob)
        {
            Log.Trace("SamcoBrokerageAPI.Authorize(): Getting new Token");
            var request = new RestRequest("/login", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new { userId = userId, password = password, yob = yob });
            /*
             
            request.AddParameter("userId", userId);
            request.AddParameter("password", password);
            request.AddParameter("yob", yob);
            */

            IRestResponse response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.Authorize: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _loginResponse = JsonConvert.DeserializeObject<loginResponse>(response.Content);
            _token = _loginResponse.sessionToken;
            return _loginResponse;
        }


        /// <summary>
        /// get the quote for an index
        /// </summary>
        /// <param name="indexName"> name of index </param>
        /// <returns>indexQuoteResponse</returns>
        /// <exception cref="Exception"></exception>
        public indexQuoteResponse GetIndexQuote(string indexName)
        {
            var request = new RestRequest("/quote/indexQuote", Method.GET);
            request.AddParameter("indexName", indexName);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetIndexQuote: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _indexQuoteResponse = JsonConvert.DeserializeObject<indexQuoteResponse>(response.Content);
            return _indexQuoteResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public QuoteResponse getQuote(string symbolName, string exchange)
        {
            var request = new RestRequest("/quote/getQuote", Method.GET);
            request.AddParameter("symbolName", symbolName);
            request.AddParameter("exchange", exchange);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.getQuote: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _QuoteResponse = JsonConvert.DeserializeObject<QuoteResponse>(response.Content);
            return _QuoteResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchSymbolName"></param>
        /// <param name="expiryDate"></param>
        /// <param name="strikePrice"></param>
        /// <param name="optionType"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public optionChainResponse getOptionChain(string searchSymbolName, string expiryDate = null, string strikePrice = null, string optionType = null, string exchange = "NSE")
        {
            var request = new RestRequest("/option/optionChain", Method.GET);
            request.AddParameter("searchSymbolName", searchSymbolName);
            request.AddParameter("exchange", exchange);

            if (expiryDate != null)
                request.AddParameter("expiryDate", expiryDate);
            if (strikePrice != null)
                request.AddParameter("strikePrice", strikePrice);
            if (optionType != null)
                request.AddParameter("optionType", optionType);

            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.getOptionChain: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _optionChainResponse = JsonConvert.DeserializeObject<optionChainResponse>(response.Content);
            return _optionChainResponse;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchSymbolName"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public eqDerivSearchResponse searchEquityDerivScrips(string searchSymbolName, string exchange = "NSE")
        {
            var request = new RestRequest("/eqDervSearch/search", Method.GET);
            request.AddParameter("exchange", exchange);
            request.AddParameter("searchSymbolName", searchSymbolName);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.searchEquityDerivScrips: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _eqDerivSearchResponse = JsonConvert.DeserializeObject<eqDerivSearchResponse>(response.Content);
            return _eqDerivSearchResponse;
        }



        /// <summary>
        /// Cancels the order, Invokes cancelOrder call from Samco api
        /// </summary>
        /// <returns>OrderResponse</returns>
        public SamcoOrderResponse CancelOrder(string orderID)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "order/cancelOrder?orderNumber={0}", orderID), Method.DELETE);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.CancelOrder: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _SamcoOrderResponse = JsonConvert.DeserializeObject<SamcoOrderResponse>(response.Content);
            return _SamcoOrderResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TradeBookResponse getTradebook()
        {
            var request = new RestRequest("/trade/tradeBook", Method.GET);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.getTradebook: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _TradeBookResponse = JsonConvert.DeserializeObject<TradeBookResponse>(response.Content);
            return _TradeBookResponse;
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _restRateLimiter.Dispose();
        }



        /// <summary>
        /// Gets HoldingsResponses which contains list of Holding Details, Invokes getHoldings call
        /// from Samco api
        /// </summary>
        /// <returns>HoldingsResponse</returns>
        public HoldingsResponse GetHoldings()
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "holding/getHoldings"), Method.GET);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetHoldings: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _holdingResponse = JsonConvert.DeserializeObject<HoldingsResponse>(response.Content);
            return _holdingResponse;
        }


        


        /// <summary>
        /// Gets orderbook from SamcoApi, Invokes orderBook call from Samco api
        /// </summary>
        /// <returns>OrderBookResponse</returns>
        public OrderBookResponse GetOrderBook()
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "order/orderBook"), Method.GET);
            var response = ExecuteRestRequest(request);
            if ((response.StatusCode != HttpStatusCode.OK) && (response.StatusDescription.Contains( "No Orders found") ))
            {
                
                throw new Exception(
                    $"SamcoBrokerage.GetOrderBook: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }


            var _orderBookResponse = JsonConvert.DeserializeObject<OrderBookResponse>(response.Content);
            return _orderBookResponse;
        }

        /// <summary>
        /// Gets Order Details, Invokes getOrderStatus call from Samco api
        /// </summary>
        /// <returns>OrderResponse</returns>
        public SamcoOrderResponse GetOrderDetails(string orderID)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "order/getOrderStatus?orderNumber={0}", orderID), Method.GET);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetOrderDetails: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _SamcoOrderResponse = JsonConvert.DeserializeObject<SamcoOrderResponse>(response.Content);
            return _SamcoOrderResponse;
        }

        /// <summary>
        /// Gets position details of the user (The details of equity, derivative, commodity,
        /// currency borrowed or owned by the user).
        /// </summary>
        /// <returns>PostionsResponse</returns>
        public PositionsResponse GetPositions(string positionType = "DAY")
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "position/getPositions?positionType={0}", positionType), Method.GET);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK && (response.StatusDescription.Contains("No Positions found")))
            {
                throw new Exception(
                    $"SamcoBrokerage.GetPositions: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var positionsReponse = JsonConvert.DeserializeObject<PositionsResponse>(response.Content);
            return positionsReponse;
        }

        /// <summary>
        /// Get quote for a given symbol.
        /// </summary>
        /// <param name="symbol">brokerage symbol</param>
        /// <param name="exchange">Exchange at which symbol is traded. Like NSE/BSE</param>
        public QuoteResponse GetQuote(string symbol, string exchange = "NSE")
        {
            string endpoint = $"/quote/getQuote?symbolName={HttpUtility.UrlEncode(symbol)}&exchange={exchange.ToUpperInvariant()}";
            var req = new RestRequest(endpoint, Method.GET);
            var response = ExecuteRestRequest(req);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetQuote: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _QuoteResponse = JsonConvert.DeserializeObject<QuoteResponse>(response.Content);
            return _QuoteResponse;
        }

        /// <summary>
        /// Gets User limits i.e. cash balances, Invokes getLimits call from Samco api
        /// </summary>
        /// <returns>UserLimitResponse</returns>
        public UserLimitResponse GetUserLimits()
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "limit/getLimits"), Method.GET);
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetUserLimits: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _userLimitResponse = JsonConvert.DeserializeObject<UserLimitResponse>(response.Content);
            return _userLimitResponse;
        }

        /// <summary>
        /// Modifies the order, Invokes modifyOrder call from Samco api
        /// </summary>
        /// <returns>OrderResponse</returns>
        public SamcoOrderResponse ModifyOrder(Order order)
        {
            var payload = new JsonObject
            {
                { "orderValidity", GetOrderValidity(order.TimeInForce) },
                { "quantity", Math.Abs(order.Quantity).ToString(CultureInfo.InvariantCulture) },
                { "orderType", ConvertOrderType(order.Type) },
                { "price", GetOrderPrice(order).ToString(CultureInfo.InvariantCulture) },
                { "triggerPrice", GetOrderTriggerPrice(order).ToString(CultureInfo.InvariantCulture) }
            };

            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "order/modifyOrder/{0}", order.Id), Method.PUT);
            request.AddJsonBody(payload.ToString());
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.ModifyOrder: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _orderResponse = JsonConvert.DeserializeObject<SamcoOrderResponse>(response.Content);
            return _orderResponse;
        }

        /// <summary>
        /// Places the order, Invokes PlaceOrder call from Samco api
        /// </summary>
        /// <returns>List of Order Details</returns>
        public SamcoOrderResponse PlaceOrder(Order order, string symbol, string exchange, string productType)
        {
            var payload = new JsonObject
            {
                { "exchange", exchange },
                { "orderValidity", GetOrderValidity(order.TimeInForce) },
                { "afterMarketOrderFlag", "NO" },
                { "productType", productType },
                { "symbolName", symbol },
                { "quantity", Math.Abs(order.Quantity).ToString(CultureInfo.InvariantCulture) },
                { "disclosedQuantity", Math.Abs(order.Quantity).ToString(CultureInfo.InvariantCulture) },
                { "transactionType", ConvertOrderDirection(order.Direction) },
                { "orderType", ConvertOrderType(order.Type) },
            };

            if (order.Type == OrderType.Market || order.Type == OrderType.StopMarket)
            {
                payload.Add("marketProtection", "2");
            }

            if (order.Type == OrderType.StopLimit || order.Type == OrderType.StopMarket || order.Type == OrderType.Limit)
            {
                payload.Add("triggerPrice", GetOrderTriggerPrice(order).ToString(CultureInfo.InvariantCulture));
            }
            if (GetOrderPrice(order).ToString(CultureInfo.InvariantCulture) != "0")
            {
                payload.Add("price", GetOrderPrice(order).ToString(CultureInfo.InvariantCulture));
            }
            var request = new RestRequest("order/placeOrder", Method.POST);
            request.AddJsonBody(payload.ToString());
            var response = ExecuteRestRequest(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.PlaceOrder: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var orderResponse = JsonConvert.DeserializeObject<SamcoOrderResponse>(response.Content);
            return orderResponse;
        }

        private static string ConvertOrderDirection(OrderDirection orderDirection)
        {
            if (orderDirection == OrderDirection.Buy || orderDirection == OrderDirection.Sell)
            {
                return orderDirection.ToString().ToUpperInvariant();
            }

            throw new NotSupportedException($"SamcoBrokerage.ConvertOrderDirection: Unsupported order direction: {orderDirection}");
        }

        private static string ConvertOrderType(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Limit:
                    return "L";

                case OrderType.Market:
                    return "MKT";

                case OrderType.StopMarket:
                    return "SL-M";

                default:
                    throw new NotSupportedException($"SamcoBrokerage.ConvertOrderType: Unsupported order type: {orderType}");
            }
        }

        /// <summary>
        /// Return a relevant price for order depending on order type Price must be positive
        /// </summary>
        /// <param name="order"></param>
        /// <returns>A price for order</returns>
        private static decimal GetOrderPrice(Order order)
        {
            switch (order.Type)
            {
                case OrderType.Limit:
                    return ((LimitOrder)order).LimitPrice;

                case OrderType.Market:
                    // Order price must be positive for market order too; refuses for price = 0
                    return 0;

                case OrderType.StopMarket:
                    return ((StopMarketOrder)order).StopPrice;
            }

            throw new NotSupportedException($"SamcoBrokerage.ConvertOrderType: Unsupported order type: {order.Type}");
        }

        /// <summary>
        /// Return a relevant price for order depending on order type Price must be positive
        /// </summary>
        /// <param name="order"></param>
        /// <returns>A trigger price for order</returns>
        private static decimal GetOrderTriggerPrice(Order order)
        {
            switch (order.Type)
            {
                case OrderType.Limit:
                    return ((LimitOrder)order).LimitPrice;

                case OrderType.Market:
                    // Order price must be positive for market order too; refuses for price = 0
                    return 0;

                case OrderType.StopMarket:
                    return ((StopMarketOrder)order).StopPrice;
            }

            throw new NotSupportedException($"SamcoBrokerage.ConvertOrderType: Unsupported order type: {order.Type}");
        }

        private static string GetOrderValidity(TimeInForce orderTimeforce)
        {
            if (orderTimeforce == TimeInForce.GoodTilCanceled || orderTimeforce == TimeInForce.Day)
            {
                return "DAY";
            }
            throw new NotSupportedException($"SamcoBrokerage.GetOrderValidity: Unsupported orderTimeforce: {orderTimeforce}");
        }

        public SamcoLogoutResponse logout()
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "/logout"), Method.DELETE);
            var response = ExecuteRestRequest(request);
            var logoutResponse = JsonConvert.DeserializeObject<SamcoLogoutResponse>(response.Content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.GetQuote: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            return logoutResponse;
        }

        
    }
}
