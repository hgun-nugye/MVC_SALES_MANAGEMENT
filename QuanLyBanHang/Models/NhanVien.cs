using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("NhanVien")]
	public class NhanVien
	{
		[Key]
		[Required(ErrorMessage = "Mã nhân viên không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhân viên tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhân Viên")]
		public string MaNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "CCCD không được để trống.")]
		[StringLength(12, ErrorMessage = "CCCD phải có 12 ký tự.")]
		[Display(Name = "CCCD")]
		public string CCCD { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên nhân viên không được để trống.")]
		[StringLength(100, ErrorMessage = "Tên nhân viên không được quá 100 ký tự.")]
		[Display(Name = "Tên Nhân Viên")]
		public string TenNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "Giới tính không được để trống.")]
		[Display(Name = "Giới Tính")]
		public bool GioiTinh { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Ngày Sinh")]
		public DateTime? NgaySinh { get; set; }

		[Required(ErrorMessage = "Số điện thoại không được để trống.")]
		[StringLength(10, ErrorMessage = "Số điện thoại tối đa 10 ký tự.")]
		[Display(Name = "Số Điện Thoại")]
		public string SDT { get; set; } = string.Empty;

		[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
		[StringLength(50, ErrorMessage = "Email tối đa 50 ký tự.")]
		[Display(Name = "Email")]
		public string? Email { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Ngày Làm")]
		public DateTime? NgayLam { get; set; }

		[StringLength(255)]
		[Display(Name = "Ảnh Nhân Viên")]
		public string? AnhNV { get; set; }

		[Required(ErrorMessage = "Địa chỉ không được để trống.")]
		[StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
		[Display(Name = "Địa Chỉ")]
		public string DiaChiNV { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã xã không được để trống.")]
		[Display(Name = "Mã Xã")]
		public short? MaXa { get; set; }

		[StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự.")]
		[Display(Name = "Tên Đăng Nhập")]
		public string? TenDNNV { get; set; }

		[StringLength(255)]
		[DataType(DataType.Password)]
		[Display(Name = "Mật Khẩu")]
		public string? MatKhauNV { get; set; }

		[NotMapped]
		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }
		[NotMapped]
		[Display(Name = "Vai Trò")]
		public string? TenVT { get; set; }

		[NotMapped]
		[Display(Name = "Upload ảnh")]
		public IFormFile? AnhFile { get; set; }

		// Quan hệ
		[ForeignKey("MaXa")]
		[Display(Name = "Xã")]
		public virtual Xa? Xa { get; set; }
	}

	[Keyless]
	public class NhanVienDetailView
	{
		
		[Display(Name = "Mã Nhân Viên")]
		public string MaNV { get; set; } = string.Empty;

		
		[Display(Name = "CCCD")]
		public string CCCD { get; set; } = string.Empty;

		[Display(Name = "Tên Nhân Viên")]
		public string TenNV { get; set; } = string.Empty;
				
		[Display(Name = "Giới Tính")]
		public bool GioiTinh { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Ngày Sinh")]
		public DateTime? NgaySinh { get; set; }
	
		[Display(Name = "Số Điện Thoại")]
		public string SDT { get; set; } = string.Empty;

		
		[Display(Name = "Email")]
		public string? Email { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Ngày Làm")]
		public DateTime? NgayLam { get; set; }

		[StringLength(255)]
		[Display(Name = "Ảnh Nhân Viên")]
		public string? AnhNV { get; set; }
		
		[Display(Name = "Địa Chỉ")]
		public string DiaChiNV { get; set; } = string.Empty;

		[Display(Name = "Mã Xã")]
		public short MaXa { get; set; }

		[Display(Name = "Tên Đăng Nhập")]
		public string? TenDNNV { get; set; }
	
		[Display(Name = "Mật Khẩu")]
		public string? MatKhauNV { get; set; }

		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		[Display(Name = "Vai Trò")]
		public string? TenVT { get; set; }

	}
}
