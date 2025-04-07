using System.Collections.Generic;

namespace CatalogApi
{
    public class FilterObject
    {
        public bool IsOr { get; set; }
        public float MinPrice { get; set; } = 0;
        public float MaxPrice { get; set; } = float.MaxValue;
        public List<string> SelectedTokens { get; set; } = new List<string>();

    }


}

