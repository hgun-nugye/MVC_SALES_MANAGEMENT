# Dashboard - Hệ thống Thống kê

## 📊 Tổng quan

Dashboard cung cấp cái nhìn tổng quan về hoạt động kinh doanh với các chỉ số quan trọng và biểu đồ trực quan.

## ✨ Tính năng

### 1. **Thẻ thống kê (Stats Cards)**

Hiển thị các chỉ số quan trọng:

| Chỉ số | Mô tả |
|--------|-------|
| **Tổng số sản phẩm** | Số lượng sản phẩm hiện có trong hệ thống |
| **Tổng số khách hàng** | Số lượng khách hàng đã đăng ký |
| **Tổng số nhân viên** | Số lượng nhân viên đang làm việc |
| **Tổng đơn bán hàng** | Tổng số đơn bán hàng đã tạo |
| **Doanh thu tháng này** | Tổng doanh thu của tháng hiện tại (VNĐ) |
| **Doanh thu năm nay** | Tổng doanh thu từ đầu năm đến nay (VNĐ) |
| **Đơn hàng tháng này** | Số đơn hàng được tạo trong tháng |

### 2. **Biểu đồ Doanh thu theo tháng**

- **Loại:** Line Chart (Biểu đồ đường)
- **Dữ liệu:** Doanh thu từng tháng trong năm hiện tại
- **Công dụng:** Theo dõi xu hướng doanh thu, phát hiện mùa cao điểm/thấp điểm

### 3. **Biểu đồ Tỷ lệ đơn hàng**

- **Loại:** Doughnut Chart (Biểu đồ tròn)
- **Dữ liệu:** Tỷ lệ giữa đơn bán hàng và đơn mua hàng
- **Công dụng:** So sánh hoạt động mua/bán

### 4. **Top 10 Sản phẩm bán chạy nhất**

Bảng hiển thị:
- Mã sản phẩm
- Tên sản phẩm
- Tổng số lượng đã bán
- Tổng doanh thu

**Sắp xếp:** Theo số lượng bán giảm dần

### 5. **Top 10 Sản phẩm ế nhất**

Bảng hiển thị:
- Mã sản phẩm
- Tên sản phẩm
- Tổng số lượng đã bán (ít nhất)
- Tổng doanh thu

**Sắp xếp:** Theo số lượng bán tăng dần

**Công dụng:** Phát hiện sản phẩm cần có chiến lược marketing hoặc giảm giá

## 🔧 Cấu trúc Code

### Files đã tạo:

```
Controllers/
  └── DashboardController.cs       # Controller xử lý requests

Services/
  └── DashboardService.cs          # Business logic và tính toán thống kê

Views/
  └── Dashboard/
      └── Index.cshtml              # Giao diện Dashboard
```

### API Endpoints:

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `/Dashboard/Index` | GET | Trang Dashboard chính |
| `/Dashboard/GetMonthlyRevenue?year={year}` | GET | API lấy doanh thu theo tháng |
| `/Dashboard/GetTopProducts?limit={limit}` | GET | API lấy top sản phẩm bán chạy |
| `/Dashboard/GetSlowMovingProducts?limit={limit}` | GET | API lấy sản phẩm ế |

## 📝 Cách sử dụng

### 1. Truy cập Dashboard

- Click vào menu **"Dashboard"** ở sidebar (mục Tổng quan)
- Hoặc truy cập trực tiếp: `/Dashboard/Index`

### 2. Xem thống kê

- Các thẻ thống kê tự động load khi vào trang
- Biểu đồ sử dụng **Chart.js** để render
- Bảng sản phẩm load qua AJAX

### 3. Tương tác

- **Hover** vào thẻ thống kê để xem hiệu ứng
- **Hover** vào biểu đồ để xem chi tiết từng điểm dữ liệu
- Bảng sản phẩm có thể scroll nếu dữ liệu nhiều

## 🎨 Giao diện

### Màu sắc thẻ thống kê:

- **Xanh lá (Green):** Sản phẩm, Đơn hàng tháng
- **Xanh dương (Blue):** Khách hàng
- **Cam (Orange):** Nhân viên
- **Tím (Purple):** Đơn bán hàng
- **Đỏ (Red):** Doanh thu tháng
- **Xanh ngọc (Teal):** Doanh thu năm

### Responsive:

- Desktop: 4 cột cho thẻ thống kê
- Tablet: 2 cột
- Mobile: 1 cột (stack vertically)

## 🚀 Mở rộng

### Thêm chỉ số mới:

1. **Cập nhật Model** trong `DashboardService.cs`:
```csharp
public class DashboardStats
{
    // ... existing properties ...
    public int NewMetric { get; set; }
}
```

2. **Tính toán trong Service**:
```csharp
public async Task<DashboardStats> GetDashboardStats()
{
    var stats = new DashboardStats
    {
        // ... existing calculations ...
        NewMetric = await CalculateNewMetric()
    };
    return stats;
}
```

3. **Hiển thị trong View**:
```cshtml
<div class="stats-card purple">
    <h3 class="stats-value">@Model.NewMetric</h3>
    <p class="stats-label">Chỉ số mới</p>
</div>
```

### Thêm biểu đồ mới:

1. Tạo API endpoint trong Controller
2. Tạo `<canvas>` trong View
3. Viết JavaScript để render Chart.js

## 📊 Công thức tính toán

### Doanh thu tháng:
```sql
SELECT SUM(SLB * DGB) 
FROM DonBanHang 
JOIN CTBH ON DonBanHang.MaDBH = CTBH.MaDBH
WHERE YEAR(NgayBH) = @Year AND MONTH(NgayBH) = @Month
```

### Top sản phẩm bán chạy:
```sql
SELECT MaSP, SUM(SLB) as TotalSold, SUM(SLB * DGB) as Revenue
FROM CTBH
GROUP BY MaSP
ORDER BY TotalSold DESC
```

### Sản phẩm ế:
```sql
SELECT sp.MaSP, sp.TenSP, ISNULL(SUM(ct.SLB), 0) as TotalSold
FROM SanPham sp
LEFT JOIN CTBH ct ON sp.MaSP = ct.MaSP
GROUP BY sp.MaSP, sp.TenSP
ORDER BY TotalSold ASC
```

## 🐛 Troubleshooting

### Biểu đồ không hiển thị?

**Kiểm tra:**
- Console browser có lỗi không? (F12)
- Chart.js CDN đã load chưa?
- API endpoint trả về dữ liệu đúng format chưa?

### Số liệu sai?

**Kiểm tra:**
- Dữ liệu trong database có đúng không?
- Logic tính toán trong `DashboardService.cs`
- Join giữa các bảng có đúng không?

### Performance chậm?

**Giải pháp:**
- Thêm index cho các cột thường query (NgayBH, MaSP, MaDBH)
- Cache kết quả thống kê (Redis, MemoryCache)
- Pagination cho bảng sản phẩm

## 🎯 Best Practices

1. **Caching:** Cache dashboard stats trong 5-10 phút để giảm tải DB
2. **Async/Await:** Luôn dùng async cho database queries
3. **Error Handling:** Wrap API calls trong try-catch
4. **Loading States:** Hiển thị spinner khi đang load dữ liệu
5. **Responsive:** Test trên nhiều kích thước màn hình

## 📚 Dependencies

- **Chart.js 4.x:** Thư viện vẽ biểu đồ
- **jQuery 3.6:** AJAX calls
- **Bootstrap 5:** Layout và styling
- **Font Awesome 6:** Icons

Tất cả đã được include trong `_Layout.cshtml` và Dashboard view.
