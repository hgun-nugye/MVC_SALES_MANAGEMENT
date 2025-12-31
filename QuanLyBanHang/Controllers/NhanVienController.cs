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
		private readonly TinhService _tinhService;
		private readonly VaiTroService _vaiTroService;
		private readonly PhanQuyenService _phanQuyenService;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public NhanVienController(
			NhanVienService nhanVienService,
			XaService xaService, 
			TinhService tinhService,
			VaiTroService vaiTroService,
			PhanQuyenService phanQuyenService,
			AppDbContext context, 
			IWebHostEnvironment webHostEnvironment)
		{
			_nhanVienService = nhanVienService;
			_context = context;
			_xaService = xaService;
			_tinhService = tinhService;
			_vaiTroService = vaiTroService;
			_phanQuyenService = phanQuyenService;
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
			var vaiTros = await _vaiTroService.GetAll();
			ViewBag.VaiTro = new SelectList(vaiTros, "MaVT", "TenVT");

			var tinhs = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(tinhs, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(NhanVien model, string maVT, string maTinh)
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

			var vaiTrosPost = await _vaiTroService.GetAll();
			ViewBag.VaiTro = new SelectList(vaiTrosPost, "MaVT", "TenVT", maVT);

			var allTinhs = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhs, "MaTinh", "TenTinh", maTinh);
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

			// Get MaVT from PhanQuyen - Dùng service (Proc)
			var listPQ = await _phanQuyenService.GetByNhanVien(id);
			var maVT = listPQ.FirstOrDefault()?.MaVT ?? "";

			var vaiTrosEdit = await _vaiTroService.GetAll();
			ViewBag.VaiTro = new SelectList(vaiTrosEdit, "MaVT", "TenVT", maVT);

			string? maTinh = !string.IsNullOrEmpty(nhanVien.MaXa) ? await _xaService.GetMaTinhByXa(nhanVien.MaXa) : null;

			var allTinhsEdit = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhsEdit, "MaTinh", "TenTinh", maTinh);
			var xaListEdit = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaListEdit, "MaXa", "TenXa", nhanVien.MaXa);
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

					model.MatKhauNV = currentNV.MatKhauNV;
					await _nhanVienService.Update(model, maVT, newPassword);

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Lỗi: {ex.Message}");
				}
			}

			// Nếu có lỗi, load lại dữ liệu cho các Dropdown
			var vaiTrosErr = await _vaiTroService.GetAll();
			ViewBag.VaiTro = new SelectList(vaiTrosErr, "MaVT", "TenVT", maVT);

			// Logic load lại Tỉnh/Xã để tránh lỗi giao diện
			string? maTinh = !string.IsNullOrEmpty(model.MaXa) ? await _xaService.GetMaTinhByXa(model.MaXa) : null;
			var tinhsErr = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(tinhsErr, "MaTinh", "TenTinh", maTinh);
			var xaListErr = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaListErr, "MaXa", "TenXa", model.MaXa);

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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			try
			{
				await _nhanVienService.ResetPassword(id, "123456");
				TempData["SuccessMessage"] = "Đã reset mật khẩu thành công";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi reset mật khẩu: " + ex.Message;
			}

			return RedirectToAction(nameof(Details), new { id });
		}
	}
}

