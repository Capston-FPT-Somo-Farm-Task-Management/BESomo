using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Notify.HubSignalR;
using SomoTaskManagement.Notify.MiddlewareExtensions;
using SomoTaskManagement.Notify.SubscribeTableDependencies;
using UITest.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<SomoTaskManagemnetContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

// DI
builder.Services.AddSingleton<TaskHub>();
builder.Services.AddSingleton<UserRepo>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<SubscribeNotificationTableDependency>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapHub<TaskHub>("/taskHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=SignIn}/{id?}");

app.UseSqlTableDependency<SubscribeNotificationTableDependency>(connectionString);
app.Run();
