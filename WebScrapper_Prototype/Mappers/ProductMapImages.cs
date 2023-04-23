using CsvHelper.Configuration;
using wazaware.co.za.Models;

namespace wazaware.co.za.Mappers
{
    public sealed class ProductMapImages : ClassMap<ProductImageURLs>
    {
        public ProductMapImages()
        {
            Map(x => x.VendorSiteOrigin).Name("web-scraper-start-url");
			Map(x => x.ProductId).Name("ScrapperProductId");
			Map(x => x.VendorSiteProduct).Name("ScrapperProductId-href");           
			Map(x => x.ImageURL).Name("Image-src");
        }
    }
}
