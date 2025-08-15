using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TestTaskAPI.Data;
using TestTaskAPI.Interface;
using TestTaskAPI.Interface.InterfaceRepository;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Repository;
using TestTaskAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// SQL
builder.Services.AddDbContext<TesttaskdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging().EnableDetailedErrors());

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBalanceRepository, BalanceRepository>();
builder.Services.AddScoped<IMeasureRepository, MeasureRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IReceiptResourceRepository, ReceiptResourceRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IShippingResourceRepository, ShippingResourceRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();

// Services
builder.Services.AddScoped<IClientServices, ClientServices>();
builder.Services.AddScoped<IBalanceServices, BalanceServices>();
builder.Services.AddScoped<IMeasureServices, MeasureServices>();
builder.Services.AddScoped<IReceiptServices, ReceiptServices>();
builder.Services.AddScoped<IReceiptResourcesServices, ReceiptResourcesServices>();
builder.Services.AddScoped<IResourceServices, ResourceServices>();
builder.Services.AddScoped<IShippingServices, ShippingServices>();
builder.Services.AddScoped<IShippingResourcesServices, ShippingResourcesServices>();
builder.Services.AddScoped<IStatusServices, StatusServices>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7037") // Blazor WASM
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
