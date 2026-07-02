using SchedulerApi.Contracts;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var holidays = builder.Configuration
    .GetSection("Holidays")
    .Get<List<HolidayDto>>() ?? new();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/api/holidays", (int year) =>
{
    var result = holidays
        .Where(h => h.Date.Year == year)
        .ToList();

    return Results.Ok(result);
}).WithName("GetHolidays");

app.MapGet("/api/is-holiday", (DateTime date) =>
{
    var isWeekend =
        date.DayOfWeek == DayOfWeek.Saturday ||
        date.DayOfWeek == DayOfWeek.Sunday;

    var holiday = holidays.FirstOrDefault(h => h.Date.Date == date.Date);
    
    return Results.Ok(new HolidayCheckResponse
    {
        Date = date,
        IsWeekend = isWeekend,
        IsHoliday = holiday != null,
        Name = holiday?.Name
    });
}).WithName("IsHoliday");

app.MapGet("/api/working-days/next", (DateTime from, int businessDays) =>
{
    var current = from;
    var remaining = businessDays;

    while (remaining > 0)
    {
        current = current.AddDays(1);

        var isWeekend =
            current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        var isHoliday = holidays.Any(h => h.Date.Date == current.Date);

        if (isWeekend || isHoliday)
            continue;

        remaining--;
    }

    return Results.Ok(new NextWorkingDayResponse
    {
        From = from,
        BusinessDays = businessDays,
        Result = current
    });
});


app.Run();


app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
