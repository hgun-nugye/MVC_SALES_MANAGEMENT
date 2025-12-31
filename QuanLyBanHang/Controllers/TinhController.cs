using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
	public class TinhController : Controller
	{
		private readonly TinhService _tinhService;
		private readonly AppDbContext _context;

		public TinhController(TinhService tinhService, AppDbContext context)
		{
			_tinhService = tinhService;
			_context = context;
		}
	
		// READ - Danh sách Tỉnh 
		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var dsTinh = await _tinhService.Search(search);
			return View(dsTinh);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var tinh = (await _tinhService.GetByID(id));

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// CREATE - GET
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Tinh model)
		{
			ModelState.Remove("MaTinh");

			if (ModelState.IsValid)
			{
				try
				{
					await _tinhService.Create(model);

					TempData["SuccessMessage"] = "Thêm tỉnh thành công!";
					return RedirectToAction(nameof(Index));
				}

				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "" + ex.Message;
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

			var tinh = (await _tinhService.GetByID(id));

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Tinh model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _tinhService.Update(model);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
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

		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var tinh = (await _tinhService.GetByID(id));

			if (tinh == null)
				return NotFound();

			return View(tinh);
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

			var tinh = (await _tinhService.GetByID(id));

			if (tinh != null)
			{
				await _tinhService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa tỉnh thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy tỉnh cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
