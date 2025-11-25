using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;
using System.Data;

namespace QuanLyBanHang.Controllers
{
	public class SanPhamController : Controller
	{
		private readonly AppDbContext _context;

		public SanPhamController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string search, string status, string type, int pageNumber = 1, int pageSize = 10)
		{
			ViewBag.Search = search;
			ViewBag.Status = status;
			ViewBag.Type = type;
			ViewBag.PageNumber = pageNumber;
			ViewBag.PageSize = pageSize;

			// Lấy danh sách Loại sản phẩm
			var loaiSPList = await _context.LoaiSP
				.FromSqlRaw("EXEC Loai_GetAll")
				.ToListAsync();

			// Tạo SelectList và set giá trị đã chọn (filter hiện tại)
			ViewBag.LoaiSPSelectList = new SelectList(loaiSPList, "MaLoai", "TenLoai", string.IsNullOrEmpty(type) ? null : type);

			var statusList = new List<SelectListItem>
			{
				new SelectListItem { Text = "Còn Hàng", Value = "Còn Hàng" },
				new SelectListItem { Text = "Hết Hàng", Value = "Hết Hàng" },
				new SelectListItem { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new SelectListItem { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			// Tạo SelectList và set giá trị đã chọn (filter hiện tại)
			ViewBag.StatusSelectList = new SelectList(statusList, "Value", "Text", status);

			var parameters = new[]
			{
				new SqlParameter("@Search", SqlDbType.NVarChar, 100) { Value = string.IsNullOrEmpty(search) ? DBNull.Value : search },
				new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(status) ? DBNull.Value : status },
				new SqlParameter("@TenLoai", SqlDbType.VarChar, 10) { Value = string.IsNullOrEmpty(type) ? DBNull.Value : type },
				new SqlParameter("@PageNumber", pageNumber),
				new SqlParameter("@PageSize", pageSize)
			};

			var data = await _context.SanPhamDtos
				.FromSqlRaw("EXEC SanPham_SearchFilter @Search, @TrangThai, @TenLoai, @PageNumber, @PageSize", parameters)
				.AsNoTracking()
				.ToListAsync();

			var countParams = new[]
			{
				new SqlParameter("@Search", SqlDbType.NVarChar, 100) { Value = string.IsNullOrEmpty(search) ? DBNull.Value : search },
				new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(status) ? DBNull.Value : status },
				new SqlParameter("@TenLoai", SqlDbType.VarChar, 10) { Value = string.IsNullOrEmpty(type) ? DBNull.Value : type }
			};

			var totalRecords = _context.SanPhamCountDtos
				.FromSqlRaw("EXEC SanPham_Count @Search, @TrangThai, @TenLoai", countParams)
				.AsEnumerable()
				.Select(x => x.TotalRecords)
				.FirstOrDefault();

			ViewBag.TotalRecords = totalRecords;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			return View(data);
		}

		// ===============================
		// CHI TIẾT SẢN PHẨM
		// ===============================
		public async Task<IActionResult> Details(string id)
		{
			if (id == null) return NotFound();

			var sp = (await _context.SanPhamDtos
			.FromSqlRaw("EXEC SanPham_GetByID @MaSP", new SqlParameter("@MaSP", id))
			.ToListAsync())
			.FirstOrDefault();

			return View(sp);
		}

		// ===============================
		// CREATE - GET
		// ===============================
		public IActionResult Create()
		{
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLoai");
			ViewBag.NhaCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC");
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH");
			var statusList = new List<SelectListItem>
			{
				new SelectListItem { Text = "Còn Hàng", Value = "Còn Hàng" },
				new SelectListItem { Text = "Hết Hàng", Value = "Hết Hàng" },
				new SelectListItem { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new SelectListItem { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			// Tạo SelectList và set giá trị đã chọn (filter hiện tại)
			ViewBag.TrangThai = new SelectList(statusList, "Value", "Text");
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
				// ========== GIỮ ẢNH CŨ NẾU SUBMIT LỖI ==========
				string oldImage = sp.AnhMH;

				// Kiểm tra Model 
				if (!ModelState.IsValid)
				{
					TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin hợp lệ!";
					LoadDropDown(sp);
					sp.AnhMH = oldImage;
					return View(sp);
				}

				// ========== XỬ LÝ FILE ẢNH ==========
				if (sp.AnhFile != null && sp.AnhFile.Length > 0)
				{
					// Tạo tên file mới
					var fileName = Guid.NewGuid() + Path.GetExtension(sp.AnhFile.FileName);

					var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);

					var savePath = Path.Combine(folderPath, fileName);
					using (var stream = new FileStream(savePath, FileMode.Create))
					{
						await sp.AnhFile.CopyToAsync(stream);
					}

					sp.AnhMH = fileName;
				}
				else
				{
					TempData["ErrorMessage"] = "Vui lòng chọn ảnh minh họa!";
					LoadDropDown(sp);
					sp.AnhMH = oldImage;
					return View(sp);
				}

				// ========== INSERT SQL ==========
				await _context.Database.ExecuteSqlRawAsync(
					"EXEC SanPham_Insert @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH",
					new SqlParameter("@TenSP", sp.TenSP),
					new SqlParameter("@DonGia", sp.DonGia),
					new SqlParameter("@GiaBan", sp.GiaBan),
					new SqlParameter("@MoTaSP", sp.MoTaSP),
					new SqlParameter("@AnhMH", sp.AnhMH),
					new SqlParameter("@TrangThai", sp.TrangThai),
					new SqlParameter("@SoLuongTon", sp.SoLuongTon),
					new SqlParameter("@MaLoai", sp.MaLoai ?? (object)DBNull.Value),
					new SqlParameter("@MaNCC", sp.MaNCC ?? (object)DBNull.Value),
					new SqlParameter("@MaGH", sp.MaGH ?? (object)DBNull.Value)
				);

				TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (SqlException ex)
			{
				TempData["ErrorMessage"] = "Lỗi SQL: " + ex.Message;
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
			}

			LoadDropDown(sp);
			return View(sp);
		}

		// Hàm load dropdown
		private void LoadDropDown(SanPham sp)
		{
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLoai", sp.MaLoai);
			ViewBag.NhaCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", sp.MaNCC);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
			var statusList = new List<SelectListItem>
			{
				new SelectListItem { Text = "Còn Hàng", Value = "Còn Hàng" },
				new SelectListItem { Text = "Hết Hàng", Value = "Hết Hàng" },
				new SelectListItem { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new SelectListItem { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			ViewBag.TrangThai = new SelectList(statusList, "Value", "Text", sp.TrangThai);
		}

		// ===============================
		// EDIT - GET
		// ===============================
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var spList = await _context.SanPham
				.FromSqlRaw("EXEC SanPham_GetByID @MaSP", new SqlParameter("@MaSP", id)).ToListAsync();

			var sp = spList.FirstOrDefault();

			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLoai", sp.MaLoai);
			ViewBag.NhaCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", sp.MaNCC);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
			var statusList = new List<SelectListItem>
			{
				new SelectListItem { Text = "Còn Hàng", Value = "Còn Hàng" },
				new SelectListItem { Text = "Hết Hàng", Value = "Hết Hàng" },
				new SelectListItem { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new SelectListItem { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			ViewBag.TrangThai = new SelectList(statusList, "Value", "Text", sp.TrangThai);

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
					"EXEC SanPham_Update @MaSP, @TenSP, @DonGia, @GiaBan, @MoTaSP, @AnhMH, @TrangThai, @SoLuongTon, @MaLoai, @MaNCC, @MaGH",
					new SqlParameter("@MaSP", sp.MaSP),
					new SqlParameter("@TenSP", sp.TenSP),
					new SqlParameter("@DonGia", sp.DonGia),
					new SqlParameter("@GiaBan", sp.GiaBan),
					new SqlParameter("@MoTaSP", sp.MoTaSP),
					new SqlParameter("@AnhMH", anhPath ?? (object)DBNull.Value),
					new SqlParameter("@TrangThai", sp.TrangThai),
					new SqlParameter("@SoLuongTon", sp.SoLuongTon),
					new SqlParameter("@MaLoai", sp.MaLoai ?? (object)DBNull.Value),
					new SqlParameter("@MaNCC", sp.MaNCC ?? (object)DBNull.Value),
					new SqlParameter("@MaGH", sp.MaGH ?? (object)DBNull.Value)
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
			ViewBag.LoaiSP = new SelectList(_context.LoaiSP, "MaLoai", "TenLoai", sp.MaLoai);
			ViewBag.NhaCC = new SelectList(_context.NhaCC, "MaNCC", "TenNCC", sp.MaNCC);
			ViewBag.GianHang = new SelectList(_context.GianHang, "MaGH", "TenGH", sp.MaGH);
			var statusList = new List<SelectListItem>
			{
				new SelectListItem { Text = "Còn Hàng", Value = "Còn Hàng" },
				new SelectListItem { Text = "Hết Hàng", Value = "Hết Hàng" },
				new SelectListItem { Text = "Cháy Hàng", Value = "Cháy Hàng" },
				new SelectListItem { Text = "Sắp Hết", Value = "Sắp Hết" }
			};
			ViewBag.TrangThai = new SelectList(statusList, "Value", "Text", sp.TrangThai);
			return View(sp);
		}


		// ===============================
		// DELETE - GET
		// ===============================
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null) return NotFound();

			var spList = await _context.SanPham
			.FromSqlRaw("EXEC SanPham_GetByID @MaSP", new SqlParameter("@MaSP", id))
			.ToListAsync();

			var sp = spList.FirstOrDefault();

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

		// ============ SEARCH ============
		[HttpGet]
		public async Task<IActionResult> Search(string keyword, string status, string type)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)keyword ?? DBNull.Value),
				new SqlParameter("@TrangThai", (object?)status ?? DBNull.Value),
				new SqlParameter("@TenLoai", (object?)type ?? DBNull.Value)
			};

			var data = await _context.SanPham
				.FromSqlRaw("EXEC GianHang_SearchFilter @Search, @TrangThai, @TenLoai", parameters)
				.ToListAsync();

			return PartialView("SanPhamTable", data);
		}


		// ============ RESET FILTER ============
		public async Task<IActionResult> ClearFilter()
		{
			var data = await _context.SanPham
				.FromSqlRaw("EXEC GianHang_SearchFilter @Search=NULL, @TrangThai=NULL, @TenLoai=NULL")
				.ToListAsync();

			return PartialView("SanPhamTable", data);
		}
	}
}
