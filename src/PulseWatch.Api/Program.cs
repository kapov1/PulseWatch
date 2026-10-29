using PulseWatch.Application.Interfaces;
using PulseWatch.Infrastructure.Configuration;
using PulseWatch.Infrastructure.MonitoringInfrastructure;
using Microsoft.Extensions.Options;
using PulseWatch.Application;
using PulseWatch.Infrastructure.DbInfrastructure;
using Microsoft.EntityFrameworkCore;
using PulseWatch.Application.Services;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

#region DI container
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddProblemDetails();

    builder.Services.AddControllers();

    builder.Services.Configure<MonitoringOptions>(
        builder.Configuration.GetSection("Monitoring")
    );

    builder.Services.AddHttpClient<IHttpSiteChecker, HttpSiteChecker>(
    (serviceProvicer, httpClient) =>
        {
            var monitoringOptions = serviceProvicer.GetRequiredService<IOptions<MonitoringOptions>>().Value;
            httpClient.Timeout = TimeSpan.FromSeconds(monitoringOptions.RequestTimeoutSeconds);

            System.Console.WriteLine();
            System.Console.WriteLine("http timeout: " + httpClient.Timeout);
            System.Console.WriteLine("optoins timeout: " + monitoringOptions.RequestTimeoutSeconds);
            System.Console.WriteLine();
        });

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connection);
    });    

    builder.Services.AddScoped<IMonitoredSiteRepository, MonitoredSiteRepository>();     
    builder.Services.AddScoped<MonitoredSiteService>();
    builder.Services.AddScoped<SiteMonitoringService>();
#endregion

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
