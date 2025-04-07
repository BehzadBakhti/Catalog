using CatalogApi;
using Newtonsoft.Json;

namespace CatalogTest
{
    public class UnitTest1
    {
        [Fact]
        public void GetAllItems_Success()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var items = catalog.GetAllItems();
            Assert.Equal(13, items.Count);
        }

        [Fact]
        public void Filter_ByPriceRange()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var filter = new FilterObject { MinPrice = 5, MaxPrice = 20 };

            var filteredProducts = catalog.Filter(catalog.GetAllItems(), filter);
            Assert.Equal(5, filteredProducts.Count);
            Assert.All(filteredProducts, p => Assert.True(p.Price >= 5 && p.Price <= 20));
        }

        [Fact]
        public void Filter_BySingleToken_IsOr()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var filter = new FilterObject { IsOr = true, SelectedTokens = new List<string> { "Gems" } };
            var filteredProducts = catalog.Filter(catalog.GetAllItems(), filter);
            Assert.Equal(7, filteredProducts.Count); 
            Assert.All(filteredProducts, p => p.Tokens.ContainsKey("Gems"));
        }

        [Fact]
        public void Filter_ByMultipleTokens_IsOr()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var filter = new FilterObject { IsOr = true, SelectedTokens = new List<string> { "Coins", "Tickets" } };
            var filteredProducts = catalog.Filter(catalog.GetAllItems(), filter);
            Assert.Equal(11, filteredProducts.Count);
            Assert.All(filteredProducts, p => Assert.True(p.Tokens.ContainsKey("Coins") || p.Tokens.ContainsKey("Tickets")));

        }

        [Fact]
        public void Filter_ByMultipleTokens_IsAll()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var filter = new FilterObject { IsOr = false, SelectedTokens = new List<string> { "Coins", "Gems" } };
            var filteredProducts = catalog.Filter(catalog.GetAllItems(), filter);
            Assert.Equal(4, filteredProducts.Count); 
            Assert.All(filteredProducts, p => Assert.True(p.Tokens.ContainsKey("Coins") && p.Tokens.ContainsKey("Gems")));
        }

        [Fact]
        public void Sort_ByName_Ascending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.Name, Decending = false };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Bag of Gems", sortedProducts.First().Name);
            Assert.Equal("Small Coin Pouch", sortedProducts[9].Name);
            Assert.Equal("Ultimate Starter Pack", sortedProducts.Last().Name);
        }

        [Fact]
        public void Sort_ByName_Descending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider()); 
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.Name, Decending = true };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Ultimate Starter Pack", sortedProducts.First().Name);
            Assert.Equal("Sparkling Gemstone", sortedProducts[2].Name);
            Assert.Equal("Bag of Gems", sortedProducts.Last().Name);
        }

        [Fact]
        public void Sort_ByPrice_Ascending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.Price, Decending = false };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Single Game Ticket", sortedProducts.First().Name);
            Assert.Equal("Jackpot Bundle", sortedProducts.Last().Name);
        }

        [Fact]
        public void Sort_ByPrice_Descending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.Price, Decending = true };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Jackpot Bundle", sortedProducts.First().Name);
            Assert.Equal("Single Game Ticket", sortedProducts.Last().Name);
        }

        [Fact]
        public void Sort_ByTokenAmount_Coins_Ascending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.TokenAmount, Decending = false, SelectedTokens = new List<string> { "Coins" } };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Jackpot Bundle", sortedProducts.Last().Name);     // Coins: 500
        }

        [Fact]
        public void Sort_ByTokenAmount_Gems_Descending()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.TokenAmount, Decending = true, SelectedTokens = new List<string> { "Gems" } };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);
            Assert.Equal("Jackpot Bundle", sortedProducts.First().Name);    // Gems: 25
        }

        [Fact]
        public void Sort_ByTokenAmount_MultipleTokens_Ascending_OrderMatters()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.TokenAmount, Decending = true, SelectedTokens = new List<string> {"Tickets", "Gems", "Coins"} };
            var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObject);

            // Expect sorting primarily by number of Tickets, then Gems and finally Coins
            Assert.Equal("Jackpot Bundle", sortedProducts[0].Name);        //Tickets: 25
            Assert.Equal("Triple Treasure Chest", sortedProducts[1].Name); //Tickets: 10
            Assert.Equal("Bundle of Tickets (5)", sortedProducts[2].Name); //Tickets: 5
            Assert.Equal("Gem & Ticket Pack", sortedProducts[3].Name);     //Tickets: 3
            Assert.Equal("Ultimate Starter Pack", sortedProducts[4].Name); //Tickets: 2
            Assert.Equal("Lucky Ticket & Coin", sortedProducts[5].Name);   //Tickets: 1, No Gems, Coins = 10
            Assert.Equal("Single Game Ticket", sortedProducts[6].Name);    //Tickets: 1, No Gems, No Coins
            Assert.Equal("Bag of Gems", sortedProducts[7].Name);           //5 Gems
            Assert.Equal("Coin & Gem Combo", sortedProducts[8].Name);      //2 Gems
            Assert.Equal("Sparkling Gemstone", sortedProducts[9].Name);    //1 Gems
            Assert.Equal("Mega Coin Stash", sortedProducts[10].Name);      //500 Coins
            Assert.Equal("Pile of Coins", sortedProducts[11].Name);        //200 Coins
            Assert.Equal("Small Coin Pouch", sortedProducts[12].Name);     //50 Coins


            Assert.NotNull(sortedProducts);
        }

        [Fact]
        public void Sort_NoSorting()
        {
            var catalog = new Catalog(new TestCatalogDataProvider());
            var result = catalog.LoadCatalog();
            Assert.True(result.IsSuccess);

            var sortObject = new SortObject { SortCriteria = SortBy.None };
            var originalData = catalog.GetAllItems();
            var sortedProducts = catalog.Sort(originalData, sortObject);
            Assert.Equal(originalData.Count, sortedProducts.Count);
            Assert.True(originalData.SequenceEqual(sortedProducts)); // Check if the order is the same
        }
    }
}

public class TestCatalogDataProvider : ICatalogDataProvider
{
    private string TestData = @"
                         {
                          ""Products"": [
                            {
                              ""Name"": ""Small Coin Pouch"",
                              ""Description"": ""A small pouch containing shiny coins."",
                              ""Price"": 4.99,
                              ""Tokens"": {
                                ""Coins"": 50
                              }
                            },
                            {
                              ""Name"": ""Sparkling Gemstone"",
                              ""Description"": ""A single, beautifully cut gem."",
                              ""Price"": 9.99,
                              ""Tokens"": {
                                ""Gems"": 1
                              }
                            },
                            {
                              ""Name"": ""Single Game Ticket"",
                              ""Description"": ""One ticket to your favorite game."",
                              ""Price"": 2.50,
                              ""Tokens"": {
                                ""Tickets"": 1
                              }
                            },
                            {
                              ""Name"": ""Pile of Coins"",
                              ""Description"": ""A generous pile of gold coins."",
                              ""Price"": 19.99,
                              ""Tokens"": {
                                ""Coins"": 200
                              }
                            },
                            {
                              ""Name"": ""Bag of Gems"",
                              ""Description"": ""A small bag filled with various gems."",
                              ""Price"": 29.99,
                              ""Tokens"": {
                                ""Gems"": 5
                              }
                            },
                            {
                              ""Name"": ""Bundle of Tickets (5)"",
                              ""Description"": ""A bundle of five game tickets."",
                              ""Price"": 9.99,
                              ""Tokens"": {
                                ""Tickets"": 5
                              }
                            },
                            {
                              ""Name"": ""Coin & Gem Combo"",
                              ""Description"": ""A mix of coins and a few gems."",
                              ""Price"": 14.50,
                              ""Tokens"": {
                                ""Coins"": 75,
                                ""Gems"": 2
                              }
                            },
                            {
                              ""Name"": ""Lucky Ticket & Coin"",
                              ""Description"": ""One lucky ticket and a single coin."",
                              ""Price"": 3.99,
                              ""Tokens"": {
                                ""Tickets"": 1,
                                ""Coins"": 10
                              }
                            },
                            {
                              ""Name"": ""Gem & Ticket Pack"",
                              ""Description"": ""A couple of gems and a few tickets."",
                              ""Price"": 17.00,
                              ""Tokens"": {
                                ""Gems"": 3,
                                ""Tickets"": 3
                              }
                            },
                            {
                              ""Name"": ""Mega Coin Stash"",
                              ""Description"": ""A huge amount of coins for the dedicated collector."",
                              ""Price"": 49.99,
                              ""Tokens"": {
                                ""Coins"": 500
                              }
                            },
                            {
                              ""Name"": ""Triple Treasure Chest"",
                              ""Description"": ""A chest filled with coins, gems, and tickets!"",
                              ""Price"": 79.99,
                              ""Tokens"": {
                                ""Coins"": 300,
                                ""Gems"": 10,
                                ""Tickets"": 10
                              }
                            },
                            {
                              ""Name"": ""Ultimate Starter Pack"",
                              ""Description"": ""Everything you need to get started: coins, gems, and tickets."",
                              ""Price"": 24.99,
                              ""Tokens"": {
                                ""Coins"": 100,
                                ""Gems"": 2,
                                ""Tickets"": 2
                              }
                            },
                            {
                              ""Name"": ""Jackpot Bundle"",
                              ""Description"": ""Hit the jackpot with this massive bundle!"",
                              ""Price"": 199.99,
                              ""Tokens"": {
                                ""Coins"": 1000,
                                ""Gems"": 25,
                                ""Tickets"": 25
                              }
                            }
                          ]
                        }";

    public Result AddProduct(string name, string description, Dictionary<string, int> products, float price)
    {

        var Data = JsonConvert.DeserializeObject<CatalogData>(TestData);

        Data ??= new CatalogData();

        if (Data.Products.Any(p => p.Name == name))
            return Result.Failure($"A Bundle with name: {name} already Exists");

        Data.Products.Add(new Product()
        {
            Name = name,
            Tokens = products,
            Description = description,
            Price = price
        });

        return Result.Success();

    }

    public Result<CatalogData> LoadCatalogData()
    {
        try
        {
            var Data = JsonConvert.DeserializeObject<CatalogData>(TestData);

            Data ??= new CatalogData();

            return Result<CatalogData>.Success(Data);
        }
        catch (Exception ex)
        {

            return Result<CatalogData>.Failure($"Loading Catalog data failed: {ex.Message}");
        }

    }
}