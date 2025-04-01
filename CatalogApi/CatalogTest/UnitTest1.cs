using CatalogApi;

namespace CatalogTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }

    }

    public class TestCatalogDataProvider : ICatalogDataProvider
    {
        public Result AddProductType()
        {
            throw new NotImplementedException();
        }

        public string LoadCatalogData()
        {
            throw new NotImplementedException();
        }

        public string LoadProductTypes()
        {
            throw new NotImplementedException();
        }

        public Result RemoveProductType()
        {
            throw new NotImplementedException();
        }
    }
}