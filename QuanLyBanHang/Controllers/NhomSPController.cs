using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class NhomSPController : Controller
	{
		private readonly AppDbContext _context;

		public NhomSPController(AppDbContext context)
		{
			_context = context;
		}


		// READ - Danh sách nhóm sản phẩm 
		public async Task<IActionResult> Index()
		{
			var dsNhomSP = await _context.NhomSP.FromSqlRaw("EXEC Nhom_GetAll")
				.ToListAsync();

			return View(dsNhomSP);
		}

		// DETAILS - Xem chi tiết
		public async Task<IActionResult> Details(string id)
		{
			var tinh = (await _context.NhomSP.FromSqlInterpolated($"EXEC Nhom_GetByID @MaNhom = {id}")
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
			return View();
		}

		// CREATE - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(NhomSP model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
						EXEC Nhom_Insert @TenNhom = {model.TenNhom}
						");

					TempData["SuccessMessage"] = "Thêm nhóm sản phẩm thành công!";
					return RedirectToAction(nameof(Create));
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

			return View(model);
		}


		// EDIT - GET
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var tinh = (await _context.NhomSP
				.FromSqlInterpolated($"EXEC Nhom_GetByID @MaNhom = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (tinh == null)
				return NotFound();

			return View(tinh);
		}

		// EDIT - POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(NhomSP model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
					EXEC Nhom_Update 
						@MaNhom = {model.MaNhom},
						@TenNhom = {model.TenNhom}
				");

					TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					if (ex is Microsoft.Data.SqlClient.SqlException sqlEx)
					{
						// Kiểm tra lỗi trùng dữ liệu (từ RAISERROR trong SQL)
						if (sqlEx.Message.Contains("đã tồn tại"))
						{
							ModelState.AddModelError("", sqlEx.Message);
							TempData["ErrorMessage"] = sqlEx.Message;
						}
						else
						{
							TempData["ErrorMessage"] = "Lỗi SQL: " + sqlEx.Message;
						}
					}
					else
					{
						TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
					}
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

			var tinh = (await _context.NhomSP.FromSqlInterpolated($"EXEC Nhom_GetByID @MaNhom = {id}")
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

			var tinh = (await _context.NhomSP.FromSqlInterpolated($"EXEC Nhom_GetByID @MaNhom = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (tinh != null)
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC Nhom_Delete @MaNhom = {id}");
				TempData["SuccessMessage"] = "Đã xóa nhóm sản phẩm thành công!";
			}
			else
			{

				TempData["ErrorMessage"] = "Không tìm thấy nhóm sản phẩm cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
