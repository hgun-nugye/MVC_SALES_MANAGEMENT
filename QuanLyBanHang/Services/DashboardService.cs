using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

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
			var stats = new DashboardStats
			{
				TotalProducts = await _context.SanPham.CountAsync(),
				TotalCustomers = await _context.KhachHang.CountAsync(),
				TotalEmployees = await _context.NhanVien.CountAsync(),
				TotalOrders = await _context.DonBanHang.CountAsync(),
				TotalPurchaseOrders = await _context.DonMuaHang.CountAsync(),
				MonthlyRevenue = await CalculateMonthlyRevenue(DateTime.Now.Year, DateTime.Now.Month) ?? 0,
				YearlyRevenue = await CalculateYearlyRevenue(DateTime.Now.Year),
				MonthlyOrders = await _context.DonBanHang
					.Where(d => d.NgayBH.Year == DateTime.Now.Year && d.NgayBH.Month == DateTime.Now.Month)
					.CountAsync(),
			};

			return stats;
		}

		public async Task<List<MonthlyRevenueData>> GetMonthlyRevenue(int year)
		{
			var result = new List<MonthlyRevenueData>();

			for (int month = 1; month <= 12; month++)
			{
				var revenue = await CalculateMonthlyRevenue(year, month);
				var orderCount = await _context.DonBanHang
					.Where(d => d.NgayBH.Year == year && d.NgayBH.Month == month)
					.CountAsync();

				result.Add(new MonthlyRevenueData
				{
					Month = month,
					MonthName = new DateTime(year, month, 1).ToString("MMM"),
					Revenue = revenue ?? 0,
					OrderCount = orderCount
				});
			}

			return result;
		}

		private async Task<decimal?> CalculateMonthlyRevenue(int year, int month)
		{
			var revenue = await _context.DonBanHang
				.Where(d => d.NgayBH.Year == year && d.NgayBH.Month == month)
				.Join(_context.CTBH,
					dbh => dbh.MaDBH,
					ctbh => ctbh.MaDBH,
					(dbh, ctbh) => new { ctbh.SLB, ctbh.DGB })
				.SumAsync(x => x.SLB * x.DGB);

			return revenue;
		}

		private async Task<decimal> CalculateYearlyRevenue(int year)
		{
			var revenue = await _context.DonBanHang
				.Where(d => d.NgayBH.Year == year)
				.Join(_context.CTBH,
					dbh => dbh.MaDBH,
					ctbh => ctbh.MaDBH,
					(dbh, ctbh) => new { ctbh.SLB, ctbh.DGB })
				.SumAsync(x => x.SLB * x.DGB);

			return revenue ?? 0;
		}

		public async Task<List<TopProductData>> GetTopSellingProducts(int limit = 10)
		{
			var topProducts = await _context.CTBH
				.GroupBy(ct => ct.MaSP)
				.Select(g => new
				{
					MaSP = g.Key,
					TotalQuantity = g.Sum(x => x.SLB),
					TotalRevenue = g.Sum(x => x.SLB * x.DGB)
				})
				.OrderByDescending(x => x.TotalQuantity)
				.Take(limit)
				.Join(_context.SanPham,
					x => x.MaSP,
					sp => sp.MaSP,
					(x, sp) => new TopProductData
					{
						MaSP = x.MaSP,
						TenSP = sp.TenSP,
						TotalQuantitySold = x.TotalQuantity ?? 0,
						TotalRevenue = x.TotalRevenue ?? 0
					})
				.ToListAsync();

			return topProducts;
		}

		public async Task<List<TopProductData>> GetSlowMovingProducts(int limit = 10)
		{
			var topSellingIds = await _context.CTBH
				.GroupBy(ct => ct.MaSP)
				.Select(g => new
				{
					MaSP = g.Key,
					TotalQuantity = g.Sum(x => x.SLB)
				})
				.OrderByDescending(x => x.TotalQuantity)
				.Take(limit)
				.Select(x => x.MaSP)
				.ToListAsync();

			var allProducts = await _context.SanPham
				.Select(sp => new { sp.MaSP, sp.TenSP })
				.ToListAsync();

			var soldProducts = await _context.CTBH
				.GroupBy(ct => ct.MaSP)
				.Select(g => new
				{
					MaSP = g.Key,
					TotalQuantity = g.Sum(x => x.SLB),
					TotalRevenue = g.Sum(x => x.SLB * x.DGB)
				})
				.ToListAsync();

			var slowMoving = allProducts
				.Where(ap => !topSellingIds.Contains(ap.MaSP))
				.GroupJoin(soldProducts,
					ap => ap.MaSP,
					sp => sp.MaSP,
					(ap, sp) => new TopProductData
					{
						MaSP = ap.MaSP,
						TenSP = ap.TenSP,
						TotalQuantitySold = sp.FirstOrDefault()?.TotalQuantity ?? 0,
						TotalRevenue = sp.FirstOrDefault()?.TotalRevenue ?? 0
					})
				.OrderBy(x => x.TotalQuantitySold)
				.Take(limit)
				.ToList();

			return slowMoving;
		}

		// Lấy chi tiết hóa đơn bán hàng
		public async Task<List<OrderDetailReport>> GetOrderDetailsReport(DateTime? fromDate, DateTime? toDate)
		{
			var query = _context.DonBanHang.AsQueryable();

			if (fromDate.HasValue)
				query = query.Where(d => d.NgayBH >= fromDate.Value);

			if (toDate.HasValue)
				query = query.Where(d => d.NgayBH <= toDate.Value);

			var orders = await query
				.Join(_context.KhachHang,
					dbh => dbh.MaKH,
					kh => kh.MaKH,
					(dbh, kh) => new { dbh, kh })
				.Join(_context.TrangThaiBH,
					x => x.dbh.MaTTBH,
					tt => tt.MaTTBH,
					(x, tt) => new { x.dbh, x.kh, tt })
				.GroupJoin(_context.CTBH,
					x => x.dbh.MaDBH,
					ct => ct.MaDBH,
					(x, cts) => new OrderDetailReport
					{
						MaDBH = x.dbh.MaDBH,
						NgayBH = x.dbh.NgayBH,
						TenKH = x.kh.TenKH,
						DiaChiDBH = x.dbh.DiaChiDBH,
						TrangThai = x.tt.TenTTBH,
						TongTien = cts.Sum(ct => ct.SLB * ct.DGB) ?? 0,
						SoLuongSP = cts.Count()
					})
				.OrderByDescending(x => x.NgayBH)
				.ToListAsync();

			return orders;
		}

		// Lấy chi tiết doanh thu theo sản phẩm
		public async Task<List<ProductRevenueReport>> GetProductRevenueReport()
		{
			var report = await _context.CTBH
				.GroupBy(ct => ct.MaSP)
				.Select(g => new
				{
					MaSP = g.Key,
					TotalQuantity = g.Sum(x => x.SLB),
					TotalRevenue = g.Sum(x => x.SLB * x.DGB),
					OrderCount = g.Select(x => x.MaDBH).Distinct().Count()
				})
				.Join(_context.SanPham,
					x => x.MaSP,
					sp => sp.MaSP,
					(x, sp) => new ProductRevenueReport
					{
						MaSP = x.MaSP,
						TenSP = sp.TenSP,
						GiaBan = sp.GiaBan,
						SoLuongBan = x.TotalQuantity ?? 0,
						DoanhThu = x.TotalRevenue ?? 0,
						SoDonHang = x.OrderCount
					})
				.OrderByDescending(x => x.DoanhThu)
				.ToListAsync();

			return report;
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
		public int NewCustomersThisMonth { get; set; }
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
