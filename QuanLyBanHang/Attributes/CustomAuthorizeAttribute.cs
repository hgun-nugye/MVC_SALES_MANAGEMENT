using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyBanHang.Attributes
{
	/// <summary>
	/// Custom Authorization Attribute để kiểm tra đăng nhập và phân quyền
	/// </summary>
	public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
	{
		private readonly string[] _allowedRoles;

		/// <summary>
		/// Khởi tạo attribute với các vai trò được phép truy cập
		/// </summary>
		/// <param name="allowedRoles">Danh sách vai trò: "Admin", "Employee", "Customer"</param>
		public CustomAuthorizeAttribute(params string[] allowedRoles)
		{
			_allowedRoles = allowedRoles ?? new string[] { };
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var session = context.HttpContext.Session;
			var isLoggedIn = session.GetString("IsLoggedIn") == "true";

			// Kiểm tra đăng nhập
			if (!isLoggedIn)
			{
				context.Result = new RedirectToActionResult("Login", "Home", null);
				return;
			}

			// Nếu không có role nào được chỉ định, chỉ cần đăng nhập là đủ
			if (_allowedRoles.Length == 0)
			{
				return;
			}

			// Kiểm tra vai trò
			var userType = session.GetString("UserType");
			var userRole = session.GetString("UserRole"); // Admin hoặc vai trò khác

			// Kiểm tra xem user có thuộc vai trò được phép không
			bool hasPermission = false;

			foreach (var role in _allowedRoles)
			{
				if (role == "Customer" && userType == "Customer")
				{
					hasPermission = true;
					break;
				}
				else if (role == "Employee" && userType == "Employee")
				{
					hasPermission = true;
					break;
				}
				else if (role == "Admin" && userRole == "Admin")
				{
					hasPermission = true;
					break;
				}
			}

			if (!hasPermission)
			{
				// Không có quyền truy cập
				context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
				return;
			}
		}
	}

	/// <summary>
	/// Chỉ cho phép Admin truy cập
	/// </summary>
	public class AdminOnlyAttribute : CustomAuthorizeAttribute
	{
		public AdminOnlyAttribute() : base("Admin") { }
	}

	/// <summary>
	/// Chỉ cho phép Employee (bao gồm Admin) truy cập
	/// </summary>
	public class EmployeeOnlyAttribute : CustomAuthorizeAttribute
	{
		public EmployeeOnlyAttribute() : base("Employee", "Admin") { }
	}

	/// <summary>
	/// Chỉ cho phép Customer truy cập
	/// </summary>
	public class CustomerOnlyAttribute : CustomAuthorizeAttribute
	{
		public CustomerOnlyAttribute() : base("Customer") { }
	}
}
