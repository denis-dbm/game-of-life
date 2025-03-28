using GameOfLife;
using GameOfLife.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDependencies();

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

app.MapBoardEndpoints();

await app.RunAsync();