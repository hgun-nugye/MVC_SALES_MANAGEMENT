using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Services
{
	public class DashboardService
	{
		private readonly AppDbContext _context;

		public DashboardService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<DashboardStats> GetDashboardStats()
		{
			var result = await _context.DashboardStats
				.FromSqlRaw("EXEC BaoCao_GetStats")
				.ToListAsync();

			return result.FirstOrDefault() ?? new DashboardStats();
		}

		public async Task<List<MonthlyRevenueData>> GetMonthlyRevenue(int year)
		{
			return await _context.MonthlyRevenueData
				.FromSqlRaw("EXEC BaoCao_GetMonthlyRevenue @Year", new SqlParameter("@Year", year))
				.ToListAsync();
		}

		public async Task<List<TopProductData>> GetTopSellingProducts(int limit = 10)
		{
			return await _context.TopProductData
				.FromSqlRaw("EXEC BaoCao_GetTopSellingProducts @Limit", new SqlParameter("@Limit", limit))
				.ToListAsync();
		}

		public async Task<List<TopProductData>> GetSlowMovingProducts(int limit = 10)
		{
			return await _context.TopProductData
				.FromSqlRaw("EXEC BaoCao_GetSlowMovingProducts @Limit", new SqlParameter("@Limit", limit))
				.ToListAsync();
		}

		// Lấy chi tiết hóa đơn bán hàng
		public async Task<List<OrderDetailReport>> GetOrderDetailsReport(DateTime? fromDate, DateTime? toDate)
		{
			var parameters = new[]
			{
				new SqlParameter("@FromDate", (object?)fromDate ?? DBNull.Value),
				new SqlParameter("@ToDate", (object?)toDate ?? DBNull.Value)
			};

			return await _context.OrderDetailReport
				.FromSqlRaw("EXEC BaoCao_GetOrderDetailsReport @FromDate, @ToDate", parameters)
				.ToListAsync();
		}

		public async Task<List<ImportOrderDetailReport>> GetImportOrderDetailsReport(DateTime? fromDate, DateTime? toDate)
		{
			var parameters = new[]
			{
				new SqlParameter("@FromDate", (object?)fromDate ?? DBNull.Value),
				new SqlParameter("@ToDate", (object?)toDate ?? DBNull.Value)
			};

			return await _context.ImportOrderDetailReport
				.FromSqlRaw("EXEC BaoCao_GetImportOrderDetailsReport @FromDate, @ToDate", parameters)
				.ToListAsync();
		}


		// Lấy chi tiết doanh thu theo sản phẩm
		public async Task<List<ProductRevenueReport>> GetProductRevenueReport()
		{
			return await _context.ProductRevenueReport
				.FromSqlRaw("EXEC BaoCao_GetProductRevenueReport")
				.ToListAsync();
		}
	}

	// Models
	public class DashboardStats
	{
		public int TotalProducts { get; set; }
		public int TotalCustomers { get; set; }
		public int TotalEmployees { get; set; }
		public int TotalOrders { get; set; }
		public int TotalPurchaseOrders { get; set; }
		public decimal MonthlyRevenue { get; set; }
		public decimal YearlyRevenue { get; set; }
		public int MonthlyOrders { get; set; }

		public decimal TotalRevenue { get; set; }
		public decimal TotalCost { get; set; }
		public decimal TotalProfit => TotalRevenue - TotalCost;
	}

	public class MonthlyRevenueData
	{
		public int Month { get; set; }
		public string MonthName { get; set; } = string.Empty;
		public decimal Revenue { get; set; }
		public int OrderCount { get; set; }
	}

	public class TopProductData
	{
		public string MaSP { get; set; } = string.Empty;
		public string TenSP { get; set; } = string.Empty;
		public int TotalQuantitySold { get; set; }
		public decimal TotalRevenue { get; set; }
	}

	public class OrderDetailReport
	{
		public string MaDBH { get; set; } = string.Empty;
		public DateTime NgayBH { get; set; }
		public string TenKH { get; set; } = string.Empty;
		public string DiaChiDBH { get; set; } = string.Empty;
		public string TrangThai { get; set; } = string.Empty;
		public decimal TongTien { get; set; }
		public int SoLuongSP { get; set; }
	}

	public class ImportOrderDetailReport
	{
		public string MaDMH { get; set; }
		public DateTime NgayMH { get; set; }
		public string TenNCC { get; set; }
		public int SoLuongSP { get; set; }
		public decimal TongTien { get; set; }
	}

	public class ProductRevenueReport
	{
		public string MaSP { get; set; } = string.Empty;
		public string TenSP { get; set; } = string.Empty;
		public decimal GiaBan { get; set; }
		public int SoLuongBan { get; set; }
		public decimal DoanhThu { get; set; }
		public int SoDonHang { get; set; }
	}
}
