using System.ComponentModel.DataAnnotations;

namespace WazaWare.co.za.Models
{
    public class OrderProducts
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
		[Required]
		public int ProductCount { get; set; }

    }
}
