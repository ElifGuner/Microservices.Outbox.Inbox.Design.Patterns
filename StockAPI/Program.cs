using MassTransit;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
using Shared.Settings;
using StockAPI.Consumers;
using StockAPI.Models;
//using StockAPI.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();

    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e =>
        e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<StockDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

var app = builder.Build();

app.Run();
