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
		private readonly XaService _xaService;
		private readonly AppDbContext _context;

		public NhanVienController(NhanVienService nhanVienService,XaService xaService, AppDbContext context)
		{
			_nhanVienService = nhanVienService;
			_context = context;
			_xaService = xaService;
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
		public async Task<IActionResult> Create()
		{
			var vaiTros = await _context.VaiTro.ToListAsync();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT");
			
			var xas = await (from x in _context.Xa
							join t in _context.Tinh on x.MaTinh equals t.MaTinh
							select new { x.MaXa, TenXa = x.TenXa + ", " + t.TenTinh })
							.ToListAsync();
			ViewBag.Xa = new SelectList(xas, "MaXa", "TenXa");
			
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(NhanVien model, string maVT, short maTinh)
		{
			// Mã nhân viên sinh bởi DB/SP
			ModelState.Remove("MaNV");
			ModelState.Remove("TenXa");
			ModelState.Remove("TenTinh");
			ModelState.Remove("AnhFile");
			
			if (ModelState.IsValid)
			{
				try
				{
					await _nhanVienService.Create(model, maVT);

					TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
					return RedirectToAction(nameof(Index));
				}

				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "" + ex.Message;
				}
			}

			var vaiTros = await _context.VaiTro.ToListAsync();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT", maVT);

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
			ViewData["MaXaSelected"] = model.MaXa;

			return View(model);
		}

		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var nhanVien = (await _nhanVienService.GetByIDEdit(id));

			if (nhanVien == null)
				return NotFound();

			// Get MaVT from PhanQuyen
			var phanQuyen = await _context.PhanQuyen
				.Where(pq => pq.MaNV == id)
				.FirstOrDefaultAsync();
			var maVT = phanQuyen?.MaVT ?? "";

			var vaiTros = await _context.VaiTro.ToListAsync();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT", maVT);

			short maTinh = nhanVien.MaXa.HasValue ? await _xaService.GetByIDWithTinh(nhanVien.MaXa.Value) : (short)0;

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", nhanVien.MaXa);
			ViewData["MaXaSelected"] = nhanVien.MaXa;

			return View(nhanVien);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(NhanVien model, string maVT)
		{
			ModelState.Remove("TenXa");
			ModelState.Remove("TenTinh");
			ModelState.Remove("AnhFile");
			
			if (ModelState.IsValid)
			{
				try
				{
					await _nhanVienService.Update(model, maVT);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");
					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				}
			}

			var vaiTros = await _context.VaiTro.ToListAsync();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT", maVT);
			
			var xas = await (from x in _context.Xa
							join t in _context.Tinh on x.MaTinh equals t.MaTinh
							select new { x.MaXa, TenXa = x.TenXa + ", " + t.TenTinh })
							.ToListAsync();
			ViewBag.Xa = new SelectList(xas, "MaXa", "TenXa", model.MaXa);
			
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

