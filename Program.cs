using LearnMicroservice.Models;
using LearnMicroservice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// =============================
// 🔐 Authentication & Authorization
// =============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			options.Authority = "https://login.microsoftonline.com/3c65f370-dc3a-4cf7-ac9e-af7e53636aea/v2.0";
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidAudience = "3d6f02fd-479d-4971-a4fa-d7b4e6871678",
			};
		});

builder.Services.AddAuthorization();

// =============================
// 🌐 CORS Configuration
// =============================
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:5173")
					.AllowAnyHeader()
					.AllowAnyMethod();
	});
});

// =============================
// 🔧 Register Services
// =============================
builder.Services.AddSingleton<IMyService, MyService>();
builder.Services.AddScoped<ScopedService>();
builder.Services.AddTransient<TransientService>();

// =============================
// 🔧 Swagger
// =============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lifetime Demo API", Version = "v1" });
});

var app = builder.Build();

// =============================
// 🧱 Middleware
// =============================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();            // ⬅️ Wajib sebelum auth
app.UseAuthentication();  // ⬅️ Auth
app.UseAuthorization();   // ⬅️ Authorization

app.Use(async (context, next) =>
{
	var scoped = context.RequestServices.GetRequiredService<ScopedService>();
	var singleton = context.RequestServices.GetRequiredService<IMyService>();
	var transient = context.RequestServices.GetRequiredService<TransientService>();

	Debug.WriteLine("=== [GLOBAL MIDDLEWARE] ===");
	Debug.WriteLine($"Scoped    : {scoped.Id}");
	Debug.WriteLine($"Transient : {transient.Id}");
	Debug.WriteLine($"Singleton : {singleton.GetInstanceId()}");

	await next();
});

// =============================
// 🌐 Public Endpoints
// =============================
app.MapGet("/", () => "hello world!");

app.MapGet("/getNumber/{number}", async (int number) =>
{
	await Task.Delay(500);
	return $"Hello : {number}";
});

app.MapGet("/lifetimes", (IMyService singleton, ScopedService scoped, TransientService transient) =>
{
	return new
	{
		Singleton = singleton.GetInstanceId(),
		Scoped = scoped.Id,
		Transient = transient.Id
	};
});

// =============================
// 🔐 Admin Group
// =============================
var adminGroup = app.MapGroup("/admin")
		.AddEndpointFilter(async (context, next) =>
		{
			Debug.WriteLine("🔐 [ADMIN MIDDLEWARE AKTIF]");
			return await next(context);
		});

adminGroup.MapGet("/", () => "Ini area admin.");

// =============================
// 🔄 POST/PUT
// =============================
app.MapPost("/submit", (DataModel data) =>
{
	return Results.Ok(new { Message = "Data diterima", Data = data });
});

app.MapPut("/update/{id}", (int id, DataModel data) =>
{
	return Results.Ok(new { Message = $"Data dengan ID {id} diperbarui", Data = data });
});

// =============================
// 🔐 Dummy Login (local)
// =============================
string SECRETKEY = "secretkey";
app.MapPost("/login", (LoginRequest guest) =>
{
	Dictionary<string, string> users = new()
		{
				{ "admin", "password123" },
				{ "user", "userpass" }
		};

	if (users.TryGetValue(guest.Username, out var password) && password == guest.Password)
	{
		return Results.Ok(new { Message = "Login berhasil!" });
	}
	else
	{
		return Results.Unauthorized();
	}
});

// =============================
// 🔐 Protected Endpoint (Microsoft Entra ID token)
// =============================
app.MapGet("/secure-data", (ClaimsPrincipal user) =>
{
	var username = user.Identity?.Name ?? "unknown";

	return Results.Ok(new
	{
		Message = "✅ Anda berhasil mengakses endpoint terlindungi.",
		User = username,
		Claims = user.Claims.Select(c => new { c.Type, c.Value })
	});
})
.RequireAuthorization();

app.Run();
