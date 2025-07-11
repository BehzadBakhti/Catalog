﻿using CatalogApi;
using Newtonsoft.Json;

namespace CatalogTest;

public class TestCatalogDataProvider : ICatalogDataProvider
{
    private readonly string _testData = @"
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

        var data = JsonConvert.DeserializeObject<CatalogData>(_testData);

        data ??= new CatalogData();

        if (data.Products.Any(p => p.Name == name))
            return Result.Failure($"A Bundle with name: {name} already Exists");

        data.Products.Add(new Product(name, description, price)
        {
            Tokens = products,
        });

        return Result.Success();

    }

    public Result<CatalogData> LoadCatalogData()
    {
        try
        {
            var data = JsonConvert.DeserializeObject<CatalogData>(_testData);

            data ??= new CatalogData();

            return Result<CatalogData>.Success(data);
        }
        catch (Exception ex)
        {

            return Result<CatalogData>.Failure($"Loading Catalog data failed: {ex.Message}");
        }

    }
}