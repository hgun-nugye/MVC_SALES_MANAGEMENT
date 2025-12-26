# Cài đặt packages cho Export Excel và PDF

## Packages cần thiết:

```bash
# EPPlus cho Excel export
dotnet add package EPPlus --version 7.0.0

# iTextSharp cho PDF export (hoặc dùng QuestPDF - modern hơn)
dotnet add package itext7 --version 8.0.2
```

## Hoặc thêm vào .csproj:

```xml
<PackageReference Include="EPPlus" Version="7.0.0" />
<PackageReference Include="itext7" Version="8.0.2" />
```

Sau đó chạy:
```bash
dotnet restore
```
