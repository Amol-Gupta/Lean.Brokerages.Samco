using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using QuantConnect.Data;
using QuantConnect.Brokerages.Samco.SamcoMessages;
using Newtonsoft.Json;
using RestSharp;

namespace QuantConnect.Brokerages.Samco
{
    public partial class SamcoBrokerageRestAPIClient 
    {
        /// <summary>
        /// Get intraday candle data for instruments other than index
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="fromDate"></param>
        /// <param name="exchange"></param>
        /// <param name="toDate"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public intradayCandleDataResponse GetIntradayCandleData(string symbolName, string fromDate, string exchange = "NSE", string toDate = null, string interval = null)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "/intraday/candleData"), Method.GET);
            request.AddParameter("symbolName", symbolName);
            request.AddParameter("fromDate", fromDate);
            if (exchange != null)
                request.AddParameter("exchange", exchange);
            if (toDate != null)
                request.AddParameter("toDate", toDate);
            if (interval != null)
                request.AddParameter("interval", interval);

            var response = ExecuteRestRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.Authorize: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }


            var _intradayCandleDataResponse = JsonConvert.DeserializeObject<intradayCandleDataResponse>(response.Content);
            return _intradayCandleDataResponse;
        }


        /// <summary>
        /// Get intraday candle data for index
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public indexIntradayCandleDataResponse GetIndexIntradayCandleData(string indexName, string fromDate, string toDate = null, string interval = null)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "/intraday/indexCandleData"), Method.GET);
            request.AddParameter("indexName", indexName);
            request.AddParameter("fromDate", fromDate);
            if (toDate != null)
                request.AddParameter("toDate", toDate);
            if (interval != null)
                request.AddParameter("interval", interval);
            var response = ExecuteRestRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.Authorize: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _indexIntradayCandleDataResponse = JsonConvert.DeserializeObject<indexIntradayCandleDataResponse>(response.Content);
            return _indexIntradayCandleDataResponse;
        }
        public historicalCandleDataResponse getHistoricalCandleData(string symbolName, string fromDate, string exchange = "NSE", string toDate = null)
        {

            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "/history/candleData"), Method.GET);
            request.AddParameter("symbolName", symbolName);
            request.AddParameter("fromDate", fromDate);
            if (exchange != null)
                request.AddParameter("exchange", exchange);
            if (toDate != null)
                request.AddParameter("toDate", toDate);
            var response = ExecuteRestRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.Authorize: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }

            var _historicalCandleDataResponse = JsonConvert.DeserializeObject<historicalCandleDataResponse>(response.Content);
            return _historicalCandleDataResponse;
        }



        /// <summary>
        /// Gets the Index historical candle data at daily time scale such as Open, high,
        /// low, close, last traded price and volume within specific dates for a specific
        /// index. From date is mandatory. End date is optional and defaults to Today.
        /// </summary>
        /// <param name="indexName"> </param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public indexHistoricalCandleDataResponse getIndexHistoricalCandleData(string indexName, string fromDate, string toDate = null)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "/history/indexCandleData"), Method.GET);
            request.AddParameter("indexName", indexName);
            request.AddParameter("fromDate", fromDate);
            if (toDate != null)
                request.AddParameter("toDate", toDate);
            var response = ExecuteRestRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    $"SamcoBrokerage.Authorize: request failed: [{(int)response.StatusCode}] {response.StatusDescription}, Content: {response.Content}, ErrorMessage: {response.ErrorMessage}"
                );
            }
            var _indexHistoricalCandleDataResponse = JsonConvert.DeserializeObject<indexHistoricalCandleDataResponse>(response.Content);
            return _indexHistoricalCandleDataResponse;
        }


        
        public IEnumerable<BaseData> GetHistory(HistoryRequest request) {

            // Samco API only allows us to support history requests for TickType.Trade
            if (request.TickType != TickType.Trade)
            {
                throw new Exception($"Samco API only allows us to support history requests for TickType.Trade" );
            }

            if (request.Symbol.SecurityType != SecurityType.Equity && request.Symbol.SecurityType != SecurityType.Future &&
                request.Symbol.SecurityType != SecurityType.Option && request.Symbol.SecurityType != SecurityType.Index &&
                request.Symbol.SecurityType != SecurityType.IndexOption)
            {
                throw new Exception($"InvalidSecurityType {request.Symbol.SecurityType} security type not supported, no history returned");
            }

            if (request.Resolution == Resolution.Tick || request.Resolution == Resolution.Second)
            {
                throw new Exception($"InvalidResolution {request.Resolution} resolution not supported, no history returned");
            }

            if (request.StartTimeUtc >= request.EndTimeUtc)
            {
                throw new Exception($"InvalidDateRange The history request start date must precede the end date, no history returned");
                
            }

            if (!(
                (request.Resolution == Resolution.Minute) ||
                (request.Resolution == Resolution.Hour) ||
                (request.Resolution == Resolution.Daily)
                ))
            {
                throw new ArgumentException($"SamcoBrokerage.ConvertResolution: Unsupported resolution type: {request.Resolution}");
            }

            var leanSymbol = request.Symbol;

            var brokerageSymbol = _symbolMapper.GetBrokerageSymbol(leanSymbol);
            var securityExchange = _securityProvider.GetSecurity(leanSymbol).Exchange;
            var exchange = _symbolMapper.GetExchange(leanSymbol);
            var isIndex = leanSymbol.SecurityType == SecurityType.Index;

            string _fromDateStr;
            string _toDateStr;
            TimeSpan _resolutionTimeSpan;
            IEnumerable<BaseData> _data;
            switch (request.Resolution)
            {
                case Resolution.Minute:_resolutionTimeSpan = TimeSpan.FromMinutes(1); break;
                case Resolution.Hour:_resolutionTimeSpan = TimeSpan.FromHours(1); break;
                case Resolution.Daily:_resolutionTimeSpan = TimeSpan.FromDays(1); break;
                default:throw new Exception($"unsupported resolution {request.Resolution}");
            }

            if (isIndex && request.Resolution == Resolution.Daily) {
                _fromDateStr = request.StartTimeUtc.ToString("yyyy-MM-dd");
                _toDateStr = request.EndTimeUtc.ToString("yyyy-MM-dd");
                _data = getIndexHistoricalCandleData(brokerageSymbol, _fromDateStr, _toDateStr).toBaseData(leanSymbol,_resolutionTimeSpan);
            }
            else if (isIndex && (request.Resolution == Resolution.Minute || request.Resolution == Resolution.Hour)) {
                _fromDateStr = request.StartTimeUtc.ToString("yyyy-MM-dd hh:mm:ss");
                _toDateStr = request.EndTimeUtc.ToString("yyyy-MM-dd hh:mm:ss");
                _data = GetIndexIntradayCandleData(brokerageSymbol, _fromDateStr, _toDateStr).toBaseData(leanSymbol, _resolutionTimeSpan);
            }
            else if (!isIndex && request.Resolution == Resolution.Daily) {
                _fromDateStr = request.StartTimeUtc.ToString("yyyy-MM-dd");
                _toDateStr = request.EndTimeUtc.ToString("yyyy-MM-dd");
                _data = getHistoricalCandleData(brokerageSymbol, _fromDateStr, _toDateStr).toBaseData(leanSymbol, _resolutionTimeSpan);
            }
            else if (!isIndex &&  (request.Resolution == Resolution.Minute || request.Resolution == Resolution.Hour)) {
                _fromDateStr = request.StartTimeUtc.ToString("yyyy-MM-dd hh:mm:ss");
                _toDateStr = request.EndTimeUtc.ToString("yyyy-MM-dd hh:mm:ss");
                _data = GetIndexIntradayCandleData(brokerageSymbol, _fromDateStr, _toDateStr).toBaseData(leanSymbol, _resolutionTimeSpan);
            }
            else { throw new Exception("unspported combination of resolution and instrument"); }
            return _data;
        }

    }
}