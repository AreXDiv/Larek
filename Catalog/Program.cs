using Catalog.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string connections = builder.Configuration.GetConnectionString("CatalogContext") ?? throw new InvalidOperationException("��� ������ ����������� � ���� ������.");
builder.Services.AddDbContext<CatalogContext>(x => x.UseNpgsql(connections));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Catalog API",
        Description = "����������� �������, ���� �� ��� ��� ������� \"����\"",
        Contact = new OpenApiContact
        {
            Name = "����",
            Url = new Uri("https://google.com")
        },
        License = new OpenApiLicense
        {
            Name = "����� � ���, ��� ��� ��������",
            Url = new Uri("https://google.com")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHttpContextAccessor();

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
