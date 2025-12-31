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
		[StringLength(11, ErrorMessage = "Mã đơn bán hàng tối đa 11 ký tự.")]
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string? MaDBH { get; set; }


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

		[Required(ErrorMessage = "Địa chỉ giao hàng không được để trống.")]
		[StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
		[Display(Name = "Địa Chỉ Giao Hàng")]
		public string DiaChiDBH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã xã không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã xã tối đa 5 ký tự")]
		[Display(Name = "Mã Xã")]
		public string MaXa { get; set; } = string.Empty;

		// Tên khách hàng chỉ hiển thị, không map DB
		[NotMapped]
		[Display(Name = "Tên Khách Hàng")]
		public string? TenKH { get; set; }

		[NotMapped]
		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[NotMapped]
		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		//  CHI TIẾT BÁN
		[Display(Name = "Chi Tiết Bán Hàng")]
		public virtual List<CTBH>? CTBHs { get; set; } = new();

		// TRẠNG THÁI ĐƠN BÁN HÀNG
		[Required(ErrorMessage = "Mã TT không được để trống.")]
		[Display(Name = "Mã Trạng thái")]
		public string? MaTTBH { get; set; }

		[NotMapped]
		[Display(Name = "Trạng thái")]
		public string? TenTTBH { get; set; }

	}

	// DTO hiển thị chi tiết đơn bán
	[Keyless]
	public class DonBanHangDetailDto
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

		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		[Display(Name = "Địa Chỉ Đơn Bán Hàng")]
		public string? DiaChiDBH { get; set; }

		[Display(Name = "Mã Xã")]
		public string? MaXa { get; set; }

		[Display(Name = "Mã trạng thái đơn hàng")]
		public string? MaTTBH { get; set; }

		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Trạng thái đơn hàng")]
		public string? TenTTBH { get; set; }
	}

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

		[Display(Name = "Tên Tỉnh")]
		public string? TenTinh { get; set; }

		[Display(Name = "Địa Chỉ Đơn Bán Hàng")]
		public string? DiaChiDBH { get; set; }

		[Display(Name = "Mã Xã")]
		public string? MaXa { get; set; }

		[Display(Name = "Mã trạng thái đơn hàng")]
		public string? MaTTBH { get; set; }

		[Display(Name = "Tên Xã")]
		public string? TenXa { get; set; }

		[Display(Name = "Thành Tiền")]
		public decimal? ThanhTien => SLB * DGB;

		[Display(Name = "Trạng thái đơn hàng")]
		public string? TenTTBH { get; set; }
	}

	// Model chỉnh sửa đơn bán
	public class DonBanHangEditCTBH
	{
		//[Required]
		[StringLength(11)]
		[Display(Name = "Mã Đơn Bán Hàng")]
		public string? MaDBH { get; set; }

		//[Required]
		[Display(Name = "Ngày Bán Hàng")]
		public DateTime NgayBH { get; set; }

		[Required]
		[StringLength(10)]
		[Display(Name = "Mã Khách Hàng")]
		public string MaKH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Địa chỉ đơn bán hàng không được để trống.")]
		[StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
		[Display(Name = "Địa Chỉ Đơn Bán Hàng")]
		public string DiaChiDBH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mã xã không được để trống.")]
		[StringLength(5, ErrorMessage = "Mã xã tối đa 5 ký tự")]
		[Display(Name = "Mã Xã")]
		public string MaXa { get; set; } = string.Empty;

		[NotMapped]
		public string? TenXa { get; set; }
		[NotMapped]
		public string? TenTinh { get; set; }

		[Required(ErrorMessage = "Chi tiết bán hàng không được để trống.")]
		[Display(Name = "Chi Tiết Bán Hàng")]
		public List<CTBH> ChiTiet { get; set; } = new();

		[Required(ErrorMessage = "Trạng thái đơn hàng không được để trống.")]
		[Display(Name = "Trạng thái đơn hàng")]
		public string MaTTBH { get; set; } = string.Empty;
	}
}
