using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Data;
using TodoApi.Providers;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TodoApiDb"));
builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITodoListProvider, TodoListProvider>();
builder.Services.AddScoped<ITodoItemProvider, TodoItemProvider>();
builder.Services.AddScoped<ITagProvider, TagProvider>();



var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();




app.Run();
