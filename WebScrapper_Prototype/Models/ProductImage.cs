using System.ComponentModel.DataAnnotations;

namespace WazaWare.co.za.Models
{
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