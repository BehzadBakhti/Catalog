using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CatalogApi
{

    public partial class Catalog
    {
        ICatalogDataProvider _dataProvider;
        List<ICatalogItem> _items = new List<ICatalogItem>();

        // for more flexible senario this can be replaced with file that developers can modify and store in the game project
        readonly static string[] ProducTypes = { "Coin", "Gem", "Ticket"};

        public Catalog(ICatalogDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public Result<List<ICatalogItem>> LoadCatalog()
        {
            try
            {
                string json = File.ReadAllText(_dataProvider.LoadCatalogData());
                var catalogData = JsonConvert.DeserializeObject<CatalogData>(json);

                if (catalogData != null)
                {
                    if (catalogData.Products != null)
                    {
                        _items.AddRange(catalogData.Products);
                    }
                    if (catalogData.Bundles != null)
                    {
                        _items.AddRange(catalogData.Bundles);
                    }
                }
                return Result<List<ICatalogItem>>.Success(_items);
            }
            catch (Exception ex)
            {
                return Result<List<ICatalogItem>>.Failure($"Loading Catalog data failed: {ex.Message}");

            }
        }

        // Sorting Methods
        public List<ICatalogItem> SortByPrice(bool ascending = true)
        {
            return ascending
                ? _items.OrderBy(item => item.Price).ToList()
                : _items.OrderByDescending(item => item.Price).ToList();
        }

        public List<ICatalogItem> SortByName(bool ascending = true)
        {
            return ascending
                ? _items.OrderBy(item => item.Name).ToList()
                : _items.OrderByDescending(item => item.Name).ToList();
        }

        public List<ICatalogItem> SortByProductType(List<string> itemOrder)
        {
            return _items.OrderBy(item =>
            {
                if (item is Product product)
                {
                    return itemOrder.IndexOf(product.ProductType);
                }
                else if (item is Bundle bundle)
                {
                    // Assign a priority based on the first item found
                    foreach (var type in itemOrder)
                    {
                        if (bundle.Products.ContainsKey(type))
                        {
                            return itemOrder.IndexOf(type);
                        }
                    }
                    return int.MaxValue; // if no matching type, send to end.
                }
                else
                {
                    return int.MaxValue; // for other types, send to the end.
                }
            }).ToList();
        }

        // Filtering Methods
        public List<ICatalogItem> FilterByProductType(List<string> filterTypes)
        {
            return _items.Where(item =>
            {
                if (item is Product product)
                {
                    return filterTypes.Contains(product.ProductType);
                }
                else if (item is Bundle bundle)
                {
                    return bundle.Products.Keys.Any(filterTypes.Contains);
                }
                return false;
            }).ToList();
        }

    }

    public partial class Catalog
    {
        public Result AddProduct(Product product)
        {
            var products = _dataProvider.LoadCatalogData();

            return Result.Success();
        }
    }
}

