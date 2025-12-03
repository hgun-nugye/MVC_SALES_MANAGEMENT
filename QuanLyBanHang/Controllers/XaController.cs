using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
	public class XaController : Controller
	{
		private readonly AppDbContext _context;
		private readonly XaService _xaService;

		public XaController(AppDbContext context, XaService xaService)
		{
			_context = context;
			_xaService = xaService;
		}

		
		//READ - Danh sách Xã
		public async Task<IActionResult> Index()
		{
			var dsXa = await _xaService.GetAllWithTinh();

			return View(dsXa);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var xa = (await _xaService.GetByIDWithTinh(id));

			if (xa == null)
				return NotFound();

			return View(xa);
		}

		// CREATE - GET
		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.MaTinhList = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Xa model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _xaService.Create(model);

					TempData["SuccessMessage"] = "Thêm xã thành công!";
					return RedirectToAction(nameof(Index));
				}
				
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;

				}
			}

			return View(model);
		}


		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var xa = (await _xaService.GetByIDWithTinh(id));

			if (xa == null)
				return NotFound();

			ViewBag.MaTinhList = new SelectList(_context.Tinh, "MaTinh", "TenTinh");

			return View(xa);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Xa model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _xaService.Update(model);

					TempData["SuccessMessage"] = "Cập nhật xã thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;

				}
			}

			ViewBag.MaTinhList = new SelectList(_context.Tinh, "MaTinh", "TenTinh", model.MaTinh);

			return View(model);
		}


		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var xa = (await _xaService.GetByIDWithTinh(id));

			if (xa == null)
				return NotFound();

			return View(xa);
		}

		// DELETE - POST
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				TempData["ErrorMessage"] = "ID không hợp lệ!";
				return BadRequest();
			}

			var xa = (await _xaService.GetByIDWithTinh(id));

			if (xa != null)
			{
				await _xaService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa xã thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy xã cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
