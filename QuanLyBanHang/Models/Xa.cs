using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
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

		//[NotMapped]
		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }

		//[ForeignKey("MaTinh")]
		//[Display(Name = "Tỉnh")]
		//public Tinh? Tinh { get; set; }
	}
}
