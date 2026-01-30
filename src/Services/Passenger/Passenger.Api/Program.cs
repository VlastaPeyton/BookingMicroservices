using BuildingBlocks.EfCore;
using BuildingBlocks.EventStore;
using BuildingBlocks.Exceptions.Middleware;
using BuildingBlocks.Jwt;
using BuildingBlocks.MassTransit;
using BuildingBlocks.MediatorBehaviours.Logging;
using BuildingBlocks.MediatorBehaviours.Validation;
using BuildingBlocks.MinimalApis;
using Carter;
using FluentValidation;
using Passenger.Api.Data;
using Passenger.Api.Events.DomainEvents;
using Passenger.Api.GrpcServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddJsonEnumConverter();
builder.Services.AddApplicationDbContext<PassengerDbContext>(builder.Configuration.GetConnectionString("PassengerDatabase")!);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddMongo(builder.Configuration);
builder.Services.AddDomainEventHandlersFromAssembly<PassengerCreatedDomainEvent>();
builder.Services.AddCustomMassTransit<PassengerDbContext>(builder.Configuration, typeof(Program).Assembly);
builder.Services.AddPassengerGrpcServer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();             // Endpoints tek nakom middlewares definisem
app.MapPassengerGrpcServer();

app.Run();
