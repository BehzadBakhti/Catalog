using System.Collections.Generic;

namespace CatalogApi
{
    public class SortObject
    {
        public bool Decending { get; set; } = false;
        public SortBy SortCriteria { get; set; } = SortBy.None;
        public List<string> SelectedTokens { get; set; } = new List<string>();
    }


}

