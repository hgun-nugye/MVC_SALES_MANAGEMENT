using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("DonMuaHang")]
	public class DonMuaHang
	{
		[Key]
		//[Required(ErrorMessage = "Mã đơn mua hàng không được để trống.")]
		[StringLength(11, ErrorMessage = "Mã đơn mua hàng tối đa 11 ký tự.")]
		[Display(Name = "Mã Đơn Mua Hàng")]
		public string? MaDMH { get; set; }

		[Required(ErrorMessage = "Ngày mua hàng không được để trống.")]
		[DataType(DataType.Date)]
		[Display(Name = "Ngày Mua Hàng")]
		public DateTime NgayMH { get; set; }

		[Required(ErrorMessage = "Mã nhà cung cấp không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhà cung cấp tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhà Cung Cấp")]
		public string MaNCC { get; set; } = string.Empty; 
		[NotMapped]
		[Display(Name = "Tên Nhà Cung Cấp")]
		public string? TenNCC { get; set; }

		[Required(ErrorMessage = "Mã nhân viên không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã nhân viên tối đa 10 ký tự.")]
		[Display(Name = "Mã Nhân Viên")]
		public string MaNV { get; set; } = string.Empty;

		[NotMapped]
		[Display(Name = "Tên Nhân Viên")]
		public string? TenNV { get; set; }

		[Display(Name = "Chi Tiết Mua Hàng")]
		public virtual List<CTMH>? CTMHs { get; set; }
	}

	[Keyless]
	public class DonMuaHangDetail
	{
		[Display(Name = "Mã Đơn Mua Hàng")]
		public string? MaDMH { get; set; }

		[Display(Name = "Ngày Mua Hàng")]
		[DataType(DataType.Date)]
		public DateTime NgayMH { get; set; }

		[Display(Name = "Mã Nhà Cung Cấp")]
		public string? MaNCC { get; set; }

		[Display(Name = "Tên Nhà Cung Cấp")]
		public string? TenNCC { get; set; }

		[Display(Name = "Mã Nhân Viên")]
		public string? MaNV { get; set; }

		[Display(Name = "Tên Nhân Viên")]
		public string? TenNV { get; set; }

		[Display(Name = "Mã Sản Phẩm")]
		public string? MaSP { get; set; }

		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[Display(Name = "Số Lượng Mua")]
		public int? SLM { get; set; }

		[Display(Name = "Đơn Giá Mua")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal? DGM { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal? ThanhTien => SLM * DGM;
	}

	public class DonMuaHangEditCTMH
	{
		[Display(Name = "Mã Đơn Mua Hàng")]
		public string? MaDMH { get; set; }

		[Display(Name = "Ngày Mua Hàng")]
		[DataType(DataType.Date)]
		public DateTime NgayMH { get; set; }

		[Display(Name = "Mã Nhà Cung Cấp")]
		public string? MaNCC { get; set; }

		[Display(Name = "Mã Nhân Viên")]
		public string? MaNV { get; set; }

		[Display(Name = "Chi Tiết Mua Hàng")]
		public List<CTMH> ChiTiet { get; set; } = new();
	}
}
