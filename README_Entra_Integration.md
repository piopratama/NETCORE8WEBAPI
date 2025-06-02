# 🔐 Microsoft Entra ID Integration - Minimal API + React + MSAL.js

README ini menjelaskan **dengan sederhana** bagaimana login menggunakan **akun Microsoft (Entra ID)** untuk mengakses API yang aman.

---

## 🧠 Alur Pemikiran Sederhana

### 1. Kita Punya Dua Aplikasi

| Aplikasi         | Tujuan                              |
|------------------|--------------------------------------|
| ✅ **Frontend**  | React SPA, tempat user login         |
| ✅ **Backend**   | ASP.NET Core API, berisi data rahasia|

---

## 🏠 Kapan Pakai SPA vs Web App?

| Tipe Aplikasi | Contoh             | Cocok Untuk             |
|---------------|--------------------|--------------------------|
| SPA (Single Page App) | React, Angular, Vue | Aplikasi modern yang jalan di browser |
| Web App (Server-side) | Razor Pages, MVC    | Butuh HTML dari server (bukan frontend JS) |

**Kita pakai SPA** karena:  
- Semua UI dan login ditangani di browser.  
- Tidak perlu backend untuk render HTML.  
- Pakai MSAL.js langsung di browser.

---

## 🔧 Apa itu Client ID, Tenant ID, dan Application ID URI?

| Nama         | Penjelasan Sederhana                                                                 |
|--------------|----------------------------------------------------------------------------------------|
| **Client ID** | ID unik aplikasi kamu di Microsoft Entra (Frontend dan Backend punya ID sendiri)     |
| **Tenant ID** | ID perusahaan atau organisasi kamu di Microsoft Entra                                 |
| **Application ID URI** | Nama unik untuk API kamu. Misalnya: `api://<app-id>`                       |

---

## 🔁 Bagaimana Semua Ini Terhubung?

### Langkah demi langkah:

1. **Frontend (React) tahu Client ID-nya**, dan tenant ID (organisasi Microsoft kamu)
2. MSAL login → User diminta masuk akun Microsoft
3. Setelah login sukses, Microsoft kasih **Access Token**
4. Token itu punya:
   - Siapa user-nya
   - Untuk aplikasi mana (`audience`)
   - Siapa yang keluarkan (`issuer`)
   - Scope: `access_as_user`
5. Frontend kirim token ke backend pakai header:  
   `Authorization: Bearer <token>`
6. **Backend cocokkan token**:
   - Token dari Microsoft?
   - Untuk API ini?
   - Token masih berlaku?
7. Jika cocok → izinkan akses! 🎉

---

## 🤔 Kenapa Tidak Bikin Token JWT Sendiri?

Membuat token JWT sendiri artinya:

- Kamu harus membuat sistem login sendiri
- Kamu harus enkripsi dan tandatangan token
- Kamu harus simpan user dan password

**Masalahnya:**  
- Sulit, rawan error, dan tidak aman jika tidak ahli
- Sulit di-maintain jika skalanya besar
- Harus tangani logout, expired token, dll

**Microsoft Entra (Azure AD)** sudah menyelesaikan semua itu:
- Login aman dengan akun Microsoft
- Token valid dan diverifikasi
- Sudah terstandar
- Bisa terhubung ke aplikasi lain (Teams, Office, Graph API, dll)

---

## 🧪 Masalah Umum (dan Solusinya)

| Masalah                    | Solusi                                                                 |
|----------------------------|------------------------------------------------------------------------|
| ❌ Tidak bisa pakai curl/Postman | Karena login Microsoft butuh interaksi manusia (popup)          |
| ❌ CORS error              | Pastikan backend izinkan origin frontend pakai `app.UseCors()`         |
| ❌ Token tidak valid       | Pastikan scope dan audience cocok, cek ID dan tenant di Azure Portal  |

---

## ✅ Ringkasan Konfigurasi

### 🔧 Frontend (React)
```ts
const msalInstance = new msal.PublicClientApplication({
  auth: {
    clientId: '<frontend-app-id>',
    authority: 'https://login.microsoftonline.com/<tenant-id>',
    redirectUri: 'http://localhost:5173',
  }
});

scopes: ['api://<backend-app-id>/access_as_user']
```

### 🔧 Backend (.NET)
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.Authority = "https://login.microsoftonline.com/<tenant-id>/v2.0";
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateAudience = true,
          ValidAudience = "<backend-app-id>",
      };
  });
```

Endpoint aman:
```csharp
app.MapGet("/secure-data", (ClaimsPrincipal user) => { ... })
   .RequireAuthorization();
```

---

Dengan sistem ini, kamu bisa membuat aplikasi modern yang:
- Aman
- Terintegrasi dengan akun Microsoft
- Tidak perlu bikin sistem login sendiri

🔥 Sangat cocok untuk perusahaan, integrasi Office, atau project modern yang butuh otentikasi serius.

---

## 🧪 Tes Token dengan [jwt.ms](https://jwt.ms)

Setelah berhasil login dengan MSAL.js dan mendapatkan **access token**, kamu bisa menyalin token tersebut dan membuka:

🔗 https://jwt.ms

### Apa yang Dilakukan?

- Website ini **men-decode JWT token** dan menampilkan isinya secara rapi.
- Kamu bisa melihat:
  - `aud`: Audience → harus cocok dengan App ID backend kamu
  - `iss`: Issuer → harus dari Microsoft (`https://login.microsoftonline.com/...`)
  - `scp`: Scope → misalnya `access_as_user`
  - `exp`: Expired timestamp (kapan token kadaluarsa)
  - `name`, `preferred_username` → informasi user

### Kenapa Ini Penting?

- Membantu debug token kamu valid atau tidak
- Bisa pastikan token dikirim oleh Microsoft dan cocok untuk backend kamu
- Cek klaim (claims) seperti email, user ID, dan scope

---

💡 Tips:
- Jangan pernah kirim token ini ke orang lain atau upload ke tempat umum.
- Gunakan hanya untuk keperluan pengujian lokal dan pribadi.