using System.Collections.Generic;

namespace CatalogApi
{
    /// <summary>
    /// Data object contaning required information for filtering the Catalog Products
    /// </summary>
    public class FilterObject
    {
        /// <summary>
        /// A flag for setting the type of combination wile filtering based on Tokens, 
        /// if 'true', filtering will Chain tokens availibility with 'OR', otherwise, 'AND' 
        /// </summary>
        public bool IsOr { get; set; }
        public float MinPrice { get; set; } = 0;
        public float MaxPrice { get; set; } = float.MaxValue;
        /// <summary>
        /// List of Tokens used for filtering the Products
        /// </summary>
        public List<string> SelectedTokens { get; set; } = new List<string>();
        public bool OnlyBundles { get; set; }

    }
}

