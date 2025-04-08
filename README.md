This repository contains a simple Catalog Api and its usage sample for managing in-app purchasing products and bundles.
- The Api code is under CatalogApi directory.
- It hase been defined as a Unity Package in order to be easily available for Unity client, as well as testing.
- The sample client code is under CatalogClient as a Unity Project.

**How to Use the API**
- Add `using CatalogApi` where ever you want to use the catalog
- Create an Instance of Catalog by passing a ICatalogDataProvider object to its constructor
- Call `Catalog.Initialize()`
- Use the Instance to access its methods

  ***Example***
```
        var catalog = new Catalog(new TestCatalogDataProvider());
        var result = catalog.Initialize();
        if(!result.IsSuccess)
          // Handle Error

        var filter = new FilterObject {/* filtering properties */ };
        var sortObj = new SortObject {/* sorting properties */ };
        ...
        //Explicitly passing a custom collection
        var filteredProducts = catalog.Filter(catalog.GetAllItems(), filter);
        var sortedProducts = catalog.Sort(catalog.GetAllItems(), sortObj);
        var finalList = catalog.FilterAndSort(catalog.GetAllItems(), filter, sortObj)

        // Filtering and Sorting implicitly all the Products
        var finalList = catalog.FilterAndSort(filter, sortObj)
  
```

**Testing The sample client:**

The 'SampleScene' in the Unity client contains a UIDocument:
![image](https://github.com/user-attachments/assets/62ffd1c9-3330-40fa-bcb1-53b18a986d23)

Also a smaple data file is placed under Resources/CatalogData/data.json
