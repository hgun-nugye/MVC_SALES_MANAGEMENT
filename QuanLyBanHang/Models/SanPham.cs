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
		[Display(Name = "Giá bán")]
		[Required(ErrorMessage = "Giá bán không được để trống")]
		[Range(0, double.MaxValue, ErrorMessage = "Giá bán phải >= 0")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal GiaBan { get; set; }

		[Display(Name = "Mô tả sản phẩm")]
		[Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
		public string MoTaSP { get; set; } = string.Empty;

		[Display(Name = "Ảnh minh họa")]
		public string? AnhMH { get; set; }

		[Display(Name = "Thành phần")]
		[Required(ErrorMessage = "Thành phần không được để trống")]
		public string ThanhPhan { get; set; } = string.Empty;

		[Display(Name = "Công dụng")]
		[Required(ErrorMessage = "Công dụng không được để trống")]
		public string CongDung { get; set; } = string.Empty;

		[Display(Name = "Hướng dẫn sử dụng")]
		[Required(ErrorMessage = "Hướng dẫn sử dụng không được để trống")]
		public string HDSD { get; set; } = string.Empty;

		[Display(Name = "Hướng dẫn bảo quản")]
		[Required(ErrorMessage = "Hướng dẫn bảo quản không được để trống")]
		public string HDBaoQuan { get; set; } = string.Empty;

		[Display(Name = "Trọng lượng")]
		[Required(ErrorMessage = "Trọng lượng không được để trống")]
		[Range(0, double.MaxValue, ErrorMessage = "Trọng lượng phải >= 0")]
		[Column(TypeName = "decimal(5,2)")]
		public decimal TrongLuong { get; set; }

		[Display(Name = "Mã Trạng Thái")]
		[Required(ErrorMessage = "Mã trạng thái không được để trống")]
		[StringLength(3, ErrorMessage = "Mã trạng thái tối đa 3 ký tự")]
		public string MaTT { get; set; } = string.Empty;

		[Display(Name = "Mã loại")]
		[Required(ErrorMessage = "Mã loại không được để trống")]
		public string MaLoai { get; set; } = string.Empty;

		[Display(Name = "Mã hãng")]
		[Required(ErrorMessage = "Mã hãng không được để trống")]
		[StringLength(5, ErrorMessage = "Mã hãng tối đa 5 ký tự")]
		public string MaHangSX { get; set; } = string.Empty;

		// Quan hệ
		//[ForeignKey("MaLoai")]
		//public LoaiSP? LoaiSP { get; set; }

		//[ForeignKey("MaHangSX")]
		//public Hang? Hang { get; set; }

		//public virtual ICollection<CTMH>? CTMHs { get; set; }
		//public virtual ICollection<CTBH>? CTBHs { get; set; }

		// Thuộc tính hiển thị (NotMapped)
		[NotMapped]
		[Display(Name = "Tên loại sản phẩm")]
		public string? TenLoai { get; set; }

		[NotMapped]
		[Display(Name = "Tên hàng")]
		public string? TenHangSX { get; set; }

		[NotMapped]
		[Display(Name = "Tên trạng thái")]
		public string? TenTT { get; set; }

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
		[Display(Name = "Giá bán")]
		[Column(TypeName = "decimal(18,2)")]
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

		[Display(Name = "Hướng dẫn bảo quản")]
		public string? HDBaoQuan { get; set; }

		[Display(Name = "Trọng lượng")]
		[Column(TypeName = "decimal(5,2)")]
		public decimal? TrongLuong { get; set; }

		[Display(Name = "Mã Trạng Thái")]
		public string? MaTT { get; set; }

		[Display(Name = "Mã loại")]
		public string? MaLoai { get; set; }

		[Display(Name = "Mã hàng")]
		public string? MaHangSX { get; set; }

		[Display(Name = "Tên loại sản phẩm")]
		public string? TenLoai { get; set; }

		[Display(Name = "Tên hàng")]
		public string? TenHangSX { get; set; }

		[Display(Name = "Tên trạng thái")]
		public string? TenTT { get; set; }

		[Display(Name = "Số lượng tồn")]
		public int? SoLuongTon { get; set; }
	}
}
