using System.Net;
using System.Xml.Serialization;
using NotificationsApi.Contracts;
using NotificationsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<NotificationStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/notifications",
    (SendNotificationRequest request, NotificationStore store) =>
    {
        if(string.IsNullOrWhiteSpace(request.Channel) ||
           string.IsNullOrWhiteSpace(request.Recipient) ||
           string.IsNullOrWhiteSpace(request.Subject) ||
           string.IsNullOrWhiteSpace(request.Message) ||
           string.IsNullOrWhiteSpace(request.ReferenceId))
            return Results.BadRequest();
        if (request.Channel == "outage") return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);;
        if (request.Channel is not ("email" or "sms" or "teams"))
            return Results.UnprocessableEntity();
        var notification = new NotificationResponse
        {
            Id = Guid.NewGuid(),
            Status = NotificationStatus.Queued,
            ReferenceId = request.ReferenceId,
            Sent = null
        };
        store.Add(notification);
        return Results.Accepted($"/api/notifications/{notification.Id}", new
        {
            id = notification.Id,
            status = notification.Status
        });
    }).WithName("AddNotification");

app.MapGet("/api/notifications/{id:guid}",
    (Guid id, NotificationStore store) =>
    {
        var notification = store.Get(id);

        if (notification is null)
        {
            return Results.NotFound();
        }

        if (notification.Status == NotificationStatus.Queued)
        {
            notification.Status = NotificationStatus.Sent;
            notification.Sent = DateTime.UtcNow;
        }
        return Results.Ok(notification);
    }).WithName("SendNotification");

app.MapGet("/api/notifications",
    (string referenceId, NotificationStore store) =>
    {
        var notification = store.GetByReferenceId(referenceId);
        return Results.Ok(notification);
    }).WithName("GetByReferenceId");


app.Run();


    