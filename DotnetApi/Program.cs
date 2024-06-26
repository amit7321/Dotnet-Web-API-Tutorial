using DotnetApi.Data;
using DotnetApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* builder.Services.AddDbContext<>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); */

builder.Services.AddScoped<DataContextDapper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });
    options.AddPolicy("ProdCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://myProductionSite.com")
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.MapControllers();
app.Run();