using Amazon;
using ItemBase.API;
using ItemBase.API.Settings;
using ItemBase.Core;
using ItemBase.Core.Settings;
using ItemBase.Core.Services;
using Microsoft.Extensions.Configuration;
using ItemBase.Core.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//var items = JsonFile<List<Item>>.Load("Resourses/items_cn.json");




builder.Services.AddItemApi(builder.Configuration);

builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
