using System;
using System.Linq;
using System.Collections.Generic;

namespace CatalogApi
{

    public partial class Catalog
    {
        ICatalogDataProvider _dataProvider;
        List<Product> _items = new List<Product>();
        HashSet<string> _productTypes = new HashSet<string>();
        bool _isInitialized = false;

        public Catalog(ICatalogDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public Result LoadCatalog()
        {
            try
            {
                var catalogDataResult = _dataProvider.LoadCatalogData();
                if (!catalogDataResult.IsSuccess)
                    return Result.Failure(catalogDataResult.ErrorMessage!);

                var catalogData = catalogDataResult.Value;

                if (catalogData.Products != null)
                {
                    _items.AddRange(catalogData.Products);
                    foreach (var product in catalogData.Products)
                    {
                        foreach (var item in product.Tokens)
                        {
                            _productTypes.Add(item.Key);
                        }
                    }
                }
                _isInitialized = true;
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Loading Catalog data failed: {ex.Message}");
            }
        }

        public List<Product> GetAllItems()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            return _items;
        }

        public List<string> GetAllProductTypes()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            return _productTypes.ToList();
        }

        // Sorting Methods
        public List<Product> Sort(List<Product> productsToSort, SortObject sortObject)
        {
            IOrderedEnumerable<Product> sortedProducts = null;

            switch (sortObject.SortCriteria)
            {
                case SortBy.None:
                    return productsToSort;
                case SortBy.Name:
                    sortedProducts = sortObject.Decending
                        ? productsToSort.OrderByDescending(p => p.Name)
                        : productsToSort.OrderBy(p => p.Name);
                    break;
                case SortBy.Price:
                    sortedProducts = sortObject.Decending
                        ? productsToSort.OrderByDescending(p => p.Price)
                        : productsToSort.OrderBy(p => p.Price);
                    break;
                case SortBy.TokenAmount: // order of SelectedTokens is important
                    if (sortObject.SelectedTokens.Any())
                    {
                        IOrderedEnumerable<Product>? initialSort = null;

                        for (int i = 0; i < sortObject.SelectedTokens.Count; i++)
                        {
                            string currentToken = sortObject.SelectedTokens[i];

                            if (i == 0)
                            {
                                initialSort = sortObject.Decending
                                    ? productsToSort.OrderByDescending(p => p.Tokens.ContainsKey(currentToken) ? p.Tokens[currentToken] : int.MinValue)
                                    : productsToSort.OrderBy(p => p.Tokens.ContainsKey(currentToken) ? p.Tokens[currentToken] : int.MinValue);
                                sortedProducts = initialSort; // Assign to sortedProducts for subsequent ThenBy
                            }
                            else if (sortedProducts != null)
                            {
                                sortedProducts = sortObject.Decending
                                    ? sortedProducts.ThenByDescending(p => p.Tokens.ContainsKey(currentToken) ? p.Tokens[currentToken] : int.MinValue)
                                    : sortedProducts.ThenBy(p => p.Tokens.ContainsKey(currentToken) ? p.Tokens[currentToken] : int.MinValue);
                            }
                        }

                        if (sortedProducts == null)
                        {
                            return productsToSort; // Return original if no tokens to sort by
                        }
                    }
                    else
                    {
                        return productsToSort; // Return original if no tokens selected
                    }
                    break;

            }

            return sortedProducts?.ToList() ?? productsToSort; // Return sorted list or original if no sorting

        }

        public List<Product> Filter(List<Product> productsToFilter, FilterObject filter)
        {
            return productsToFilter.Where(item =>
                (filter.MinPrice <= item.Price && item.Price <= filter.MaxPrice) &&
                (!filter.SelectedTokens.Any() || // No tokens to filter by
                 (filter.IsOr
                 ? filter.SelectedTokens.Any(token => item.Tokens.ContainsKey(token))
                 : filter.SelectedTokens.All(token => item.Tokens.ContainsKey(token)))))
                .ToList();
        }

        public List<Product> ApplyFilterAndSort(FilterObject filterObject, SortObject sortObject)
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            var filtered = Filter(_items, filterObject);
            return Sort(filtered, sortObject);
        }

        public (float minPrice, float maxPrice) GetMinMaxPrice()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            if (!_items.Any())
            {
                return (float.MaxValue, float.MinValue); // Return defaults for an empty or null list
            }

            float minPrice = _items.Min(p => p.Price);
            float maxPrice = _items.Max(p => p.Price);

            return (minPrice, maxPrice);
        }
    }

    public class FilterObject
    {
        public bool IsOr { get; set; }
        public List<string> SelectedTokens { get; set; } = new List<string>();
        public float MinPrice { get; set; } = 0;

        public float MaxPrice { get; set; } = float.MaxValue;

    }

    public enum SortBy
    {
        None = -1,
        Name = 0,
        Price = 1,
        TokenAmount = 2
    }


    public class SortObject
    {
        public bool Decending { get; set; } = false;
        public SortBy SortCriteria { get; set; } = SortBy.None;
        public List<string> SelectedTokens { get; set; } = new List<string>();
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

