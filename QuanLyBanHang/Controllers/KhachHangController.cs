using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly AppDbContext _context;

		public KhachHangController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string search, string province, int pageNumber = 1, int pageSize = 10)
		{
			ViewBag.Search = search;
			ViewBag.PageNumber = pageNumber;
			ViewBag.PageSize = pageSize;

			var parameters = new[]
			{
				new SqlParameter("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
				new SqlParameter("@DiaChiFilter", string.IsNullOrEmpty(province) ? DBNull.Value : province),
				new SqlParameter("@PageNumber", pageNumber),
				new SqlParameter("@PageSize", pageSize)
			};

			var data = await _context.KhachHang
				.FromSqlRaw("EXEC KhachHang_SearchFilter @Search, @DiaChiFilter, @PageNumber, @PageSize", parameters)
				.ToListAsync();
						
			ViewBag.SelectedProvince = province;

			// Lấy tổng số bản ghi (1 row)
			var countParams = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@TinhFilter", (object?)province ?? DBNull.Value),
			};
			var totalRecords = _context.KhachHangCountDtos
				.FromSqlRaw("EXEC KhachHang_Count @Search, @TinhFilter", countParams)
				.AsEnumerable()
				.Select(x => x.TotalRecords)
				.FirstOrDefault();

			ViewBag.TotalRecords = totalRecords;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			return View(data);
		}


		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var kh = (await _context.KhachHang.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (kh == null)
				return NotFound();

			return View(kh);
		}

		// CREATE - GET
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(KhachHang model)
		{
			try
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"
					EXEC KhachHang_Insert 
						@TenKH = {model.TenKH}, 
						@DienThoaiKH = {model.DienThoaiKH}, 
						@EmailKH = {model.EmailKH}, 
						@DiaChiKH = {model.DiaChiKH}
				");


				TempData["SuccessMessage"] = "Thêm Khách hàng thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"{ex.Message}");
				TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				return View(model);
			}
		}

		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var kh = (await _context.KhachHang
				.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (kh == null)
				return NotFound();

			return View(kh);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(KhachHang model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC KhachHang_Update 
                    @MaKH = {model.MaKH},
                    @TenKH = {model.TenKH}, 
                    @DienThoaiKH = {model.DienThoaiKH}, 
                    @EmailKH = {model.EmailKH}, 
                    @DiaChiKH = {model.DiaChiKH}
            ");
					TempData["SuccessMessage"] = "Cập nhật thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}
			return View(model);
		}


		// DELETE - GET
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var kh = (await _context.KhachHang.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (kh == null)
				return NotFound();

			return View(kh);
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

			// Kiểm tra tồn tại
			var kh = (await _context.KhachHang.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (kh != null)
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC KhachHang_Delete @MaKH = {id}");
				TempData["SuccessMessage"] = "Đã xóa Khách hàng thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy Khách hàng cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}

		// ============ SEARCH ============
		[HttpGet]
		public async Task<IActionResult> Search(string search, string tinhFilter)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@TinhFilter", (object?)tinhFilter ?? DBNull.Value)
			};

			var data = await _context.KhachHang
				.FromSqlRaw("EXEC KhachHang_SearchFilter @Search, @TinhFilter", parameters)
				.ToListAsync();

			return PartialView("KhachHangTable", data);
		}


		// ============ RESET FILTER ============
		public async Task<IActionResult> ClearFilter()
		{
			var data = await _context.KhachHang
				.FromSqlRaw("EXEC KhachHang_SearchFilter @Search=NULL, @TinhFilter=NULL")
				.ToListAsync();

			return PartialView("KhachHangTable", data);
		}
	}
}
