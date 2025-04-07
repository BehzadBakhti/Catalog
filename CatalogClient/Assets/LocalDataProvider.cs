using CatalogApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class LocalDataProvider : ICatalogDataProvider
{
    public Result<CatalogData> LoadCatalogData()
    {
        try
        {
            var json = Resources.Load<TextAsset>("CatalogData/data").text;
            var Data = JsonConvert.DeserializeObject<CatalogData>(json);

            Data ??= new CatalogData();

            return Result<CatalogData>.Success(Data);
        }
        catch (Exception ex)
        {
            return Result<CatalogData>.Failure($"Loading Catalog data failed: {ex.Message}");
        }
    }
}