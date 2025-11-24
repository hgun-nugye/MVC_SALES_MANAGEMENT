using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class LoaiSPController : Controller
	{
		private readonly AppDbContext _context;

		public LoaiSPController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
		{
			// Lưu giá trị search và phân trang vào ViewBag
			ViewBag.Search = search;
			ViewBag.PageNumber = pageNumber;
			ViewBag.PageSize = pageSize;

			// Tham số cho SP lấy danh sách
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@PageNumber", pageNumber),
				new SqlParameter("@PageSize", pageSize)
			};

			// Lấy danh sách LoaiSP theo stored procedure
			var model = await _context.LoaiSP
				.FromSqlRaw("EXEC Loai_Search @Search, @PageNumber, @PageSize", parameters)
				.ToListAsync();

			// Lấy tổng số bản ghi
			var countParams = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
			};

			// Sử dụng FirstOrDefaultAsync để lấy tổng số bản ghi một cách async
			var totalRecords = (await _context.LoaiSPCountDtos
				.FromSqlRaw("EXEC Loai_Count @Search", parameters)
				.ToListAsync())
				.Select(x => x.TotalRecords)
				.FirstOrDefault();


			ViewBag.TotalRecords = totalRecords;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			return View(model);
		}



		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var tinh = (await _context.LoaiSP.FromSqlInterpolated($"EXEC Loai_GetByID @MaLoai = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// CREATE - GET
		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.MaNhomList = new SelectList(_context.NhomSP, "MaNhom", "TenNhom");
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(LoaiSP model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
				EXEC Loai_Insert 
					@TenLoai = {model.TenLoai},
					@MaNhom = {model.MaNhom}
			");

					TempData["SuccessMessage"] = "Thêm nhóm sản phẩm thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					// Bắt lỗi SQL (RAISERROR hoặc THROW từ SP)
					ModelState.AddModelError("", ex.Message);
					TempData["ErrorMessage"] = ex.Message;
				}
			}
			else
			{
				TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";
			}
			Console.WriteLine($"TenLoai = {model.TenLoai}, MaNhom = {model.MaNhom}");

			// Không redirect nếu có lỗi — ở lại form
			return View(model);
		}


		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var loai = (await _context.LoaiSP
				.FromSqlInterpolated($"EXEC Loai_GetByID @MaLoai = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (loai == null)
				return NotFound();

			// Lấy danh sách nhóm sản phẩm, set giá trị mặc định
			var nhomSPs = await _context.NhomSP.ToListAsync();
			ViewBag.NhomSPList = new SelectList(nhomSPs, "MaNhom", "TenNhom", loai.MaNhom);

			return View(loai);
		}


		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(LoaiSP model)
		{
			if (!ModelState.IsValid)
			{
				// Nếu model lỗi, set lại dropdown để form không bị rỗng
				var nhomSPs = await _context.NhomSP.ToListAsync();
				ViewBag.NhomSPList = new SelectList(nhomSPs, "MaNhom", "TenNhom", model.MaNhom);
				return View(model);
			}

			try
			{
				// Thực thi stored procedure
				await _context.Database.ExecuteSqlInterpolatedAsync($@"
					EXEC Loai_Update 
						@MaLoai = {model.MaLoai},
						@TenLoai = {model.TenLoai},
						@MaNhom = {model.MaNhom}
				");

				TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Microsoft.Data.SqlClient.SqlException sqlEx)
			{
				ModelState.AddModelError("", sqlEx.Message);
				TempData["ErrorMessage"] = sqlEx.Message;

				var nhomSPs = await _context.NhomSP.ToListAsync();
				ViewBag.NhomSPList = new SelectList(nhomSPs, "MaNhom", "TenNhom", model.MaNhom);
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				var nhomSPs = await _context.NhomSP.ToListAsync();
				ViewBag.NhomSPList = new SelectList(nhomSPs, "MaNhom", "TenNhom", model.MaNhom);
				return View(model);
			}
		}

		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var tinh = (await _context.LoaiSP.FromSqlInterpolated($"EXEC Loai_GetByID @MaLoai = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (tinh == null)
				return NotFound();

			return View(tinh);
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

			var tinh = (await _context.LoaiSP.FromSqlInterpolated($"EXEC Loai_GetByID @MaLoai = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (tinh != null)
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC Loai_Delete @MaLoai = {id}");
				TempData["SuccessMessage"] = "Đã xóa nhóm sản phẩm thành công!";
			}
			else
			{

				TempData["ErrorMessage"] = "Không tìm thấy nhóm sản phẩm cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}

		// ============ SEARCH ============
		[HttpGet]
		public async Task<IActionResult> Search(string keyword)
		{
			var parameters = new SqlParameter("@Search", (object?)keyword ?? DBNull.Value);

			var data = await _context.LoaiSP
				.FromSqlRaw("EXEC Loai_Search @Search", parameters)
				.ToListAsync();

			return PartialView("LoaiSPTable", data);
		}


		// ============ RESET FILTER ============
		public async Task<IActionResult> ClearFilter()
		{
			var data = await _context.LoaiSP
				.FromSqlRaw("EXEC Loai_Search @Search=NULL")
				.ToListAsync();

			return PartialView("LoaiSPTable", data);
		}
	}
}
