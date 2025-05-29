# 📦 Lifetime Demo API - ASP.NET Core Minimal API

Aplikasi ini adalah contoh sederhana dari ASP.NET Core Minimal API yang dibangun untuk mendemonstrasikan beberapa konsep fundamental dalam pengembangan web modern, termasuk:

- Dependency Injection dan berbagai jenis service lifetime (Singleton, Scoped, Transient)
- Middleware global dan grup menggunakan Endpoint Filter
- Endpoint GET, POST, dan PUT
- Integrasi Swagger/OpenAPI untuk dokumentasi otomatis

---

## 🚀 Konsep Utama

### 1. Dependency Injection (DI) dan Service Lifetimes

ASP.NET Core menyediakan container DI bawaan yang mendukung tiga jenis lifetime:

#### 🔁 a. Singleton
```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```
- Hanya satu instance sepanjang masa hidup aplikasi
- Cocok untuk konfigurasi global atau resource yang mahal
- Contoh: `IMyService`

#### 🔄 b. Scoped
```csharp
builder.Services.AddScoped<ScopedService>();
```
- Satu instance per HTTP request
- Cocok untuk konteks database (e.g., EF Core DbContext)
- Contoh: `ScopedService`

#### 🌀 c. Transient
```csharp
builder.Services.AddTransient<TransientService>();
```
- Instance baru setiap kali diminta
- Cocok untuk service stateless dan ringan
- Contoh: `TransientService`

**🔍 Tes di endpoint `/lifetimes` untuk melihat perbedaan ID dari tiap jenis lifetime.**

---

### 2. Middleware

#### 🌐 Global Middleware
```csharp
app.Use(async (context, next) => { ... });
```
- Dieksekusi di semua request
- Contoh: logging service ID di middleware global

#### 🔐 Group Middleware dengan `MapGroup` dan `AddEndpointFilter`
```csharp
var adminGroup = app.MapGroup("/admin")
    .AddEndpointFilter(async (context, next) => {
        Console.WriteLine("🔐 [ADMIN MIDDLEWARE AKTIF]");
        return await next(context);
    });
```
- Khusus endpoint dalam grup `/admin`
- Lebih modular dan bersih di .NET 7/8

---

### 3. Endpoint

#### ✅ GET
- `/` – Hello World
- `/getNumber/{number}` – Tampilkan nomor
- `/lifetimes` – Lihat ID dari service Singleton, Scoped, dan Transient
- `/admin` – Endpoint admin dengan middleware grup

#### ➕ POST
```http
POST /submit
Content-Type: application/json

{
  "name": "Nama Pengguna",
  "value": 123
}
```

#### ✏️ PUT
```http
PUT /update/{id}
Content-Type: application/json

{
  "name": "Nama Baru",
  "value": 456
}
```

---

### 4. 📚 Swagger / OpenAPI

- Swagger diaktifkan dengan:
```csharp
builder.Services.AddSwaggerGen();
```
- Akses di:
  ```
  http://localhost:5000/swagger
  ```
- Anda bisa menjelajahi dan menguji semua endpoint secara interaktif

---

## 📂 Struktur File (Contoh)
```
LearnMicroservice/
├── Models/
│   └── DataModel.cs
├── Services/
│   ├── IMyService.cs
│   ├── MyService.cs
│   ├── ScopedService.cs
│   └── TransientService.cs
├── Program.cs
└── README.md
```

---

## 🧪 Cara Menjalankan

```bash
dotnet run
```

Lalu akses:
- `http://localhost:5000/` – Hello World
- `http://localhost:5000/lifetimes` – Tes DI Lifetimes
- `http://localhost:5000/admin` – Endpoint admin
- `http://localhost:5000/swagger` – UI Swagger

---

## 🧠 Catatan Tambahan

- Gunakan `ILogger<T>` untuk logging yang lebih baik daripada `Console.WriteLine`.
- Gunakan tool seperti Postman atau Swagger UI untuk testing endpoint POST/PUT.

---

## ✅ Teknologi yang Digunakan

- ASP.NET Core 8
- .NET SDK 8
- Swagger (Swashbuckle)
