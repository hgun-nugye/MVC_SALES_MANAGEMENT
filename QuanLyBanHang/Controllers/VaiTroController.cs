using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class VaiTroController : Controller
	{
		private readonly VaiTroService _service;

		public VaiTroController(VaiTroService service)
		{
			_service = service;
		}

		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var data = await _service.Search(search);
			return View(data);
		}

		public async Task<IActionResult> Details(string id)
		{
			var model = await _service.GetById(id);

			if (model == null)
				return NotFound();

			return View(model);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(VaiTro model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.Create(model);
					TempData["SuccessMessage"] = "Thêm vai trò thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
					TempData["ErrorMessage"] = ex.Message;
				}
			}

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				return BadRequest();

			var model = await _service.GetById(id);

			if (model == null)
				return NotFound();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(VaiTro model)
		{
			if (model == null)
			{
				return BadRequest();
			}

			// Đảm bảo MaVT không bị thay đổi
			if (string.IsNullOrWhiteSpace(model.MaVT))
			{
				ModelState.AddModelError("MaVT", "Mã vai trò không được để trống.");
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật vai trò thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var model = await _service.GetById(id);

			if (model == null)
				return NotFound();

			return View(model);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _service.Delete(id);
				TempData["SuccessMessage"] = "Xóa vai trò thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

