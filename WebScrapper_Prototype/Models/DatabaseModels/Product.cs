using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace wazaware.co.za.Models.DatabaseModels
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string? ProductName { get; set; }
        [Required]
        public string? ProductStock { get; set; }
        [Required]
        public string? ProductDescription { get; set; }
        [Required]
        public string? ProductCategory { get; set; }
        [Required]
        public decimal? ProductPriceBase { get; set; }
        [Required]
        public decimal? ProductPriceSale { get; set; }
        [Required]
        public string? ProductVendorName { get; set; }
        [Required]
        public string? ProductVendorUrl { get; set; }
        [Required]
        public string? ProductVisibility { get; set; }
        [Required]
        public string? ProductDataBatchNo { get; set; }
        [Required]
        public string? ProductImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ProductPic { get; set; }
    }
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string? ImageFileName { get; set; }
        [Required]
        public string? ImageFileType { get; set; }
        [Required]
        public byte[]? ImageFileContent { get; set; }
    }
}
