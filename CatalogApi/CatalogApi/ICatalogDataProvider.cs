using System;

namespace CatalogApi
{
    // This interface can potentialy have editing functionalities to modify the data set
    public interface ICatalogDataProvider
    {
        Result<CatalogData> LoadCatalogData();
    }
}

