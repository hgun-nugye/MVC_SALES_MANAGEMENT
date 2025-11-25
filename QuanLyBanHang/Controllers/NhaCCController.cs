using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class NhaCCController : Controller
	{
		private readonly AppDbContext _context;

		public NhaCCController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string? search, string province, int pageNumber = 1, int pageSize = 10)
		{
			ViewBag.Search = search;
			ViewBag.PageNumber = pageNumber;
			ViewBag.PageSize = pageSize;

			// Tham số cho SP lấy danh sách
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@DiaChiFilter", string.IsNullOrEmpty(province) ? DBNull.Value : province),
				new SqlParameter("@PageNumber", pageNumber),
				new SqlParameter("@PageSize", pageSize)
			};

			// Lấy danh sách nhà cung cấp (chỉ các cột trong entity NhaCC)
			var model = await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_SearchFilter @Search, @DiaChiFilter, @PageNumber, @PageSize", parameters)
				.ToListAsync();
			ViewBag.SelectedProvince = province;

			// Lấy tổng số bản ghi (1 row)
			var countParams = new[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@DiaChiFilter", string.IsNullOrEmpty(province) ? DBNull.Value : province)

			};

			var totalRecords = _context.NhaCCCountDtos
							.FromSqlRaw("EXEC NhaCC_Count @Search, @DiaChiFilter", countParams)
							.AsEnumerable()
							.Select(x => x.TotalRecords)
							.FirstOrDefault();


			ViewBag.TotalRecords = totalRecords;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			return View(model);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var ncc = (await _context.NhaCC.FromSqlInterpolated($"EXEC NhaCC_GetByID @MaNCC = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (ncc == null)
				return NotFound();

			return View(ncc);
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
		public async Task<IActionResult> Create(NhaCC model)
		{
			try
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"
					EXEC NhaCC_Insert 
						@TenNCC = {model.TenNCC}, 
						@DienThoaiNCC = {model.DienThoaiNCC}, 
						@EmailNCC = {model.EmailNCC}, 
						@DiaChiNCC = {model.DiaChiNCC}
				");


				TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
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

			var ncc = (await _context.NhaCC
				.FromSqlInterpolated($"EXEC NhaCC_GetByID @MaNCC = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (ncc == null)
				return NotFound();

			return View(ncc);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(NhaCC model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
					EXEC NhaCC_Update 
						@MaNCC = {model.MaNCC},
						@TenNCC = {model.TenNCC}, 
						@DienThoaiNCC = {model.DienThoaiNCC}, 
						@EmailNCC = {model.EmailNCC}, 
						@DiaChiNCC = {model.DiaChiNCC}
				");

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"{ex.Message}");

					TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
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

			var ncc = (await _context.NhaCC.FromSqlInterpolated($"EXEC NhaCC_GetByID @MaNCC = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (ncc == null)
				return NotFound();

			return View(ncc);
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
			var ncc = (await _context.NhaCC.FromSqlInterpolated($"EXEC NhaCC_GetByID @MaNCC = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (ncc != null)
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC NhaCC_Delete @MaNCC = {id}");
				TempData["SuccessMessage"] = "Đã xóa nhà cung cấp thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}

		// ============ SEARCH ============
		[HttpGet]
		public async Task<IActionResult> Search(string keyword, string? tinhFilter)
		{
			var parameters = new[]
			{
				new SqlParameter("@Search", (object?)keyword ?? DBNull.Value),
				new SqlParameter("@TinhFilter", (object?)tinhFilter ?? DBNull.Value)

			};

			var data = await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_SearchFilter @Search, @TinhFilter", parameters)
				.ToListAsync();

			return PartialView("NhaCCTable", data);
		}


		// ============ RESET FILTER ============
		public async Task<IActionResult> ClearFilter()
		{
			var data = await _context.NhaCC
				.FromSqlRaw("EXEC NhaCC_SearchFilter @Search=NULL, @TinhFilter=NULL")
				.ToListAsync();

			return PartialView("NhaCCTable", data);
		}

	}
}
