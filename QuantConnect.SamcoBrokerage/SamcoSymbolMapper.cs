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

using CsvHelper;
using CsvHelper.Configuration;
using QuantConnect.Brokerages.Samco.SamcoMessages;
using QuantConnect.Util;
using QuantConnect.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace QuantConnect.Brokerages.Samco
{
    /// <summary>
    /// Provides the mapping between Lean symbols and Samco symbols.
    /// </summary>
    public class SamcoSymbolMapper : ISymbolMapper
    {
        string ISymbolMapper.GetBrokerageSymbol(Symbol symbol)
        {
            return SamcoInstrumentList.Instance().getSymbolCodeFromLeanSymbol(symbol);
        }

        Symbol ISymbolMapper.GetLeanSymbol(string brokerageSymbol, SecurityType securityType, string market, DateTime expirationDate, decimal strike, OptionRight optionRight)
        {
            Symbol _sym= SamcoInstrumentList.Instance().GetLeanSymbolFromSymbolCode(brokerageSymbol);
            
            if(_sym.SecurityType==securityType && _sym.ID.Market==market && _sym.ID.StrikePrice==strike && _sym.ID.Date==expirationDate) { return _sym; }
            else { throw new ArgumentException($"{WhoCalledMe.GetMethodName(1)} Failed to map symbolCode/LisingID to lean symbol"); }
        }
    }
}