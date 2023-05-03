using System.ComponentModel.DataAnnotations.Schema;

namespace WazaWare.co.za.Models
{
    public class AddCSV
    {
        [NotMapped]
        public IFormFile? CSVFile { get; set; }
    }
}
