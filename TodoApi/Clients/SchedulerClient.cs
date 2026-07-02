using SchedulerApi.Contracts;
using TodoApi.Clients.Interfaces;

namespace TodoApi.Clients;

public class SchedulerClient(HttpClient http) : ISchedulerClient
{
    private readonly HttpClient _http = http;

    public async Task<HolidayCheckResponse?> IsHolidayAsync(
        DateTime date,
        CancellationToken ct)
    {
        return await _http.GetFromJsonAsync<HolidayCheckResponse>(
            $"/api/workingdays/is-holiday?date={date:O}",
            ct);
    }

    public async Task<NextWorkingDayResponse?> NextWorkingDayAsync(
        DateTime from,
        int businessDays,
        CancellationToken ct)
    {
        return await _http.GetFromJsonAsync<NextWorkingDayResponse>(
            $"/api/workingdays/next?from={from:O}&businessDays={businessDays}",
            ct);
    }
}