using System.Data;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETCoreVSCodeDemo.Controllers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<DBConnect>(options=> options.UseSqlServer(builder.Configuration.GetConnectionString("OrclConnection")));
builder.Services.AddDbContext<DBConnect>(OptionsBuilder => OptionsBuilder.UseOracle(builder.Configuration.GetConnectionString("OrclConnection")));

var logFilePath = builder.Configuration["Logging:FileLogging:LogFilePath"];
var logFileDirectory = builder.Configuration["Logging:FileLogging:logFileDirectory"];
if (!Directory.Exists(logFileDirectory)) Directory.CreateDirectory(logFileDirectory);
builder.Logging.AddProvider(new FileLoggerProvider(logFileDirectory + "\\" + logFilePath));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
