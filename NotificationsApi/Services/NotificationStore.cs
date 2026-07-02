using System.Collections.Concurrent;
using NotificationsApi.Contracts;

namespace NotificationsApi.Services;

public class NotificationStore
{
    private readonly ConcurrentDictionary<Guid, NotificationResponse?> _storage = new();

    public void Add(NotificationResponse notification)
    {
        _storage.TryAdd(notification.Id, notification);
    }

    public NotificationResponse? Get(Guid id)
    {
        return _storage.GetValueOrDefault(id);
    }

    public IEnumerable<NotificationResponse?> GetByReferenceId(string referenceId)
    {
        return _storage.Values.Where(n =>
            n != null && n.ReferenceId == referenceId);
    }
}