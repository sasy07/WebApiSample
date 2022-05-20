using System.Net;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using WebApiSample.Data;
using WebApiSample.Data.Contracts;
using WebApiSample.Data.Repositories;
using WebApiSample.WebFramework.Middlewares;


var logger = NLog.LogManager.Setup()
.LoadConfigurationFromAppSettings()
.GetCurrentClassLogger();

logger.Debug("init main");


try
{
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SomeeConnection"));
    });
    builder.Services.AddControllers();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();



    #region elmah

//
// builder.Services.AddElmah<SqlErrorLog>(options =>
// {
//     options.Path = "/elmah-errors";
//     options.ConnectionString = builder.Configuration.GetConnectionString("SomeeElmahConnection");
// });

    #endregion

    #region app-services

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    #endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseCustomExceptionHandler();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

// app.UseElmah();
    WebApplication.CreateBuilder().WebHost
        .ConfigureLogging(options => options.ClearProviders());
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}

finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}