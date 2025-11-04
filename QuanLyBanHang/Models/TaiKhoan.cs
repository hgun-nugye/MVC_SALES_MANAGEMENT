using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class TaiKhoan
	{
		[Key]
		[Display(Name = "Tên đăng nhập")]
		public required string TenUser { get; set; }

		[Required]
		[Display(Name = "Mật khẩu")]
		[DataType(DataType.Password)]
		public required string MatKhau { get; set; }

		[Display(Name = "Vai trò")]
		public required string VaiTro { get; set; }

		[Display(Name = "Trạng thái")]
		public required bool TrangThai { get; set; }

		[Display(Name = "Ngày tạo")]
		[DataType(DataType.DateTime)]
		public required DateTime NgayTao { get; set; }

		[Display(Name = "Mã khách hàng")]
		public string? MaKH { get; set; }

		[ForeignKey("MaKH")]
		public KhachHang? KhachHang { get; set; }
	}
}
