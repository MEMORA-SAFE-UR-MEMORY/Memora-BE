using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Repositories;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<PostgresContext>(options =>
{
	options.UseNpgsql(dataSource)
		   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Settings
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));
// No need to configure RevenueCatSettings here as we are injecting IConfiguration directly.

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IUserThemeRepository, UserThemeRepository>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();


builder.Services.AddControllers(options =>
{
}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/api/webhooks/revenuecat"))
	{
		context.Request.EnableBuffering();
	}
	await next();
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();