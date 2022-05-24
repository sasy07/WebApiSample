using System.Net;
using Common;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using WebApiSample.Data;
using WebApiSample.Data.Contracts;
using WebApiSample.Data.Repositories;
using WebApiSample.Services.Services;
using WebApiSample.WebFramework.Configuration;
using WebApiSample.WebFramework.Middlewares;


var logger = NLog.LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

logger.Debug("init main");


try
{
    var builder = WebApplication.CreateBuilder(args);
    var _siteSetting = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
// Add services to the container.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
    });
    builder.Services.AddControllers(options => options.Filters.Add(new AuthorizeFilter()));

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();


    #region elmah
    //
    // builder.Services.AddElmah<SqlErrorLog>(options =>
    // {
    //     options.Path = _siteSetting.ElmahPath;
    //     options.ConnectionString = builder.Configuration.GetConnectionString("SomeeElmahConnection");
    // });

    #endregion

    #region app-services

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddJwtAuthentication(_siteSetting.JwtSettings);

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

    app.UseAuthentication();

// app.UseElmah();
    WebApplication.CreateBuilder().WebHost
        .ConfigureLogging(options => options.ClearProviders());
    app.UseHttpsRedirection();

    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
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