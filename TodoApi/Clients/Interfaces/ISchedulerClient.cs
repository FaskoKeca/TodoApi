using SchedulerApi.Contracts;

namespace TodoApi.Clients.Interfaces;

public interface ISchedulerClient
{
    Task<HolidayCheckResponse?> IsHolidayAsync(DateTime date, CancellationToken ct);
    Task<NextWorkingDayResponse?> NextWorkingDayAsync(DateTime from, int businessDays, CancellationToken ct);
}