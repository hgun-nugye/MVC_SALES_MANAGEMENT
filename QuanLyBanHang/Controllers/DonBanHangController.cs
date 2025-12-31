// Đảm bảo danh sách không null để không lỗi View
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class DonBanHangController : Controller
	{
		private readonly DonBanHangService _dbhService;
		private readonly CTBHService _ctbhService;
		private readonly SanPhamService _spService;
		private readonly TrangThaiBHService _ttdhService;
		private readonly XaService _xaService;
		private readonly TinhService _tinhService;
		private readonly AppDbContext _context;

		public DonBanHangController(
			XaService xaService,
			TinhService tinhService,
			DonBanHangService service,
			CTBHService ctbhService,
			SanPhamService spService,
			TrangThaiBHService ttdhService,
			AppDbContext context)
		{
			_dbhService = service;
			_ctbhService = ctbhService;
			_spService = spService;
			_ttdhService = ttdhService;
			_context = context;
			_xaService = xaService;
			_tinhService = tinhService;
		}

		private bool IsCustomerMode()
		{
			return HttpContext.Session.GetString("IsCustomer") == "true";
		}

		public async Task<IActionResult> Index(string? search, int? month, int? year, string? MaTTBH)
		{
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;
			ViewBag.MaTTBH = MaTTBH;

			ViewBag.TrangThaiBH = new SelectList(await _ttdhService.GetAll(), "MaTTBH", "TenTTBH", MaTTBH);

			var model = await _dbhService.Search(search, month, year, MaTTBH);

			// Nếu là khách hàng, chỉ hiển thị đơn hàng của họ
			if (IsCustomerMode())
			{
				var userId = HttpContext.Session.GetString("UserId");
				if (!string.IsNullOrEmpty(userId))
				{
					model = model.Where(x => x.MaKH == userId).ToList();
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Details(string id, string? search, int? month, int? year, string? MaTTBH)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			ViewBag.IsCustomer = IsCustomerMode(); 
			ViewBag.Search = search;
			ViewBag.Month = month;
			ViewBag.Year = year;
			ViewBag.MaTTBH = MaTTBH;

			var result = await _dbhService.GetByID(id);
			return View(result);
		}

		private async Task LoadDropdowns(string? selectedKH = null, string? selectedTTDH = null, string? selectedXa = null)
		{
			// Nạp danh sách Khách hàng - Dùng service (Proc) thay vì _context
			//var khachHangs = await (new KhachHangService(_context)).GetAllWithXa();
			//ViewBag.MaKH = new SelectList(khachHangs, "MaKH", "TenKH", selectedKH);

			var khachHangs = await _context.KhachHang
							.Select(kh => new { kh.MaKH, kh.TenKH })
							.ToListAsync();

			ViewBag.MaKH = new SelectList(khachHangs, "MaKH", "TenKH", selectedKH);


			// Nạp danh sách Trạng thái bán hàng - Dùng service (Proc) thay vì _context
			var trangThais = await _ttdhService.GetAll();
			ViewBag.MaTTBH = new SelectList(trangThais, "MaTTBH", "TenTTBH", selectedTTDH);

			// Nạp danh sách Sản phẩm cho chi tiết đơn hàng - Dùng service (Proc) thay vì _context
			var sanPhams = await _spService.GetAll();
			ViewBag.MaSP = new SelectList(sanPhams, "MaSP", "TenSP");
		}

		public async Task<IActionResult> Create()
		{
			// Nếu là khách hàng, kiểm tra đã đăng nhập chưa
			if (IsCustomerMode())
			{
				var userId = HttpContext.Session.GetString("UserId");
				if (string.IsNullOrEmpty(userId))
				{
					TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng.";
					return RedirectToAction("Login", "Home");
				}
			}

			var tinhs = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(tinhs, "MaTinh", "TenTinh");
			ViewBag.Xa = new SelectList(Enumerable.Empty<object>(), "MaXa", "TenXa");
			
			var sanPhams = await _spService.GetAll();
			ViewBag.MaSP = new SelectList(sanPhams, "MaSP", "TenSP");
			await LoadDropdowns();
			ViewData["MaXaSelected"] = null;

			var model = new DonBanHang();

			// Nếu là khách hàng, tự động gán MaKH
			if (IsCustomerMode())
			{
				var userId = HttpContext.Session.GetString("UserId");
				model.MaKH = userId;
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DonBanHang model, string maTinh)
		{
			try
			{
				bool isCustomer = IsCustomerMode();

				ModelState.Remove("MaDBH");
				ModelState.Remove("Xa");
				ModelState.Remove("MaXa");

				// Nếu là khách hàng, tự động gán MaKH từ session
				if (isCustomer)
				{
					var userId = HttpContext.Session.GetString("UserId");
					if (!string.IsNullOrEmpty(userId))
					{
						model.MaKH = userId;
					}

					if (string.IsNullOrEmpty(model.MaTTBH))
					{
						model.MaTTBH = "CHO";
					}
				}
				else
				{
					if (string.IsNullOrEmpty(model.MaTTBH))
					{
						ModelState.AddModelError("MaTTBH", "Vui lòng chọn trạng thái đơn hàng.");
					}
				}

				// Kiểm tra Khách hàng
				if (string.IsNullOrEmpty(model.MaKH))
				{
					ModelState.AddModelError("MaKH", isCustomer
						? "Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại."
						: "Vui lòng chọn khách hàng.");
				}

				// Kiểm tra Địa chỉ
				if (string.IsNullOrEmpty(model.DiaChiDBH))
				{
					ModelState.AddModelError("DiaChiDBH", "Vui lòng nhập địa chỉ nhận hàng.");
				}

				// Kiểm tra Tỉnh/Xã (Nếu maTinh = "" hoặc MaXa = null)
				if (string.IsNullOrEmpty(maTinh))
				{
					ModelState.AddModelError("maTinh", "Vui lòng chọn Tỉnh/Thành phố.");
				}
				else if (string.IsNullOrEmpty(model.MaXa))
				{
					ModelState.AddModelError("MaXa", "Vui lòng chọn Xã/Phường.");
				}

				// Kiểm tra Ngày bán/mua
				if (model.NgayBH == default(DateTime))
				{
					ModelState.AddModelError("NgayBH", isCustomer
						? "Vui lòng chọn ngày mua hàng."
						: "Vui lòng chọn ngày bán hàng.");
				}

				var allTinhs = await _tinhService.GetAll();
				ViewBag.Tinh = new SelectList(allTinhs, "MaTinh", "TenTinh", maTinh);

				var xaList = await _xaService.GetByIDTinh(maTinh);
				ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
				ViewData["MaXaSelected"] = model.MaXa;

				model.CTBHs ??= new List<CTBH>();

				// Lưu lại danh sách gốc để trả về view khi lỗi
				var originalDetails = model.CTBHs.ToList();

				var cleanedDetails = model.CTBHs
					.Where(x => !string.IsNullOrEmpty(x.MaSP))
					.ToList();

				if (!cleanedDetails.Any())
				{
					ModelState.AddModelError("CTBHs", "Vui lòng chọn ít nhất 1 sản phẩm.");
				}

				// GÁN GIÁ TRỊ HỢP LỆ
				foreach (var ct in cleanedDetails)
				{
					ct.SLB ??= 1;
					ct.DGB ??= 0;
				}

				// Nếu là khách hàng: tự động dùng giá bán từ bảng Sản phẩm, KH không được nhập giá
				Dictionary<string, decimal> priceLookup = new();
				if (isCustomer && cleanedDetails.Any())
				{
					var sanPhamsForPrice = await _spService.GetAll();
					priceLookup = sanPhamsForPrice
						.Where(x => !string.IsNullOrEmpty(x.MaSP))
						.ToDictionary(x => x.MaSP!, x => x.GiaBan ?? 0m);

					foreach (var ct in cleanedDetails)
					{
						if (!string.IsNullOrEmpty(ct.MaSP) &&
							priceLookup.TryGetValue(ct.MaSP, out var giaBan))
						{
							ct.DGB = giaBan;
						}
					}
				}

				// Kiểm tra tồn kho
				var selectedIds = cleanedDetails
					.Select(x => x.MaSP!)
					.Distinct()
					.ToList();

				var stockLookup = new Dictionary<string, int>();
				if (selectedIds.Any())
				{
					var sanPhams = await _spService.GetAll();
					stockLookup = sanPhams
						.Where(x => !string.IsNullOrEmpty(x.MaSP) && selectedIds.Contains(x.MaSP))
						.ToDictionary(x => x.MaSP!, x => x.SoLuongTon ?? 0);
				}

				for (int i = 0; i < cleanedDetails.Count; i++)
				{
					var ct = cleanedDetails[i];
					if (!string.IsNullOrEmpty(ct.MaSP) && stockLookup.TryGetValue(ct.MaSP, out var ton))
					{
						var slb = ct.SLB ?? 0;
						if (slb > ton)
						{
							ModelState.AddModelError($"CTBHs[{i}].SLB", $"Số lượng bán ({slb}) vượt tồn kho ({ton}).");
						}
					}
				}

				if (ModelState.IsValid && cleanedDetails.Any())
				{
					model.CTBHs = cleanedDetails;
					await _dbhService.Create(model);

					TempData["SuccessMessage"] = isCustomer
						? "Đặt hàng thành công!"
						: "Thêm đơn bán hàng thành công!";
					return RedirectToAction(nameof(Index));
				}

				TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đơn hàng và chi tiết sản phẩm.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi thêm đơn bán hàng: " + ex.Message;
			}

			// Khôi phục dữ liệu để người dùng không mất thông tin
			if (model.CTBHs == null || !model.CTBHs.Any())
			{
				model.CTBHs = new List<CTBH> { new CTBH() };
			}

			await LoadDropdowns(model.MaKH, model.MaTTBH, model.MaXa);

			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			if (id == null) return NotFound();

			var rows = await _dbhService.GetByID(id);
			if (rows == null || !rows.Any()) return NotFound();

			var header = rows.First();

			// Lấy chi tiết
			var details = rows.Select(x => new CTBH
			{
				MaDBH = x.MaDBH!,
				MaSP = x.MaSP!,
				SLB = x.SLB ?? 0,
				DGB = x.DGB ?? 0,
				TenSP = x.TenSP
			}).ToList();

			var ct = new DonBanHang
			{
				MaDBH = header.MaDBH!,
				NgayBH = header.NgayBH,
				MaKH = header.MaKH!,
				DiaChiDBH = header.DiaChiDBH ?? "",
				MaXa = header.MaXa ?? "",
				TenXa = header.TenXa,
				TenTinh = header.TenTinh,
				CTBHs = details,
				MaTTBH = header.MaTTBH ?? string.Empty
			};

			string? currentMaTinh = null;

			if (!string.IsNullOrEmpty(header.MaXa))
			{
				currentMaTinh = await _xaService.GetMaTinhByXa(header.MaXa);
			}

			var allTinhsForEdit = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(allTinhsForEdit, "MaTinh", "TenTinh", currentMaTinh);

			var listXa = !string.IsNullOrEmpty(currentMaTinh)
			? await _xaService.GetByIDTinh(currentMaTinh)
			: new List<Xa>();

			ViewData["MaXaSelected"] = header.MaXa;
			ViewBag.Xa = new SelectList(listXa, "MaXa", "TenXa", header.MaXa);

			await LoadDropdowns(header.MaKH, header.MaTTBH);

			return View(ct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DonBanHang model, string maTinh)
		{
			// Xóa các lỗi validation không cần thiết cho việc Update
			ModelState.Remove("MaDBH");
			ModelState.Remove("MaXa");
			
			if (model.CTBHs != null)
			{
				for (int i = 0; i < model.CTBHs.Count; i++)
				{
					ModelState.Remove($"CTBHs[{i}].MaDBH");
					ModelState.Remove($"CTBHs[{i}].MaSP"); // Key of nested item
				}
			}

			try
			{
				model.CTBHs ??= new List<CTBH>();
				var cleanedDetails = model.CTBHs
					.Where(x => !string.IsNullOrEmpty(x.MaSP))
					.ToList();

				if (!cleanedDetails.Any())
				{
					ModelState.AddModelError("CTBHs", "Vui lòng chọn ít nhất 1 sản phẩm.");
				}

				// Gán giá trị mặc định và lấy tên SP để hiển thị lại nếu lỗi
				var sanPhamsForNames = await _spService.GetAll();
				var nameLookup = sanPhamsForNames.ToDictionary(x => x.MaSP!, x => x.TenSP);

				foreach (var ct in cleanedDetails)
				{
					ct.SLB ??= 1;
					ct.DGB ??= 0;
					if (string.IsNullOrEmpty(ct.TenSP) && !string.IsNullOrEmpty(ct.MaSP))
					{
						nameLookup.TryGetValue(ct.MaSP, out var ten);
						ct.TenSP = ten;
					}
				}

				// Kiểm tra tồn kho
				var selectedIds = cleanedDetails
					.Select(x => x.MaSP!)
					.Distinct()
					.ToList();

				var stockLookup = new Dictionary<string, int>();
				if (selectedIds.Any())
				{
					stockLookup = sanPhamsForNames
						.Where(x => !string.IsNullOrEmpty(x.MaSP) && selectedIds.Contains(x.MaSP))
						.ToDictionary(x => x.MaSP!, x => x.SoLuongTon ?? 0);
				}

				for (int i = 0; i < cleanedDetails.Count; i++)
				{
					var ct = cleanedDetails[i];
					if (!string.IsNullOrEmpty(ct.MaSP) && stockLookup.TryGetValue(ct.MaSP, out var ton))
					{
						var slb = ct.SLB ?? 0;
						if (slb > ton)
						{
							ModelState.AddModelError($"CTBHs[{i}].SLB", $"Số lượng bán ({slb}) vượt tồn kho ({ton}).");
						}
					}
				}

				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join(" | ", errors);

					await LoadDropdowns(model.MaKH, model.MaTTBH, model.MaXa);
					var tinhsForErr = await _tinhService.GetAll();
					ViewBag.Tinh = new SelectList(tinhsForErr, "MaTinh", "TenTinh", maTinh);
					var xaList = await _xaService.GetByIDTinh(maTinh);
					ViewBag.Xa = new SelectList(xaList, "MaXa", "TenXa", model.MaXa);
					ViewData["MaXaSelected"] = model.MaXa;
					model.CTBHs = cleanedDetails;
					return View(model);
				}

				model.CTBHs = cleanedDetails;
				await _dbhService.Update(model);

				TempData["SuccessMessage"] = "Cập nhật đơn bán hàng thành công!";
				return RedirectToAction(nameof(Details), new { id = model.MaDBH });
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi cập nhật: " + ex.Message;
			}

			await LoadDropdowns(model.MaKH, model.MaTTBH, model.MaXa);
			var tinhsFinal = await _tinhService.GetAll();
			ViewBag.Tinh = new SelectList(tinhsFinal, "MaTinh", "TenTinh", maTinh);
			var finalXaList = await _xaService.GetByIDTinh(maTinh);
			ViewBag.Xa = new SelectList(finalXaList, "MaXa", "TenXa", model.MaXa);
			ViewData["MaXaSelected"] = model.MaXa;

			if (model.CTBHs == null || !model.CTBHs.Any())
				model.CTBHs = new List<CTBH> { new CTBH() };

			return View(model);
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();


			var dbh = await _dbhService.GetByID(id);
			if (dbh == null || !dbh.Any()) return NotFound();

			return View(dbh);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dbhService.Delete(id);
				TempData["SuccessMessage"] = "Xóa đơn bán hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> DeleteDetail(string maDBH, string maSP)
		{
			var model = await _dbhService.GetDetail(maDBH, maSP);
			if (model == null) return NotFound();
			return View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDetailConfirmed(string maDBH, string maSP)
		{
			try
			{
				await _dbhService.DeleteDetail(maDBH, maSP);

				TempData["SuccessMessage"] = "Xóa chi tiết sản phẩm thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
			}

			return RedirectToAction("Details", new { id = maDBH });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmOrder(string id, string? search, int? month, int? year, string? MaTTBH)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			// Chỉ nhân viên mới được xác nhận đơn
			if (IsCustomerMode()) return Forbid();

			try
			{
				await _dbhService.ConfirmOrder(id);
				TempData["SuccessMessage"] = "Đã xác nhận đơn hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi xác nhận đơn hàng: " + ex.Message;
			}

			return RedirectToAction(nameof(Details), new { id = id, search, month, year, MaTTBH });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CancelOrder(string id, string? search, int? month, int? year, string? MaTTBH)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();

			try
			{
				var rows = await _dbhService.GetByID(id);
				var don = rows.FirstOrDefault();
				if (don == null) return NotFound();

				if (IsCustomerMode())
				{
					var userId = HttpContext.Session.GetString("UserId");
					if (don.MaKH != userId) return Forbid();

					if (don.MaTTBH?.Trim() != "CHO")
					{
						TempData["ErrorMessage"] = "Bạn chỉ có thể hủy đơn hàng khi đơn đang ở trạng thái Chờ xác nhận.";
						return RedirectToAction(nameof(Details), new { id, search, month, year, MaTTBH });
					}
				}

				await _dbhService.CancelOrder(id.Trim());
				TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công!";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi hủy đơn hàng: " + ex.Message;
			}

			return RedirectToAction(nameof(Details), new { id, search, month, year, MaTTBH });
		}
	}
}


