using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Data;

namespace QuanLyBanHang.Controllers
{
	public class DonBanHangController : Controller
	{
		private readonly DonBanHangService _dbhService;
		private readonly AppDbContext _context;

		public DonBanHangController(DonBanHangService service, AppDbContext context)
		{
			_dbhService = service;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, int? month, int? year)
		{
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;

			var model = await _dbhService.Search(search, month, year);
			return View(model);
		}


		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var result = await _dbhService.GetByID(id);
			return View(result);
		}

		public IActionResult Create()
		{
			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH");
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");

			var model = new DonBanHang
			{
				NgayBH = DateTime.Today,
				CTBHs = new List<CTBH>
				{
					new CTBH()
				}
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonBanHang model)
		{
			try
			{
				if (ModelState.IsValid && model.CTBHs != null && model.CTBHs.Any())
				{
					await _dbhService.Create(model);

					TempData["SuccessMessage"] = $"Thêm đơn bán hàng thành công!";
					return RedirectToAction(nameof(Index));
				}

				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn hàng và chi tiết sản phẩm.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi thêm đơn bán hàng: " + ex.Message;
			}

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");
			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var result = await _dbhService.GetByID(id);
			if (result == null) return NotFound();

			var ct = new DonBanHangEditCTBH
			{
				MaDBH = result.MaDBH!,
				NgayBH = result.NgayBH,
				MaKH = result.MaKH!,
				ChiTiet = result.CTBHs!	
			};

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", ct.MaKH);

			return View(ct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonBanHangEditCTBH model)
		{
			if (!ModelState.IsValid) return View(model);
			try
			{

				await _dbhService.Update(model);

				//// Update each product
				//foreach (var ct in model.ChiTiet)
				//{
				//	await _context.Database.ExecuteSqlRawAsync(
				//		"EXEC CTBH_Update @MaDBH, @MaSP, @SLB, @DGB",
				//		new SqlParameter("@MaDBH", model.MaDBH),
				//		new SqlParameter("@MaSP", ct.MaSP),
				//		new SqlParameter("@SLB", ct.SLB),
				//		new SqlParameter("@DGB", ct.DGB)
				//	);
				//}

				TempData["SuccessMessage"] = "Cập nhật đơn bán hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaKH = new SelectList(_context.KhachHang, "MaKH", "TenKH", model.MaKH);
			return View(model);

		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();


			var dbh = await _dbhService.GetByID(id);
			if (dbh == null) return NotFound();

			return View(dbh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dbhService.Delete(id);
				TempData["SuccessMessage"] = "Xóa đơn bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string maDBH, string maSP)
		{
			var model = await _dbhService.GetDetail(maDBH, maSP);
			if (model == null) return NotFound();
			return View(model);
			//return View("DeleteDetail", ctbh);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string maDBH, string maSP)
		{
			try
			{
				await _dbhService.DeleteDetail(maDBH, maSP);

				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex) {
				TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
			}

			return RedirectToAction("Details", new { id = maDBH });
		}

		[HttpGet]
		public async Task<IActionResult> Search(string keyword, int? month, int? year)
		{
			var data= await _dbhService.Search(keyword, month, year);
			return PartialView("DonBanHangTable", data);
		}


		// ============ RESET  ============
		public async Task<IActionResult> Clear()
		{
			var data = await _dbhService.Reset();

			return PartialView("DonBanHangTable", data);
		}
	}
}


