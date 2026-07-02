namespace NotificationsApi.Contracts;

public class SendNotificationRequest
{
    public string Channel { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string ReferenceId { get; set; } = string.Empty;
}