using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly AppDbContext _context;
		private readonly KhachHangService _khService;
		private readonly XaService _xaService;
		private readonly TinhService _tinhService;

		public KhachHangController(AppDbContext context, KhachHangService khService, XaService xaService, TinhService tinhService)
		{
			_context = context;
			_khService = khService;
			_xaService = xaService;
			_tinhService = tinhService;
		}

		public async Task<IActionResult> Index(string? search, string? tinh)
		{

			ViewBag.Search = search;
			ViewBag.Tinh = tinh;

			var tinhList = await _tinhService.GetAll();
			ViewBag.TinhList = new SelectList(tinhList, "MaTinh", "TenTinh", tinh);

			var dsKhachHang = await _khService.Search(search, tinh);
			return View(dsKhachHang);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var kh = await _khService.GetByIDWithXa(id);
			if (kh == null)
				return NotFound();

			return View(kh);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(KhachHang model, short maTinh)
		{
			try
			{
				ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;

				if (model.AnhFile == null)
				{
					TempData["ErrorMessage"] = "Vui lòng chọn ảnh minh họa!";
					return View(model);
				}

				await _khService.Create(model, model.AnhFile);

				TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				ModelState.AddModelError("", ex.Message);

				// Nếu lỗi validation hoặc lỗi khác
				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;

				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			var kh = await _khService.GetByID(id);
			if (kh == null) return NotFound();

			short maTinh = await _xaService.GetByIDWithTinh(kh.MaXa);

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", kh.MaXa);
			ViewData["MaXaSelected"] = kh.MaXa;
			return View(kh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(KhachHang model, IFormFile? AnhFile)
		{
			if (!ModelState.IsValid)
			{
				short maTinh = await _xaService.GetByIDWithTinh(model.MaXa);
				ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;
				return View(model);
			}

			try
			{
				await _khService.Update(model, AnhFile);
				TempData["SuccessMessage"] = "Cập nhật thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				TempData["ErrorMessage"] = ex.Message;

				short maTinh = await _xaService.GetByIDWithTinh(model.MaXa);
				ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);

				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			var kh = await _khService.GetByIDWithXa(id);
			if (kh == null) return NotFound();

			return View(kh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				TempData["ErrorMessage"] = "ID không hợp lệ!";
				return BadRequest();
			}

			try
			{
				await _khService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa Khách hàng thành công!";
			}
			catch (KeyNotFoundException)
			{
				TempData["ErrorMessage"] = "Không tìm thấy Khách hàng cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> GetXaByTinh(short maTinh)
		{
			var xaList = await _xaService.GetByIDTinh(maTinh);
			return Json(xaList);
		}


	}
}
