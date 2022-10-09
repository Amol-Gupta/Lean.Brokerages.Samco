/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace QuantConnect.Brokerages.Samco.SamcoMessages
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class loginResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string sessionToken { get; set; }
        public string accountID { get; set; }
        public string accountName { get; set; }
        public string[] exchangeList { get; set; }
        public string[] orderTypeList { get; set; }
        public string[] productList { get; set; }
    }

    public class eqDerivSearchResponse
    {
        public class searchResult
        {
            public string exchange { get; set; }
            public string scripDescription { get; set; }
            public string tradingSymbol { get; set; }
            public string isin { get; set; }
            public string bodLotQuantity { get; set; }
            public decimal tickSize { get; set; }
            public string instrument { get; set; }
            public int quantityInLots { get; set; }
        }
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }

        
        public IList<searchResult> searchResults { get; set; }
    }

    

    public class HoldingsResponse
    {
        public class HoldingDetail
        {
            public decimal averagePrice { get; set; }
            public string exchange { get; set; }
            public string markToMarketPrice { get; set; }
            public decimal lastTradedPrice { get; set; }
            public string previousClose { get; set; }
            public string productCode { get; set; }
            public string symbolDescription { get; set; }
            public string tradingSymbol { get; set; }
            public string calculatedNetQuantity { get; set; }
            public decimal holdingsQuantity { get; set; }
            public string collateralQuantity { get; set; }
            public string holdingsValue { get; set; }
            public string ISIN { get; set; }
            public string sellableQuantity { get; set; }
            public string totalMarketToMarketPrice { get; set; }
        }

        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public IList<HoldingDetail> holdingDetails { get; set; }
    }


    public class intradayCandleDataResponse
    {
        public class IntradayCandleData
        {
            public string dateTime { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
            public string volume { get; set; }
        }
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public IntradayCandleData[] intradayCandleData { get; set; }

    }

    public class indexIntradayCandleDataResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public class IndexIntradayCandleData
        {
            public string dateTime { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
            public string volume { get; set; }
        }
        public IndexIntradayCandleData[] indexIntradayCandleData { get; set; }
    }



    public class historicalCandleDataResponse
    {
        public class HistoricalCandleData
        {
            public string date { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
            public string ltp { get; set; }
            public string volume { get; set; }
        }
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public HistoricalCandleData[] historicalCandleData { get; set; }
    }

    public class indexHistoricalCandleDataResponse
    {
        public class IndexHistoricalCandleData
        {
            public string date { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
            public string ltp { get; set; }
            public string volume { get; set; }
        }
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public IndexHistoricalCandleData[] indexHistoricalCandleData { get; set; }
    }

    //TODO: to be phased out with newer implementation of API
    public class CandleResponse
    {
        public class IntradayCandleData
        {
            public DateTime dateTime { get; set; }
            public decimal open { get; set; }
            public decimal high { get; set; }
            public decimal low { get; set; }
            public decimal close { get; set; }
            public decimal volume { get; set; }
        }

        public class HistoricalCandleData
        {
            public DateTime date { get; set; }
            public decimal open { get; set; }
            public decimal high { get; set; }
            public decimal low { get; set; }
            public decimal close { get; set; }
            public decimal volume { get; set; }
        }

        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public IList<IntradayCandleData> intradayCandleData { get; set; }
        public IList<HistoricalCandleData> historicalCandleData { get; set; }
    }

    public class SamcoOrderResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string orderStatus { get; set; }
        public string statusMessage { get; set; }
        public string exchangeOrderStatus { get; set; }
        public string rejectionReason { get; set; }
        public OrderDetails orderDetails { get; set; }
        public IList<string> validationErrors { get; set; }
    }

    public class indexQuoteResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }

        public string indexName { get; set; }
        public string listingId { get; set; }
        public string lastTradedTime { get; set; }
        public decimal lastTradedPrice { get; set; }
        public decimal spotPrice { get; set; }
        public decimal changePercentage { get; set; }
        public int lastTradedQuantity { get; set; }
        public decimal averagePrice { get; set; }
        public decimal openValue { get; set; }
        public decimal highValue { get; set; }
        public decimal lowValue { get; set; }
        public decimal closeValue { get; set; }
        public int totalBuyQuantity { get; set; }
        public int totalSellQuantity { get; set; }
        public decimal totalTradedValue { get; set; }
        public decimal totalTradedVolume { get; set; }
        public decimal openInterest { get; set; }
        public decimal getoIChangePer { get; set; }
    }


    public class optionChainResponse
    {
        public class Bid
        {
            public int number { get; set; }
            public int quantity { get; set; }
            public string price { get; set; }
        }
        public class Ask
        {
            public int number { get; set; }
            public int quantity { get; set; }
            public string price { get; set; }
        }
        public class optionChainDetail
        {
            public string tradingSymbol { get; set; }
            public string exchange { get; set; }
            public string symbol { get; set; }
            public string strikePrice { get; set; }
            public string expiryDate { get; set; }
            public string instrument { get; set; }
            public string optionType { get; set; }
            public string underLyingSymbol { get; set; }
            public string spotPrice { get; set; }
            public string lastTradedPrice { get; set; }
            public string openInterest { get; set; }
            public string openInterestChange { get; set; }
            public string oichangePer { get; set; }
            public  Bid[] bestBids { get; set; }
            public Ask[] bestAsks { get; set; }

        }

        public string serverTime { get; set; }

        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public optionChainDetail[] optionChainDetails { get; set; }
    }

    public class QuoteResponse
    {
        public class BestBid
        {
            public string number { get; set; }
            public string quantity { get; set; }
            public decimal price { get; set; }
        }

        public class BestAsk
        {
            public string number { get; set; }
            public string quantity { get; set; }
            public decimal price { get; set; }
        }

        public string serverTime { get; set; }

        public string msgId { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string tradingSymbol { get; set; }
        public string exchange { get; set; }
        public string companyName { get; set; }
        public string lastTradedTime { get; set; }
        public string lastTradedPrice { get; set; }
        public string previousClose { get; set; }
        public string changeValue { get; set; }
        public string changePercentage { get; set; }
        public string lastTradedQuantity { get; set; }
        public string lowerCircuitLimit { get; set; }
        public string upperCircuitLimit { get; set; }
        public string averagePrice { get; set; }
        public string openValue { get; set; }
        public string highValue { get; set; }
        public string lowValue { get; set; }
        public string closeValue { get; set; }
        public string totalBuyQuantity { get; set; }
        public string totalSellQuantity { get; set; }
        public string totalTradedValue { get; set; }
        public decimal totalTradedVolume { get; set; }
        public string yearlyHighPrice { get; set; }
        public string yearlyLowPrice { get; set; }
        public string tickSize { get; set; }
        public string openInterest { get; set; }
        public IList<BestBid> bestBids { get; set; }
        public IList<BestAsk> bestAsks { get; set; }
        public string expiryDate { get; set; }
        public string spotPrice { get; set; }
        public string instrument { get; set; }
        public string lotQuantity { get; set; }
        public string listingId { get; set; }
        public string openInterestChange { get; set; }
        public string getoIChangePer { get; set; }
    }

    public class AuthRequest
    {
        public string userId { get; set; }
        public string password { get; set; }
        public string yob { get; set; }
    }

    public class Subscription
    {
        public class Symbol
        {
            public string symbol { get; set; }
        }

        public class Data
        {
            public List<Symbol> symbols { get; set; } = new List<Symbol>();
        }

        public class Request
        {
            public string streaming_type { get; set; } = "quote";
            public Data data { get; set; } = new Data();
            public string request_type { get; set; } = "subscribe";
            public string response_format { get; set; } = "json";
        }

        public Request request { get; set; } = new Request();
    }

    public class QuoteUpdate
    {
        public class Data
        {
            public decimal aPr { get; set; }
            public decimal aSz { get; set; }
            public decimal avgPr { get; set; }
            public decimal bPr { get; set; }
            public decimal bSz { get; set; }
            public string c { get; set; }
            public string ch { get; set; }
            public string chPer { get; set; }
            public string h { get; set; }
            public string l { get; set; }
            public DateTime lTrdT { get; set; }
            public decimal ltp { get; set; }
            public decimal ltq { get; set; }
            public string ltt { get; set; }
            public string lttUTC { get; set; }
            public string o { get; set; }
            public string oI { get; set; }
            public string oIChg { get; set; }
            public string sym { get; set; }
            public string tBQ { get; set; }
            public string tSQ { get; set; }
            public string ttv { get; set; }
            public string vol { get; set; }
            public string yH { get; set; }
            public string yL { get; set; }
        }

        public class Response
        {
            public Data data { get; set; }
            public string streaming_type { get; set; }
        }

        public Response response { get; set; }
    }

    public class OrderDetails
    {
        public string pendingQuantity { get; set; }
        public string avgExecutionPrice { get; set; }
        public string orderPlacedBy { get; set; }
        public string tradingSymbol { get; set; }
        public string triggerPrice { get; set; }
        public string exchange { get; set; }
        public string totalQuantity { get; set; }
        public string expiry { get; set; }
        public string transactionType { get; set; }
        public string productType { get; set; }
        public string orderType { get; set; }
        public string quantity { get; set; }
        public string filledQuantity { get; set; }
        public string orderPrice { get; set; }
        public string filledPrice { get; set; }
        public string exchangeOrderNo { get; set; }
        public string orderValidity { get; set; }

        public string orderNumber { get; set; }

        public string orderStatus { get; set; }
        public string orderTime { get; set; }
    }

    public class OrderBookResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }

        public List<OrderDetails> orderBookDetails { get; set; }
    }

    public class UserLimitResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }

        
        public SegmentLimit equityLimit { get; set; }
        public SegmentLimit commodityLimit { get; set; }
    }

    public class SegmentLimit
    {
        public string grossAvailableMargin { get; set; }
        public decimal payInToday { get; set; }
        public decimal notionalCash { get; set; }
        public decimal collateralMarginAgainstShares { get; set; }
        public string marginUsed { get; set; }
        public string netAvailableMargin { get; set; }
    }

    public class ScripMaster
    {
        [Name("exchange")]
        public string Exchange { get; set; }

        [Name("exchangeSegment")]
        public string ExchangeSegment { get; set; }

        [Name("symbolCode")]
        public string SymbolCode { get; set; }

        [Name("tradingSymbol")]
        public string TradingSymbol { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("lastPrice")]
        public decimal LastPrice { get; set; }

        [Name("instrument")]
        public string Instrument { get; set; }

        [Name("lotSize")]
        public string LotSize { get; set; }

        [Name("strikePrice")]
        public string StrikePrice { get; set; }

        [Name("expiryDate")]
        public string ExpiryDate { get; set; }

        [Name("tickSize")]
        public string TickSize { get; set; }
    }

    public class PositionsResponse
    {
        [JsonProperty("serverTime")]
        public string serverTime { get; set; }

        [JsonProperty("msgId")]
        public string msgId { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("positionSummary")]
        public PositionSummary PositionSummary { get; set; }

        [JsonProperty("positionDetails")]
        public PositionDetail[] PositionDetails { get; set; }
    }

    public class PositionDetail
    {
        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("markToMarketPrice")]
        public string MarkToMarketPrice { get; set; }

        [JsonProperty("lastTradedPrice")]
        public string LastTradedPrice { get; set; }

        [JsonProperty("previousClose")]
        public string PreviousClose { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("tradingSymbol")]
        public string TradingSymbol { get; set; }

        [JsonProperty("calculatedNetQuantity")]
        public string CalculatedNetQuantity { get; set; }

        [JsonProperty("averageBuyPrice")]
        public string AverageBuyPrice { get; set; }

        [JsonProperty("averageSellPrice")]
        public string AverageSellPrice { get; set; }

        [JsonProperty("boardLotQuantity")]
        public long BoardLotQuantity { get; set; }

        [JsonProperty("boughtPrice")]
        public string BoughtPrice { get; set; }

        [JsonProperty("buyQuantity")]
        public long BuyQuantity { get; set; }

        [JsonProperty("carryForwardQuantity")]
        public long CarryForwardQuantity { get; set; }

        [JsonProperty("carryForwardValue")]
        public string CarryForwardValue { get; set; }

        [JsonProperty("multiplier")]
        public long Multiplier { get; set; }

        [JsonProperty("netPositionValue")]
        public string NetPositionValue { get; set; }

        [JsonProperty("netQuantity")]
        public long NetQuantity { get; set; }

        [JsonProperty("netValue")]
        public string NetValue { get; set; }

        [JsonProperty("positionType")]
        public string PositionType { get; set; }

        [JsonProperty("positionConversions")]
        public string[] PositionConversions { get; set; }

        [JsonProperty("soldValue")]
        public string SoldValue { get; set; }

        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("realizedGainAndLoss")]
        public string RealizedGainAndLoss { get; set; }

        [JsonProperty("unrealizedGainAndLoss")]
        public string UnrealizedGainAndLoss { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
    }

    public class PositionSummary
    {
        [JsonProperty("gainingTodayCount")]
        public long GainingTodayCount { get; set; }

        [JsonProperty("losingTodayCount")]
        public long LosingTodayCount { get; set; }

        [JsonProperty("totalGainAndLossAmount")]
        public string TotalGainAndLossAmount { get; set; }

        [JsonProperty("dayGainAndLossAmount")]
        public string DayGainAndLossAmount { get; set; }
    }

    public class SamcoLogoutResponse
    {
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
    }

    public class TradeBookResponse
    {
        public class tradeBookDetail
        {
            public string orderNumber { get; set; }
            public string exchange { get; set; }
            public string tradingSymbol { get; set; }
            public string symbolDescription { get; set; }
            public string transactionType { get; set; }
            public string productCode { get; set; }
            public string orderType { get; set; }
            public string orderPrice { get; set; }
            public string quantity { get; set; }
            public string disclosedQuantity { get; set; }
            public string triggerPrice { get; set; }
            public string marketProtection { get; set; }
            public string orderValidity { get; set; }
            public string orderStatus { get; set; }
            public string orderValue { get; set; }
            public string instrumentName { get; set; }
            public string orderTime { get; set; }
            public string userId { get; set; }
            public string filledQuantity { get; set; }
            public string unfilledQuantity { get; set; }
            public string exchangeConfirmationTime { get; set; }
            public string coverOrderPercentage { get; set; }
            public string exchangeOrderNumber { get; set; }
            public string tradeNumber { get; set; }
            public string tradePrice { get; set; }
            public string tradeDate { get; set; }
            public string tradeTime { get; set; }
            public string strikePrice { get; set; }
            public string optionType { get; set; }
            public string lastTradePrice { get; set; }
            public string expiry { get; set; }
        }
        public string serverTime { get; set; }
        public string msgId { get; set; }
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public tradeBookDetail[] tradeBookDetails { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
