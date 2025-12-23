using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("KhachHang")]
	public class KhachHang
	{
		[Key]
		[Required(ErrorMessage = "Mã khách hàng không được để trống")]
		[StringLength(10, ErrorMessage = "Mã khách hàng không được quá 10 ký tự")]
		[Display(Name = "Mã khách hàng")]
		public string MaKH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên khách hàng không được để trống")]
		[StringLength(50, ErrorMessage = "Tên khách hàng không được quá 50 ký tự")]
		[Display(Name = "Tên khách hàng")]
		public string TenKH { get; set; } = string.Empty;

		[StringLength(255)]
		[Display(Name = "Ảnh khách hàng")]
		public string? AnhKH { get; set; }

		[Required(ErrorMessage = "Giới tính không được để trống")]
		[Display(Name = "Giới Tính")]
		public bool? GioiTinh { get; set; }

		[EmailAddress(ErrorMessage = "Email không hợp lệ")]
		[StringLength(255)]
		[Display(Name = "Email khách hàng")]
		public string? EmailKH { get; set; }

		[Required(ErrorMessage = "Số điện thoại không được để trống")]
		[StringLength(10, ErrorMessage = "Số điện thoại không được quá 10 ký tự")]
		[Display(Name = "Số điện thoại")]
		public string DienThoaiKH { get; set; } = string.Empty;

		[StringLength(255)]
		[Display(Name = "Địa chỉ")]
		public string? DiaChiKH { get; set; }

		[Display(Name = "Mã xã")]
		public short? MaXa { get; set; }

		
		//   Tên đăng nhập & mật khẩu
		
		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		[StringLength(50, ErrorMessage = "Tên đăng nhập không được quá 50 ký tự")]
		[Display(Name = "Tên đăng nhập")]
		public string TenDNKH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[StringLength(255)]
		[Display(Name = "Mật khẩu")]
		public string MatKhauKH { get; set; } = string.Empty;

		
		//   Thuộc tính hiển thị
		
		[NotMapped]
		[Display(Name = "Tên xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }

		[NotMapped]
		[Display(Name = "Ảnh upload")]
		public IFormFile? AnhFile { get; set; }
		
		//   Quan hệ
		
		[ForeignKey("MaXa")]
		[Display(Name = "Xã")]
		public virtual Xa? Xa { get; set; }
	}

	[Keyless]
	public class KhachHangDetailView
	{
		[Display(Name = "Mã khách hàng")]
		public string? MaKH { get; set; }

		[Display(Name = "Tên khách hàng")]
		public string? TenKH { get; set; }

		[Display(Name = "Số điện thoại")]
		public string? DienThoaiKH { get; set; }

		[Display(Name = "Ảnh khách hàng")]
		public string? AnhKH { get; set; }

		[Display(Name = "Email khách hàng")]
		public string? EmailKH { get; set; }

		[Display(Name = "Địa chỉ")]
		public string? DiaChiKH { get; set; }

		[Display(Name = "Tên đăng nhập")]
		public string? TenDNKH { get; set; }

		[Display(Name = "Tên xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Tên tỉnh")]
		public string? TenTinh { get; set; }
	}
}
