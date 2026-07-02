using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Clients;
using TodoApi.Data;
using TodoApi.Providers;
using TodoApi.Repositories;
using TodoApi.Middleware;
using TodoApi.Providers.Interfaces;
using TodoApi.Repositories.Interfaces;
using ISchedulerClient = TodoApi.Clients.Interfaces.ISchedulerClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(conn));
builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITodoListProvider, TodoListProvider>();
builder.Services.AddScoped<ITodoItemProvider, TodoItemProvider>();
builder.Services.AddScoped<ITagProvider, TagProvider>();
builder.Services.AddHttpClient<ISchedulerClient, SchedulerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:SchedulerApi:BaseUrl"]!);

    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHttpClient<ISchedulerClient, SchedulerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:SchedulerApi:BaseUrl"]!);
});


var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();



app.Run();
