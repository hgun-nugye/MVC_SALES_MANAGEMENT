using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("SanPham")]
	public class SanPham
	{
		[Key]
		[Display(Name = "Mã sản phẩm")]
		[Required(ErrorMessage = "Mã sản phẩm không được để trống")]
		[StringLength(10, ErrorMessage = "Mã sản phẩm tối đa 10 ký tự")]
		public string MaSP { get; set; } = string.Empty;

		[Display(Name = "Tên sản phẩm")]
		[Required(ErrorMessage = "Tên sản phẩm không được để trống")]
		[StringLength(50, ErrorMessage = "Tên sản phẩm tối đa 50 ký tự")]
		public string TenSP { get; set; } = string.Empty;

		[DataType(DataType.Currency)]
		[Display(Name = "Đơn giá")]
		[Required(ErrorMessage = "Đơn giá không được để trống")]
		[Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải >= 0")]
		public decimal DonGia { get; set; }

		[DataType(DataType.Currency)]
		[Display(Name = "Giá bán")]
		[Required(ErrorMessage = "Giá bán không được để trống")]
		[Range(0, double.MaxValue, ErrorMessage = "Giá bán phải >= 0")]
		public decimal GiaBan { get; set; }

		[Display(Name = "Mô tả sản phẩm")]
		[StringLength(1000)]
		public string? MoTaSP { get; set; }

		[Display(Name = "Ảnh minh họa")]
		public string? AnhMH { get; set; }

		[Display(Name = "Thành phần")]
		public string? ThanhPhan { get; set; }

		[Display(Name = "Công dụng")]
		public string? CongDung { get; set; }

		[Display(Name = "Hướng dẫn sử dụng")]
		public string? HDSD { get; set; }

		[Display(Name = "Xuất xứ")]
		public string? XuatXu { get; set; }

		[Display(Name = "Bảo quản")]
		public string? BaoQuan { get; set; }

		[Display(Name = "Trạng thái")]
		[Required(ErrorMessage = "Trạng thái không được để trống")]
		[StringLength(50)]
		public string TrangThai { get; set; } = string.Empty;

		[Display(Name = "Số lượng tồn")]
		[Required(ErrorMessage = "Số lượng tồn không được để trống")]
		[Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải >= 0")]
		public int SoLuongTon { get; set; }

		[Display(Name = "Mã loại")]
		[Required(ErrorMessage = "Mã loại không được để trống")]
		public string MaLoai { get; set; } = string.Empty;

		[Display(Name = "Mã hàng")]
		[Required(ErrorMessage = "Mã hàng không được để trống")]
		public string MaHang { get; set; } = string.Empty;

		// Quan hệ
		[ForeignKey("MaLoai")]
		public LoaiSP? LoaiSP { get; set; }

		[ForeignKey("MaHang")]
		public Hang? Hang { get; set; }

		public virtual ICollection<CTMH>? CTMHs { get; set; }
		public virtual ICollection<CTBH>? CTBHs { get; set; }

		// Thuộc tính hiển thị (NotMapped)
		[NotMapped]
		[Display(Name = "Tên loại sản phẩm")]
		public string? TenLoai { get; set; }

		[NotMapped]
		[Display(Name = "Tên hàng")]
		public string? TenHang { get; set; }

		[NotMapped]
		[Display(Name = "Upload ảnh")]
		public IFormFile? AnhFile { get; set; }
	}

	[Keyless]
	public class SanPhamDto
	{
		[Display(Name = "Mã sản phẩm")]
		public string? MaSP { get; set; }

		[Display(Name = "Tên sản phẩm")]
		public string? TenSP { get; set; }

		[DataType(DataType.Currency)]
		[Display(Name = "Đơn giá")]
		public decimal? DonGia { get; set; }

		[DataType(DataType.Currency)]
		[Display(Name = "Giá bán")]
		public decimal? GiaBan { get; set; }

		[Display(Name = "Mô tả sản phẩm")]
		public string? MoTaSP { get; set; }

		[Display(Name = "Ảnh minh họa")]
		public string? AnhMH { get; set; }

		[Display(Name = "Thành phần")]
		public string? ThanhPhan { get; set; }

		[Display(Name = "Công dụng")]
		public string? CongDung { get; set; }

		[Display(Name = "Hướng dẫn sử dụng")]
		public string? HDSD { get; set; }

		[Display(Name = "Xuất xứ")]
		public string? XuatXu { get; set; }

		[Display(Name = "Bảo quản")]
		public string? BaoQuan { get; set; }

		[Display(Name = "Trạng thái")]
		public string? TrangThai { get; set; }

		[Display(Name = "Số lượng tồn")]
		public int? SoLuongTon { get; set; }

		[Display(Name = "Mã loại")]
		public string? MaLoai { get; set; }

		[Display(Name = "Mã hàng")]
		public string? MaHang { get; set; }

		[Display(Name = "Tên loại sản phẩm")]
		public string? TenLoai { get; set; }

		[Display(Name = "Tên hàng")]
		public string? TenHang { get; set; }
	}
}
