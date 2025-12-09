using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("Hang")]
	public class Hang
	{
		[Key]
		[Required(ErrorMessage = "Mã hãng không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã hãng tối đa 5 ký tự.")]
		[Display(Name = "Mã Hãng")]
		public string MaHang { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên hãng không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên hãng tối đa 100 ký tự.")]
		[Display(Name = "Tên Hãng")]
		public string TenHang { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã nước không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã nước tối đa 5 ký tự.")]
		[Display(Name = "Mã Nước")]
		public string MaNuoc { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên Nước")]
		public string? TenNuoc { get; set; }

		[ForeignKey("MaNuoc")]
		[Display(Name = "Nước Sản Xuất")]
		public virtual Nuoc? Nuoc { get; set; }
	}
}
