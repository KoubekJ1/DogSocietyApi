using Microsoft.EntityFrameworkCore;
using DogSocietyApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("localhost");
builder.Services.AddDbContext<DogSocietyDbContext>(opt =>
    opt.UseNpgsql(connectionString));

var app = builder.Build();

/*using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<DogSocietyDbContext>();
await dbContext.Database.MigrateAsync();*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
