using MassTransit;
using OrderOutboxTablePublisherService.Entities;
using Quartz;
using Shared.OrderEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderOutboxTablePublisherService.Jobs
{
    public class OrderOutboxPublishJob : IJob
    {
        public IPublishEndpoint _publishEndpoint { get; set; }
        public OrderOutboxPublishJob(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderOutboxSingletonDatabase.DataReaderState)
            {
                OrderOutboxSingletonDatabase.DataReaderBusy();

                try
                {
                    List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM ORDEROUTBOXES WHERE PROCESSEDDATE IS NULL ORDER BY OCCURREDON ASC")).ToList();

                    foreach (var orderOutbox in orderOutboxes)
                    {
                        if (orderOutbox.Type == nameof(OrderCreatedEvent))
                        {
                            OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);

                            if (orderCreatedEvent != null)
                            {
                                await _publishEndpoint.Publish(orderCreatedEvent);
                                await OrderOutboxSingletonDatabase.ExecuteAsync($"update OrderOutboxes set ProcessedDate=getdate() where IdempotentToken='{orderOutbox.IdempotentToken}'");
                            }
                        }
                    }
                }
                catch (Exception exp)
                {

                    Console.WriteLine("Exception :" + exp);
                }

                OrderOutboxSingletonDatabase.DataReaderReady();
            }

            Console.WriteLine("OrderOutbox table is checked..." + DateTime.UtcNow.Second);
        }
    }
}
