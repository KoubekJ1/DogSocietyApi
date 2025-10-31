using Microsoft.EntityFrameworkCore;
using DogSocietyApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DogSociety API",
        Description = "An ASP.NET Core Web API for managing DogSociety."
        + "\nAll endpoints except the ones from the User controller require the client to provide an authentication cookie!"
        + "\nIn the Swagger UI, running the /User/login endpoint with correct credentials is enough to provide authentication."
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddServer(new OpenApiServer
    {
        Url = "http://localhost:5295"
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("localhost");
builder.Services.AddDbContext<DogSocietyDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// Source: https://www.reddit.com/r/dotnet/comments/i5gitt/question_how_can_i_return_a_401_unauthorized/
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(cookiePolicyOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
