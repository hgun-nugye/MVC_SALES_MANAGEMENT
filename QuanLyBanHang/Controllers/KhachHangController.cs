using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly AppDbContext _context;
		private readonly KhachHangService _khService;
		private readonly XaService _xaService;
		private readonly TinhService _tinhService;
		private readonly IWebHostEnvironment _environment;

		public KhachHangController(AppDbContext context, KhachHangService khService, XaService xaService, TinhService tinhService, IWebHostEnvironment environment)
		{
			_context = context;
			_khService = khService;
			_xaService = xaService;
			_tinhService = tinhService;
			_environment = environment;
		}

		public async Task<IActionResult> Index(string? search, string? tinh)
		{

			ViewBag.Search = search;
			ViewBag.Tinh = tinh;

			var tinhList = await _tinhService.GetAll();
			ViewBag.TinhList = new SelectList(tinhList, "MaTinh", "TenTinh", tinh);

			var dsKhachHang = await _khService.Search(search, tinh);
			return View(dsKhachHang);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var kh = await _khService.GetByIDWithXa(id);
			if (kh == null)
				return NotFound();

			return View(kh);
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var userId = Helpers.SessionHelper.GetUserId(HttpContext.Session);
			if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

			var kh = await _khService.GetByIDWithXa(userId);
			if (kh == null) return NotFound();

			return View(kh);
		}

		[HttpGet]
		public async Task<IActionResult> EditProfile()
		{
			var userId = Helpers.SessionHelper.GetUserId(HttpContext.Session);
			if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

			var kh = await _khService.GetByID(userId);
			if (kh == null) return NotFound();

			string? maTinh = !string.IsNullOrEmpty(kh.MaXa) ? await _xaService.GetMaTinhByXa(kh.MaXa) : null;

			var allTinhs = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhs, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", kh.MaXa);
			ViewData["MaXaSelected"] = kh.MaXa;
			return View(kh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile(KhachHang model, IFormFile? AnhFile)
		{
			var userId = Helpers.SessionHelper.GetUserId(HttpContext.Session);
			if (string.IsNullOrEmpty(userId) || model.MaKH != userId) return RedirectToAction("Login", "Home");

			// Bỏ qua kiểm tra mật khẩu
			ModelState.Remove("MatKhauKH");

			if (!ModelState.IsValid)
			{
				string? maTinh = !string.IsNullOrEmpty(model.MaXa) ? await _xaService.GetMaTinhByXa(model.MaXa) : null;
				var allTinhsErr = await _tinhService.GetAll();
				ViewBag.Tinh = new SelectList(allTinhsErr, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;
				return View(model);
			}

			try
			{
				await _khService.Update(model, AnhFile);
                
                // Cập nhật lại session nếu có đổi ảnh hoặc tên
                if (AnhFile != null)
                {
                    var updatedKh = await _khService.GetByID(userId);
                    HttpContext.Session.SetString(Helpers.SessionKeys.UserAvatar, updatedKh?.AnhKH ?? "");
                }
                HttpContext.Session.SetString(Helpers.SessionKeys.UserName, model.TenKH ?? "");

				TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
				return RedirectToAction(nameof(Profile));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				TempData["ErrorMessage"] = ex.Message;

				string? maTinh = !string.IsNullOrEmpty(model.MaXa) ? await _xaService.GetMaTinhByXa(model.MaXa) : null;
				var allTinhsFail = await _tinhService.GetAll();
				ViewBag.Tinh = new SelectList(allTinhsFail, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);

				return View(model);
			}
		}

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(Helpers.SessionHelper.GetUserId(HttpContext.Session)))
                return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = Helpers.SessionHelper.GetUserId(HttpContext.Session);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

            var kh = await _khService.GetByID(userId);
            if (kh == null) return NotFound();

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, kh.MatKhauKH))
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            try
            {
                await _khService.ResetPassword(userId, model.NewPassword);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi đổi mật khẩu: " + ex.Message;
                return View(model);
            }
        }

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var tinhsCreate = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(tinhsCreate, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			ViewData["MaXaSelected"] = null;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(KhachHang model, string maTinh, IFormFile? AnhFile)
		{
			// Mã khách hàng sinh bởi DB/SP
			ModelState.Remove("MaKH");

			var hasExistingImage = !string.IsNullOrEmpty(model.AnhKH);
			if (AnhFile != null && AnhFile.Length > 0)
			{
				if (AnhFile.Length > 5 * 1024 * 1024) // 5MB
				{
					ModelState.AddModelError("AnhFile", "Kích thước ảnh không được vượt quá 5MB!");
				}
				else
				{
					var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
					var extension = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
					if (!allowedExtensions.Contains(extension))
					{
						ModelState.AddModelError("AnhFile", "Chỉ chấp nhận file ảnh: JPG, PNG, GIF, WEBP!");
					}
				}
			}

			// Lưu file ngay khi hợp lệ để giữ lại khi reload form
			string? filePath = model.AnhKH;
				var hasFileError = ModelState.TryGetValue("AnhFile", out var fileState) && fileState.Errors.Count > 0;
			var shouldSaveFile = AnhFile != null && AnhFile.Length > 0 && !hasFileError;
			if (shouldSaveFile)
			{
				try
				{
					var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnhFile!.FileName);
					var folderPath = Path.Combine(_environment.WebRootPath, "images", "customers");
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);

					var savePath = Path.Combine(folderPath, fileName);
					using var stream = new FileStream(savePath, FileMode.Create);
					await AnhFile.CopyToAsync(stream);
					filePath = fileName;
					model.AnhKH = fileName; // giữ lại khi return View
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Lỗi khi lưu file: " + ex.Message);
				}
			}


			var allTinhsPost = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhsPost, "MaTinh", "TenTinh", maTinh);

			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
			ViewData["MaXaSelected"] = model.MaXa;
			if (!ModelState.IsValid)
			{
				// Giữ lại đường dẫn ảnh đã upload để không phải chọn lại
				model.AnhKH = filePath;
				return View(model);
			}

			try
			{
				await _khService.Create(model, model.AnhFile);

				TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(filePath))
				{
					try
					{
						var fileToDelete = Path.Combine(_environment.WebRootPath, "images", "customers",filePath);
						if (System.IO.File.Exists(fileToDelete))
							System.IO.File.Delete(fileToDelete);
					}
					catch { }
				}

				ModelState.AddModelError("", "Lỗi khi thêm khách hàng: " + ex.Message);
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			var kh = await _khService.GetByID(id);
			if (kh == null) return NotFound();

			string? maTinh = !string.IsNullOrEmpty(kh.MaXa) ? await _xaService.GetMaTinhByXa(kh.MaXa) : null;

			var allTinhsEdit = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhsEdit, "MaTinh", "TenTinh", maTinh);
			var xaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", kh.MaXa);
			ViewData["MaXaSelected"] = kh.MaXa;
			return View(kh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(KhachHang model, IFormFile? AnhFile)
		{
			// Bỏ qua kiểm tra mật khẩu vì không cập nhật mật khẩu ở view này
			ModelState.Remove("MatKhauKH");

			if (!ModelState.IsValid)
			{
				string? maTinh = !string.IsNullOrEmpty(model.MaXa) ? await _xaService.GetMaTinhByXa(model.MaXa) : null;
				var allTinhsEditPost = await _tinhService.GetAll();
				ViewBag.Tinh = new SelectList(allTinhsEditPost, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;
				return View(model);
			}

			try
			{
				await _khService.Update(model, AnhFile);
				TempData["SuccessMessage"] = "Cập nhật thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				TempData["ErrorMessage"] = ex.Message;

				string? maTinh = !string.IsNullOrEmpty(model.MaXa) ? await _xaService.GetMaTinhByXa(model.MaXa) : null;
				var allTinhsEditFail = await _tinhService.GetAll();
				ViewBag.Tinh = new SelectList(allTinhsEditFail, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);

				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			var kh = await _khService.GetByIDWithXa(id);
			if (kh == null) return NotFound();

			return View(kh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				TempData["ErrorMessage"] = "ID không hợp lệ!";
				return BadRequest();
			}

			try
			{
				await _khService.Delete(id);
				TempData["SuccessMessage"] = "Đã xóa Khách hàng thành công!";
			}
			catch (KeyNotFoundException)
			{
				TempData["ErrorMessage"] = "Không tìm thấy Khách hàng cần xóa!";
			}
			catch
			{
				TempData["ErrorMessage"] = $"Không thể xóa Khách hàng!";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> GetXaByTinh(string maTinh)
		{
			var xaList = await _xaService.GetByIDTinh(maTinh);
			return Json(xaList);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(string id)
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			try
			{
				await _khService.ResetPassword(id, "123456");
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
