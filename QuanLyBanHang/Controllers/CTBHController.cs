using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class CTBHController : Controller
	{
		private readonly CTBHService _service;
		private readonly AppDbContext _context;

		public CTBHController(AppDbContext context)
		{
			_context = context;
			_service = new CTBHService(context);
		}

		public async Task<IActionResult> Index(string search, string maDBH)
		{
			var list = await _service.GetAll();
            
            if (!string.IsNullOrEmpty(maDBH))
            {
                list = list.Where(x => x.MaDBH == maDBH).ToList();
                ViewBag.MaDBH = maDBH;
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                list = list.Where(x => 
                    x.MaDBH.ToLower().Contains(search) || 
                    x.TenSP.ToLower().Contains(search)
                ).ToList();
            }

            ViewBag.Search = search;
			return View(list);
		}

        public IActionResult Create(string maDBH)
        {
            ViewBag.MaDBH = maDBH;
            ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP");
            return View(new CTBH { MaDBH = maDBH });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CTBH model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Insert(model);
                    TempData["SuccessMessage"] = "Thêm chi tiết bán hàng thành công!";
                    return RedirectToAction(nameof(Index), new { maDBH = model.MaDBH });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                }
            }
            ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
            return View(model);
        }

		public async Task<IActionResult> Details(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var data = await _service.GetDetail(maDBH, maSP);
			if (data == null) return NotFound();

			return View(data);
		}

		//EDIT
		public async Task<IActionResult> Edit(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctbh = await _service.GetDetail(maDBH, maSP);
			if (ctbh == null) return NotFound();

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", ctbh.MaSP);
			return View(ctbh);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CTBH model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
				// Trả về DTO thay vì model để tương thích với View
				var dto = await _service.GetDetail(model.MaDBH!, model.MaSP!);
				if (dto != null)
				{
					dto.SLB = model.SLB ?? 0;
					dto.DGB = model.DGB ?? 0;
					return View(dto);
				}
				return View(model); // Fallback
			}

			try
			{
				await _service.Update(model);
				TempData["SuccessMessage"] = "Cập nhật chi tiết bán hàng thành công!";
				return RedirectToAction("Details", "DonBanHang", new { id = model.MaDBH });
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			ViewBag.MaSP = new SelectList(_context.SanPham, "MaSP", "TenSP", model.MaSP);
			var errorDto = await _service.GetDetail(model.MaDBH!, model.MaSP!);
			if (errorDto != null)
			{
				errorDto.SLB = model.SLB ?? 0;
				errorDto.DGB = model.DGB ?? 0;
				return View(errorDto);
			}
			return View(model);
		}

		public async Task<IActionResult> Delete(string maDBH, string maSP)
		{
			if (string.IsNullOrEmpty(maDBH) || string.IsNullOrEmpty(maSP))
				return NotFound();

			var ctbh = await _service.GetDetail(maDBH, maSP);
			if (ctbh == null) return NotFound();

			return View(ctbh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maDBH, string maSP)
		{
			try
			{
				await _service.Delete(maDBH, maSP);
				TempData["SuccessMessage"] = "Xóa chi tiết bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
