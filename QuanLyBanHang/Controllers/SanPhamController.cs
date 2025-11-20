using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class SanPhamController : Controller
	{
		private readonly AppDbContext _context;

		public SanPhamController(AppDbContext context)
		{
			_context = context;
		}

		// ===============================
		// DANH SÁCH SẢN PHẨM
		// ===============================
		public async Task<IActionResult> Index()
		{
			var ds = await _context.SanPham
				.Join(
					_context.LoaiSP,
					sp => sp.MaLoai,
					lsp => lsp.MaLoai,
					(sp, lsp) => new { sp, lsp }
				)
				.Join(
					_context.GianHang,
					temp => temp.sp.MaGH,
					gh => gh.MaGH,
					(temp, gh) => new SanPham
					{
						MaSP = temp.sp.MaSP,
						TenSP = temp.sp.TenSP,
						DonGia = temp.sp.DonGia,
						GiaBan = temp.sp.GiaBan,
						MoTaSP = temp.sp.MoTaSP,
						AnhMH = temp.sp.AnhMH,
						MaLoai = temp.sp.MaLoai,
						TenLoai = temp.lsp.TenLSP,
						MaGH = temp.sp.MaGH,
						TenGH = gh.TenGH,
						TrangThai = temp.sp.TrangThai,
						SoLuongTon = temp.sp.SoLuongTon,
						MaNCC = temp.sp.MaNCC
					}
				)
				.ToListAsync();

			return View(ds);
		}

		// ===============================
		// CHI TIẾT SẢN PHẨM
		// ===============================
		public async Task<IActionResult> Details(string id)
		{
			if (id == null) return NotFound();

			var sp = (await _context.SanPham
			.FromSqlRaw("EXEC SanPham_GetByID_Detail @MaSP", new SqlParameter("@MaSP", id))
			.ToListAsync())
			.FirstOrDefault();

			return View(sp);
		}

		// ===============================
		// CREATE - GET
		// ===============================
		public IActionResult Create()
		{
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP");
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH");
			return View();
		}

		// ===============================
		// CREATE - POST
		// ===============================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SanPham sp)
		{
			try
			{
				// Kiểm tra file ảnh
				if (sp.AnhFile == null || sp.AnhFile.Length == 0)
				{
					TempData["ErrorMessage"] = "Vui lòng chọn ảnh minh họa!";
					ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP", sp.MaLoai);
					ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
					return View(sp);
				}

				// Kiểm tra các thông tin còn lại
				if (!ModelState.IsValid)
				{
					TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin hợp lệ!";
					ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP", sp.MaLoai);
					ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
					return View(sp);
				}

				// Upload file
				var fileName = Guid.NewGuid() + Path.GetExtension(sp.AnhFile.FileName); // tạo tên duy nhất
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
				if (!Directory.Exists(folderPath))
					Directory.CreateDirectory(folderPath);

				var savePath = Path.Combine(folderPath, fileName);
				using (var stream = new FileStream(savePath, FileMode.Create))
				{
					await sp.AnhFile.CopyToAsync(stream);
				}
				sp.AnhMH = fileName;

				// Gọi procedure SQL để insert
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC SanPham_Insert @TenSP, @DonGia, @MoTaSP, @AnhMH, @MaLoai, @MaGH",
					new SqlParameter("@TenSP", sp.TenSP),
					new SqlParameter("@DonGia", sp.DonGia),
					new SqlParameter("@MoTaSP", sp.MoTaSP),
					new SqlParameter("@AnhMH", sp.AnhMH),
					new SqlParameter("@MaLoai", sp.MaLoai ?? (object)DBNull.Value),
					new SqlParameter("@MaGH", sp.MaGH)
				);

				TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (SqlException ex)
			{
				TempData["ErrorMessage"] = "Lỗi từ SQL: " + ex.Message;
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
			}

			// 5️⃣ Nếu có lỗi, load lại dropdown và trả view
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP", sp.MaLoai);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
			return View(sp);
		}


		// ===============================
		// EDIT - GET
		// ===============================
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var spList = await _context.SanPham
				.FromSqlRaw("EXEC SanPham_GetByID_Detail @MaSP", new SqlParameter("@MaSP", id)).ToListAsync();

			var sp = spList.FirstOrDefault();

			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP", sp.MaLoai);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);

			return View(sp);
		}

		// ===============================
		// EDIT - POST
		// ===============================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, SanPham sp, IFormFile? AnhFile)
		{
			if (id != sp.MaSP) return NotFound();

			try
			{
				if (ModelState.IsValid)
				{
					// Lấy sản phẩm cũ từ DB để giữ ảnh nếu không chọn file mới
					var oldSP = await _context.SanPham
						.AsNoTracking()
						.FirstOrDefaultAsync(x => x.MaSP == sp.MaSP);

					if (oldSP == null) return NotFound();

					string? anhPath;

					if (AnhFile != null && AnhFile.Length > 0)
					{
						// Lưu file mới
						var fileName = Guid.NewGuid() + Path.GetExtension(AnhFile.FileName);
						var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
						using (var stream = new FileStream(savePath, FileMode.Create))
						{
							await AnhFile.CopyToAsync(stream);
						}
						anhPath = "/images/" + fileName;
					}
					else
					{
						// Giữ ảnh cũ
						anhPath = oldSP.AnhMH; // phải lấy nguyên giá trị DB
					}

					// Gọi stored procedure để update
					await _context.Database.ExecuteSqlRawAsync(
						"EXEC SanPham_Update @MaSP, @TenSP, @DonGia, @MoTaSP, @AnhMH, @MaLoai, @MaGH",
						new SqlParameter("@MaSP", sp.MaSP),
						new SqlParameter("@TenSP", sp.TenSP),
						new SqlParameter("@DonGia", sp.DonGia),
						new SqlParameter("@MoTaSP", sp.MoTaSP ?? string.Empty),
						new SqlParameter("@AnhMH", anhPath ?? string.Empty),
						new SqlParameter("@MaLoai", sp.MaLoai),
						new SqlParameter("@MaGH", sp.MaGH)
					);

					TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
					return RedirectToAction(nameof(Index));
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			// Nếu validation lỗi, load lại select list
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLSP", sp.MaLoai);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);

			return View(sp);
		}


		// ===============================
		// DELETE - GET
		// ===============================
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null) return NotFound();

			var sp = await _context.SanPham
				.FromSqlRaw("EXEC SanPham_GetByID_Detail @MaSP", new SqlParameter("@MaSP", id))
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (sp == null) return NotFound();

			return View(sp);
		}

		// ===============================
		// DELETE - POST
		// ===============================
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC SanPham_Delete @MaSP",
					new SqlParameter("@MaSP", id)
				);
				TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
