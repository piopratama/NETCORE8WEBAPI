# Lifetime Demo API - ASP.NET Core Minimal API

[![DOI](https://zenodo.org/badge/992501251.svg)](https://doi.org/10.5281/zenodo.18732528)

This application is a simple ASP.NET Core Minimal API project built to
demonstrate fundamental concepts in modern web development, including:

-   Dependency Injection and service lifetimes (Singleton, Scoped,
    Transient)
-   Global and group middleware using Endpoint Filters
-   GET, POST, and PUT endpoints
-   Swagger/OpenAPI integration for automatic documentation

------------------------------------------------------------------------

## Core Concepts

### 1. Dependency Injection (DI) and Service Lifetimes

#### Singleton

``` csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

#### Scoped

``` csharp
builder.Services.AddScoped<ScopedService>();
```

#### Transient

``` csharp
builder.Services.AddTransient<TransientService>();
```

Test the `/lifetimes` endpoint to observe different service instance
IDs.

------------------------------------------------------------------------

## Middleware

### Global Middleware

``` csharp
app.Use(async (context, next) => { ... });
```

### Group Middleware

``` csharp
var adminGroup = app.MapGroup("/admin")
    .AddEndpointFilter(async (context, next) => {
        Console.WriteLine("[ADMIN MIDDLEWARE ACTIVE]");
        return await next(context);
    });
```

------------------------------------------------------------------------

## Endpoints

### GET

-   `/`
-   `/getNumber/{number}`
-   `/lifetimes`
-   `/admin`

### POST

``` http
POST /submit
Content-Type: application/json

{
  "name": "User Name",
  "value": 123
}
```

### PUT

``` http
PUT /update/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "value": 456
}
```

------------------------------------------------------------------------

## Swagger

Enable Swagger:

``` csharp
builder.Services.AddSwaggerGen();
```

Access: http://localhost:5000/swagger

------------------------------------------------------------------------

## How to Run

``` bash
dotnet run
```

------------------------------------------------------------------------

## Technologies Used

-   ASP.NET Core 8
-   .NET SDK 8
-   Swagger (Swashbuckle)
