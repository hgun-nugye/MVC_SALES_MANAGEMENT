using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("Nuoc")]
	public class Nuoc
	{
		[Key]
		[Required(ErrorMessage = "Mã nước không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã nước tối đa 5 ký tự.")]
		[Display(Name = "Mã Nước")]
		public string MaNuoc { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên nước không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên nước tối đa 100 ký tự.")]
		[Display(Name = "Tên Nước")]
		public string TenNuoc { get; set; } = string.Empty;

		[Display(Name = "Danh sách Hãng")]
		public ICollection<Hang>? Hangs { get; set; }
	}

	[Keyless]
	public class NuocDto
	{
		[Display(Name = "Mã Nước")]
		public string? MaNuoc { get; set; }

		[Display(Name = "Tên Nước")]
		public string? TenNuoc { get; set; }
	}
}
