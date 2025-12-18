using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("VaiTro")]
	public class VaiTro
	{
		[Key]
		[Required(ErrorMessage = "Mã vai trò không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã vai trò tối đa 5 ký tự.")]
		[Display(Name = "Mã Vai Trò")]
		public string MaVT { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên vai trò không được để trống.")]
		[StringLength(50, ErrorMessage = "Tên vai trò tối đa 50 ký tự.")]
		[Display(Name = "Tên Vai Trò")]
		public string TenVT { get; set; } = string.Empty;
	}
}

