using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class NhaCCController : Controller
	{
		private readonly NhaCCService _nhaCCService;
		private readonly TinhService _tinhService; 
		private readonly XaService _xaService; 
		private readonly AppDbContext _context;

		public NhaCCController(NhaCCService service, TinhService tinhService, XaService xaService, AppDbContext context)
		{
			_nhaCCService = service;
			_tinhService = tinhService;
			_xaService = xaService;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, short? tinh)
		{
			ViewBag.Search = search;
			ViewBag.Tinh = tinh;

			var model = await _nhaCCService.Search(search, tinh);

			var tinhList = await _tinhService.GetAll();
			ViewBag.TinhList = new SelectList(tinhList, "MaTinh", "TenTinh", tinh);

			return View(model);
		}

		public async Task<IActionResult> Details(string id)
		{
			var ncc = await _nhaCCService.GetById(id);
			if (ncc == null) return NotFound();
			return View(ncc);
		}

		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(NhaCC model, short maTinh)
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

				await _nhaCCService.Create(model);
				TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var ncc = await _nhaCCService.GetById(id);
			if (ncc == null) return NotFound();

			short maTinh = await _xaService.GetByIDWithTinh(ncc.MaXa);

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", ncc.MaXa);
			ViewData["MaXaSelected"] = ncc.MaXa;

			return View(ncc);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(NhaCC model)
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
				await _nhaCCService.Update(model);
				TempData["SuccessMessage"] = "Cập nhật thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var ncc = await _nhaCCService.GetById(id);
			if (ncc == null) return NotFound();
			return View(ncc);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _nhaCCService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa thành công!";
			}
			catch (KeyNotFoundException)
			{
				TempData["ErrorMessage"] = "Không tìm thấy Nhà cung cấp cần xóa!";
			}
			catch
			{
				TempData["ErrorMessage"] = $"Không thể xóa Nhà cung cấp!";
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
