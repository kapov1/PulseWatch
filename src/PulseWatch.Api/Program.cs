using PulseWatch.Application.Interfaces;
using PulseWatch.Infrastructure.Configuration;
using PulseWatch.Infrastructure.MonitoringInfrastructure;
using Microsoft.Extensions.Options;
using PulseWatch.Application;
using PulseWatch.Infrastructure.DbInfrastructure;

var builder = WebApplication.CreateBuilder(args);

#region DI contaiiner
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddControllers();

    builder.Services.Configure<MonitoringOptions>(
        builder.Configuration.GetSection("Monitorig")
    );

    builder.Services.AddHttpClient<IHttpSiteChecker, HttpSiteChecker>(
    (serviceProvicer, httpClient) =>
        {
            var monitoringOptions = serviceProvicer.GetRequiredService<IOptions<MonitoringOptions>>().Value;
            httpClient.Timeout = TimeSpan.FromSeconds(monitoringOptions.RequestTimeoutSeconds);
        });

    builder.Services.AddScoped<IMonitoredSiteRepository, MonitoredSiteRepository>();     
#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
