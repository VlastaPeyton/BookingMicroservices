using Booking.Api.DomainEventHandlers;
using Booking.Api.GrpcClient;
using Booking.Api.ValueObjects;
using BuildingBlocks.EfCore;
using BuildingBlocks.EventStore;
using BuildingBlocks.Exceptions.Middleware;
using BuildingBlocks.Jwt;
using BuildingBlocks.MediatorBehaviours.Logging;
using BuildingBlocks.MediatorBehaviours.Validation;
using BuildingBlocks.MinimalApis;
using BuildingBlocks.UserProviders;
using Carter;
using Flight.Api.GrpcServer;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddCurrentUserProvider();
builder.Services.AddJsonEnumConverter();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);  
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddEventStore(builder.Configuration);
builder.Services.AddMongo(builder.Configuration);
builder.Services.AddEventTypeMapperAndCheckpoint();
builder.Services.AddEventStoreRepositories<Booking.Api.Models.Booking, BookingId>();
builder.Services.AddEventStoreSubscriptionToAll();
builder.Services.AddDomainEventHandlersFromAssembly<BookingCreatedDomainEventHandler>();
builder.Services.AddBookingGrpcClient();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();             // Endpoints tek nakom middlewares
app.MapFlightGrpcServer();

app.Run();