namespace CatalogApi
{
    public interface ICatalogItem
    {
        string Name { get; set; }
        public string Description { get; set; }

        public float Price { get; set; }

    }
}
