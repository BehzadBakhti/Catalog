using System.Collections.Generic;

namespace CatalogApi
{
    // With current requirements the Product can cover Bundles as well,
    // but in case of more complexity requiremen these to can be saperated and implement a base interface
    public class Product
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public Dictionary<string, int> Tokens { get; set; } = new Dictionary<string, int>(3);
        public bool IsBundle => Tokens.Count > 1;

    }
}
