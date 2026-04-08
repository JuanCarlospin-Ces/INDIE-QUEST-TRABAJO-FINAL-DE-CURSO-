using IQ_Api.Domain.Repository;
using IQ_Api.Infrastructure.Repository.InMemory;
using IQ_Api.Application.Queries.GetAllUsers;
using IQ_Api.Application.Queries.GetUserById;
using IQ_Api.Application.Queries.GetAllPosts;
using IQ_Api.Application.Queries.GetPostById;
using IQ_Api.Application.Queries.GetPostsByUserId;
using IQ_Api.Application.Command.Users;
using IQ_Api.Application.Command.Posts;

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


// Register repositories
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();

// Register Query Handlers for Users
builder.Services.AddSingleton<GetAllUsersQueryHandler>();
builder.Services.AddSingleton<GetUserByIdQueryHandler>();

// Register Query Handlers for Posts
builder.Services.AddSingleton<GetAllPostsQueryHandler>();
builder.Services.AddSingleton<GetPostByIdQueryHandler>();
builder.Services.AddSingleton<GetPostsByUserIdQueryHandler>();

// Register Command Handlers for Users
builder.Services.AddSingleton<CreateUserCommandHandler>();
builder.Services.AddSingleton<UpdateUserCommandHandler>();
builder.Services.AddSingleton<DeleteUserCommandHandler>();

// Register Command Handlers for Posts
builder.Services.AddSingleton<CreatePostCommandHandler>();
builder.Services.AddSingleton<UpdatePostCommandHandler>();
builder.Services.AddSingleton<DeletePostCommandHandler>();

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
