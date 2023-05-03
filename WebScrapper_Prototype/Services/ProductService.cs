using CsvHelper;
using System.Text;
using WazaWare.co.za.Mappers;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.Services
{
    public class ProductService
    {
        public List<Product> ReadCSVFileSingle(string path)
        {
            Console.WriteLine(path);
            try
            {
                using (var reader = new StreamReader(path, Encoding.Default))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<ProductMapSingle>();
                    var rows = csv.GetRecords<Product>().ToList();
                    return rows;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
		public List<ProductImageURLs> ReadCSVFileImage(string path)
		{
			Console.WriteLine(path);
			try
			{
				using (var reader = new StreamReader(path, Encoding.Default))
				using (var csv = new CsvReader(reader))
				{
					csv.Configuration.RegisterClassMap<ProductMapImages>();
					var rows = csv.GetRecords<ProductImageURLs>().ToList();
					return rows;
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
		public void SaveCSVFile(string path, List<Product> product)
        {
            using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(true)))
            using (CsvWriter csvw = new CsvWriter(sw))
            {
                csvw.WriteHeader<Product>();
                csvw.NextRecord();
                foreach (Product prod in product)
                {
                    csvw.WriteRecord<Product>(prod);
                    csvw.NextRecord();
                }
            }
        }
    }
}
