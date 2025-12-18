using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("Xa")]
	public class Xa
	{
		[Key]
		[Display(Name = "Mã xã")]
		public short MaXa { get; set; }

		[Required(ErrorMessage = "Tên xã không được để trống")]
		[StringLength(90, ErrorMessage = "Tên xã không được quá 90 ký tự")]
		[Display(Name = "Tên xã")]
		public string TenXa { get; set; } = string.Empty;

		[Display(Name = "Mã tỉnh")]
		public short MaTinh { get; set; }

		[NotMapped]
		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }

	}

	[Keyless]
	public class XaDTO
	{
		[Display(Name = "Mã xã")]
		public short? MaXa { get; set; }

		[Display(Name = "Tên xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Mã tỉnh")]
		public short? MaTinh { get; set; }

		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }
	}
}
