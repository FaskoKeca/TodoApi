using TodoApi.Domain.Entities;

namespace TodoApi.Clients.Interfaces;

public interface INotificationClient
{
    Task<NotificationResponse?> SendAsync(
        SendNotificationRequest request,
        CancellationToken ct);
}