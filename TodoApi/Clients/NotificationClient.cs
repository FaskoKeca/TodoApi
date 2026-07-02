using TodoApi.Clients.Interfaces;
using TodoApi.Domain.Entities;

namespace TodoApi.Clients;

public class NotificationClient(HttpClient http) : INotificationClient
{
    private readonly HttpClient _http = http;

    public async Task<NotificationResponse?> SendAsync(
        SendNotificationRequest request,
        CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync(
            "/api/notifications",
            request,
            ct);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<NotificationResponse>(ct);
    }
}