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
		[StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự.")]
		[Display(Name = "Số Điện Thoại")]
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
		[Display(Name = "Mã Xã")]
		public short MaXa { get; set; }

		[NotMapped]
		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Mã Tỉnh")]
		public short? MaTinh { get; set; }

		[NotMapped]
		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		//[Display(Name = "Danh Sách Sản Phẩm")]
		//public ICollection<SanPham>? SanPhams { get; set; }
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
		public short MaXa { get; set; }

		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }
	}
}
