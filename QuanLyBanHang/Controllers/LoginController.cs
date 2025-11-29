using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers
{
	public class LoginController : Controller
	{
		private readonly AppDbContext _context;

		public LoginController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View(new TaiKhoan());
		}

		[HttpPost]
		public IActionResult Index(TaiKhoan model)
		{
			if (!ModelState.IsValid)
				return View(model);

			// Lấy user từ database
			var user = _context.TaiKhoan.FromSqlRaw("SELECT * FROM TaiKhoan WHERE TenUser = {0} AND MatKhau = {1}", model.TenUser, model.MatKhau)
				.FirstOrDefault();

			if (user != null)
			{
				return RedirectToAction("Index", "Home");
			}

			// Sai tài khoản hoặc mật khẩu
			ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
			return View(model);
		}

	}
}
