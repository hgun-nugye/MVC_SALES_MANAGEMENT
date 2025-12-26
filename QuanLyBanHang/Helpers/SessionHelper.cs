namespace QuanLyBanHang.Helpers
{
	/// <summary>
	/// Helper class để quản lý Session keys một cách tập trung
	/// </summary>
	public static class SessionKeys
	{
		public const string IsLoggedIn = "IsLoggedIn";
		public const string UserId = "UserId";
		public const string UserName = "UserName";
		public const string UserType = "UserType"; // "Customer" hoặc "Employee"
		public const string UserRole = "UserRole"; // "Admin", "Quản lý", "Nhân viên", etc.
		public const string UserAvatar = "UserAvatar";
		public const string IsCustomer = "IsCustomer";
	}

	/// <summary>
	/// Helper class để làm việc với Session
	/// </summary>
	public static class SessionHelper
	{
		/// <summary>
		/// Lưu thông tin đăng nhập của Customer
		/// </summary>
		public static void SetCustomerSession(ISession session, string userId, string userName, string avatar)
		{
			session.SetString(SessionKeys.IsLoggedIn, "true");
			session.SetString(SessionKeys.UserId, userId);
			session.SetString(SessionKeys.UserName, userName);
			session.SetString(SessionKeys.UserType, "Customer");
			session.SetString(SessionKeys.UserRole, "Customer");
			session.SetString(SessionKeys.UserAvatar, avatar ?? "");
			session.SetString(SessionKeys.IsCustomer, "true");
		}

		/// <summary>
		/// Lưu thông tin đăng nhập của Employee
		/// </summary>
		public static void SetEmployeeSession(ISession session, string userId, string userName, string role, string avatar)
		{
			session.SetString(SessionKeys.IsLoggedIn, "true");
			session.SetString(SessionKeys.UserId, userId);
			session.SetString(SessionKeys.UserName, userName);
			session.SetString(SessionKeys.UserType, "Employee");
			session.SetString(SessionKeys.UserRole, role ?? "Employee"); // Admin, Quản lý, Nhân viên
			session.SetString(SessionKeys.UserAvatar, avatar ?? "");
			session.SetString(SessionKeys.IsCustomer, "false");
		}

		/// <summary>
		/// Xóa toàn bộ session (đăng xuất)
		/// </summary>
		public static void ClearSession(ISession session)
		{
			session.Clear();
		}

		/// <summary>
		/// Kiểm tra user đã đăng nhập chưa
		/// </summary>
		public static bool IsLoggedIn(ISession session)
		{
			return session.GetString(SessionKeys.IsLoggedIn) == "true";
		}

		/// <summary>
		/// Kiểm tra user có phải là Customer không
		/// </summary>
		public static bool IsCustomer(ISession session)
		{
			return session.GetString(SessionKeys.UserType) == "Customer";
		}

		/// <summary>
		/// Kiểm tra user có phải là Employee không
		/// </summary>
		public static bool IsEmployee(ISession session)
		{
			return session.GetString(SessionKeys.UserType) == "Employee";
		}

		/// <summary>
		/// Kiểm tra user có phải là Admin không
		/// </summary>
		public static bool IsAdmin(ISession session)
		{
			return session.GetString(SessionKeys.UserRole) == "Admin";
		}

		/// <summary>
		/// Lấy User ID
		/// </summary>
		public static string? GetUserId(ISession session)
		{
			return session.GetString(SessionKeys.UserId);
		}

		/// <summary>
		/// Lấy User Name
		/// </summary>
		public static string? GetUserName(ISession session)
		{
			return session.GetString(SessionKeys.UserName);
		}

		/// <summary>
		/// Lấy User Role
		/// </summary>
		public static string? GetUserRole(ISession session)
		{
			return session.GetString(SessionKeys.UserRole);
		}
	}
}
