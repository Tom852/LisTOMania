using LisTOMania.Common.Model.DataBase;
using LisTOMania.DataAccess.Neo4J;
using ListTOMania.Web.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Serilog;
using System.Xml.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IListDataAccess, N4JListDataAccess>();
builder.Services.AddTransient<IItemDataAccess, N4JItemDataAcess>();
builder.Services.AddTransient<IUserDataAccess, N4JUserDataAcess>();
builder.Services.AddTransient<IRechteDataAccess, RechteDataAccess>();
builder.Services.AddTransient<ITagDataAccess<N4JItem>, N4JTagDataAcess<N4JItem>>();

builder.Services.AddTransient<IListManager, ListManager>();
builder.Services.AddTransient<IItemManager, ItemManager>();
builder.Services.AddTransient<ITagManager, TagManager>();
builder.Services.AddTransient<IUserManager, UserManager>();
builder.Services.AddTransient<IRechteManager, RechteManager>();
builder.Services.AddTransient<IHealthCheck, Neo4jHealthCheck>();

builder.Services.AddTransient<RepeatableItemResetter>(); // Replace YourService with the actual name of your service class.
builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<RepeatableItemResetter>(t =>
    {
        t.WithCronSchedule("0 0 2 * * ?");
    });
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .WriteTo.File(builder.Configuration["Logfile"], rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.AddSerilog(logger);

builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => false;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Listomania"; // Change to your preferred name
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(90); // Set the cookie expiration to 3 month
        options.LoginPath = "/Login"; // Set the login page URL
        options.LogoutPath = "/Login/Logout"; // Set the login page URL
        options.AccessDeniedPath = "/Login/AccessDenied"; // Set the access denied page URL
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IGraphClient>(sp =>
{
    var client = new BoltGraphClient(new Uri(builder.Configuration["Neo4j:ConnectionString"]), builder.Configuration["Neo4j:User"], builder.Configuration["Neo4j:Password"]);
    //var client = new GraphClient(new Uri("http://localhost:7474"), "listo", "listomania");
    client.ConnectAsync().Wait();
    return client;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});

builder.Services.AddHealthChecks().AddCheck("Neo4jDatabase", new Neo4jHealthCheck(builder.Services.BuildServiceProvider().GetService<IGraphClient>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseExceptionHandler("/Error");
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();