using wazaware.co.za.Models.DatabaseModels;
using wazaware.co.za.Models.ViewModels;
using wazaware.co.za.DAL;

namespace wazaware.co.za.Services
{
	public class WebServices
	{
		private readonly wazaware_db_context _DbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public WebServices(wazaware_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_DbContext = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public UserView LoadViewUser(string userEmail)
		{
			var user = _DbContext.UserAccountDb!.Where(x => x.Equals(userEmail)).FirstOrDefault();
			var userView = new UserView
			{
				FirstName = user!.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Phone = user.Phone
			};
			return userView!;
		}
		public List<Order> LoadOrders(int userId)
		{
			// Database Query : Get OrderDb => WHERE : userId is TRUE
			var userOrders = _DbContext.OrderDb!
				.Where(s => s.UserId == userId).ToList();		
			return userOrders;
		}
		public List<OrderCartView> LoadOrderedProductsView(int userId)
		{
			// Database Query : Get OrderDb => WHERE : userId is TRUE
			var userOrders = _DbContext.OrderDb!
				.Where(s => s.UserId == userId).ToList();
			var orderedProducts = _DbContext.OrderedProductsDb!
				.Where(s => userOrders.Select(s => s.OrderId).Contains(s.OrderId)).ToList();
			var products = _DbContext.ProductDb!
				.Where(p => orderedProducts.Select(s => s.ProductId).Contains(p.ProductId)).ToList();
			var joinOrders = orderedProducts
				.Join(products, c => c.ProductId, p => p.ProductId, (c, p)
				=> new { orderedProducts = c, Product = p });
			//.Join(userOrders, c => c.orderedProducts.OrderId, o => o.OrderId,(c, o)
			//=> new {Order = o,  ;
			var totalOrder = joinOrders.Select(s => s.orderedProducts.ProductTotal).Sum();
			var view = joinOrders.Select(s => new OrderCartView
			{
				OrderId = s.orderedProducts.OrderId,
				ProductId = s.Product.ProductId,
				ProductName = s.Product.ProductName,
				ProductPriceBase = s.Product.ProductPriceBase,
				ProductPriceSale = s.Product.ProductPriceSale,
				ProductImageUrl = s.Product.ProductImageUrl,
				// Count : Quantity of a ProductInfomation in Cart
				ProductCount = s.orderedProducts.ProductCount,
				// Calculate : ProductDb Sale Total
				ProductTotal = s.orderedProducts.ProductTotal,
				ProductBaseSaleDiff = s.Product.ProductPriceBase - s.Product.ProductPriceSale,
				OrderSaleTotal = totalOrder,
				ShippingCost = userOrders
				.Where(a => a.OrderId == s.orderedProducts.OrderId).Select(a => a.ShippingPrice).First(),
				ProductPic = s.Product.ProductPic
			}).ToList();
			return view;
		}
		public void UpdateLoadedUser(UserAccount model)
		{
			const string cookieName = "wazaware.co.za-auto-sign-in";
			var requestCookies = _httpContextAccessor.HttpContext!.Request.Cookies;
			_ = requestCookies[cookieName];
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			if (!requestCookies.ContainsKey(cookieName))
			{
				_httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, model.Email!, cookieOptions);
			}
			else
			{
				_httpContextAccessor.HttpContext!.Response.Cookies.Delete(cookieName);
				_httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, model.Email!, cookieOptions);
			}
		}

		public List<ShoppingCartView> LoadCart(int userId)
		{
			// Query Database for UserAccount's Shopping Cart using variable userId
			// Convert Database response to a List using : .ToList()
			var ShoppingCart = _DbContext.ShoppingCartDb!
				.Where(c => c.UserId == userId)
				.ToList();
			// Select ProductInfomation Ids in Shopping Cart List
			var productIds = ShoppingCart
				.Select(c => c.ProductId)
				.ToList();
			// Query Database for ProductDb related to UserAccount's Shopping Cart
			// Convert Database response to a List using : .ToList()
			var products = _DbContext.ProductDb!
				.Where(p => productIds.Contains(p.ProductId))
				.ToList();
			// Join ProductDb & Shopping Cart Lists
			var joint = ShoppingCart
				.Join(products, c => c.ProductId, p => p.ProductId, (c, p) => new { ShoppingCart = c, Product = p })
				.ToList();
			// Queue ProductInfomation Total Values 
			Queue<decimal?> totalBase = new();
			Queue<decimal?> totalSale = new();
			foreach (var product in joint)
			{
				totalBase.Enqueue(product.Product.ProductPriceBase * product.ShoppingCart.ProductCount);
				totalSale.Enqueue(product.Product.ProductPriceSale * product.ShoppingCart.ProductCount);
			}
			// Calculations & Building ShopViewModel Model:
			decimal? shippingThreshold = 1500;
			decimal? shippingCost = 0;
			decimal? tryGetFreeShipping = 0;
			// Calculate : Shipping Costs
			if (totalSale.Sum() < shippingThreshold)
			{
				shippingCost = 450;
				tryGetFreeShipping = 1500 - totalSale.Sum();
			}
			// Calculate Sum : Cart Base Total
			decimal? cartBaseTotal = totalBase.Sum() + shippingCost;
			// Calculate Sum : Cart Sale Total
			decimal? cartSaleTotal = totalSale.Sum() + shippingCost;
			// Calculate Difference : Cart Base Total - Cart Sale Total
			decimal? cartBaseSaleDiff = cartBaseTotal - cartSaleTotal;
			// Build Model
			var productsInCart = products
				.Select(x => new ShoppingCartView
				{
					ProductId = x.ProductId,
					ProductName = x.ProductName,
					ProductPriceBase = x.ProductPriceBase,
					ProductPriceSale = x.ProductPriceSale,
					ProductImageUrl = x.ProductImageUrl,
					// Count : Quantity of a ProductInfomation in Cart
					ProductCount = ShoppingCart
					.Where(p => p.ProductId == x.ProductId)
					.Select(s => s.ProductCount)
					.First(),
					// Calculate : ProductDb Sale Total
					ProductTotal = ShoppingCart
					.Where(p => p.ProductId == x.ProductId)
					.Select(s => s.ProductCount)
					.First() * x.ProductPriceSale,
					ProductBaseSaleDiff = x.ProductPriceBase - x.ProductPriceSale,
					CartBaseTotal = cartBaseTotal,
					CartSaleTotal = cartSaleTotal,
					CartBaseSaleDiff = cartBaseSaleDiff,
					ShippingCost = shippingCost,
					TryGetFreeShipping = tryGetFreeShipping,
					ProductPic = x.ProductPic
				});
			// Return UserAccount's Cart Information in a List Data Type to Relevant Action Method
			return productsInCart.ToList();

		}
		public UserAccount? LoadDbUser()
		{
			const string cookieName = "wazaware.co.za-auto-sign-in";

			var requestCookies = _httpContextAccessor.HttpContext!.Request.Cookies;
			var intialRequest = requestCookies[cookieName];
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			if (intialRequest != null)
			{
				if (_DbContext.UserAccountDb!.Any(u => u.Email == intialRequest))
				{
					var user = _DbContext.UserAccountDb!.Where(x => x.Email == intialRequest).FirstOrDefault();
					return user!;
				}
				else
				{
					var email = CreateCookieReferance().Result;

					_httpContextAccessor.HttpContext!.Response.Cookies
						.Append(cookieName, email, cookieOptions);
					var user = _DbContext.UserAccountDb!
						.Where(x => x.Email == email).FirstOrDefault();
					return user!;
				}
			}
			else
			{
				var email = CreateCookieReferance().Result;
				_httpContextAccessor.HttpContext!.Response.Cookies
					.Append(cookieName, email, cookieOptions);
				var user = _DbContext.UserAccountDb!
					.Where(x => x.Email == email).FirstOrDefault();
				return user!;
			}
		}
		public async Task<string> CreateCookieReferance()
		{
			string generatedName = "wazaware.co.za-auto-sign-in";
			string generatedEmail = "wazaware.co.za-auto-sign-in" + Guid.NewGuid().ToString() + "@wazaware.co.za";
			string generatedPhone = "1234567890";
			string generatedPwd = "3%@D8Iy2?Kt7*ceK";
			var user = new UserAccount
			{
				FirstName = generatedName,
				LastName = generatedName,
				Email = generatedEmail,
				Phone = generatedPhone,
				Password = generatedPwd,
				Joined = DateTime.Now
			};
			_DbContext!.UserAccountDb!.Add(user);
			await _DbContext!.SaveChangesAsync();
			return generatedEmail;
		}
		public Dictionary<string, string> Capitilize(Dictionary<string, string> data)
		{
			Dictionary<string, string> updatedData = new();
			foreach (KeyValuePair<string, string> dict in data)
			{
				string updatedString = string.Empty;
				char[] stringCharacters = dict.Value.ToCharArray();
				for (int i = 0; i < stringCharacters.Length; i++)
				{
					if (i == 0)
					{
						string tmp = stringCharacters[0].ToString().ToUpper();
						updatedString += tmp;
					}
					else
						updatedString += stringCharacters[i];
				}
				updatedData.Add(dict.Key, updatedString);
			}			
			return updatedData;
		}

	}
}
