using Amazon;
using ItemBase.Presentation.Settings;
using ItemBase.Presentation;
using ItemBase.Presentation.Middleware;
using System.Text.Json;
using ItemBase.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddItemBaseCore(builder.Configuration);

builder.Services.AddItemBasePresentation(builder.Configuration);

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

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
