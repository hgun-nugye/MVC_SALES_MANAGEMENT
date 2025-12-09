using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("NhanVien")]
	public class NhanVien
	{
		[Key]
		[Required(ErrorMessage = "Mã nhân viên không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhân viên tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhân Viên")]
		public string MaNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên nhân viên không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên nhân viên không được quá 100 ký tự.")]
		[Display(Name = "Tên Nhân Viên")]
		public string TenNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "Vai trò không được để trống.")]
		[StringLength(50)]
		[Display(Name = "Vai Trò")]
		[RegularExpression("Quản Trị|Nhân Viên|Quản Lý",
			ErrorMessage = "Vai trò phải là 'Quản Trị', 'Nhân Viên' hoặc 'Quản Lý'.")]
		public string VaiTro { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
		[StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự.")]
		[Display(Name = "Tên Đăng Nhập")]
		public string TenDNNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mật khẩu không được để trống.")]
		[StringLength(255)]
		[DataType(DataType.Password)]
		[Display(Name = "Mật Khẩu")]
		public string MatKhauNV { get; set; } = string.Empty;

		// Quan hệ: Danh sách đơn mua hàng do nhân viên thực hiện
		public virtual List<DonMuaHang>? DonMuaHangs { get; set; }
	}
}
