using CsvHelper.Configuration;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.Mappers
{
    public sealed class ProductMapSingle : ClassMap<Product>
    {
        public ProductMapSingle()
        {
            Map(x => x.ProductVendorName).Name("web-scraper-start-url");
			Map(x => x.ProductVendorUrl).Name("ScapperProdcutId-href");
           // Map(x => x.ProductId).Name("ScapperProdcutId");
            Map(x => x.ProductCategory).Name("Cat");
			Map(x => x.ProductName).Name("ProductName");
            Map(x => x.ProductStock).Name("ProductStock");
            Map(x => x.ProductPriceBase).Name("ProductPriceBase");
            Map(x => x.ProductPriceSale).Name("ProductPriceSale");
            Map(x => x.ProductDescription).Name("ProductShortDescription");
            Map(x => x.ProductImageUrl).Name("ProductImage-src");

        }
    }
}
