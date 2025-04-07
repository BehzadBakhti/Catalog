This repository contains a simple Catalog Api and its usage sample for managing in-app purchasing products and bundles.
- The Api code is under CatalogApi directory.
- It hase been defined as a Unity Package in order to be easily available for Unity client, as well as testing.
- The sample client code is under CatalogClient as a Unity Project.

  How to Use:
- Add `using CatalogApi` where ever you want to use the catalog
- Create an Instance of Catalog by passing a ICatalogDataProvider object to its constructor
- Call `Catalog.Initialize()`
- Use the Instance to access its methods

