using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;

namespace wazaware.co.za.Services
{
	public class WebServices
	{
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public WebServices(WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public UserModel LoadUser(string userEmail)
		{
			var user = _context.Users.Where(x => x.Equals(userEmail)).FirstOrDefault();
			return user!;
		}
		public List<ProductsInCartModel> LoadCart(int userId)
		{
			// Query Database for User's Shopping Cart using variable userId
			// Convert Database response to a List using : .ToList()
			var userShoppingCart = _context.UsersShoppingCarts
				.Where(c => c.UserId == userId)
				.ToList();
			// Select Product Ids in Shopping Cart List
			var productIds = userShoppingCart
				.Select(c => c.ProductId)
				.ToList();
			// Query Database for Products related to User's Shopping Cart
			// Convert Database response to a List using : .ToList()
			var products = _context.Products
				.Where(p => productIds.Contains(p.ProductId))
				.ToList();
			// Join Products & Shopping Cart Lists
			var joint = userShoppingCart
				.Join(products, c => c.ProductId, p => p.ProductId, (c, p) => new { ShoppingCart = c, Product = p })
				.ToList();
			// Queue Product Total Values 
			Queue<decimal?> totalBase = new();
			Queue<decimal?> totalSale = new();
			foreach (var product in joint)
			{
				totalBase.Enqueue(product.Product.ProductPriceBase * product.ShoppingCart.ProductCount);
				totalSale.Enqueue(product.Product.ProductPriceSale * product.ShoppingCart.ProductCount);
			}
			// Calculations & Building ViewModels Model:
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
				.Select(x => new ProductsInCartModel
				{
					ProductId = x.ProductId,
					ProductName = x.ProductName,
					ProductPriceBase = x.ProductPriceBase,
					ProductPriceSale = x.ProductPriceSale,
					ProductImageUrl = x.ProductImageUrl,
					// Count : Quantity of a Product in Cart
					ProductCount = userShoppingCart
					.Where(p => p.ProductId == x.ProductId)
					.Select(s => s.ProductCount)
					.First(),
					// Calculate : Products Sale Total
					ProductTotal = userShoppingCart
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
			// Return User's Cart Information in a List Data Type to Relevant Action Method
			return productsInCart.ToList();

		}
		public async Task<string> CreateCookieReferance()
		{
			string generatedName = "wazaware.co.za-auto-sign-in";
			string generatedEmail = "wazaware.co.za-auto-sign-in" + Guid.NewGuid().ToString() + "@wazaware.co.za";
			string generatedPhone = "1234567890";
			string generatedPwd = "3%@D8Iy2?Kt7*ceK";

			var userModel = new UserModel
			{
				FirstName = generatedName,
				LastName = generatedName,
				Email = generatedEmail,
				Phone = generatedPhone,
				Password = generatedPwd,
				Joined = DateTime.Now
			};
			_context!.Users.Add(userModel);
			await _context!.SaveChangesAsync();
			var user = _context!.Users.Where(s => s.Email!.Equals(generatedEmail)).FirstOrDefault();
			return user!.Email!;
		}

	}
}
