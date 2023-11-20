using Delivery.Client;
using Delivery.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Refit;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string connections = builder.Configuration.GetConnectionString("DeliveryContext") ?? throw new InvalidOperationException("Нет строки подключения к базе данных.");
builder.Services.AddDbContext<DeliveryContext>(x => x.UseNpgsql(connections));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Delivery API",
        Description = "Микросервис доставки, один из трёх для проекта \"Ларёк\"",
        Contact = new OpenApiContact
        {
            Name = "Влад",
            Url = new Uri("https://google.com")
        },
        License = new OpenApiLicense
        {
            Name = "Хвост и Усы, вот моя лицензия",
            Url = new Uri("https://google.com")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services
    .AddRefitClient<IOrderApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://192.168.0.2:9001"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, sstErrors) => true });

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
