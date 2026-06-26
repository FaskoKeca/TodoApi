using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Data;
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



var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();




app.Run();
