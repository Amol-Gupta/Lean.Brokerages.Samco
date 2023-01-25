using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Packets;
using QuantConnect.Util;
using System;
using System.Collections.Generic;

namespace QuantConnect.Brokerages.Samco
{
    /// <summary>
    /// SamcoBrokerage: IDataQueueHandler implementation
    /// </summary>
    public partial class SamcoBrokerage
    {
        public IEnumerable<Symbol> LookupSymbols(Symbol symbol, bool includeExpired, string securityCurrency = null)
        {
            if (symbol.SecurityType.IsOption())
            {
                return _algorithm.OptionChainProvider.GetOptionContractList(symbol.Underlying, _algorithm.Time);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool CanPerformSelection()
        {
            return true;
        }
    }
}
