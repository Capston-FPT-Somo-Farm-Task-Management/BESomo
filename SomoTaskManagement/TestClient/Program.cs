using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using TestClient.Repos;

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

builder.Services.AddSingleton<UserRepo>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddSingleton<NotifyHub>();
//builder.Services.AddSingleton<SubscribeNotificationTableDependency>();
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
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<NotifyHub>("/notifyHub");
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Task}/{action=Index}/{id?}");
//});

//app.UseSqlTableDependency<SubscribeNotificationTableDependency>(connectionString);
app.Run();
