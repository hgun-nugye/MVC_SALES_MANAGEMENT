using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class TrangThaiMHController : Controller
	{
		private readonly TrangThaiMHService _service;

		public TrangThaiMHController(TrangThaiMHService service)
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
		public async Task<IActionResult> Create(TrangThaiMH model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.Create(model);
					TempData["SuccessMessage"] = "Thêm trạng thái mua hàng thành công!";
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
		public async Task<IActionResult> Edit(TrangThaiMH model)
		{
			if (model == null)
			{
				return BadRequest();
			}

			// Đảm bảo MaTTMH không bị thay đổi
			if (string.IsNullOrWhiteSpace(model.MaTTMH))
			{
				ModelState.AddModelError("MaTTMH", "Mã trạng thái mua hàng không được để trống.");
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật trạng thái mua hàng thành công!";
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
				TempData["SuccessMessage"] = "Xóa trạng thái mua hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

