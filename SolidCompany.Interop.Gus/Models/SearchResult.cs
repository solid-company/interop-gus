using System.Collections.Generic;

namespace SolidCompany.Interop.Gus.Models
{
    /// <summary>
    /// Represents a search result.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Returns returned error. This field is not null if and only if <see cref="Success"/> is <pre>false</pre>.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Returns a list of legal entities found. This field is not null if and only if <see cref="Success"/> is <pre>true</pre>.
        /// </summary>
        public IReadOnlyList<LegalEntity> Entities { get; }

        /// <summary>
        /// Indicates whether operation succeeded.
        /// </summary>
        public bool Success { get; }

        private SearchResult(IReadOnlyList<LegalEntity> entities)
        {
            Entities = entities;
            Success = true;
        }

        private SearchResult(Error error)
        {
            Error = error;
            Success = false;
        }

#pragma warning disable 1591
        public static SearchResult CreateSuccess(IReadOnlyList<LegalEntity> entities)
        {
            return new SearchResult(entities);
        }

        public static SearchResult CreateFailure(Error error)
        {
            return new SearchResult(error);
        }
    }
}