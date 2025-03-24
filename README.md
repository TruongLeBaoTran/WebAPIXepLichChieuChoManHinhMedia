# WebAPIXepLichChieuChoManHinhMedia

## 📌 Giới thiệu
WebAPIXepLichChieuChoManHinhMedia là một Web API được phát triển bằng **ASP.NET Core 6** nhằm quản lý các bản ghi, cho phép lên lịch chiếu các bản ghi trên màn hình media đảm bảo không trùng lịch phát trong cùng 1 thời điểm.

## 🚀 Công nghệ sử dụng
- **.NET 6 (ASP.NET Core Web API)**
- **Entity Framework Core** (Code-First, Fluent API)
- **Microsoft SQL Server** (Lưu trữ dữ liệu)
- **AutoMapper** (Chuyển đổi dữ liệu giữa DTO và Model)
- **Swagger UI** (Tài liệu API)
- **Repository Pattern** (Quản lý dữ liệu)

## 📚 Cấu trúc thư mục
```
WebAPIXepLichChieuChoManHinhMedia/
│-- Controllers/         # Xử lý request và trả về response cho client
│-- Models/              # Data Transfer Objects
│-- Data/                # (Domain models) DbContext, Models và cấu hình EF Core
│-- Mappings/            # Cấu hình AutoMapper
│-- Migrations/          # Lưu trữ các migration của database
│-- Properties/          # Cấu hình dự án
│-- Repository/          # Repository Pattern
│-- Services/            # Chứa logic nghiệp vụ
│-- wwwroot/              # Thư mục lưu trữ file upload
│-- Validator/           # Kiểm tra dữ liệu đầu vào
│-- Program.cs           # Cấu hình ứng dụng
```

## 🔑 Chức năng chính
✅ **Quản lý các bản ghi**: Tạo mới, cập nhật và xóa thông tin các bản ghi.  
✅ **Lên lịch phát bản ghi**: Lên lịch phát bản ghi theo chu kì và đảm bảo không trùng lịch phát   

## 📚 API Documentation
Sử dụng **Postman** để kiểm thử API
