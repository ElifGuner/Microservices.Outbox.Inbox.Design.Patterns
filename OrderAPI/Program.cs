using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Context;
using OrderAPI.Models;
using OrderAPI.ViewModels;
using OrderAPI.Enums;
using Shared.OrderEvents;
using Shared.Messages;
using MassTransit.Transports;
using Shared.Settings;
using OrderAPI.Models.Entities;
using System.Text.Json;
//using OrderAPI.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) => 
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});


builder.Services.AddDbContext<OrderDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/create-order",async (CreateOrderVM model, OrderDBContext orderDBContext, ISendEndpointProvider sendEndpointProvider) =>
{
    Order order = new()
    {
        BuyerId = model.BuyerId,
       // OrderStatus = OrderStatus.Suspend,
        CreatedDate = DateTime.UtcNow,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(oi => new OrderItem
        {
            ProductId = oi.ProductId,
            Count = oi.Count,
            Price = oi.Price
        }).ToList(),
    };
    
    await orderDBContext.Orders.AddAsync(order);
    await orderDBContext.SaveChangesAsync();

    Guid idempotentToken = Guid.NewGuid();
    OrderCreatedEvent orderCreatedEvent = new()
    {
        OrderId = order.Id,
        BuyerId = order.BuyerId,
        TotalPrice = order.TotalPrice,
        OrderItems = model.OrderItems.Select(oi => new OrderItemMessage
        {
            ProductId = oi.ProductId,
            Count = oi.Count,
            Price = oi.Price,         
        }).ToList(),
        IdempotentToken = idempotentToken
    };

    OrderOutbox orderOutbox = new()
    {
        OccurredOn = DateTime.UtcNow,
        ProcessedDate = null,
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        Type = nameof(OrderCreatedEvent),
        IdempotentToken = idempotentToken
    };

    await orderDBContext.OrderOutboxes.AddAsync(orderOutbox);
    await orderDBContext.SaveChangesAsync();

    //var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEventQueue}"));
    //await sendEndpoint.Send<OrderCreatedEvent>(orderCreatedEvent);

});

app.Run();
