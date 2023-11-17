using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Orders.Client;
using Orders.Context;
using Refit;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string connections = builder.Configuration.GetConnectionString("OrdersContext") ?? throw new InvalidOperationException("Нет строки подключения к базе данных.");
builder.Services.AddDbContext<OrdersContext>(x => x.UseNpgsql(connections));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Orders API",
        Description = "Микросервис заказов, один из трёх для проекта \"Ларёк\"",
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

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddRefitClient<ICatalogApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://192.168.0.2:9003"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, sstErrors) => true });

builder.Services
    .AddRefitClient<IDeliveryApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://192.168.0.2:9005"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, sstErrors) => true });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
