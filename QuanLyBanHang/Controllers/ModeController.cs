using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanHang.Controllers
{
	public class ModeController : Controller
	{
		/// <summary>
		/// Đổi chế độ hiển thị giữa Khách hàng (Customer) và Nhân viên/Admin (Admin)
		/// Lưu trong cookie "UserMode" để dùng lại ở tất cả View.
		/// </summary>
		/// <param name="mode">Customer | Admin</param>
		/// <param name="returnUrl">Url quay lại sau khi đổi chế độ</param>
		public IActionResult Set(string mode, string? returnUrl = null)
		{
			// Chuẩn hóa giá trị
			mode = (mode ?? string.Empty).ToLower() == "customer"
				? "Customer"
				: "Admin";

			// Lưu cookie 1 ngày (tùy chỉnh được)
			Response.Cookies.Append("UserMode", mode, new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddDays(1),
				HttpOnly = false,
				IsEssential = true
			});

			// Điều hướng về trang cũ nếu hợp lệ, nếu không quay về Home
			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}


