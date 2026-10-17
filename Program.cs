HttpClient httpClient = new();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<UrlService>();
builder.Services.AddScoped<MonitoringService>();
builder.Services.AddSingleton<UrlStorage>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpLogging();

app.UseSwagger();
app.UseSwaggerUI();   

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapControllers();

app.Run();
