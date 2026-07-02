namespace TodoApi.Clients.Contracts;

public class NextWorkingDayResponse
{
    public DateTime From { get; init; }
    public int BusinessDays { get; init; }
    public DateTime Result { get; init; }
}