using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;
using WazaWare.co.za.Services;
using X.PagedList;

namespace WazaWare.co.za.Controllers
{
	public class BackendController : Controller
	{
		private readonly WazaWare_db_context _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string API_KEY = "b4798a48-3db7-4bfd-8cdf-7e1d4dde5ed2";
		private static readonly HttpClient client = new();
		public BackendController(WazaWare_db_context context, IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_webHostEnvironment = webHost;
			_httpContextAccessor = httpContextAccessor;
		}
		/// <summary>
		/// Index ViewModels for Backend
		/// </summary>
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> ScrapeUrl(int startScraper)
		{
			if (startScraper > 0)
			{
				WebscrapperIoApiClient webscrapper = new(_context);
				await webscrapper.StartRequests();
			}
			return View();
		}
        public IActionResult Portal()
		{
			return View();
		}
        public IActionResult ManageUsers()
		{
			var users = _context.Users!.ToList();
			var viewModel = new UserModel
			{
				Users = users
			};
			return View(viewModel);
		}
		public async Task DeleteAllUsers()
		{
			var users = _context.Users!.ToList();
			_context.Users!.RemoveRange(users!);
			await _context.SaveChangesAsync();
		}
        public async Task DeleteCookies()
        {
            var cookies = _context.Users!.Where(s => s.Email!.Contains("wazaware.co.za")).ToList();
            _context.Users!.RemoveRange(cookies!);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Downloads and Image is URL. Saves Downloaded Image to Database
        /// </summary>
        [HttpGet]
		public async Task DownloadAndSaveImage(Dictionary<int, string> productUrls)
		{
			Console.WriteLine("Preparing to Download Images...");
			string proxy = "https://proxy.scrapeops.io/v1/";
			string apiKey = "b4798a48-3db7-4bfd-8cdf-7e1d4dde5ed2";
			var productImages = from s in _context.ProductImages
								select s.ImageFileName;
			foreach (KeyValuePair<int, string> productUrl in productUrls)
			{
				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
				httpClient.Timeout = TimeSpan.FromMinutes(5);
				var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
				query["api_key"] = apiKey;
				query["url"] = productUrl.Value;

				var proxyUrl = $"{productUrl.Value}?{query}";

				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine();
				Console.WriteLine("\n------------------------------------------------->>");
				Console.WriteLine(
					$"	Connecting to Proxy:\n" +
					$"		Proxy:\n" +
					$"		{proxy}\n" +
					$"		Target:\n" +
					$"		{proxyUrl}\n");
				using var response = await httpClient.GetAsync(proxyUrl);
				if (response.IsSuccessStatusCode)
				{
					Console.Beep(1500, 100);
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.Green;
					Console.WriteLine();
					Console.WriteLine("\n<<-------------------------------------------------");
					Console.WriteLine(
						$"	{proxy} Responded!\n" +
						$"		Response:\n" +
						$"		{response.StatusCode}\n");
					var imageData = await response.Content.ReadAsByteArrayAsync();
					var fileName = Path.GetFileName(productUrl.Value);
					var fileType = Path.GetExtension(productUrl.Value);
					var Image = new ProductImage
					{
						ProductId = productUrl.Key,
						ImageFileName = $"{fileName}",
						ImageFileType = fileType,
						ImageFileContent = imageData
					};
					if (productImages.Contains(fileName))
					{
						Console.WriteLine("File Exists");
					}
					else
					{
						Console.WriteLine(
						$"Saving File...\n" +
						$"File: {Image.ImageFileName}");
						_context.ProductImages.Add(Image);
						await _context.SaveChangesAsync();
					}
				}
				else
				{
					Console.WriteLine($"Fail!\n" +
						$"Response: {response}\n");
				}
			}
			/// <summary>
			///try
			///{
			///	using (var httpClient = new HttpClient())
			///	{
			///		consoleLogs.Add("Starting..."); ;
			///		httpClient.DefaultRequestHeaders.Add("UserAccount-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0");
			///		httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
			///		httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
			///		httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
			///		httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			///		httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			///		httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
			///		httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
			///		httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
			///		httpClient.DefaultRequestHeaders.Add("Sec-Fetch-UserAccount", "?1");
			///		httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
			///		HttpResponseMessage response = await httpClient.GetAsync(url);
			///		//var response = await httpClient.GetAsync(url);
			///		if (response.IsSuccessStatusCode)
			///	{
			///			consoleLogs.Add("Download and Save Image Task has Started!");
			///			Console.WriteLine();
			///			consoleLogs.Add("---------------->> " + response);
			///			consoleLogs.Add("Fetching Image Data...");
			///			var imageData = await response.ImageFileContent.ReadAsByteArrayAsync();
			///			consoleLogs.Add($"{imageData}" + " <<--------------");
			///			var fileName = Path.GetFileName(url);
			///			consoleLogs.Add($"{fileName}" + " <<--------------");
			///			var fileType = Path.GetExtension(url);
			///			consoleLogs.Add($"{fileType}" + " <<--------------");
			///			consoleLogs.Add("Creating ProductImage...");
			///			var Image = new ProductImage
			///			{
			///				ProductKey = id,
			///				FileName = fileName,
			///				FileType = fileType,
			///				Content = imageData
			///			};
			///			consoleLogs.Add("Updating Database...");
			///			_context.ProductImages.Add(Image);
			///			await _context.SaveChangesAsync();
			///			GetImage(id);
			///			consoleLogs.Add("DONE!");
			///			Console.WriteLine("------------------------------------>>");
			///		}
			///		else
			///		{
			///			consoleLogs.Add("---------------->> " + response);
			///		}
			///		foreach (var item in consoleLogs)
			///		{
			///			Console.WriteLine(item);
			///		}
			///	}
			///}
			///catch (NotSupportedException ex)
			///{
			///	Console.WriteLine(ex.Message);
			///	Console.WriteLine("------------>> " + "!COMPLETED!" + " <<--------------");
			///};
			///return consoleLogs;
			/// </summary>
		}
		/// <summary>
		/// ViewModels method for Downloading and Saving Images. Gets and passes image URL to correct methods
		/// </summary>
		//[HttpGet]
		public async Task<IActionResult> GetImages(int startAutoDownload, int clear)
		{
			var products = from p in _context.Products
						   select p;
			var images = from i in _context.ProductImages
						 select i;
			int counter = 0;
			if (clear > 0)
			{
				await ClearImages();
			}
			if (startAutoDownload > 0)
			{
				Dictionary<int, string> productUrls = new();
				Dictionary<int, string> imageFiles = new();
				Queue<int> id = new();
				Queue<string> url = new();
				int productCount = products.Count();
				foreach (var product in products)
				{
					Console.WriteLine("Loading Product...\n" +
						$"ProductId: {product.ProductId}\n" +
						$"URL: {product.ProductImageUrl}");
					productUrls.Add(product.ProductId, product.ProductImageUrl!);
				}
				foreach (var image in images)
				{
					Console.WriteLine("Loading Image...\n" +
						$"Product Id: {image.ProductId}\n" +
						$"File Name: {image.ImageFileName}");
					imageFiles.Add(image.ProductId, image.ImageFileName!);
				}
				foreach (KeyValuePair<int, string> product in productUrls)
				{
					foreach (KeyValuePair<int, string> image in imageFiles)
					{
						if (product.Key == image.Key)
						{
							Console.WriteLine(product.Value + " : Image already Exists!");
							productUrls.Remove(product.Key);
							Console.WriteLine(product.Value + " : Removed from Download Queue!");
							counter++;
						}
					}
				}
				Console.Write($"Total Product Images : {products.Count()}\n" +
					$"Original Download Queue : {productUrls.Count + counter}" +
					$"Duplicate Images : {counter}\n" +
					$"Optimized Download Queue : {productUrls.Count} Images to Download\n" +
					"Enter (yes / no) to Confirm\n>");
				var input = Console.ReadLine();
				if (input == "yes" | input!.Equals("yes"))
					await DownloadAndSaveImage(productUrls);
				else
					Console.Write("You have Exited!");
			}
			return View();
		}
		/// <summary>
		/// Method called to Delete all images from Database
		/// </summary>
		[HttpPost]
		public async Task ClearImages()
		{
			var imageModel = _context.ProductImages;
			foreach (var image in imageModel)
			{
				_context.ProductImages.Remove(image);
			}
			await _context.SaveChangesAsync();
		}
		/// <summary>
		/// Gets Image. Method called.
		/// </summary>
		[HttpGet]
		public IActionResult GetImage(int id)
		{
			var imageModel = _context.ProductImages!.FirstOrDefault(img => img.ProductId.Equals(id));
			if (imageModel != null && imageModel.ImageFileContent != null)
				return File(imageModel.ImageFileContent, "image/jpeg");
			else return NotFound();

		}
		/// <summary>
		/// Automates Adding Products from CSV file. Calls Services.
		/// </summary>
		[HttpGet]
		public IActionResult AutoProductCreateTestImage()
		{
			AddCSV product = new();
			return View(product);
		}
		/// <summary>
		/// Automates Adding Products from CSV file. Calls Services.
		/// </summary>
		[HttpPost]
		public ActionResult AutoProductCreateTestImage(AddCSV addCSV)
		{
			var _productService = new ProductService();
			string uniqueCSVFileName = ProcessUploadedCSVFile(addCSV);
			var location = "wwwroot\\Uploads\\" + uniqueCSVFileName;
			var rowData = _productService.ReadCSVFileImage(location);
			foreach (ProductImageURLs item in rowData)
			{
				ProductImageURLs productImages = new()
				{
					VendorSiteOrigin = item.VendorSiteOrigin,
					VendorSiteProduct = item.VendorSiteProduct,
					ProductId = item.ProductId,
					ImageURL = item.ImageURL
				};
				_context.Attach(productImages);
				_context.Entry(productImages).State = EntityState.Added;
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(Index));
		}
		/// <summary>
		/// Automates Adding Products from CSV file. Calls Services.
		/// </summary>
		[HttpGet]
		public IActionResult AutoProductCreateTest()
		{
			AddCSV product = new();
			return View(product);
		}
		/// <summary>
		/// Automates Adding Products from CSV file. Calls Services.
		/// </summary>
		[HttpPost]
		public ActionResult AutoProductCreateTest(AddCSV addCSV)
		{
			var _productService = new ProductService();
			string uniqueCSVFileName = ProcessUploadedCSVFile(addCSV);
			var location = "wwwroot\\Uploads\\" + uniqueCSVFileName;
			var rowData = _productService.ReadCSVFileSingle(location);
			foreach (Product item in rowData)
			{
				Product product = new()
				{
					ProductVendorName = item.ProductVendorName,
					ProductVendorUrl = item.ProductVendorUrl,
					ProductCategory = item.ProductCategory,
					ProductName = item.ProductName,
					ProductStock = item.ProductStock,
					ProductPriceBase = item.ProductPriceBase,
					ProductPriceSale = item.ProductPriceSale,
					ProductDescription = item.ProductDescription,
					ProductImageUrl = item.ProductImageUrl,
					ProductVisibility = "ProductVisibility",
					ProductDataBatchNo = "March23"
				};
				_context.Attach(product);
				_context.Entry(product).State = EntityState.Added;
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(Index));
		}
		/// <summary>
		/// Method is called to process CSV File
		/// </summary>
		private string ProcessUploadedCSVFile(AddCSV model)
		{
			string uniqueFileName = "ERROR";
			string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (model.CSVFile != null)
			{
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CSVFile.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using var fileStream = new FileStream(filePath, FileMode.Create);
				model.CSVFile.CopyTo(fileStream);
			}
			return uniqueFileName;
		}
		/// <summary>
		/// EXPERIMENTAL FEATURE
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> SendSMSToTelkom(string mobileNumber, string message)
		{
			try
			{
				SmtpClient smtpClient = new("smtp.gmail.com", 587)
				{
					EnableSsl = true,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential("brandenconnected@gmail.com", "mueadqbombixceuk")
				};

				MailMessage mailMessage = new()
				{
					From = new MailAddress("brandenconnected@gmail.com")
				};
				mailMessage.To.Add($"{mobileNumber}@sms.co.za");// use the appropriate email-to-SMS gateway domain
				mailMessage.CC.Add("brandenconnected@gmail.com");
				mailMessage.Subject = "SMS message";
				mailMessage.Body = message[..Math.Min(160, message.Length)]; // limit the message to 160 characters

				await smtpClient.SendMailAsync(mailMessage);
				Console.WriteLine("Successfully Sent SMS\n" +
					$"From : {mailMessage.From}\n" +
					$"To : {mailMessage.To}\n" +
					$"Subject : {mailMessage.Subject}\n" +
					$"Body : {mailMessage.Body}\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while sending the SMS message: {ex.Message}");
			}
			return RedirectToAction("Index");
		}
		/// <summary>
		/// EXPERIMENTAL FEATURE
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> Gpt3Recipe(Gpt3 ingredients)
		{
			string recipe = string.Empty;
			string prompt = "Please give me a recipe based on the following ingredients:"
							+ " " + ingredients.Ingredient1
							+ " " + ingredients.Ingredient2
							+ " " + ingredients.Ingredient3
							+ " " + ingredients.Ingredient4
							+ " " + ingredients.Ingredient5;
			Console.WriteLine(prompt);
			HttpClient client = new();
			client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
			var content = new StringContent("{\"model\": \"text-davinci-001\", \"prompt\": \"" + prompt + "\",\"temperature\": 1,\"max_tokens\": 250}",
				Encoding.UTF8, "application/json");

			HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

			string? responseString = await response.Content.ReadAsStringAsync();
			Console.WriteLine(responseString);
			if (responseString != null)
			{
				dynamic? responseObj = JsonConvert.DeserializeObject(responseString);
				if (responseObj != null)
				{
					if (responseObj.choices.Count > 0)
					{
						recipe = responseObj.choices[0].text;
						Console.WriteLine(recipe);
						recipe = recipe.Replace("\n\n", "</p><p style='text-align:center; padding:15px 0;'>");
						recipe = recipe.Replace("\n", "<br style='text-align:center; padding:15px 0;'>");
						recipe = $"<p style='text-align:center; padding:15px 0;'>{recipe}</p>";
						ViewBag.recipe = recipe;
						return RedirectToAction("Gpt3", new RouteValueDictionary(new { text = recipe }));
					}
					else
					{
						ViewBag.recipe = "No recipe found.";
						return RedirectToAction("Gpt3", new RouteValueDictionary(new { recipe = "No recipe found." }));

					}
				}
			}
			return RedirectToAction("Gpt3", new RouteValueDictionary(new { text = recipe }));
		}
		/// <summary>
		/// EXPERIMENTAL FEATURE
		/// </summary>
		[HttpGet]
		public IActionResult Gpt3(string text)
		{
			ViewBag.recipe = text;
			return View();
		}

	}
}

