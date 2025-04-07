using System;

namespace CatalogApi
{
    public interface ICatalogDataProvider
    {
        Result<CatalogData> LoadCatalogData();
    }
}

