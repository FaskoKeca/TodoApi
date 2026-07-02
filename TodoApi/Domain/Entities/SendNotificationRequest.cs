namespace TodoApi.Domain.Entities;

public class SendNotificationRequest()
{
    public string Channel { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int ReferenceId { get; set; }
}