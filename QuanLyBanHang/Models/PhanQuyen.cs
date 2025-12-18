using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("PhanQuyen")]
	public class PhanQuyen
	{
		[Key, Column(Order = 0)]
		[Required(ErrorMessage = "Mã vai trò không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã vai trò tối đa 5 ký tự.")]
		[Display(Name = "Mã Vai Trò")]
		public string MaVT { get; set; } = string.Empty;

		[Key, Column(Order = 1)]
		[Required(ErrorMessage = "Mã nhân viên không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhân viên tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhân Viên")]
		public string MaNV { get; set; } = string.Empty;

		// Navigation properties
		[ForeignKey("MaVT")]
		[Display(Name = "Vai Trò")]
		public virtual VaiTro? VaiTro { get; set; }

		[ForeignKey("MaNV")]
		[Display(Name = "Nhân Viên")]
		public virtual NhanVien? NhanVien { get; set; }
	}
}

