using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Clients;
using TodoApi.Clients.Interfaces;
using TodoApi.Data;
using TodoApi.Middleware;
using TodoApi.Providers;
using TodoApi.Providers.Interfaces;
using TodoApi.Repositories;
using TodoApi.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(conn));

// Repositories
builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Providers
builder.Services.AddScoped<ITodoListProvider, TodoListProvider>();
builder.Services.AddScoped<ITodoItemProvider, TodoItemProvider>();
builder.Services.AddScoped<ITagProvider, TagProvider>();

// Scheduler API client
builder.Services.AddHttpClient<ISchedulerClient, SchedulerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:SchedulerApi:BaseUrl"]!);

    client.Timeout = TimeSpan.FromSeconds(5);
});

// Notification API client
builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:NotificationApi:BaseUrl"]!);

    client.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();