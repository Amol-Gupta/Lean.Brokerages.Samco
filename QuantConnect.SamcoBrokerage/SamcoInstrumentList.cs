using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using QuantConnect.Brokerages.Samco.SamcoMessages;
using System.Collections.Generic;
using System;
using QuantConnect.Logging;
using System.Linq;

namespace QuantConnect.Brokerages.Samco
{
    public sealed class SamcoInstrumentList
    {

        private readonly object objectToLock = new object();
        private static readonly SamcoInstrumentList instance = new SamcoInstrumentList();
        private readonly string _getSymbolsEndpoint = "https://developers.stocknote.com/doc/ScripMaster.csv";
        private readonly TimeOnly updateReferenceTime=new TimeOnly(8,45);
        private DateTime lastUpdateDateTime;
        public  List<ScripMaster> _samcoTradableScripList ;
        public  List<Symbol> _leanSymbolList ;
        public  List<Symbol> _equityLeanSymbolList;
        public  List<ScripMaster> _equityScripList;
        private Dictionary<string, ScripMaster> _symbolCodetoScripMap ;
        private Dictionary<Symbol,string > _leanSymbolTosymbolCodeMap ;
        private Dictionary<string, Symbol> _symbolCodeToLeanSymbolMap ;


        // constructor
        protected SamcoInstrumentList()
        {
            _samcoTradableScripList=new List<ScripMaster>();
            _leanSymbolList=new List<Symbol>();
            _equityLeanSymbolList= new List<Symbol>();
            _equityScripList= new List<ScripMaster>();
            _symbolCodetoScripMap = new Dictionary<string, ScripMaster>();
            _leanSymbolTosymbolCodeMap= new Dictionary<Symbol, string>();
            _symbolCodeToLeanSymbolMap= new Dictionary<string, Symbol>();


            {
                updateData();
            }
        }
        public ScripMaster GetScripMasterFromSymbolCode(string symbolCode)
        {
            ScripMaster _scrip;
            if(_symbolCodetoScripMap.TryGetValue(symbolCode, out _scrip)) { return _scrip; } else
            {
                throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} failed to map/find symbolCode {symbolCode} to scripMaster Symbol");
            }
        }
        public string getTradingSymbolFromLeanSymbol(Symbol _sym)
        {
            string _symbolCode=getSymbolCodeFromLeanSymbol(_sym);
            return getTradingSymbolFromSymbolCode(_symbolCode);
        }
        public Symbol getLeanSymbolFromTradingSymbol(string tradingSymbol)
        {
            string symbolCode = getLeanSymbolFromTradingSymbol(tradingSymbol);
            return GetLeanSymbolFromSymbolCode(symbolCode);
        }
        public string getTradingSymbolFromSymbolCode(string symbolCode)
        {
            lock (objectToLock)
            {
                 ScripMaster _scrip;
                if(_symbolCodetoScripMap.TryGetValue(symbolCode, out _scrip)) {return _scrip.TradingSymbol;}
                else {throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)}: incorrect symbolCode. Failed to convert symbolcode {symbolCode} to corrosponding tradingsymbol");}
            }
        }
        public string getSymbolCodeFromLeanSymbol(Symbol leanSymbol)
        {
            lock (objectToLock)
            {
                string symbolCode;
                if(_leanSymbolTosymbolCodeMap.TryGetValue(leanSymbol, out symbolCode)) { return symbolCode; } 
                else { throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} Failed to convert Lean symbol{leanSymbol} to corrosponding symbolCode"); }
            }
        }
        public Symbol GetLeanSymbolFromSymbolCode(string symbolCode)
        {
            lock (objectToLock)
            {
                Symbol _symbol;
                if (_symbolCodeToLeanSymbolMap.TryGetValue(symbolCode, out _symbol)) { return _symbol; }
                else { throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} failed to convert symbolCode {symbolCode} to lean Symbol"); }
            }
        }

        private void checkForUpdate()
        {
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly updateDateOnly = DateOnly.FromDateTime(lastUpdateDateTime);
            TimeOnly updateTimeOnly = TimeOnly.FromDateTime(lastUpdateDateTime);
            if (!(todayDate==updateDateOnly && updateTimeOnly >= updateReferenceTime))
            {
                updateData();
            }
        }

        private void updateData()
        {
            lock (objectToLock)
            {
                _samcoTradableScripList?.Clear();
                _leanSymbolList?.Clear();
                _symbolCodetoScripMap?.Clear();
                _leanSymbolTosymbolCodeMap?.Clear();
                _symbolCodeToLeanSymbolMap?.Clear();

                string [] lines = _getSymbolsEndpoint.DownloadData().Split(Environment.NewLine);
                var Header = lines[0];
                var body = string.Join(Environment.NewLine, (
                            lines
                            .Skip(1)
                            .ToArray())); 
                String s;
                s =
@"
NSE,nse_index,-21,NIFTY,NIFTY 50,0,INDEX,1,,,0.01
NSE,nse_index,-22,BANKNIFTY,NIFTY BANk,0,INDEX,1,,,0.01
NSE,nse_index,-29,FINNIFTY,NIFTY FIN SERVICE,0,INDEX,1,,,0.01
";
                var csvString = (Header+s + body).Trim();
                TextReader sr = new StringReader(csvString);
                CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };
                var csv = new CsvReader(sr, configuration);
                var scrips = csv.GetRecords<ScripMaster>().ToList<ScripMaster>();
                sr.Close();
                foreach (var scrip in scrips)
                {
                    if ( (scrip.Exchange == "NSE" || scrip.Exchange == "NFO") &&
                        (
                        scrip.Instrument=="EQ" || scrip.Instrument == "FUTSTK" || scrip.Instrument == "OPTSTK" ||
                        scrip.Instrument == "INDEX" || scrip.Instrument=="FUTIDX" ||scrip.Instrument == "OPTIDX" 
                        ) 
                       )
                    {
                        Symbol _sym = CreateLeanSymbol(scrip);

                        _samcoTradableScripList.Add(scrip);
                        _leanSymbolList.Add(_sym);

                        _symbolCodetoScripMap[scrip.SymbolCode] = scrip;
                        _leanSymbolTosymbolCodeMap[_sym] = scrip.SymbolCode;
                        _symbolCodeToLeanSymbolMap[scrip.SymbolCode] = _sym;
                    }
                }
                lastUpdateDateTime= DateTime.Now;
            }
        }

        public static SamcoInstrumentList Instance()
        {
            return instance;
        }

        public static Symbol CreateLeanSymbol(ScripMaster scrip)
        {
            if (scrip == null)
            {
                throw new ArgumentNullException(nameof(scrip));
            }

            var securityType = SecurityType.Equity;
            var market = Market.India;
            OptionRight optionRight = 0;
            switch (scrip.Instrument)
            {

                // index
                case "INDEX":
                    return createIndexSymbol(scrip);
                    break;
                //Index Options
                case "OPTIDX":
                    return createIndexOptionSymbol(scrip);
                    break;
                //Index Futures
                case "FUTIDX":
                    return createIndexFuture(scrip);
                    break;
                //Equities
                case "EQ":
                    return createEquitSymbol(scrip);
                    break;
                //Stock Futures
                case "FUTSTK":
                    return createStkFutureSymbol(scrip);
                    break;
                //Stock options
                case "OPTSTK":
                    return createEquityOptionSymbol(scrip);
                    break;
                //Commodity Futures
                case "FUTCOM":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Commodity Options
                case "OPTCOM":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Bullion Options
                case "OPTBLN":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Energy Futures
                case "FUTENR":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Currenty Options
                case "OPTCUR":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Currency Futures
                case "FUTCUR":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Bond Futures
                case "FUTIRC":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Bond Futures
                case "FUTIRT":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
                //Bond Option
                case "OPTIRC":
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;

                default:
                    securityType = SecurityType.Base;
                    throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported");
                    break;
            }


            //return symbol;
        }

        public static Symbol createIndexFuture(ScripMaster scrip)
        {
            DateTime _expiry;
            if (scrip.Instrument != "FUTIDX")
            {
                throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported. This function is for FUTSTK");
            }
            //Symbol _index = Symbol.Create(scrip.Name, SecurityType.Index, Market.India);
            _expiry = DateTime.ParseExact(scrip.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Symbol.CreateFuture(scrip.Name, Market.India, _expiry);
        }
        public static Symbol createStkFutureSymbol(ScripMaster scrip)
        { DateTime _expiry;
            if (scrip.Instrument != "FUTSTK")
            {
                throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported. This function is for FUTSTK");
            }
            _expiry = DateTime.ParseExact(scrip.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Symbol.CreateFuture(scrip.Name, Market.India,_expiry);
        }
        public static Symbol createIndexSymbol(ScripMaster scrip)
        {
            if (scrip.Instrument != "INDEX")
            {
                throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {scrip.Instrument} is not supported. This function is for INDEX");
            }
            return Symbol.Create(scrip.TradingSymbol, SecurityType.Index, Market.India);
        }
        public static Symbol createEquitSymbol(ScripMaster eqScrip)
        {
            if(eqScrip.Instrument != "EQ"){
                throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {eqScrip.Instrument} is not supported. This function is for EQ");
            }
            string LeanTicker;
            if (eqScrip.TradingSymbol.EndsWith("-EQ")) { LeanTicker = eqScrip.TradingSymbol.Remove(eqScrip.TradingSymbol.Length-3); } else { LeanTicker = eqScrip.TradingSymbol; }
            return Symbol.Create(LeanTicker,SecurityType.Equity,Market.India);
        }

        public static Symbol createIndexOptionSymbol(ScripMaster optionScrip)
        {
            OptionRight optionRight; decimal strikePrice; DateTime expiryDate; Symbol _underlying;
            if (optionScrip.Instrument != "OPTIDX")
            {throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} instrument type {optionScrip.Instrument} is not supported. This function is for OPTIDX");}

            if (optionScrip.TradingSymbol.EndsWithInvariant("PE", true))
            { optionRight = OptionRight.Put; }
            else if (optionScrip.TradingSymbol.EndsWithInvariant("CE", true))
            { optionRight = OptionRight.Call; }
            else { throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} Invalid Samco script. Failed to determinine option right. TradingSymbol {optionScrip.TradingSymbol} nither ends with CE nor PE"); }

            strikePrice = Convert.ToDecimal(optionScrip.StrikePrice, CultureInfo.InvariantCulture);
            expiryDate = DateTime.ParseExact(optionScrip.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            _underlying = Symbol.Create(optionScrip.Name, SecurityType.Index, Market.India);

            return Symbol.CreateOption(_underlying, Market.India, OptionStyle.European, optionRight, strikePrice, expiryDate);
        }

        public static Symbol createEquityOptionSymbol(ScripMaster optionScrip)
        {
            OptionRight optionRight; decimal strikePrice; DateTime expiryDate; Symbol _underlying;
            if (optionScrip.Instrument != "OPTSTK")
            {throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} Invalid Samco script. Function expects scrip with instrument==OPTSTK {optionScrip}");}
            
            if (optionScrip.TradingSymbol.EndsWithInvariant("PE", true))
            { optionRight = OptionRight.Put; }
            else if (optionScrip.TradingSymbol.EndsWithInvariant("CE", true))
            {optionRight = OptionRight.Call; }
            else { throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} Invalid Samco script. Failed to determinine option right. TradingSymbol {optionScrip.TradingSymbol} nither ends with CE nor PE"); }

            strikePrice = Convert.ToDecimal(optionScrip.StrikePrice, CultureInfo.InvariantCulture);
            expiryDate = DateTime.ParseExact(optionScrip.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            _underlying = Symbol.Create(optionScrip.Name, SecurityType.Equity, Market.India);
            return Symbol.CreateOption(_underlying, Market.India, OptionStyle.European, optionRight, strikePrice, expiryDate);
        }

    }
}
