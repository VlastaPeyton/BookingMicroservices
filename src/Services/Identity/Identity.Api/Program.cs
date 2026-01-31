using BuildingBlocks.EfCore;
using BuildingBlocks.Exceptions.Middleware;
using BuildingBlocks.MediatorBehaviours.Logging;
using BuildingBlocks.MediatorBehaviours.Validation;
using BuildingBlocks.MinimalApis;
using Carter;
using Identity.Api.Configurations;
using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{   // IdentityRole je AspNetRoles tabela. AddIdentity ce napraviti i povezati SVE Identity tabele iz IdentityDbContext, a ne samo ove 2.
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    options.User.RequireUniqueEmail = true;         // Da ne mogu dva usera da imaju isti Email prilikom njihovog dodavanja u AspNetUsers tabelu u Register endpoint

}).AddEntityFrameworkStores<IdentityContextDb>() // AddEntityFrameworkStores tells Identity to use EF Core to store Identity data (users, roles, tokens, etc.) in the database using ApplicationDbContext.
  .AddDefaultTokenProviders();                   // Is needed for email confirmation in Register or password reset in ForgotPassword endpoint


builder.Services.AddIdentityServer()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<User>()
            .AddResourceOwnerValidator<UserValidator>();

builder.Services.AddCarter();
builder.Services.AddJsonEnumConverter();
builder.Services.AddDbContext<IdentityContextDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDatabase")!));

builder.Services.AddScoped<IApplicationDbContext>(sp =>sp.GetRequiredService<IdentityContextDb>());

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseGlobalExceptionHandler();
app.MapCarter();
app.UseIdentityServer(); // Doda sve autoamtske endpointe iz kojih dohvata signing key

app.Run();