using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Repositories;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Missing SupabaseDb connection string");
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new Exception("Missing Jwt:Secret");

builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(connString));
builder.Services.AddScoped<IUserService>(sp => new UserService(
    sp.GetRequiredService<IUserRepository>(), jwtSecret));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 🔥 thêm swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/register", async (
    [FromServices] IUserService auth,
    [FromBody] RegisterRequest req) =>
{
    await auth.RegisterAsync(req.Username, req.Password);
    return Results.Ok("User registered");
});

app.MapPost("/login", async (
    [FromServices] IUserService auth,
    [FromBody] LoginRequest req) =>
{
    var token = await auth.LoginAsync(req.Username, req.Password);
    if (token == null) return Results.BadRequest("Invalid login");
    return Results.Ok(new { token });
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // 🔥 bật swagger và swagger ui
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

record RegisterRequest(string Username, string Password);
record LoginRequest(string Username, string Password);
