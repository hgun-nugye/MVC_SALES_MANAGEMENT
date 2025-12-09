using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTMH")]
	public class CTMH
	{
		// ======================
		// KHÓA CHÍNH GHÉP
		// ======================
		[Key, Column(Order = 0)]
		[Required(ErrorMessage = "Mã đơn mua hàng không được để trống.")]
		[StringLength(11, ErrorMessage = "Mã đơn mua hàng tối đa 11 ký tự.")]
		[Display(Name = "Mã Đơn Mua Hàng")]
		public string MaDMH { get; set; } = string.Empty;

		[Key, Column(Order = 1)]
		[Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
		[StringLength(10, ErrorMessage = "Mã sản phẩm tối đa 10 ký tự.")]
		[Display(Name = "Mã Sản Phẩm")]
		public string MaSP { get; set; } = string.Empty;

		// ======================
		// THUỘC TÍNH
		// ======================
		[Required(ErrorMessage = "Số lượng mua không được để trống.")]
		[Range(1, int.MaxValue, ErrorMessage = "Số lượng mua phải lớn hơn 0.")]
		[Display(Name = "Số Lượng Mua")]
		public int SLM { get; set; }

		[Required(ErrorMessage = "Đơn giá mua không được để trống.")]
		[Range(0, double.MaxValue, ErrorMessage = "Đơn giá mua phải >= 0.")]
		[Column(TypeName = "decimal(18,2)")]
		[Display(Name = "Đơn Giá Mua")]
		public decimal DGM { get; set; }

		// ======================
		// QUAN HỆ
		// ======================
		[ForeignKey("MaDMH")]
		[Display(Name = "Đơn Mua Hàng")]
		public virtual DonMuaHang? DonMuaHang { get; set; }

		[ForeignKey("MaSP")]
		[Display(Name = "Sản Phẩm")]
		public virtual SanPham? SanPham { get; set; }

		// ======================
		// THUỘC TÍNH HIỂN THỊ (không lưu DB)
		// ======================
		[NotMapped]
		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal ThanhTien => SLM * DGM;
	}

	// ======================
	// DTO chi tiết CTMH
	// ======================
	[Keyless]
	public class CTMHDetailDto
	{
		[Display(Name = "Mã Đơn Mua Hàng")]
		public string? MaDMH { get; set; }

		[Display(Name = "Mã Sản Phẩm")]
		public string? MaSP { get; set; }

		[Display(Name = "Tên Sản Phẩm")]
		public string? TenSP { get; set; }

		[Display(Name = "Số Lượng Mua")]
		public int SLM { get; set; }

		[Display(Name = "Đơn Giá Mua")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal DGM { get; set; }

		[NotMapped]
		[Display(Name = "Thành Tiền")]
		public decimal ThanhTien => SLM * DGM;
	}
}
