namespace SchedulerApi.Contracts;

public class NextWorkingDayResponse
{
    public DateTime From { get; set; }
    public int BusinessDays { get; set; }
    public DateTime Result { get; set; }
}