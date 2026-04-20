using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using IndieQuest_Api.Application.Queries.GetAllUsers;
using IndieQuest_Api.Application.Queries.GetUserById;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Application.Command.Posts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger implementation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure PostgreSQL connection from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

builder.Services.AddDbContext<IndieQuestDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories with PostgreSQL implementations
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
builder.Services.AddScoped<IPostRepository, PostgreSqlPostRepository>();

// Register Query Handlers for Users
builder.Services.AddScoped<GetAllUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();

// Register Query Handlers for Posts
builder.Services.AddScoped<GetAllPostsQueryHandler>();
builder.Services.AddScoped<GetPostByIdQueryHandler>();
builder.Services.AddScoped<GetPostsByUserIdQueryHandler>();

// Register Command Handlers for Users
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<UpdateUserCommandHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();

// Register Command Handlers for Posts
builder.Services.AddScoped<CreatePostCommandHandler>();
builder.Services.AddScoped<UpdatePostCommandHandler>();
builder.Services.AddScoped<DeletePostCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IQ-Api v1");
        c.RoutePrefix = "swagger"; // optional: route /swagger
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Inserta un endpoint válido");

app.Run();
