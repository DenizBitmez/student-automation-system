# Öğrenci Otomasyon Sistemi

## Proje Açıklaması

Bu proje, öğrenci, öğretmen ve admin rollerini destekleyen kapsamlı bir öğrenci yönetim sistemidir. .NET 9, Entity Framework Core, PostgreSQL ve Blazor WebAssembly teknolojileri kullanılarak geliştirilmiştir.

## 🚀 Teknolojiler

### Backend
- **.NET 9** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Veritabanı
- **ASP.NET Identity** - Kimlik doğrulama ve yetkilendirme
- **JWT Bearer Token** - API güvenliği
- **Swagger/OpenAPI** - API dokümantasyonu

### Frontend
- **Blazor WebAssembly** - SPA framework
- **Bootstrap 5** - UI framework
- **Blazored.LocalStorage** - Token yönetimi

## 🏗️ Proje Yapısı

```
OgrenciOtomasyonSistemi/
├── StudentManagementApi/          # Backend API
│   ├── Controllers/               # API Controllers
│   ├── Data/                      # DbContext ve Seed
│   ├── Domain/                    # Entity sınıfları
│   ├── Dtos/                      # Data Transfer Objects
│   ├── Services/                  # Business logic
│   └── Extensions/                # Service extensions
├── StudentManagementFrontend/     # Blazor WebAssembly
│   ├── Components/                # Razor components
│   ├── Models/                    # Frontend models
│   └── Services/                  # HTTP services
└── README.md
```

## 📋 Özellikler

### ✅ Temel Özellikler

#### Kullanıcı Yönetimi
- ✅ Kayıt (Register) ve Giriş (Login)
- ✅ Üç rol: **Admin**, **Teacher**, **Student**
- ✅ JWT tabanlı kimlik doğrulama
- ✅ Şifre güvenliği (ASP.NET Identity)

#### Öğrenci İşlemleri (CRUD)
- ✅ Admin ve öğretmen öğrenci ekleyebilir, güncelleyebilir, listeleyebilir
- ✅ Öğrenci kendi bilgilerini görüntüleyebilir

#### Öğretmen İşlemleri (CRUD)
- ✅ Admin öğretmen ekleyebilir, güncelleyebilir, listeleyebilir

#### Ders Yönetimi (CRUD)
- ✅ Admin ders oluşturabilir
- ✅ Öğretmen kendi derslerini görebilir
- ✅ Öğretmen derse öğrenci ekleyebilir, silebilir
- ✅ Ders durumu güncellenebilir

#### Not ve Devamsızlık
- ✅ Öğretmen öğrencilerine ders bazında not ekleyebilir
- ✅ Öğrenciler notlarını görebilir
- ✅ Devamsızlık kaydı tutulabilir
- ✅ Öğretmen öğrencilerini yorumlayabilir

#### Frontend Sayfaları
- ✅ Login/Register ekranları
- ✅ Öğrenci listesi ve detay sayfaları
- ✅ Öğretmen listesi ve detay sayfaları
- ✅ Ders listesi ve detay sayfaları

### 🎯 Bonus Özellikler

#### Tamamlanan Bonus Özellikler
- ✅ **Docker Desteği** - Full-stack Docker Compose, Nginx reverse proxy
- ✅ **Swagger/API Dokümantasyonu** - OpenAPI entegrasyonu
- ✅ **Clean Code** - SOLID prensipleri ve temiz kod yapısı
- ✅ **Unit/Integration Testleri** - xUnit ile kapsamlı test coverage
- ✅ **CI/CD Pipeline** - GitHub Actions ile otomatik deployment
- ✅ **PDF Export** - Öğrenci listesi, notlar, devamsızlık raporları
- ✅ **CSV Export** - Veri dışa aktarma
- ✅ **Tema Özelleştirme** - Dark/Light mode, renk seçenekleri, layout ayarları
- ✅ **Bildirim Sistemi** - SignalR ile real-time bildirimler
- ✅ **Gelişmiş Arama ve Filtreleme** - Multi-criteria search, sorting
- ✅ **Şifre Sıfırlama** - Email tabanlı güvenli şifre sıfırlama

## ⚙️ Kurulum ve Çalıştırma

### Ön Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [Visual Studio Code](https://code.visualstudio.com/)

### 1. Repository'yi Klonlayın

```bash
git clone [repository-url]
cd OgrenciOtomasyonSistemi
```

### 2. PostgreSQL Veritabanını Ayarlayın

PostgreSQL'de yeni bir veritabanı oluşturun:

```sql
CREATE DATABASE StudentManagementDb;
```

### 3. Backend'i Ayarlayın

```bash
cd StudentManagementApi
```

`appsettings.json` dosyasında bağlantı stringini güncelleyin:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=StudentManagementDb;Username=your_username;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "StudentManagementApi",
    "Audience": "StudentManagementClient"
  }
}
```

Veritabanı migration'larını çalıştırın:

```bash
dotnet ef database update
```

Backend'i çalıştırın:

```bash
dotnet run
```

API şu adreste çalışacak: `https://localhost:7001`

### 4. Frontend'i Ayarlayın

Yeni terminal açın:

```bash
cd StudentManagementFrontend
```

`appsettings.json` dosyasında backend URL'sini kontrol edin:

```json
{
  "BaseUrl": "https://localhost:7001/"
}
```

Frontend'i çalıştırın:

```bash
dotnet run
```

Frontend şu adreste çalışacak: `https://localhost:5001`

### 5. Docker ile Çalıştırma (Opsiyonel)

Backend için Docker container oluşturun:

```bash
cd StudentManagementApi
docker build -t student-management-api .
docker run -p 8080:8080 student-management-api
```

## 👥 Test Kullanıcıları

Sistem ilk çalıştırıldığında otomatik olarak test kullanıcıları oluşturulur:

### Admin
- **Email:** admin@test.com
- **Şifre:** Passw0rd!
- **Yetki:** Tüm işlemler

### Öğretmen
- **Email:** teacher@test.com
- **Şifre:** Passw0rd!
- **Yetki:** Öğrenci yönetimi, not verme, devamsızlık işlemleri

### Öğrenci
- **Email:** student@test.com
- **Şifre:** Passw0rd!
- **Yetki:** Kendi bilgilerini görüntüleme, notlarını görme

## 🔧 API Endpoints

### Kimlik Doğrulama
- `POST /api/auth/register` - Yeni kullanıcı kaydı
- `POST /api/auth/login` - Kullanıcı girişi

### Öğrenci İşlemleri
- `GET /api/student` - Öğrenci listesi
- `POST /api/student` - Yeni öğrenci ekleme
- `PUT /api/student/{id}` - Öğrenci güncelleme
- `DELETE /api/student/{id}` - Öğrenci silme

### Öğretmen İşlemleri
- `GET /api/teacher` - Öğretmen listesi
- `POST /api/teacher` - Yeni öğretmen ekleme
- `PUT /api/teacher/{id}` - Öğretmen güncelleme
- `DELETE /api/teacher/{id}` - Öğretmen silme

### Ders İşlemleri
- `GET /api/course` - Ders listesi
- `POST /api/course` - Yeni ders ekleme
- `PUT /api/course/{id}` - Ders güncelleme
- `DELETE /api/course/{id}` - Ders silme

### Not İşlemleri
- `POST /api/grade` - Not verme
- `GET /api/grade/by-student/{studentId}` - Öğrenci notları

### Devamsızlık İşlemleri
- `POST /api/attendance/tick/{enrollmentId}` - Devamsızlık kaydetme
- `GET /api/attendance/by-student/{studentId}` - Öğrenci devamsızlık listesi

## 🔍 Swagger Dokümantasyonu

Backend çalıştırıldığında Swagger dokümantasyonuna şu adresten erişebilirsiniz:
`https://localhost:7001/swagger`

## 🚀 Geliştirme ve Katkı

### Kod Yapısı
- **Controllers:** API endpoint'leri
- **Services:** İş mantığı
- **Domain:** Entity sınıfları
- **Data:** Veritabanı işlemleri
- **DTOs:** Veri transfer nesneleri

### Coding Standards
- Clean Code prensipleri
- SOLID principles
- Dependency Injection
- Async/await pattern
- Error handling

## 📝 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

## 📞 İletişim

Herhangi bir sorunuz olursa lütfen iletişime geçin.

---

**Geliştirme Tarihi:** Eylül 2025  
**Teknoloji Stack:** .NET 9 + Blazor WebAssembly + PostgreSQL