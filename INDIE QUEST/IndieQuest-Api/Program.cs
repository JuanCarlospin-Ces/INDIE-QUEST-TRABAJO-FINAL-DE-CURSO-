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
using Microsoft.Extensions.FileProviders;

// Npgsql 6+ requiere DateTimeKind.Utc para timestamptz.
// Esta opción permite enviar fechas con Kind=Unspecified como si fueran UTC (comportamiento heredado).
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for large file uploads
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 524288000; // 500MB
});

// Swagger implementation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configure FormOptions to allow larger file uploads (500MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = 524288000; // 500MB
    options.MultipartBodyLengthLimit = 524288000; // 500MB
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Allow all origins for development
        // In production, restrict to specific origins:
        // policy.WithOrigins(
        //     "https://indiequest.vercel.app",
        //     "https://indiequest-api-xxxx.loca.lt"
        // )
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

// Serve static files from the parent directory (IndieQuest-LocalData)
var staticFileOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "IndieQuest-LocalData")),
    RequestPath = "/IndieQuest-LocalData"
};
app.UseStaticFiles(staticFileOptions);

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Inserta un endpoint válido");

app.Run();
