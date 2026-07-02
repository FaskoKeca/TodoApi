using System.Collections.Concurrent;
using NotificationsApi.Contracts;

namespace NotificationsApi.Services;

public class NotificationStore(ConcurrentDictionary<Guid, NotificationResponse?> storage)
{
    ConcurrentDictionary<Guid, NotificationResponse?> Storage { get; } = storage;

    public void Add(NotificationResponse notification)
    {
        Storage.TryAdd(notification.Id, notification);
    }

    public NotificationResponse? Get(Guid id)
    {
        return Storage.TryGetValue(id, out NotificationResponse? notification) ? notification : new NotificationResponse();
    }

    public IEnumerable<NotificationResponse?> GetByReferenceId(string referenceId)
    {
        return Storage.Where(x => x.Value != null && x.Value.ReferenceId == referenceId).Select(x => x.Value);
    }
}