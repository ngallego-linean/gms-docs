using GMS.Business.Services;
using GMS.Data.Repositories;
using Ctc.GMS.Web.UI.Configuration;
using Ctc.GMS.Web.UI.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
Env.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register R4 application services
builder.Services.AddSingleton<MockRepository>();
builder.Services.AddScoped<IGrantService, GrantService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IIHETemplateService, IHETemplateService>();

// Register DocuSign services
builder.Services.Configure<DocuSignConfig>(options =>
{
    options.IntegrationKey = Environment.GetEnvironmentVariable("INTEGRATION_KEY") ?? "";
    options.UserId = Environment.GetEnvironmentVariable("USER_ID") ?? "";
    options.AccountId = Environment.GetEnvironmentVariable("API_ACCOUNT_ID") ?? "";
    options.PrivateKey = Environment.GetEnvironmentVariable("RSA_PRIVATE_KEY") ?? "";
    options.BasePath = Environment.GetEnvironmentVariable("ACCOUNT_BASE_URI") ?? "https://demo.docusign.net";
    options.BasePath = options.BasePath.TrimEnd('/') + "/restapi";
});
builder.Services.AddScoped<IDocuSignService, DocuSignService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
