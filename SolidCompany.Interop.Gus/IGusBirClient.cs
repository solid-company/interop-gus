using System.Threading.Tasks;
using SolidCompany.Interop.Gus.Models;

namespace SolidCompany.Interop.Gus
{
    /// <summary>
    /// Abstraction for GUS BIR API.
    /// </summary>
    public interface IGusBirClient
    {
        /// <summary>
        /// Searches for legal entity.
        /// </summary>
        /// <param name="nip">NIP (Polish TAX ID)</param>
        /// <returns>Return legal entity data.</returns>
        Task<SearchResult> FindByNipAsync(string nip);

        /// <summary>
        /// Searches for legal entity.
        /// </summary>
        /// <param name="regon">REGON number</param>
        /// <returns>Return legal entity data.</returns>
        Task<SearchResult> FindByRegonAsync(string regon);

        /// <summary>
        /// Searches for legal entity.
        /// </summary>
        /// <param name="krs">KRS number</param>
        /// <returns>Return legal entity data.</returns>
        Task<SearchResult> FindByKrsAsync(string krs);
    }
}