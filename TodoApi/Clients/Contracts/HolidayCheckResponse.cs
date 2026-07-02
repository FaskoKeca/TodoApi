namespace TodoApi.Clients.Contracts;

public class HolidayCheckResponse
{
    public DateTime Date { get; init; }
    public bool IsWeekend { get; init; }
    public bool IsHoliday { get; init; }
    public string? Name { get; init; }               
}