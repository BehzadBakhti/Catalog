﻿using System.Collections.Generic;

namespace CatalogApi
{
    // Data Structure for JSON Deserialization
    public class CatalogData
    {
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

