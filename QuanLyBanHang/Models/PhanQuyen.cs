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

		[ForeignKey("MaVT")]
		[Display(Name = "Vai Trò")]
		public virtual VaiTro? VaiTro { get; set; }

		[ForeignKey("MaNV")]
		[Display(Name = "Nhân Viên")]
		public virtual NhanVien? NhanVien { get; set; }

		[NotMapped]
		[Display(Name = "Tên Vai Trò")]
		public string TenVT { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên Nhân Viên")]
		public string TenNV { get; set; } = string.Empty;
	}

	public class PhanQuyenDto
	{
		public string MaVT { get; set; } = string.Empty;
		public string TenVT { get; set; } = string.Empty;
		public string MaNV { get; set; } = string.Empty;
		[NotMapped]
		public string TenNV { get; set; } = string.Empty;
	}
}

