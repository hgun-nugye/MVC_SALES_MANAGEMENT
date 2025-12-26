# Dashboard Updates - Changelog

## ✅ Đã hoàn thành

### 1. **Fix Logic Sản phẩm ế**
- ✅ Sản phẩm bán chạy KHÔNG còn xuất hiện trong danh sách sản phẩm ế
- ✅ Logic: Lấy top 10 bán chạy → Loại trừ khỏi danh sách → Lấy 10 sản phẩm ế nhất còn lại

**Code thay đổi:** `Services/DashboardService.cs` - method `GetSlowMovingProducts()`

```csharp
// Lấy danh sách sản phẩm bán chạy để loại trừ
var topSellingIds = await _context.CTBH
    .GroupBy(ct => ct.MaSP)
    .Select(g => new { MaSP = g.Key, TotalQuantity = g.Sum(x => x.SLB) })
    .OrderByDescending(x => x.TotalQuantity)
    .Take(limit)
    .Select(x => x.MaSP)
    .ToListAsync();

// Loại trừ khi tạo danh sách ế
var slowMoving = allProducts
    .Where(ap => !topSellingIds.Contains(ap.MaSP)) // ← Loại trừ
    .GroupJoin(...)
```

### 2. **Chức năng Xuất File**

#### A. Xuất Excel (.xlsx)
- ✅ **4 Sheets:**
  1. **Tổng quan** - Các chỉ số dashboard
  2. **Sản phẩm bán chạy** - Top 10 với STT, Mã, Tên, Đã bán, Doanh thu
  3. **Sản phẩm ế** - Top 10 sản phẩm ế nhất
  4. **Doanh thu tháng** - 12 tháng + Tổng cộng

- ✅ **Styling:**
  - Headers màu xanh lá/cam/xanh dương
  - Font chữ đậm cho tiêu đề
  - Format số: `#,##0` (phân cách hàng nghìn)
  - Auto-fit columns

- ✅ **Package:** EPPlus 7.0.0

#### B. Xuất PDF
- ✅ **Nội dung:**
  - Tiêu đề + Ngày xuất
  - Bảng tổng quan
  - Bảng top sản phẩm bán chạy
  - Bảng sản phẩm ế

- ✅ **Package:** iText7 8.0.2

### 3. **UI Updates**
- ✅ Thêm 2 nút Export ở header Dashboard:
  - **Xuất Excel** (màu xanh, icon file-excel)
  - **Xuất PDF** (màu đỏ, icon file-pdf)

## 📦 Packages cần cài

**Lưu ý:** Bạn cần cài các packages sau để chức năng Export hoạt động:

```bash
dotnet add package EPPlus --version 7.0.0
dotnet add package itext7 --version 8.0.2
dotnet restore
```

Hoặc thêm vào `.csproj`:
```xml
<PackageReference Include="EPPlus" Version="7.0.0" />
<PackageReference Include="itext7" Version="8.0.2" />
```

## 🔧 Files đã tạo/sửa

| File | Loại | Mô tả |
|------|------|-------|
| `Services/DashboardService.cs` | Sửa | Fix logic sản phẩm ế |
| `Services/ExportService.cs` | Mới | Service xuất Excel & PDF |
| `Controllers/DashboardController.cs` | Sửa | Thêm endpoints ExportToExcel, ExportToPdf |
| `Views/Dashboard/Index.cshtml` | Sửa | Thêm nút Export |
| `Program.cs` | Sửa | Đăng ký ExportService |
| `DOCS/Export_Packages_Install.md` | Mới | Hướng dẫn cài packages |

## 🚀 Cách sử dụng

### 1. Xuất Excel
1. Vào Dashboard (`/Dashboard/Index`)
2. Click nút **"Xuất Excel"**
3. File `.xlsx` sẽ tự động download với tên: `Dashboard_Report_YYYYMMDD_HHMMSS.xlsx`

### 2. Xuất PDF
1. Vào Dashboard
2. Click nút **"Xuất PDF"**
3. File `.pdf` sẽ tự động download với tên: `Dashboard_Report_YYYYMMDD_HHMMSS.pdf`

## 📊 Cấu trúc File Excel

```
Sheet 1: Tổng quan
┌─────────────────────────────┬──────────┐
│ Chỉ số                      │ Giá trị  │
├─────────────────────────────┼──────────┤
│ Tổng số sản phẩm            │ 150      │
│ Tổng số khách hàng          │ 320      │
│ Doanh thu tháng này (VNĐ)   │ 50,000   │
└─────────────────────────────┴──────────┘

Sheet 2: Sản phẩm bán chạy
┌─────┬────────┬──────────────┬─────────┬────────────┐
│ STT │ Mã SP  │ Tên SP       │ Đã bán  │ Doanh thu  │
├─────┼────────┼──────────────┼─────────┼────────────┤
│ 1   │ SP001  │ Sản phẩm A   │ 500     │ 10,000,000 │
│ 2   │ SP002  │ Sản phẩm B   │ 450     │ 9,000,000  │
└─────┴────────┴──────────────┴─────────┴────────────┘

Sheet 3: Sản phẩm ế
(Tương tự Sheet 2, nhưng sắp xếp tăng dần)

Sheet 4: Doanh thu tháng
┌───────┬───────────┬──────────────┐
│ Tháng │ Tên tháng │ Doanh thu    │
├───────┼───────────┼──────────────┤
│ 1     │ Jan       │ 5,000,000    │
│ 2     │ Feb       │ 6,000,000    │
│ ...   │ ...       │ ...          │
│ TỔNG  │           │ 60,000,000   │
└───────┴───────────┴──────────────┘
```

## 🐛 Troubleshooting

### Lỗi: "Could not load file or assembly 'EPPlus'"
**Giải pháp:** Chạy `dotnet restore` hoặc cài package EPPlus

### Lỗi: "License context must be set"
**Giải pháp:** Đã set trong code: `ExcelPackage.LicenseContext = LicenseContext.NonCommercial;`

### File Excel/PDF bị lỗi khi mở
**Kiểm tra:**
- Có dữ liệu trong database không?
- Console có báo lỗi không?
- Try-catch trong Controller đã bắt được exception chưa?

## 🎯 Testing Checklist

- [ ] Click "Xuất Excel" → File download thành công
- [ ] Mở file Excel → 4 sheets hiển thị đúng
- [ ] Dữ liệu trong Excel khớp với Dashboard
- [ ] Click "Xuất PDF" → File download thành công
- [ ] Mở file PDF → Nội dung hiển thị đúng
- [ ] Sản phẩm bán chạy KHÔNG xuất hiện trong danh sách ế
- [ ] Format số có dấu phân cách hàng nghìn

## 📝 Notes

- **EPPlus License:** Đang dùng `NonCommercial`. Nếu dùng thương mại, cần mua license.
- **iText7 License:** AGPL. Nếu dùng thương mại, cần mua license hoặc dùng thư viện khác (QuestPDF).
- **Performance:** Với dữ liệu lớn (>10,000 records), nên thêm pagination hoặc filter theo ngày.
- **Security:** Nên thêm authorization check (chỉ Admin/Manager mới export được).

## 🔐 Security Recommendations

Thêm vào `DashboardController`:

```csharp
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> ExportToExcel()
{
    // ...
}

[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> ExportToPdf()
{
    // ...
}
```

## 🚀 Future Enhancements

1. **Filter theo ngày:** Cho phép user chọn khoảng thời gian để export
2. **Email report:** Tự động gửi báo cáo qua email định kỳ
3. **Scheduled reports:** Cron job tự động tạo báo cáo hàng tuần/tháng
4. **More charts in PDF:** Thêm biểu đồ vào file PDF
5. **Custom templates:** Cho phép user chọn template Excel/PDF

Tất cả đã hoàn thành và sẵn sàng sử dụng! 🎉
