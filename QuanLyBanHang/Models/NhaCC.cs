using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("NhaCC")]
	public class NhaCC
	{
		[Key]
		[Required(ErrorMessage = "Mã nhà cung cấp không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhà cung cấp tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhà Cung Cấp")]
		public string MaNCC { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên nhà cung cấp không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên nhà cung cấp tối đa 100 ký tự.")]
		[Display(Name = "Tên Nhà Cung Cấp")]
		public string TenNCC { get; set; } = string.Empty;

		[Required(ErrorMessage = "Số điện thoại không được để trống.")]
		[Display(Name = "Số Điện Thoại")]
		[RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa chữ số")]

		public string DienThoaiNCC { get; set; } = string.Empty;

		[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
		[StringLength(100)]
		[Display(Name = "Email")]
		public string? EmailNCC { get; set; }

		[Required(ErrorMessage = "Địa chỉ không được để trống.")]
		[StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
		[Display(Name = "Địa Chỉ")]
		public string DiaChiNCC { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã xã không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã xã tối đa 5 ký tự")]
		[Display(Name = "Mã Xã")]
		public string MaXa { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Mã Tỉnh")]
		public string? MaTinh { get; set; }

		[NotMapped]
		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }
	}

	[Keyless]
	public class NhaCCDetailView
	{
		[Display(Name = "Mã Nhà Cung Cấp")]
		public string? MaNCC { get; set; }

		[Display(Name = "Tên Nhà Cung Cấp")]
		public string? TenNCC { get; set; }

		[Display(Name = "Số Điện Thoại")]
		public string? DienThoaiNCC { get; set; }

		[Display(Name = "Email")]
		public string? EmailNCC { get; set; }

		[Display(Name = "Địa Chỉ")]
		public string? DiaChiNCC { get; set; }

		[Display(Name = "Mã Xã")]
	public string? MaXa { get; set; }

	[Display(Name = "Tên Xã")]
	public string? TenXa { get; set; }

	[Display(Name = "Tên Tỉnh")]
	public string? TenTinh { get; set; }
}
}
