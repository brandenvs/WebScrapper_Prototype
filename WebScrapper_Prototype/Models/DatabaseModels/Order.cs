using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wazaware.co.za.Models.DatabaseModels
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int? ShippingPrice { get; set; }
        [Required]
        public decimal? OrderTotal { get; set; }
        [Required]
        public Boolean IsOrderPayed { get; set; }
        [Required]
        public DateTime? OrderCreatedOn { get; set; }
    }
    public class OrderedProducts
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ProductCount { get; set; }
        [Required]
        public decimal? ProductTotal { get; set; }

    }
    public class BillingAddress
    {
        [Key]
        public int BillingId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string? PaymentMethod { get; set; }

        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }
        [Required]
        [Phone]
        public string? Phone { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public int UnitNo { get; set; }

        [Required]
        [MaxLength(100)]
        public string? StreetAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Suburb { get; set; }

        [Required]
        [MaxLength(50)]
        public string? City { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Province { get; set; }
        [Required]
        public int PostalCode { get; set; }
    }

}
