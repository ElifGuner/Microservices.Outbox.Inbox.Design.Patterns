using MassTransit;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
using Shared.Messages;
using Shared.OrderEvents;
using Shared.Settings;
using StockAPI.Models;
using StockAPI.Models.Entities;
//using Shared.StockEvents;
//using StockAPI.Context;
using System;
using System.Text.Json;

namespace StockAPI.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        public StockDBContext _stockDBContext { get; set; }
        public OrderCreatedEventConsumer(StockDBContext stockDBContext)
        {
            _stockDBContext = stockDBContext;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var result = await _stockDBContext.OrderInboxes.AnyAsync(oi => oi.IdempotentToken == context.Message.IdempotentToken);

            if (!result)
            { 
                OrderInbox orderInbox = new()
                {
                    Processed = false,
                    Payload = JsonSerializer.Serialize(context.Message),
                    IdempotentToken = context.Message.IdempotentToken
                };

                await _stockDBContext.OrderInboxes.AddAsync(orderInbox);
                await _stockDBContext.SaveChangesAsync();
            }

            List<OrderInbox> orderInboxes = await _stockDBContext.OrderInboxes.Where(oi => oi.Processed == false).ToListAsync();
            foreach (var orderInbx in orderInboxes) 
            {               
                OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbx.Payload);
                await Console.Out.WriteLineAsync($"{orderCreatedEvent.OrderId} order ID değerine sahip siparişin stok işlemleri başarıyla tamamlanmıştır.");
                orderInbx.Processed = true;
                await _stockDBContext.SaveChangesAsync();
            }
            //await Console.Out.WriteLineAsync(JsonSerializer.Serialize(context.Message));
        }
    }
}
