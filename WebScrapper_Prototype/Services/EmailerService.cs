﻿using System.Net.Mail;
using System.Net;
using WazaWare.co.za.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using WazaWare.co.za.DAL;
using Moq;

namespace wazaware.co.za.Services
{
	public class EmailerService
	{
		//private readonly Orders _orders;
		//private readonly UserModel _user;
		//public EmailerService(Orders order, UserModel user) 
		//{
		//	_orders = order;
		//	_user = user;
		//}
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IRazorViewEngine _viewEngine;
		ITempDataProvider _tempData;

		public EmailerService(IHttpContextAccessor httpContextAccessor, IRazorViewEngine viewEngine, ITempDataProvider tempData)
		{
			_httpContextAccessor = httpContextAccessor;
			_viewEngine = viewEngine;
			_tempData = tempData;
		}
		public async Task SendEmail(ViewModels viewModel) 
		{
			var serviceProvider = new ServiceCollection()
	   .AddScoped<IUrlHelper>(x => Mock.Of<IUrlHelper>())
	   .BuildServiceProvider();

			//var razorViewEngine = serviceProvider.GetRequiredService<RazorViewEngine>();
			//var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();
			SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
			SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
			MailMessage mailMessage = new MailMessage();
			// START
			mailMessage.From = new MailAddress("brandenconnected@gmail.com");
			mailMessage.To.Add(viewModel.userView!.Email!);
			mailMessage.CC.Add("brandenconnected@gmail.com");
			mailMessage.Subject = "Your Order - wazaware.co.za";

			string htmlContent = await RenderViewToStringAsync(_viewEngine, _tempData, "OrderPending", viewModel);           
			//string htmlContent = File.ReadAllText("wwwroot/email/OrderPending.cshtml");
			mailMessage.Body = htmlContent;
			mailMessage.IsBodyHtml = true;
			//END

			SmtpServer.Timeout = 5000;
			SmtpServer.EnableSsl = true;
			SmtpServer.UseDefaultCredentials = false;
			SmtpServer.Credentials = new NetworkCredential("brandenconnected@gmail.com", "mueadqbombixceuk");

			try
			{
				await SmtpServer.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
        private async Task<string> RenderViewToStringAsync(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, string viewName, object model)
		{
			var actionContext = new ActionContext(_httpContextAccessor.HttpContext!, new RouteData(), new ActionDescriptor());

			using (var sw = new StringWriter())
			{
				var viewResult = razorViewEngine.FindView(actionContext, viewName, false);

				if (viewResult.View == null)
				{
					throw new ArgumentNullException($"{viewName} does not match any available view");
				}

				var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				};

				var viewContext = new ViewContext(
					actionContext,
					viewResult.View,
					viewDictionary,
					new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
					sw,
					new HtmlHelperOptions()
				);

				await viewResult.View.RenderAsync(viewContext);

				return sw.ToString();
			}
		}
    }
}
