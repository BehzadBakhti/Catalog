using System;
using System.Linq;
using System.Collections.Generic;

namespace CatalogApi
{
    // Catalog can potentially have Data modification Api too, but 
    public class Catalog
    {
        private bool _isInitialized;
        private ICatalogDataProvider _dataProvider;
        private List<Product> _items = new List<Product>();
        private HashSet<string> _productTypes = new HashSet<string>();

        // No functionality added for this, only for demonstration purpuse
        // This is where we can notify subscribers to catalog events if required ...
        public event Action SomeEvent;

        public Catalog(ICatalogDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Initializes the Catalog by loading and storing the product list
        /// </summary>
        /// <returns></returns>
        public Result Initialize()
        {
            _isInitialized = false;
            _items.Clear();
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

        /// <summary>
        /// Returns a list of all the defined product items 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<Product> GetAllItems()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            return _items;
        }

        /// <summary>
        /// Returns all the token types that are used in the products or bundles
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<string> GetAllTokenTypes()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            return _productTypes.ToList();
        }

        /// <summary>
        /// Sorts and returns a list of product based on the <see cref="SortObject">SortObject</see> passed to it.
        /// </summary>
        /// <param name="productsToSort"></param>
        /// <param name="sortObject"></param>
        /// <returns></returns>
        public IReadOnlyList<Product> Sort(IReadOnlyList<Product> productsToSort, SortObject sortObject)
        {
            IOrderedEnumerable<Product> sortedProducts = null;

            switch (sortObject.SortCriteria)
            {
                case SortBy.None:
                    return productsToSort;
                case SortBy.Name:
                    sortedProducts = sortObject.Descending
                        ? productsToSort.OrderByDescending(p => p.Name)
                        : productsToSort.OrderBy(p => p.Name);
                    break;
                case SortBy.Price:
                    sortedProducts = sortObject.Descending
                        ? productsToSort.OrderByDescending(p => p.Price)
                        : productsToSort.OrderBy(p => p.Price);
                    break;
                case SortBy.TokenAmount: // order of SelectedTokens is important
                    if (sortObject.SelectedTokens.Any())
                    {
                        for (var i = 0; i < sortObject.SelectedTokens.Count; i++)
                        {
                            var currentToken = sortObject.SelectedTokens[i];

                            if (i == 0)
                            {
                                var initialSort = sortObject.Descending
                                    ? productsToSort.OrderByDescending(p => p.Tokens.GetValueOrDefault(currentToken, int.MinValue))
                                    : productsToSort.OrderBy(p => p.Tokens.GetValueOrDefault(currentToken, int.MinValue));
                                sortedProducts = initialSort; // Assign to sortedProducts for subsequent ThenBy
                            }
                            else if (sortedProducts != null)
                            {
                                sortedProducts = sortObject.Descending
                                    ? sortedProducts.ThenByDescending(p => p.Tokens.GetValueOrDefault(currentToken, int.MinValue))
                                    : sortedProducts.ThenBy(p => p.Tokens.GetValueOrDefault(currentToken, int.MinValue));
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

        /// <summary>
        /// Filters and returns a list of product based on the <see cref="FilterObject">FilterObject</see> passed to it.
        /// </summary>
        /// <param name="productsToFilter"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IReadOnlyList<Product> Filter(IReadOnlyList<Product> productsToFilter, FilterObject filter)
        {
            return productsToFilter.Where(item =>
                (filter.MinPrice <= item.Price && item.Price <= filter.MaxPrice) &&
                (!filter.OnlyBundles || item.IsBundle) && // Apply bundle filter
                (!filter.SelectedTokens.Any() || (filter.IsOr
                     ? filter.SelectedTokens.Any(token => item.Tokens.ContainsKey(token))
                     : filter.SelectedTokens.All(token => item.Tokens.ContainsKey(token)))))
                .ToList();
        }

        /// <summary>
        /// Filters and Sorts (Based on <see cref="FilterObject">FilterObject</see> and <see cref="SortObject">SortObject</see>) all the items defined in the Catalog, and returns a list of products.
        /// </summary>
        /// <param name="filterObject"></param>
        /// <param name="sortObject"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<Product> FilterAndSort(FilterObject filterObject, SortObject sortObject)
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            return FilterAndSort(_items, filterObject, sortObject);
        }

        /// <summary>
        /// Filters and Sorts (Based on <see cref="FilterObject">FilterObject</see> and <see cref="SortObject">SortObject</see>) and returns a list of Products.
        /// </summary>
        /// <param name="productsToFilterAndSort"></param>
        /// <param name="filterObject"></param>
        /// <param name="sortObject"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<Product> FilterAndSort(IReadOnlyList<Product> productsToFilterAndSort, FilterObject filterObject, SortObject sortObject)
        {
            var filtered = Filter(productsToFilterAndSort, filterObject);
            return Sort(filtered, sortObject);
        }

        /// <summary>
        /// Returns a price range for all the products and bundles in the Catalog as a tuple
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public (float minPrice, float maxPrice) GetMinMaxPrice()
        {
            if (!_isInitialized)
                throw new InvalidOperationException(" Catalog is not initialized ...");

            if (!_items.Any())
                return (0.0f, float.MinValue); // Return defaults for an empty or null list

            var minPrice = _items.Min(p => p.Price);
            var maxPrice = _items.Max(p => p.Price);

            return (minPrice, maxPrice);
        }
    }
}

