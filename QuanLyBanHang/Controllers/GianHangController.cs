using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class GianHangController : Controller
	{
		private readonly AppDbContext _context;
		private readonly GianHangService _gianHangService;
		private readonly XaService _xaService;
		private readonly TinhService _tinhService;

		public GianHangController(AppDbContext context, GianHangService gianHangService, XaService xaService, TinhService tinhService)
		{
			_context = context;
			_gianHangService = gianHangService;
			_xaService = xaService;
			_tinhService = tinhService;
		}

		public async Task<IActionResult> Index(string? search, short? tinh)
		{
			ViewBag.Search = search;
			ViewBag.SelectedTinh = tinh;

			var model = await _gianHangService.Search(search, tinh);

			var tinhList = await _tinhService.GetAll();
			ViewBag.TinhList = new SelectList(tinhList, "MaTinh", "TenTinh", tinh);

			return View(model);
		}


		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = await _gianHangService.GetById(id);
			if (gh == null)
				return NotFound();

			return View(gh);
		}

		public IActionResult Create()
		{
			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(GianHang model, short maTinh)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Tinh = new SelectList(await _tinhService.GetAll(), "MaTinh", "TenTinh", maTinh);
				ViewBag.Xa = new SelectList(await _xaService.GetByIDTinh(maTinh), "MaXa", "TenXa", model.MaXa);

				TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";

				return View(model);
			}

			try
			{
				ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;

				await _gianHangService.Insert(model);
				TempData["SuccessMessage"] = "Thêm gian hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}

		// EDIT (GET)
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = await _gianHangService.GetById(id);
			if (gh == null)
				return NotFound();

			short maTinh = await _xaService.GetByIDWithTinh(gh.MaXa);

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", gh.MaXa);
			ViewData["MaXaSelected"] = gh.MaXa;
			return View(gh);
		}

		// EDIT (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(GianHang model)
		{
			if (!ModelState.IsValid)
			{
				short maTinh = await _xaService.GetByIDWithTinh(model.MaXa);
				ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;

				TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";
				return View(model);
			}

			try
			{
				await _gianHangService.Update(model);
				TempData["SuccessMessage"] = "Cập nhật gian hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}

		// DELETE (GET)
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = await _gianHangService.GetById(id);
			if (gh == null)
				return NotFound();

			return View(gh);
		}

		// DELETE (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _gianHangService.Delete(id);
				TempData["SuccessMessage"] = "Xóa gian hàng thành công!";
			}
			catch
			{
				TempData["ErrorMessage"] = "Không thể xóa gian hàng!";
			}

			return RedirectToAction(nameof(Index));
		}

	}
}
