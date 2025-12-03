using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class CTMHController : Controller
	{
		private readonly CTMHService _service;
		private readonly AppDbContext _context;

		public CTMHController(AppDbContext context)
		{
			_context = context;
			_service = new CTMHService(context);
		}

		public async Task<IActionResult> Index()
		{
			var list = await _service.GetAll();
			return View(list);
		}

		public async Task<IActionResult> Details(string maDMH, string maSP)
		{
			if (string.IsNullOrEmpty(maDMH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var data = await _service.GetDetail(maDMH, maSP);
			if (data == null) return NotFound();

			return View(data);
		}

		public async Task<IActionResult> Edit(string maDMH, string maSP)
		{
			if (string.IsNullOrEmpty(maDMH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctmh = await _service.GetByID(maDMH, maSP);
			if (ctmh == null) return NotFound();

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", ctmh.MaSP);
			return View(ctmh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CTMH model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
				return View(model);
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật chi tiết mua hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
			return View(model);
		}

		public async Task<IActionResult> Delete(string maDMH, string maSP)
		{
			if (string.IsNullOrEmpty(maDMH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctmh = await _service.GetByID(maDMH, maSP);
			if (ctmh == null) return NotFound();

			return View(ctmh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maDMH, string maSP)
		{
			try
			{
				await _service.Delete(maDMH, maSP);
				TempData["SuccessMessage"] = "Xóa chi tiết mua hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
