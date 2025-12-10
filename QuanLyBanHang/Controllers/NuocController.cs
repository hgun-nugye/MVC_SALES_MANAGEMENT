using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
	public class NuocController : Controller
	{
		private readonly NuocService _nuocService;
		private readonly AppDbContext _context;

		public NuocController(NuocService nuocService, AppDbContext context)
		{
			_nuocService = nuocService;
			_context = context;
		}
	
		// READ - Danh sách Nước 
		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var dsNuoc = await _nuocService.Search(search);
			return View(dsNuoc);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var nuoc = (await _nuocService.GetByID(id));

			if (nuoc == null)
				return NotFound();

			return View(nuoc);
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
		public async Task<IActionResult> Create(Nuoc model)
		{
			// Mã nước sinh bởi DB/SP
			ModelState.Remove("MaNuoc");
			if (ModelState.IsValid)
			{
				try
				{
					await _nuocService.Create(model);

					TempData["SuccessMessage"] = "Thêm nước thành công!";
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

			var nuoc = (await _nuocService.GetByID(id));

			if (nuoc == null)
				return NotFound();

			return View(nuoc);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Nuoc model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _nuocService.Update(model);

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

			var nuoc = (await _nuocService.GetByID(id));

			if (nuoc == null)
				return NotFound();

			return View(nuoc);
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

			var nuoc = (await _nuocService.GetByID(id));

			if (nuoc != null)
			{
				await _nuocService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa nước thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy nước cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

