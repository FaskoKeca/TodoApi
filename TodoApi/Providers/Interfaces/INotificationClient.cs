using TodoApi.Domain.Entities;

namespace TodoApi.Providers;

public interface INotificationClient
{
    Task<NotificationResponse?> SendAsync(SendNotificationRequest req, CancellationToken ct);
}