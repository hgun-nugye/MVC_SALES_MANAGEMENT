using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class TrangThaiBHController : Controller
	{
		private readonly TrangThaiBHService _service;

		public TrangThaiBHController(TrangThaiBHService service)
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
		public async Task<IActionResult> Create(TrangThaiBH model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.Create(model);
					TempData["SuccessMessage"] = "Thêm trạng thái đơn hàng thành công!";
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
		public async Task<IActionResult> Edit(TrangThaiBH model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.Update(model);
					TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
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
				TempData["SuccessMessage"] = "Xóa trạng thái đơn hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

