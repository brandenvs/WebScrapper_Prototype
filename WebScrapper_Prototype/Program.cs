using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Helpers;
using Microsoft.AspNetCore.HttpOverrides;
using wazaware.co.za.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

//var assembly = Assembly.GetExecutingAssembly();
//var currentPath = System.IO.Path.GetDirectoryName(assembly.Location);

//builder.Configuration.SetBasePath(currentPath);
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
//builder.Services.AddDbContext<wazaware.co.zaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("wazaware.co.zaContext") ?? throw new InvalidOperationException("Connection string 'wazaware.co.zaContext' not found."))//);
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.")));

//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
var connectionString = builder.Configuration.GetConnectionString("wazaware_db_context") ?? throw new InvalidOperationException("Connection string 'wazaware_db_context' not found.");
builder.Services.AddDbContext<wazaware_db_context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("wazaware_db_context") ?? throw new InvalidOperationException("Connection string 'wazaware_db_context' not found.")));

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

AddAuthorizationPolicies(builder.Services);

AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

var app = builder.Build();

var forwardedHeaderOptions = new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaderOptions.KnownNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeaderOptions);
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Shop/WebsiteCritical");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Shop}/{action=Index}/{id?}");

app.Run();

void AddAuthorizationPolicies(IServiceCollection services)
{
	services.AddAuthorization(options =>
	{
		options.AddPolicy("OwnerOnly", policy => policy.RequireClaim("OwnerId"));
	});

	services.AddAuthorization(options =>
	{
		options.AddPolicy("RequireAdmin", policy => policy.RequireClaim("Administrator"));
		options.AddPolicy("RequireManager", policy => policy.RequireClaim("Manager"));
	});
}
