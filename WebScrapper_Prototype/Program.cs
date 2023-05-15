using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMvcCore().AddRazorViewEngine();
builder.Services.AddMvcCore().AddViews().AddCookieTempDataProvider();

// AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

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
	app.UseExceptionHandler("/Home/WebsiteCritical");
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
	pattern: "{controller=Shop}/{action=Home}/{id?}");

app.Run();