using BuildingBlocks.EfCore;
using BuildingBlocks.EventStore;
using BuildingBlocks.Exceptions.Middleware;
using BuildingBlocks.Jwt;
using BuildingBlocks.MediatorBehaviours.Logging;
using BuildingBlocks.MediatorBehaviours.Validation;
using BuildingBlocks.MinimalApis;
using Carter;
using Flight.Api.Data;
using Flight.Api.Flights.DomainEventHandlers;
using Flight.Api.GrpcServer;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddJsonEnumConverter();
builder.Services.AddApplicationDbContext<FlightDbContext>(builder.Configuration.GetConnectionString("FlightDatabase")!);
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
builder.Services.AddDomainEventHandlersFromAssembly<FlightCreatedDomainEventHandler>();
builder.Services.AddFlightGrpcServer();

var app = builder.Build();

app.UseGlobalExceptionHandler();  
app.UseHttpsRedirection();       
app.UseAuthentication();          
app.UseAuthorization();          
app.MapCarter();             // Endpoints tek nakom middlewares
app.MapFlightGrpcServer();   

app.Run();
