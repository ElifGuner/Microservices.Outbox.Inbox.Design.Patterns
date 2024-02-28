using MassTransit;
using OrderAPI.Context;
using OrderOutboxTablePublisherService;
using OrderOutboxTablePublisherService.Jobs;
using Quartz;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();



var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddQuartz(configurator =>
{
    JobKey jobKey = new JobKey("OrderOutboxPublishJob");

    configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobKey));

    TriggerKey triggerKey = new("OrderOutboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobKey).WithIdentity(triggerKey)
    .StartAt(DateTime.UtcNow)  //triggerýn ne zaman tetkileneceðini ifade eder.
    .WithSimpleSchedule(builder => builder
        .WithIntervalInSeconds(5) // 5 saniyede 1 çalýþacak
        .RepeatForever()));            //sonsuza kadar çalýþacak
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});


var host = builder.Build();

host.Run();