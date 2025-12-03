using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class NhaCCController : Controller
	{
		private readonly NhaCCService _nhaCCService;
		private readonly TinhService _tinhService;

		public NhaCCController(NhaCCService service, TinhService tinhService)
		{
			_nhaCCService = service;
			_tinhService = tinhService;
		}

		public async Task<IActionResult> Index(string? search, string province)
		{
			ViewBag.Search = search;
			ViewBag.SelectedProvince = province;

			var model = await _nhaCCService.GetList(search, province);

			ViewBag.Provinces = await _tinhService.GetAll();
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
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(NhaCC model)
		{
			try
			{
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
			return View(ncc);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(NhaCC model)
		{
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
			await _nhaCCService.Delete(id);
			TempData["SuccessMessage"] = "Đã xóa thành công!";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Search(string keyword, string? tinh)
		{
			var data = await _nhaCCService.Search(keyword, tinh);
			return PartialView("NhaCCTable", data);
		}

		public async Task<IActionResult> Clear()
		{
			var data = await _nhaCCService.Clear();
			return PartialView("NhaCCTable", data);
		}
	}
}
