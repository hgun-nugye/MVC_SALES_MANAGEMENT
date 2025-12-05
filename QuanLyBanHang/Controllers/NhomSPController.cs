using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class NhomSPController : Controller
	{
		private readonly NhomSPService _nspService;

		public NhomSPController(NhomSPService service)
		{
			_nspService = service;
		}

		public async Task<IActionResult> Index()
		{
			var data = await _nspService.GetAll();
			return View(data);
		}

		public async Task<IActionResult> Details(string id)
		{
			var tinh = await _nspService.GetById(id);

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// CREATE GET
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// CREATE POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(NhomSP model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _nspService.Insert(model);

					TempData["SuccessMessage"] = "Thêm nhóm sản phẩm thành công!";
					return RedirectToAction(nameof(Create));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
					TempData["ErrorMessage"] = ex.Message;
				}
			}

			return View(model);
		}

		// EDIT GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var model = await _nspService.GetById(id);

			if (model == null)
				return NotFound();

			return View(model);
		}

		// EDIT POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(NhomSP model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _nspService.Update(model);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			return View(model);
		}

		// DELETE GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var model = await _nspService.GetById(id);

			if (model == null)
				return NotFound();

			return View(model);
		}

		// DELETE POST
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _nspService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa nhóm sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
