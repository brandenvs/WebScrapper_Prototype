using System.ComponentModel.DataAnnotations.Schema;

namespace wazaware.co.za.Models
{
	public class AddCSV
	{
		[NotMapped]
		public IFormFile? CSVFile { get; set; }
	}
}
