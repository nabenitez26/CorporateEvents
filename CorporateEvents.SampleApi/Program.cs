using CorporateEvents.Abstractions.Configuration;
using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Core.Bus;
using CorporateEvents.Core.Subscriptions;
using CorporateEvents.Persistence.SqlServer.DeadLetter;
using CorporateEvents.Persistence.SqlServer.Idempotency;
using CorporateEvents.Persistence.SqlServer.Metrics;
using CorporateEvents.SampleApi.Events;
using CorporateEvents.SampleApi.Handlers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EventBusOptions>(
    builder.Configuration.GetSection("EventBus"));

builder.Services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddScoped<IDeadLetterQueueService, DeadLetterQueueService>();
builder.Services.AddScoped<IIdempotencyService, SqlIdempotencyService>();
builder.Services.AddScoped<IMetricsService, SqlMetricsService>();

builder.Services.AddScoped<SendEmailHandler>();
builder.Services.AddScoped<SendsSmslHandler>();
builder.Services.AddScoped<AuditHandler>();

var app = builder.Build();

// Registrar las relaciones Evento -> Handler
using (var scope = app.Services.CreateScope())
{
    var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    bus.Subscribe<PolicyIssuedEvent, SendEmailHandler>();
    bus.Subscribe<PolicyIssuedEvent, SendsSmslHandler>();
    bus.Subscribe<PolicyIssuedEvent, AuditHandler>();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
