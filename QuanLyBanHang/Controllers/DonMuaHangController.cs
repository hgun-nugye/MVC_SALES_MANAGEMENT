using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class DonMuaHangController : Controller
	{
		private readonly DonMuaHangService _dmhService;
		private readonly AppDbContext _context;

		public DonMuaHangController(DonMuaHangService service, AppDbContext context)
		{
			_dmhService = service;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, int? month, int? year)
		{
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;

			var model = await _dmhService.Search(search, month, year);

			return View(model);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var result = await _dmhService.GetById(id);
			return View(result);
		}

		public IActionResult Create()
		{
			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC");
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");

			var model = new DonMuaHang
			{
				NgayMH = DateTime.Today,
				CTMHs = new List<CTMH> { new CTMH() }
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonMuaHang model)
		{
			if (ModelState.IsValid && model.CTMHs != null && model.CTMHs.Any())
			{
				try
				{
					await _dmhService.Create(model);
					TempData["SuccessMessage"] = "Thêm đơn mua hàng thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = ex.Message;
				}
			}
			else
			{
				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn hàng và chi tiết sản phẩm.";
			}

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", model.MaNCC);
			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");
			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var data = await _dmhService.GetById(id);
			if (data == null) return NotFound();

			var ct = new DonMuaHangEditCTMH
			{
				MaDMH = data.MaDMH!,
				NgayMH = data.NgayMH,
				MaNCC = data.MaNCC!,
				ChiTiet = data.CTMHs
			};

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", ct.MaNCC);
			return View(ct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonMuaHangEditCTMH model)
		{
			if (!ModelState.IsValid) return View(model);

			try
			{
				await _dmhService.Update(model);
				TempData["SuccessMessage"] = "Cập nhật đơn mua hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaNCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", model.MaNCC);
			return View(model);
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			var data = await _dmhService.GetById(id);
			if (data == null) return NotFound();

			return View(data);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dmhService.Delete(id);
				TempData["SuccessMessage"] = "Xóa đơn mua hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		
		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string MaDMH, string maSP)
		{
			var model = await _dmhService.GetDetail(MaDMH, maSP);
			if (model == null) return NotFound();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string MaDMH, string maSP)
		{
			try
			{
				await _dmhService.DeleteDetail(MaDMH, maSP);
				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("Details", new { id = MaDMH });
		}

		
		[HttpGet]
		public async Task<IActionResult> Search(string? search, int? month, int? year)
		{
			var data = await _dmhService.Search(search, month, year);
			return PartialView("DonMuaHangTable", data);
		}

		public async Task<IActionResult> Clear()
		{
			var data = await _dmhService.Reset();
			return PartialView("DonMuaHangTable", data);
		}
	}
}
