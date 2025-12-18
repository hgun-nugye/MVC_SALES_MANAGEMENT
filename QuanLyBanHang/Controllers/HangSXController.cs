using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
	public class HangSXController : Controller
	{
		private readonly HangSXService _hangService;
		private readonly NuocService _nuocService;
		private readonly AppDbContext _context;

		public HangSXController(HangSXService hangService, NuocService nuocService, AppDbContext context)
		{
			_hangService = hangService;
			_nuocService = nuocService;
			_context = context;
		}
	
		// READ - Danh sách Hãng 
		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var dsHang = await _hangService.Search(search);
			return View(dsHang);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var hang = (await _hangService.GetByID(id));

			if (hang == null)
				return NotFound();

			return View(hang);
		}

		// CREATE - GET
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var nuocList = await _nuocService.GetAll();
			ViewBag.MaNuoc = new SelectList(nuocList, "MaNuoc", "TenNuoc");
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(HangSX model)
		{
			// Mã hãng được sinh ở DB/SP
			ModelState.Remove("MaHangSX");
			if (ModelState.IsValid)
			{
				try
				{
					await _hangService.Create(model);

					TempData["SuccessMessage"] = "Thêm hãng thành công!";
					return RedirectToAction(nameof(Index));
				}

				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "" + ex.Message;
				}
			}

			var nuocList = await _nuocService.GetAll();
			ViewBag.MaNuoc = new SelectList(nuocList, "MaNuoc", "TenNuoc", model.MaNuoc);
			return View(model);
		}

		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var hang = (await _hangService.GetByID(id));

			if (hang == null)
				return NotFound();

			var nuocList = await _nuocService.GetAll();
			ViewBag.MaNuoc = new SelectList(nuocList, "MaNuoc", "TenNuoc", hang.MaNuoc);
			return View(hang);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(HangSX model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _hangService.Update(model);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");
					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				}
			}

			var nuocList = await _nuocService.GetAll();
			ViewBag.MaNuoc = new SelectList(nuocList, "MaNuoc", "TenNuoc", model.MaNuoc);
			return View(model);
		}

		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var hang = (await _hangService.GetByID(id));

			if (hang == null)
				return NotFound();

			return View(hang);
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

			var hang = (await _hangService.GetByID(id));

			if (hang != null)
			{
				await _hangService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa hãng thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy hãng cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

