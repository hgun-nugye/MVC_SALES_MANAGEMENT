using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("DonBanHang")]
	public class DonBanHang
	{
		
		//  KHÓA CHÍNH
		
		[Key]
		//[Required]
		[StringLength(11, ErrorMessage = "Mã đơn bán hàng tối đa 11 ký tự.")]
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string MaDBH { get; set; } = string.Empty;

		
		//  NGÀY BÁN
		
		[Required(ErrorMessage = "Ngày bán hàng không được để trống.")]
		[DataType(DataType.Date)]
		[Display(Name = "Ngày Bán Hàng")]
		public DateTime NgayBH { get; set; } = DateTime.Now;

		
		//  KHÁCH HÀNG
		
		[Required(ErrorMessage = "Mã khách hàng không được để trống.")]
		[StringLength(10)]
		[Display(Name = "Mã Khách Hàng")]
		public string MaKH { get; set; } = string.Empty;

		// Tên khách hàng chỉ hiển thị, không map DB
		//[NotMapped]
		[Display(Name = "Tên Khách Hàng")]
		public string? TenKH { get; set; }

		//[ForeignKey("MaKH")]
		//[Display(Name = "Khách Hàng")]
		//public virtual KhachHang? KhachHang { get; set; }

		//  CHI TIẾT BÁN
		[Display(Name = "Chi Tiết Bán Hàng")]
		public virtual List<CTBH>? CTBHs { get; set; } = new();

	}

	// DTO hiển thị chi tiết đơn bán
	[Keyless]
	public class DonBanHangDetail
	{
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string? MaDBH { get; set; }

		[Display(Name = "Ngày Bán Hàng")]
		public DateTime NgayBH { get; set; }

		[Display(Name = "Mã Khách Hàng")]
		public string? MaKH { get; set; }

		[Display(Name = "Tên Khách Hàng")]
		public string? TenKH { get; set; }

		[Display(Name = "Mã Sản Phẩm")]
		public string? MaSP { get; set; }

		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[Display(Name = "Số Lượng Bán")]
		public int? SLB { get; set; }

		[Display(Name = "Đơn Giá Bán")]
		public decimal? DGB { get; set; }

		[NotMapped]
		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		[NotMapped]
		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal? ThanhTien => SLB * DGB;
	}

	// Model chỉnh sửa đơn bán
	public class DonBanHangEditCTBH
	{
		//[Required]
		[StringLength(11)]
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string MaDBH { get; set; } = string.Empty;

		//[Required]
		[Display(Name = "Ngày Bán Hàng")]
		public DateTime NgayBH { get; set; }

		[Required]
		[StringLength(10)]
		[Display(Name = "Mã Khách Hàng")]
		public string MaKH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Chi tiết bán hàng không được để trống.")]
		[Display(Name = "Chi Tiết Bán Hàng")]
		public List<CTBH> ChiTiet { get; set; } = new();
	}
}
