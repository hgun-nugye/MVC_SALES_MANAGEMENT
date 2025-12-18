using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class PhanQuyenController : Controller
	{
		private readonly PhanQuyenService _service;
		private readonly VaiTroService _vaiTroService;
		private readonly NhanVienService _nhanVienService;
		private readonly AppDbContext _context;

		public PhanQuyenController(
			PhanQuyenService service,
			VaiTroService vaiTroService,
			NhanVienService nhanVienService,
			AppDbContext context)
		{
			_service = service;
			_vaiTroService = vaiTroService;
			_nhanVienService = nhanVienService;
			_context = context;
		}

		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var data = await _service.Search(search);
			return View(data);
		}

		public async Task<IActionResult> Details(string maVT, string maNV)
		{
			var model = await _service.GetById(maVT, maNV);

			if (model == null)
				return NotFound();

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var vaiTroList = await _vaiTroService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();

			ViewBag.MaVT = new SelectList(vaiTroList, "MaVT", "TenVT");
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PhanQuyen model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _service.Create(model);
					TempData["SuccessMessage"] = "Thêm phân quyền thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
					TempData["ErrorMessage"] = ex.Message;
				}
			}

			var vaiTroList = await _vaiTroService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();

			ViewBag.MaVT = new SelectList(vaiTroList, "MaVT", "TenVT", model.MaVT);
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string maVT, string maNV)
		{
			if (string.IsNullOrWhiteSpace(maVT) || string.IsNullOrWhiteSpace(maNV))
				return BadRequest();

			var model = await _service.GetById(maVT, maNV);

			if (model == null)
				return NotFound();

			var vaiTroList = await _vaiTroService.GetAll();
			var nhanVienList = await _nhanVienService.GetAll();

			ViewBag.MaVT = new SelectList(vaiTroList, "MaVT", "TenVT", model.MaVT);
			ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);
			ViewBag.OldMaVT = model.MaVT;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(PhanQuyen model, string oldMaVT)
		{
			if (model == null)
			{
				return BadRequest();
			}

			// Validate các trường bắt buộc
			if (string.IsNullOrWhiteSpace(model.MaVT))
			{
				ModelState.AddModelError("MaVT", "Mã vai trò không được để trống.");
			}

			if (string.IsNullOrWhiteSpace(model.MaNV))
			{
				ModelState.AddModelError("MaNV", "Mã nhân viên không được để trống.");
			}

			if (string.IsNullOrWhiteSpace(oldMaVT))
			{
				ModelState.AddModelError(string.Empty, "Thông tin phân quyền cũ không hợp lệ.");
			}

			if (!ModelState.IsValid)
			{
				var vaiTroList = await _vaiTroService.GetAll();
				var nhanVienList = await _nhanVienService.GetAll();

				ViewBag.MaVT = new SelectList(vaiTroList, "MaVT", "TenVT", model.MaVT);
				ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);
				ViewBag.OldMaVT = oldMaVT ?? model.MaVT;

				return View(model);
			}

			try
			{
				await _service.Update(model.MaNV, oldMaVT, model.MaVT);
				TempData["SuccessMessage"] = "Cập nhật phân quyền thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				TempData["ErrorMessage"] = ex.Message;

				var vaiTroList = await _vaiTroService.GetAll();
				var nhanVienList = await _nhanVienService.GetAll();

				ViewBag.MaVT = new SelectList(vaiTroList, "MaVT", "TenVT", model.MaVT);
				ViewBag.MaNV = new SelectList(nhanVienList, "MaNV", "TenNV", model.MaNV);
				ViewBag.OldMaVT = oldMaVT ?? model.MaVT;

				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string maVT, string maNV)
		{
			var model = await _service.GetById(maVT, maNV);

			if (model == null)
				return NotFound();

			return View(model);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maVT, string maNV)
		{
			try
			{
				await _service.Delete(maVT, maNV);
				TempData["SuccessMessage"] = "Xóa phân quyền thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

