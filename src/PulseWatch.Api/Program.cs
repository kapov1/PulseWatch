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
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

#region DI container
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddProblemDetails();

    builder.Services.AddHttpLogging();

    builder.Services.Configure<MonitoringOptions>(
        builder.Configuration.GetSection("Monitoring"));

    builder.Services.AddHttpClient<IHttpSiteChecker, HttpSiteChecker>(
        (serviceProvicer, httpClient) =>
            {
                var monitoringOptions = 
                    serviceProvicer.GetRequiredService<IOptions<MonitoringOptions>>().Value;

                httpClient.Timeout = 
                    TimeSpan.FromSeconds(monitoringOptions.RequestTimeoutSeconds);
            });

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connection);
    });

    builder.Services.AddScoped<IMonitoredSiteRepository, MonitoredSiteRepository>();
    builder.Services.AddScoped<MonitoredSiteService>();
    builder.Services.AddScoped<SiteMonitoringService>();

    builder.Services.AddControllers();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5500",
                    "http://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
#endregion

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpLogging();

app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
