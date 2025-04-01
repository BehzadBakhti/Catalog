namespace CatalogApi
{
    public class Product : ICatalogItem
    {
        public string Name { get; set; }
        public string ProductType { get; set; } 
        public string Description { get; set; }

        public int Amount { get; set; }

        public float Price { get; set; }

    }
}
