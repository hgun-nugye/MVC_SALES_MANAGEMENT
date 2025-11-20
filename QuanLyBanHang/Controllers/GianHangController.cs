using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class GianHangController : Controller
	{
		private readonly AppDbContext _context;

		public GianHangController(AppDbContext context)
		{
			_context = context;
		}

		//  READ - DANH SÁCH 
		public async Task<IActionResult> Index()
		{
			var dsGH = await _context.GianHang
				.FromSqlRaw("EXEC GianHang_GetAll")
				.ToListAsync();

			return View(dsGH);
		}

		//  DETAILS 
		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = (await _context.GianHang
				.FromSqlInterpolated($"EXEC GianHang_GetByID @MaGH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (gh == null)
				return NotFound();

			return View(gh);
		}

		//  CREATE (GET) 
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		//  CREATE (POST) 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(GianHang model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        EXEC GianHang_Insert 
                            @TenGH = {model.TenGH},
                            @MoTaGH = {model.MoTaGH},
                            @DienThoaiGH = {model.DienThoaiGH},
                            @EmailGH = {model.EmailGH},
                            @DiaChiGH = {model.DiaChiGH}
                    ");

					TempData["SuccessMessage"] = "Thêm gian hàng thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
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

		//  EDIT (GET) 
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = (await _context.GianHang
				.FromSqlInterpolated($"EXEC GianHang_GetByID @MaGH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (gh == null)
				return NotFound();

			return View(gh);
		}

		//  EDIT (POST) 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(GianHang model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        EXEC GianHang_Update 
                            @MaGH = {model.MaGH},
                            @TenGH = {model.TenGH},
                            @MoTaGH = {model.MoTaGH},
                            @DienThoaiGH = {model.DienThoaiGH},
                            @EmailGH = {model.EmailGH},
                            @DiaChiGH = {model.DiaChiGH}
                    ");

					TempData["SuccessMessage"] = "Cập nhật gian hàng thành công!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
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

		//  DELETE (GET) 
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var gh = (await _context.GianHang
				.FromSqlInterpolated($"EXEC GianHang_GetByID @MaGH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (gh == null)
				return NotFound();

			return View(gh);
		}

		//  DELETE (POST) 
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				TempData["ErrorMessage"] = "ID không hợp lệ!";
				return BadRequest();
			}

			var gh = (await _context.GianHang
				.FromSqlInterpolated($"EXEC GianHang_GetByID @MaGH = {id}")
				.ToListAsync())
				.FirstOrDefault();

			if (gh != null)
			{
				await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC GianHang_Delete @MaGH = {id}");
				TempData["SuccessMessage"] = "Xóa gian hàng thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Không tìm thấy gian hàng cần xóa!";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
