using QuantConnect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Brokerages.Samco
{
    /// <summary>
    /// An implementation of <see cref="IFutureChainProvider"/> that fetches the list of contracts
    /// from the Samco StockNote API
    /// </summary>
    public class SamcoLiveFutureChainProvider : IFutureChainProvider
    {
        public IEnumerable<Symbol> GetFutureContractList(Symbol symbol, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
