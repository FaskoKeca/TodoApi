namespace NotificationsApi.Contracts;

public class NotificationResponse
{
    public Guid Id { get; set; }

    public NotificationStatus Status { get; set; }

    public DateTime? Sent { get; set; }

    public string ReferenceId { get; set; } = string.Empty;
}