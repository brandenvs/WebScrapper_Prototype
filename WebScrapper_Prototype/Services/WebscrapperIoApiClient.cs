using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Web;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.Services
{
	public class WebscrapperIoApiClient
	{
		private readonly WazaWare_db_context _context;
		readonly Dictionary<int, string?> VendorSite = new();
		readonly Dictionary<int, string?> VendorProductURL = new();
		readonly Dictionary<int, string?> ProductKey = new();
		readonly Dictionary<int, string?> ProductCategory = new();

		//Dictionary<int, string?> ProductName = new();
		readonly Dictionary<int, string?> ProductStock = new();
		readonly Dictionary<int, decimal?> ProductBasePrice = new();
		readonly Dictionary<int, decimal?> ProductSalePrice = new();
		readonly Dictionary<int, string?> ProductDescription = new();
		readonly Dictionary<int, string?> ProductImageURL = new();
		readonly Dictionary<int, string?> Visible = new();
		readonly Dictionary<int, string?> dataBatch = new();

		private readonly string url = "https://proxy.scrapeops.io/v1/";
		private readonly string targetUrl = "https://www.wootware.co.za/sale";
		private readonly string apiKey = "b4798a48-3db7-4bfd-8cdf-7e1d4dde5ed2";
		public WebscrapperIoApiClient(WazaWare_db_context context)
		{
			_context = context;
		}
		public async Task StartRequests()
		{
			string filename = "response.html";
			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
			httpClient.Timeout = TimeSpan.FromMinutes(5);
			// Check if the file already exists
			if (File.Exists(filename))
			{
				Console.WriteLine(
					"*************************************************************************\n" +
					"********** Welcome to my custom C# Web Crawler & Scraper\n" +
					"********** Created by:\n" +
					"********** Branden van Staden\n\n\n");
				Console.WriteLine("\n<<-------------------------------------------------");
				Console.WriteLine(
					"	Found Start URL: " +
					$"		{filename}\n");
				Console.Write(
					"	Update File(y/n)?");
				string? consoleInput = Console.ReadLine();
				if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("y") || consoleInput.ToLower().Equals("yes")))
					await NewRequestWithProxy();
				else if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("n") || consoleInput.ToLower().Equals("no")))
					await StartScraping(filename);
				else
					Console.WriteLine("Invalid Input");
			}
			else
				await NewRequestWithProxy();
		}
		public async Task StartScraping(string filename)
		{
			int countId = -1;
			Console.WriteLine("	<<-------------------------------------------------");
			Console.WriteLine(
				"	Getting Children\n" +
				"		File: " + $"{filename}");
			Dictionary<int, string> missingChildren = new();
			Dictionary<int, string> kidnappedChildren = new();

			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(filename);

			// Get the parent nodes
			HtmlNodeCollection CatNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='box-collateral block-soldtogether-cutomer soldtogether-block']");

			// Iterate over each parent node
			foreach (HtmlNode grand in CatNodes)
			{
				// Get the category name
				HtmlNode productCat = grand.SelectSingleNode(".//div[@class='h2']");
				string category = productCat.InnerText.Trim();

				// Get the product nodes for this category
				HtmlNodeCollection ProductNodes = grand.SelectNodes(".//h3[@class='product-name']/a");

				// Iterate over each product node for this category
				foreach (HtmlNode parent in ProductNodes)
				{
					countId++;
					string href = parent.GetAttributeValue("href", "");
					filename = Path.Combine(@"K:\Web", "response" + countId + ".html");

					Console.WriteLine("------------------------------------------------->>");
					Console.WriteLine($"Checking if Child already exists in\n" +
									  $"    File: {filename}\n" +
									  $"    Category: {category}\n" +
									  $"    Hyperlink: {href}");
					if (!File.Exists(filename))
					{
						missingChildren.Add(countId, href);
						Console.WriteLine($"Couldn't find child {filename}");
					}
					else
					{
						Console.WriteLine($"Found Child {filename}");
						VendorProductURL.Add(countId, href);
						VendorSite.Add(countId, url);
						ProductCategory.Add(countId, category);
						kidnappedChildren.Add(countId, filename);
					}
				}
			}
			Console.WriteLine("<<-------------------------------------------------");
			Console.WriteLine(
				$"	Couldn't find {missingChildren.Count} children\n" +
				$"	Found {kidnappedChildren.Count} children\n");
			Console.Write(
				$"	What would you like to do next?\n" +
				$"		1) Use Proxy to scrape {url} & Find {missingChildren.Count} missing children?\n" +
				$"		2) Scrape existing {kidnappedChildren.Count} children?\n");
			string? consoleInput = Console.ReadLine();
			if (!string.IsNullOrEmpty(consoleInput) && consoleInput != null && consoleInput.ToLower().Equals("1"))
				await ProxyScrapeOps(missingChildren);
			else if (!string.IsNullOrEmpty(consoleInput) && consoleInput != null && consoleInput.ToLower().Equals("2"))
				await ScrapeWebPage(kidnappedChildren);
		}
		public async Task ProxyScrapeOps(Dictionary<int, string> productUrls)
		{
			Console.WriteLine("Web Crawler has starting...");
			var foundproducts = new List<string>();
			Queue<int> id = new();
			Queue<string> urls = new();
			int count = 0;
			foreach (KeyValuePair<int, string> pair in productUrls)
			{
				id.Enqueue(pair.Key);
				urls.Enqueue(pair.Value);
			}
			while (id.Count > 0)
			{
				int freq = 1000;
				int duration = 2000;
				string filename = Path.Combine(@"K:\Web", "response" + id.Dequeue() + ".html");
				bool test = true;
				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
				httpClient.Timeout = TimeSpan.FromMinutes(5);
				string url = urls.Dequeue();

				var query = HttpUtility.ParseQueryString(string.Empty);
				string proxy = "https://proxy.scrapeops.io/v1/";
				string apiKey = "b4798a48-3db7-4bfd-8cdf-7e1d4dde5ed2";
				query["api_key"] = apiKey;
				query["url"] = url;
				var proxyUrl = $"{proxy}?{query}";

				var query2 = HttpUtility.ParseQueryString(string.Empty);
				string proxy2 = "http://api.scraperapi.com/";
				string apiKey2 = "cad808d14e0dfe3e03e1e5c6a576a609";
				query2["api_key"] = apiKey2;
				query2["url"] = url;
				var proxyUrl2 = $"{proxy2}?{query2}";

				while (test)
				{
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
					HttpResponseMessage response = await httpClient.GetAsync(proxyUrl);
					if (response.IsSuccessStatusCode)
					{
						Console.Beep(freq, duration);
						Console.ForegroundColor = ConsoleColor.Black;
						Console.BackgroundColor = ConsoleColor.Green;
						Console.WriteLine();
						Console.WriteLine("\n<<-------------------------------------------------");
						Console.WriteLine(
							$"	Attempt {count} has Finished!");
						Console.WriteLine(
							$"	{proxy} Responded!\n" +
							$"		Response:\n" +
							$"		{response.StatusCode}\n");
						var responseBody = await response.Content.ReadAsStringAsync();
						File.WriteAllText(filename, responseBody);
						Console.WriteLine(
							$"	Storing Response\n" +
							$"		SUCCESS!\n" +
							$"			File location:\n" +
							$"			{filename} \n");
						Console.WriteLine("\n------------------------------------------------->>");
						Console.WriteLine("	Crawling next URL");
						test = false;
						count = 0;
						Console.WriteLine();
					}
					else
					{
						Console.Beep(500, 1000);
						Console.BackgroundColor = ConsoleColor.DarkYellow;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine();
						Console.WriteLine("\n<<-------------------------------------------------");
						Console.WriteLine(
							$"	{proxy} Connection Closed\n");
						Console.ForegroundColor = ConsoleColor.White;
						Console.BackgroundColor = ConsoleColor.DarkMagenta;
						Console.WriteLine();
						Console.WriteLine("\n------------------------------------------------->>");
						Console.WriteLine(
							$"	Connecting to Proxy:\n" +
							$"		Proxy:\n" +
							$"		{proxy2}\n" +
							$"		Target:\n" +
							$"		{proxyUrl2}\n");
						HttpResponseMessage response2 = await httpClient.GetAsync(proxyUrl2);
						if (response2.IsSuccessStatusCode)
						{
							Console.Beep(freq, duration);
							Console.ForegroundColor = ConsoleColor.Black;
							Console.BackgroundColor = ConsoleColor.Green;
							Console.WriteLine("\n<<-------------------------------------------------");
							Console.WriteLine(
								$"	Attempt {count} has Finished!");
							Console.WriteLine(
								$"	{proxy2} Responded!\n" +
								$"		Response:\n" +
								$"		{response2.StatusCode}");
							var responseBody2 = await response2.Content.ReadAsStringAsync();
							File.WriteAllText(filename, responseBody2);
							Console.WriteLine(
								$"	Storing Response\n" +
								$"		SUCCESS!\n" +
								$"		 File location:\n" +
								$"		 {filename} \n");
							test = false;
							count = 0;
							Console.WriteLine();
						}
						else
						{
							Console.BackgroundColor = ConsoleColor.Red;
							Console.ForegroundColor = ConsoleColor.Black;
							Console.WriteLine();
							Console.WriteLine("\n<<-------------------------------------------------");
							Console.WriteLine(
								$"Attempt {count} has Finished!\n" +
								$"	{proxy} Responded!\n" +
								$"		Response:\n" +
								$"		{response.StatusCode}\n");
							Console.WriteLine(
								$"	{proxy2} Responded!\n" +
								$"		Response:\n" +
								$"		{response2.StatusCode}\n");
							Console.WriteLine("\n<<-------------------------------------------------");
							Console.WriteLine(
								$">>Attempt {count} has Terminated!<<");
							Console.WriteLine("\n------------------------------------------------->>");
							Console.WriteLine(
								"I'll do better next time <3\n" +
								"Retrying...");
							Console.WriteLine();
							count++;
						}
					}
				}

			}
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine();
			Console.WriteLine("\n<<-------------------------------------------------------------\n");
			Console.Write(
				$"	FINISHED SCRAPING!\n" +
				$"	{foundproducts.Count} were children kidnapped\n\n");
		}
		public async Task NewRequestWithProxy()
		{
			int count = 0;
			string filename = "response.html";
			bool test = true;

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
			httpClient.Timeout = TimeSpan.FromMinutes(5);
			Console.WriteLine("Starting Crawler & Spiders ;)\n" +
				"Target: " + $"{targetUrl}");

			var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
			query["api_key"] = apiKey;
			query["url"] = targetUrl;
			var proxyUrl = $"{url}?{query}";

			Console.WriteLine("Connecting to Proxy..." +
				"Target: " + $"{proxyUrl}");

			while (test)
			{
				Console.WriteLine("Waiting for Response...");
				using var response = await httpClient.GetAsync(proxyUrl);
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Response: " + response.StatusCode);
					Console.WriteLine("Reading HTML Response...");
					var responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine("Creating file to store response...");
					File.WriteAllText(filename, responseBody);
					Console.WriteLine("Success!\n\n");
					Console.Write("Do you want to scrap file(y/n)?");
					string? consoleInput = Console.ReadLine();
					if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("y") || consoleInput.ToLower().Equals("yes")))
						await StartScraping(filename);
					else if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("n") || consoleInput.ToLower().Equals("no")))
						Console.WriteLine("DONE!");
					else
						Console.WriteLine("Invalid Input");
					test = false;
				}
				else
				{
					count++;
					Console.WriteLine("Response Status Code: " + response.StatusCode);
					var responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine("----------------------------------------------\n" + responseBody + "\n----------------------------------------------\n");
					Console.WriteLine(count + " Attempt\n" + "Trying Again...");
				}
				Thread.Sleep(5000);
			}
		}
		public async Task ScrapeWebPage(Dictionary<int, string> productUrls)
		{
			Console.Beep(1800, 500);
			foreach (var productUrl in productUrls)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine();
				Console.WriteLine(">Starting to Scraping Children<");
				Console.WriteLine("\n------------------------------------------------->>");
				Console.WriteLine(
					$"	Loading File to be Scraped\n" +
					$"		Child No. {productUrl.Key}" +
					$"		File: {productUrl.Value}\n");
				Console.WriteLine();
				HtmlDocument doc = new();
				doc.Load(productUrl.Value);

				// Get the product name				
				var productNameNode = doc.DocumentNode.SelectSingleNode("//h1[@itemprop='name']");
				if (productNameNode != null && !string.IsNullOrEmpty(productNameNode.InnerHtml))
					ProductKey.Add(productUrl.Key, productNameNode.InnerHtml);
				else
					ProductKey.Add(productUrl.Key, "404");
				// Get the product stock
				var productStockNode = doc.DocumentNode.SelectSingleNode("//div[@class='availability-in-stock']");
				if (productStockNode != null && !string.IsNullOrEmpty(productStockNode.InnerHtml))
					ProductStock.Add(productUrl.Key, productStockNode.InnerHtml);
				else
					ProductStock.Add(productUrl.Key, "404");
				// Get the product description
				var shortDescriptionNode = doc.DocumentNode.SelectSingleNode("//div[@class='short-description']/div[@class='std']");
				if (shortDescriptionNode != null && !string.IsNullOrEmpty(shortDescriptionNode.InnerHtml))
					ProductDescription.Add(productUrl.Key, shortDescriptionNode.InnerText.Trim());
				else
					ProductDescription.Add(productUrl.Key, "404");
				// Image URL
				// Select the anchor element using the XPath
				HtmlNode anchorNode = doc.DocumentNode.SelectSingleNode("//*[@id='product_addtocart_form']/div[2]/div/div[1]/a");

				// Check if the anchor node exists and has an href attribute
				if (anchorNode != null && anchorNode.Attributes["href"] != null)
				{
					// Retrieve the value of the href attribute
					string hrefValue = anchorNode.Attributes["href"].Value;
					Console.WriteLine(hrefValue);
					ProductImageURL.Add(productUrl.Key, hrefValue);
				}
				else
				{
					Console.WriteLine("Anchor element not found or href attribute missing");
				}

				// Find price box element
				HtmlNode priceBox = doc.DocumentNode.SelectSingleNode("//div[@class='price-box']");
				if (priceBox != null)
				{
					// Extract old price
					HtmlNode oldPriceNode = priceBox.SelectSingleNode("//p[@class='old-price']//span[@class='price']");
					if (oldPriceNode != null && !string.IsNullOrEmpty(oldPriceNode.InnerHtml))
						ProductBasePrice.Add(productUrl.Key, decimal.Parse(oldPriceNode.InnerText.Replace("R", "").Replace(",", "")));
					else
						ProductBasePrice.Add(productUrl.Key, 404m);

					// Extract special price
					HtmlNode specialPriceNode = priceBox.SelectSingleNode("//p[@class='special-price']//span[@class='price']");
					if (specialPriceNode != null && !string.IsNullOrEmpty(specialPriceNode.InnerHtml))
						ProductSalePrice.Add(productUrl.Key, decimal.Parse(specialPriceNode.InnerText.Replace("R", "").Replace(",", "")));
					else
						ProductSalePrice.Add(productUrl.Key, 404m);
				}
				else
				{
					ProductBasePrice.Add(productUrl.Key, 404m);
					ProductSalePrice.Add(productUrl.Key, 404m);
				}
				Visible.Add(productUrl.Key, "ProductVisibility");
				dataBatch.Add(productUrl.Key, "March2023");
			}
			Console.Beep(1500, 1000);
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine();
			Console.WriteLine("\n<<-------------------------------------------------------------");
			Console.WriteLine(
				$"FINISHED!\n" +
				$"Successfully Scraped {productUrls.Count} Children!");
			Console.WriteLine();
			Console.Write(
				$"ViewModels Products(y/n)?");
			string? consoleInput = Console.ReadLine();
			if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("y") || consoleInput.ToLower().Equals("yes")))
				await ViewProducts(productUrls);
			else if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("n") || consoleInput.ToLower().Equals("no")))
				Console.WriteLine("DONE!");
			else
				Console.WriteLine("Invalid Input");
		}
		public async Task ViewProducts(Dictionary<int, string> productUrls)
		{
			foreach (var productUrl in productUrls)
			{
				int key = productUrl.Key;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Green;
				Console.WriteLine();
				Console.WriteLine("\n<<-------------------------------------------------------------");
				Console.WriteLine($"Now Viewing Product: " +
					$"\n{productUrl.Key} | {productUrl.Value}");
				foreach (var site in VendorSite.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in VendorProductURL.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductKey.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductCategory.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductStock.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductBasePrice.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductSalePrice.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductDescription.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in ProductImageURL.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in Visible.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
				foreach (var site in dataBatch.Where(s => s.Key == key))
					Console.WriteLine(site.Value);
			}
			Console.WriteLine();
			Console.Beep(1500, 1000);
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine();
			Console.WriteLine("\n<<-------------------------------------------------------------");
			Console.Write(
				$"Save Products to Database(y/n)?");
			string? consoleInput = Console.ReadLine();
			if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("y") || consoleInput.ToLower().Equals("yes")))
				await SaveToDatabase(productUrls);
			else if (string.IsNullOrEmpty(consoleInput) || consoleInput != null && (consoleInput.ToLower().Equals("n") || consoleInput.ToLower().Equals("no")))
				Console.WriteLine("DONE!");
			else
				Console.WriteLine("Invalid Input");
		}
		public async Task SaveToDatabase(Dictionary<int, string> productUrls)
		{
			var products = from p in _context.Products
						   select p;
			int counterRemoved = 0;
			int counterAdded = 0;

			foreach (var productUrl in productUrls)
			{
				var productSample = new List<string>();
				int key = productUrl.Key;
				foreach (var site in ProductKey.Where(s => s.Key == key))
					productSample.Add(site.Value!);
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.WriteLine();
				Console.WriteLine($"Preparing Database:\n" +
					$"Checking {productSample.Count} Products...");
				foreach (var item in productSample)
				{
					Console.WriteLine("\n------------------------------------------------------------->>");
					Console.WriteLine($"	Removing Product {item}");
					var productRowsToDelete = products.Where(s => s.ProductName!.Contains(item));
					counterRemoved++;
					_context.RemoveRange(productRowsToDelete);

				}
				await _context.SaveChangesAsync();
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Green;
				Console.WriteLine();
				Console.WriteLine("\n<<-------------------------------------------------------------");
				Console.WriteLine("Database is updated!");
				Console.Write(
					$"Total Products Removed: {counterRemoved}\n" +
					$"Total Products Added: {counterRemoved}\n");
				Console.WriteLine();
				Console.WriteLine("\n------------------------------------------------------------->>");
				Console.WriteLine($"Now Saving Product to Database: " +
					$"\n{productUrl.Key} | {productUrl.Value}");
				Product product = new();
				foreach (var site in ProductKey.Where(s => s.Key == key))
					product.ProductName = site.Value;
				foreach (var site in ProductStock.Where(s => s.Key == key))
					product.ProductStock = site.Value;
				foreach (var site in ProductDescription.Where(s => s.Key == key))
					product.ProductDescription = site.Value;
				foreach (var site in ProductCategory.Where(s => s.Key == key))
					product.ProductCategory = site.Value;
				foreach (var site in ProductBasePrice.Where(s => s.Key == key))
					product.ProductPriceBase = site.Value;
				foreach (var site in ProductSalePrice.Where(s => s.Key == key))
					product.ProductPriceSale = site.Value;
				foreach (var site in VendorSite.Where(s => s.Key == key))
					product.ProductVendorName = site.Value;
				foreach (var site in VendorProductURL.Where(s => s.Key == key))
					product.ProductVendorUrl = site.Value;
				foreach (var site in Visible.Where(s => s.Key == key))
					product.ProductVisibility = site.Value;
				foreach (var site in dataBatch.Where(s => s.Key == key))
					product.ProductDataBatchNo = site.Value;
				//foreach (var site in ProductId.Where(s => s.Key == key))
				//	product.ProductId = site.Value;
				foreach (var site in ProductImageURL.Where(s => s.Key == key))
					product.ProductImageUrl = site.Value;
				counterAdded++;
				Console.WriteLine("\nSaving to Database...");
				Console.WriteLine(
					$"{product.ProductName}\n" +
					$"{product.ProductVendorUrl}\n" +
					//$"{product.ProductId}\n" +
					$"{product.ProductCategory}\n" +
					$"{product.ProductStock}\n" +
					$"{product.ProductPriceBase}\n" +
					$"{product.ProductPriceSale}\n" +
					$"{product.ProductDescription}\n" +
					$"{product.ProductImageUrl}\n" +
					$"{product.ProductVisibility}\n" +
					$"{product.ProductDataBatchNo}\n");
				_context.Attach(product);
				_context.Entry(product).State = EntityState.Added;
				await _context.SaveChangesAsync();
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Green;
				Console.WriteLine();
				Console.WriteLine("\n<<-------------------------------------------------------------");
				Console.WriteLine("DONE!");
				Console.Write(
					$"Total Products Removed: {counterRemoved}\n" +
					$"Total Products Added: {counterRemoved}\n");
				Console.WriteLine();
			}
			Console.Beep(1500, 1200);
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("\n<<-------------------------------------------------------------");
			Console.WriteLine("Database is updated!");
			Console.Write(
				$"Total Products Removed: {counterRemoved}\n" +
				$"Total Products Added: {counterRemoved}\n");
			Console.WriteLine();
		}
	}
}