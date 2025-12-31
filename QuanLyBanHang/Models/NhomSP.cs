using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("NhomSP")]
	public class NhomSP
	{
		[Key]
		[Required(ErrorMessage = "Mã nhóm không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhóm tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhóm Sản Phẩm")]
		public string MaNhom { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên nhóm không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên nhóm tối đa 100 ký tự.")]
		[Display(Name = "Tên Nhóm Sản Phẩm")]
		public string TenNhom { get; set; } = string.Empty;
	}

	[Keyless]
	public class NhomSPDto
	{
		[Display(Name = "Mã Nhóm Sản Phẩm")]
		public string? MaNhom { get; set; }

		[Display(Name = "Tên Nhóm Sản Phẩm")]
		public string? TenNhom { get; set; }
	}
}
