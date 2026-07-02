namespace SchedulerApi.Contracts;

public class HolidayCheckResponse
{
    public DateTime Date { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsHoliday { get; set; }
    public string? Name { get; set; }               
}