using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("Xa")]
	public class Xa
	{
		[Key]
		[Required(ErrorMessage = "Mã xã không được để trống")]
		[StringLength(5, ErrorMessage = "Mã xã tối đa 5 ký tự")]
		[Display(Name = "Mã xã")]
		public string MaXa { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên xã không được để trống")]
		[StringLength(50, ErrorMessage = "Tên xã không được quá 50 ký tự")]
		[Display(Name = "Tên xã")]
		public string TenXa { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã tỉnh không được để trống")]
		[StringLength(2, ErrorMessage = "Mã tỉnh tối đa 2 ký tự")]
		[Display(Name = "Mã tỉnh")]
		public string MaTinh { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }

	}

	[Keyless]
	public class XaDTO
	{
		[Display(Name = "Mã xã")]
		public string? MaXa { get; set; }

		[Display(Name = "Tên xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Mã tỉnh")]
		public string? MaTinh { get; set; }

		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }
	}
}
