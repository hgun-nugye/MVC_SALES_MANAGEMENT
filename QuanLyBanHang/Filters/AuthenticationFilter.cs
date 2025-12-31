using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyBanHang.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var isLoggedIn = session.GetString("IsLoggedIn") == "true";
            var userRole = session.GetString("UserRole");

            // Danh sách các controller/action yêu cầu quyền Admin/Employee
            var restrictedAdminControllers = new[]
            {
                "NhanVien", "PhanQuyen", "Dashboard", "NhaCC", "HangSX", "LoaiSP", "Nuoc", "Tinh", "Xa", "TrangThai", "DonBanHang", "DonMuaHang"
            };

            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Nếu chưa đăng nhập -> Chặn truy cập các trang quản trị
            if (!isLoggedIn)
            {
                if (restrictedAdminControllers.Contains(controller))
                {
                   context.Result = new RedirectToActionResult("Login", "Home", null);
                   return;
                }

                // Chặn create/edit/delete trong SanPham nếu chưa login (thực tế Guest chỉ xem)
                if (controller == "SanPham" && (action == "Create" || action == "Edit" || action == "Delete"))
                {
                     context.Result = new RedirectToActionResult("Login", "Home", null);
                     return;
                }
            }
            else
            {
                // Đã đăng nhập nhưng là Customer check quyền
                if (string.IsNullOrEmpty(userRole)) // Customer
                {
                     // Khách hàng KHÔNG được vào các trang quản trị thuần túy
                     var adminOnly = new[] { 
                        "NhanVien", "PhanQuyen", "Dashboard", "NhaCC", "HangSX", "LoaiSP", "Nuoc", "Tinh", "Xa", "TrangThai", "DonMuaHang" 
                     };

                     if (adminOnly.Contains(controller))
                     {
                         context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                         return;
                     }

                     // Khách hàng chỉ được xem danh sách đơn hàng của mình, không được sửa/xóa/quản lý đơn của người khác 
                     // (Logic này cần xử lý kỹ hơn trong Controller, nhưng ở đây chặn các Action quản trị nếu có)
                     if (controller == "DonBanHang" && (action == "Edit" || action == "Delete" || action == "Create")) 
                     {
                          // Khách hàng tạo đơn qua Cart/Checkout, không qua DonBanHang/Create trực tiếp
                          // Hoặc nếu DonBanHang/Create dùng cho Employee thì chặn.
                          // Tùy logic dự án, tạm thời chặn Create trực tiếp nếu không phải luồng Checkout.
                          if (action != "Create") { // Cho phép Create nếu logic checkout dùng action này (thường checkout gọi Cart.PlaceOrder)
                             context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                             return;
                          }
                     }

                      if (controller == "SanPham" && (action == "Create" || action == "Edit" || action == "Delete"))
                    {
                         context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                         return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
