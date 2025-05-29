using LearnMicroservice.Models;
using LearnMicroservice.Services;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// =============================
// 🔧 Register Services
// =============================
builder.Services.AddSingleton<IMyService, MyService>();          // Singleton
builder.Services.AddScoped<ScopedService>();                     // Scoped
builder.Services.AddTransient<TransientService>();               // Transient

// =============================
// 🔧 Enable Swagger
// =============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lifetime Demo API", Version = "v1" });
});

var app = builder.Build();

// =============================
// 📜 Global Middleware
// =============================
app.UseSwagger();
app.UseSwaggerUI();

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

// Important: Add UseRouting and UseAuthorization/UseAuthentication if you plan to use those
// app.UseRouting();
// app.UseAuthorization(); // Or UseAuthentication, depending on your setup

// =============================
// 🌐 Endpoints
// =============================
app.MapGet("/", () => "hello world!");

app.MapGet("/getNumber/{number}", async (int number) =>
{
	await Task.Delay(500); // Simulate delay
	return $"Hello : {number}";
});

app.MapGet("/lifetimes", (IMyService singleton, ScopedService scoped, TransientService transient) =>
{
	Debug.WriteLine("=== [ENDPOINT /lifetimes] ===");
	Debug.WriteLine($"Scoped    : {scoped.Id}");
	Debug.WriteLine($"Transient : {transient.Id}");
	Debug.WriteLine($"Singleton : {singleton.GetInstanceId()}");

	return new
	{
		Singleton = singleton.GetInstanceId(),
		Scoped = scoped.Id,
		Transient = transient.Id
	};
});

// =============================
// 🔐 Middleware dan endpoint untuk /admin (FIXED)
// =============================
var adminGroup = app.MapGroup("/admin")
		.AddEndpointFilter(async (context, next) =>
		{
			// This is like an inline middleware for the group
			Debug.WriteLine("🔐 [ADMIN MIDDLEWARE AKTIF]");

			// You can inject services here if needed (e.g., for logging, authorization)
			// var myService = context.HttpContext.RequestServices.GetRequiredService<IMyService>();

			return await next(context);
		});

adminGroup.MapGet("/", () => "Ini area admin.");
// You can add more admin-specific endpoints here, and they will all use the filter above
// adminGroup.MapGet("/dashboard", () => "Admin Dashboard!");


// =============================
// 🔄 POST dan PUT endpoint tambahan
// =============================
app.MapPost("/submit", (DataModel data) =>
{
	Debug.WriteLine($"📩 Received POST: {data.Name} - {data.Value}");
	return Results.Ok(new { Message = "Data diterima", Data = data });
});

app.MapPut("/update/{id}", (int id, DataModel data) =>
{
	Debug.WriteLine($"✏️ Updated ID {id} with Name: {data.Name}, Value: {data.Value}");
	return Results.Ok(new { Message = $"Data dengan ID {id} diperbarui", Data = data });
});

app.Run();