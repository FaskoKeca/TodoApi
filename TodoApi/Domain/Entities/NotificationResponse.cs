using TodoApi.Clients.Contracts;

namespace TodoApi.Domain.Entities;

public abstract class NotificationResponse
{

    public Guid Id { get; set; }

    public NotificationStatus Status { get; set; }

    public DateTime? SentUtc { get; set; }

    public string ReferenceId { get; set; } = string.Empty;

}