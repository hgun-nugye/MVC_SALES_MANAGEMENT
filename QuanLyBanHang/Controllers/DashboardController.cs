using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class DashboardController : Controller
	{
		private readonly DashboardService _dashboardService;
		private readonly ExportService _exportService;

		public DashboardController(DashboardService dashboardService, ExportService exportService)
		{
			_dashboardService = dashboardService;
			_exportService = exportService;
		}

		public async Task<IActionResult> Index()
		{
			var stats = await _dashboardService.GetDashboardStats();
			return View(stats);
		}

		// API endpoints
		[HttpGet]
		public async Task<IActionResult> GetMonthlyRevenue(int year)
		{
			var data = await _dashboardService.GetMonthlyRevenue(year);
			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetTopProducts(int limit = 10)
		{
			var data = await _dashboardService.GetTopSellingProducts(limit);
			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetSlowMovingProducts(int limit = 10)
		{
			var data = await _dashboardService.GetSlowMovingProducts(limit);
			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetOrderDetails(DateTime? fromDate, DateTime? toDate)
		{
			var data = await _dashboardService.GetOrderDetailsReport(fromDate, toDate);
			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetProductRevenue()
		{
			var data = await _dashboardService.GetProductRevenueReport();
			return Json(data);
		}

		// Export to Excel với đầy đủ báo cáo
		[HttpGet]
		public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate)
		{
			try
			{
				var stats = await _dashboardService.GetDashboardStats();
				var monthlyRevenue = await _dashboardService.GetMonthlyRevenue(DateTime.Now.Year);
				var orders = await _dashboardService.GetOrderDetailsReport(fromDate, toDate);
				var productRevenue = await _dashboardService.GetProductRevenueReport();
				var topProducts = await _dashboardService.GetTopSellingProducts(10);
				var slowProducts = await _dashboardService.GetSlowMovingProducts(10);

				var fileBytes = _exportService.ExportDashboardToExcel(
					stats,
					monthlyRevenue,
					orders,
					productRevenue,
					topProducts,
					slowProducts
				);

				var fileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
				return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi xuất file Excel: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}
	}
}
