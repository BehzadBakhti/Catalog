using System;

namespace CatalogApi
{
    public interface ICatalogDataProvider
    {
        string LoadCatalogData();
        string LoadProductTypes();
        Result AddProductType();
        Result RemoveProductType();
    }
}

