using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTBH")]
	public class CTBH
	{
		
		[Key, Column(Order = 0)]
		//[Required(ErrorMessage = "Mã đơn bán hàng không được để trống.")]
		[StringLength(11, ErrorMessage = "Mã đơn bán hàng tối đa 11 ký tự.")]
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string? MaDBH { get; set; } = string.Empty;

		[Key, Column(Order = 1)]
		//[Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã sản phẩm tối đa 10 ký tự.")]
		[Display(Name = "Mã Sản Phẩm")]
		public string? MaSP { get; set; } = string.Empty;

		
		// THUỘC TÍNH
		
		//[Required(ErrorMessage = "Số lượng bán không được để trống.")]
		[Range(1, int.MaxValue, ErrorMessage = "Số lượng bán phải lớn hơn 0.")]
		[Display(Name = "Số Lượng Bán")]
		public int? SLB { get; set; }

		//[Required(ErrorMessage = "Đơn giá bán không được để trống.")]
		[Range(0, double.MaxValue, ErrorMessage = "Đơn giá bán phải >= 0.")]
		[Column(TypeName = "decimal(18,2)")]
		[Display(Name = "Đơn Giá Bán")]
		public decimal? DGB { get; set; }
		
		[NotMapped]
		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal? ThanhTien => SLB * DGB;
	}
		
	[Keyless]
	public class CTBHDetailDto
	{
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string? MaDBH { get; set; }

		[Display(Name = "Mã Sản Phẩm")]
		public string? MaSP { get; set; }

		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[Display(Name = "Giá Bán")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal? GiaBan { get; set; }

		[Display(Name = "Số Lượng Bán")]
		public int SLB { get; set; }
		
		[Display(Name = "Số Lượng Tồn")]
		public int SoLuongTon { get; set; }

		[Display(Name = "Đơn Giá Bán")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal DGB { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal? ThanhTien => SLB * DGB;

		[Display(Name = "Ngày Bán Hàng")]
		[DataType(DataType.Date)]
		public DateTime NgayBH { get; set; }

		[Display(Name = "Tên Khách Hàng")]
		public string? TenKH { get; set; }
	}
}
