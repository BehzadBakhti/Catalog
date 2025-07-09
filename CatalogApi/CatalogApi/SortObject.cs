using System.Collections.Generic;

namespace CatalogApi
{
    /// <summary>
    /// Data object for defining the required information for Sorting the Catalog products
    /// </summary>
    public class SortObject
    {
        public bool Descending { get; set; } = false;
        public SortBy SortCriteria { get; set; } = SortBy.None;
        /// <summary>
        /// Ordered List of token that is used for sorting the products. 
        /// The lower the index of a token, the more priority it has for sorting.
        /// only Applicable for '<see cref="SortBy.TokenAmount"/>' Sorting criteria
        /// </summary>
        public List<string> SelectedTokens { get; set; } = new List<string>();
    }


}

