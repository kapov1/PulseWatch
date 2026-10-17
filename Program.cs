var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UrlService>();
builder.Services.AddSingleton<UrlStorage>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();   

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapControllers();


app.Run();
