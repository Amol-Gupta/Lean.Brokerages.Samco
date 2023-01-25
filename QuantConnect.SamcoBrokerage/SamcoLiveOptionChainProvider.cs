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

using QuantConnect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantConnect.Brokerages.Samco
{
    /// <summary>
    /// An implementation of <see cref="IOptionChainProvider"/> that fetches the list of contracts
    /// from the Samco StockNote API
    /// </summary>
    public class SamcoLiveOptionChainProvider : IOptionChainProvider
    {
       

        /// <summary>
        /// Static constructor for the <see cref="SamcoLiveOptionChainProvider"/> class
        /// </summary>
        public SamcoLiveOptionChainProvider()
        {
            
        }

        /// <summary>
        /// Gets the option chain associated with the underlying Symbol
        /// </summary>
        /// <param name="symbol">Underlying symbol to get the option chain for</param>
        /// <param name="date">Unused</param>
        /// <returns>Option chain</returns>
        public IEnumerable<Symbol> GetOptionContractList(Symbol symbol, DateTime date)
        {
            var l = SamcoInstrumentList.Instance()._leanSymbolList;
            var m=l.Where(s =>
               ( ( s.SecurityType == SecurityType.Option) || (s.SecurityType == SecurityType.IndexOption))
            && s.ID.Underlying == symbol.ID);
            return m;
        }
    }
}