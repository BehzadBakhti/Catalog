using System.Collections.Generic;

namespace CatalogApi
{
    public class Bundle : ICatalogItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public Dictionary<string, int> Products { get; set; } = new Dictionary<string, int>(3);

    }
}
