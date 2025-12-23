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
		private readonly IWebHostEnvironment _webHostEnvironment;
		public NhanVienController(NhanVienService nhanVienService,XaService xaService, AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_nhanVienService = nhanVienService;
			_context = context;
			_xaService = xaService;
			_webHostEnvironment = webHostEnvironment;
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

			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
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
			ModelState.Remove("MatKhau");

			if (ModelState.IsValid)
			{
				try
				{
					// XỬ LÝ LƯU FILE ẢNH
					if (model.AnhFile != null)
					{
						string wwwRootPath = _webHostEnvironment.WebRootPath;
						string folderPath = Path.Combine(wwwRootPath, @"images\employees");

						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AnhFile.FileName);
						string filePath = Path.Combine(folderPath, fileName);

						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await model.AnhFile.CopyToAsync(fileStream);
						}

						model.AnhNV = fileName;
					}

					await _nhanVienService.Create(model, maVT);
					TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
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
		public async Task<IActionResult> Edit(NhanVien model, string maVT, string? newPassword)
		{
			ModelState.Remove("TenXa");
			ModelState.Remove("TenTinh");
			ModelState.Remove("AnhFile");

			var currentNV = await _nhanVienService.GetByIDEdit(model.MaNV);
			if (currentNV == null) return NotFound();

			if (model.AnhFile == null)
			{
				model.AnhNV = currentNV.AnhNV;
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (model.AnhFile != null && model.AnhFile.Length > 0)
					{
						string wwwRootPath = _webHostEnvironment.WebRootPath;
						string folderPath = Path.Combine(wwwRootPath, @"images\employees");

						if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AnhFile.FileName);
						string filePath = Path.Combine(folderPath, fileName);

						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await model.AnhFile.CopyToAsync(fileStream);
						}

						model.AnhNV = fileName; // Gán tên file mới
					}
					else
					{
						model.AnhNV = currentNV.AnhNV;
					}

					string finalPassword = string.IsNullOrEmpty(newPassword) ? currentNV.MatKhauNV : newPassword;

					await _nhanVienService.Update(model, maVT, finalPassword);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Lỗi: {ex.Message}");
				}
			}

			// Nếu có lỗi, load lại dữ liệu cho các Dropdown
			var vaiTros = await _context.VaiTro.ToListAsync();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT", maVT);

			// Logic load lại Tỉnh/Xã để tránh lỗi giao diện
			short maTinh = model.MaXa.HasValue ? await _xaService.GetByIDWithTinh(model.MaXa.Value) : (short)0;
			ViewBag.Tinh = new SelectList(_context.Tinh, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);

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

