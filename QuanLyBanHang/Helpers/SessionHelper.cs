namespace QuanLyBanHang.Helpers
{
	/// Helper class để quản lý Session keys một cách tập trung

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


	/// Helper class để làm việc với Session
	public static class SessionHelper
	{
		/// Lưu thông tin đăng nhập của Customer
		public static void SetCustomerSession(ISession session, string userId, string userName, string avatar)
		{
			session.SetString(SessionKeys.IsLoggedIn, "true");
			session.SetString(SessionKeys.UserId, userId);
			session.SetString(SessionKeys.UserName, userName);
			session.SetString(SessionKeys.UserType, "Customer");
			session.SetString(SessionKeys.UserRole, "Khách hàng");
			session.SetString(SessionKeys.UserAvatar, avatar ?? "");
			session.SetString(SessionKeys.IsCustomer, "true");
		}


		/// Lưu thông tin đăng nhập của Employee
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


		/// Xóa toàn bộ session (đăng xuất)
		public static void ClearSession(ISession session)
		{
			session.Clear();
		}

		/// Kiểm tra user đã đăng nhập chưa
		public static bool IsLoggedIn(ISession session)
		{
			return session.GetString(SessionKeys.IsLoggedIn) == "true";
		}

		/// Lấy User ID
		public static string? GetUserId(ISession session)
		{
			return session.GetString(SessionKeys.UserId);
		}
	}
}
