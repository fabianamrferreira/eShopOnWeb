using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{

    public enum Currency {
        USD,
        EUR,
        GBP,

    }

    /// <sumary>
    /// Abstraction for converting monetary values
    /// </sumary>
    public interface ICurrencyService
    {
        /// <sumary>
        /// Convert monetary values from source to target currency
        /// </sumary>
        /// <param name="value">Monetary value</param>
        /// <param name="source">Source currency</param>
        /// <param name="target">Target currency</param>
        /// <param name="cancellationToken">Token used to cancel the operation</param>
        /// <return></return>
        Task <decimal> Convert(
            decimal value, Currency source, Currency target,
            CancellationToken cancellationToken = default(CancellationToken)
        );
    }
}
