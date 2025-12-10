using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class CTBHController : Controller
	{
		private readonly CTBHService _service;
		private readonly AppDbContext _context;

		public CTBHController(AppDbContext context)
		{
			_context = context;
			_service = new CTBHService(context);
		}

		public async Task<IActionResult> Index()
		{
			var list = await _service.GetAll();
			return View(list);
		}

		public async Task<IActionResult> Details(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var data = await _service.GetByID(maDBH, maSP);
			if (data == null) return NotFound();

			return View(data);
		}

		//EDIT
		public async Task<IActionResult> Edit(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctbh = await _service.GetDetail(maDBH, maSP);
			if (ctbh == null) return NotFound();

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", ctbh.MaSP);
			return View(ctbh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CTBH model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
				return View(model);
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật chi tiết bán hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
			return View(model);
		}

		public async Task<IActionResult> Delete(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctbh = await _service.GetByID(maDBH, maSP);
			if (ctbh == null) return NotFound();

			return View(ctbh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maDBH, string maSP)
		{
			try
			{
				await _service.Delete(maDBH, maSP);
				TempData["SuccessMessage"] = "Xóa chi tiết bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
