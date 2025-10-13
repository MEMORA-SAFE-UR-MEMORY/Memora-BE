// Memora.BackEnd/Memora.BackEnd.Api/Program.cs
using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Repositories;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Libraries;
using Memora.BackEnd.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// CORS
// ==========================================================
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
	{
		policy.WithOrigins("http://localhost:4200", "https://memora-official.com",
				"http://localhost:3000")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

// ==========================================================
// Database
// ==========================================================
var connString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<PostgresContext>(options =>
{
	options.UseNpgsql(dataSource)
		   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// ==========================================================
// Settings
// ==========================================================
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));
// THÊM DÒNG NÀY ĐỂ NẠP CẤU HÌNH PAYOS
builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOS"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>()
	?? throw new InvalidOperationException("JWT Settings are not configured.");
jwtSettings.IsValid();

var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);

// ==========================================================
// Authentication
// ==========================================================
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = builder.Environment.IsProduction();
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = true,
		ValidIssuer = jwtSettings.Issuer,
		ValidateAudience = true,
		ValidAudience = jwtSettings.Audience,
		ClockSkew = TimeSpan.Zero
	};
});

// ==========================================================
// Services & Repositories
// ==========================================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IUserThemeRepository, UserThemeRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<ISupabaseFileService, SupabaseFileService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddHttpClient<IPayOsService, PayOsService>((serviceProvider, client) =>
{
	var payOsSettings = serviceProvider.GetRequiredService<IConfiguration>().GetSection("PayOS").Get<PayOSSettings>()
						?? throw new InvalidOperationException("PayOS settings not configured.");
	client.BaseAddress = new Uri(payOsSettings.BaseUrl);
	client.DefaultRequestHeaders.Add("x-client-id", payOsSettings.ClientId);
	client.DefaultRequestHeaders.Add("x-api-key", payOsSettings.ApiKey);
});
// ==========================================================
// Controllers
// ==========================================================
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ==========================================================
// Swagger
// ==========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Memora API",
		Version = "v1",
		Description = "Memora Backend API with JWT authentication"
	});

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

// ==========================================================
// Build app
// ==========================================================
var app = builder.Build();

app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Memora API v1");
	c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Special RevenueCat webhook
app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/api/RevenueCatWebhook/revenuecat"))
	{
		context.Request.EnableBuffering();
	}
	await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();