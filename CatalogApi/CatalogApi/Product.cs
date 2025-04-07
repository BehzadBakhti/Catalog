using System.Collections.Generic;

namespace CatalogApi
{
    public class Product
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public Dictionary<string, int> Tokens { get; set; } = new Dictionary<string, int>(3);
        public bool IsBundle => Tokens.Count > 0;

    }
}
