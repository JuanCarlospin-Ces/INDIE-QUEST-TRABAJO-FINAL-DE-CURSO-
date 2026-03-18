using IQ_Api.Domain.Model;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


var app = builder.Build();

app.MapGet("/", () => "Inserta un endpoint válido");

app.Run();
