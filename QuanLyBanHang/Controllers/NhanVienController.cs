using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Controllers
{
	public class NhanVienController : Controller
	{
		private readonly NhanVienService _nhanVienService;
		private readonly AppDbContext _context;

		public NhanVienController(NhanVienService nhanVienService, AppDbContext context)
		{
			_nhanVienService = nhanVienService;
			_context = context;
		}
	
		// READ - Danh sách Nhân Viên 
		public async Task<IActionResult> Index(string? search)
		{
			ViewBag.Search = search;

			var dsNhanVien = await _nhanVienService.Search(search);
			return View(dsNhanVien);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var nhanVien = (await _nhanVienService.GetByID(id));

			if (nhanVien == null)
				return NotFound();

			return View(nhanVien);
		}

		// CREATE - GET
		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.VaiTro = new SelectList(new[]
			{
				new { Value = "Quản Trị", Text = "Quản Trị" },
				new { Value = "Quản Lý", Text = "Quản Lý" },
				new { Value = "Nhân Viên", Text = "Nhân Viên" }
			}, "Value", "Text");
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(NhanVien model)
		{
			// Mã nhân viên sinh bởi DB/SP
			ModelState.Remove("MaNV");
			if (ModelState.IsValid)
			{
				try
				{
					await _nhanVienService.Create(model);

					TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
					return RedirectToAction(nameof(Index));
				}

				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "" + ex.Message;
				}
			}

			ViewBag.VaiTro = new SelectList(new[]
			{
				new { Value = "Quản Trị", Text = "Quản Trị" },
				new { Value = "Quản Lý", Text = "Quản Lý" },
				new { Value = "Nhân Viên", Text = "Nhân Viên" }
			}, "Value", "Text", model.VaiTro);
			return View(model);
		}

		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var nhanVien = (await _nhanVienService.GetByID(id));

			if (nhanVien == null)
				return NotFound();

			ViewBag.VaiTro = new SelectList(new[]
			{
				new { Value = "Quản Trị", Text = "Quản Trị" },
				new { Value = "Quản Lý", Text = "Quản Lý" },
				new { Value = "Nhân Viên", Text = "Nhân Viên" }
			}, "Value", "Text", nhanVien.VaiTro);
			return View(nhanVien);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(NhanVien model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _nhanVienService.Update(model);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");
					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				}
			}

			ViewBag.VaiTro = new SelectList(new[]
			{
				new { Value = "Quản Trị", Text = "Quản Trị" },
				new { Value = "Quản Lý", Text = "Quản Lý" },
				new { Value = "Nhân Viên", Text = "Nhân Viên" }
			}, "Value", "Text", model.VaiTro);
			return View(model);
		}

		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var nhanVien = (await _nhanVienService.GetByID(id));

			if (nhanVien == null)
				return NotFound();

			return View(nhanVien);
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

			var nhanVien = (await _nhanVienService.GetByID(id));

			if (nhanVien != null)
			{
				await _nhanVienService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa nhân viên thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy nhân viên cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

