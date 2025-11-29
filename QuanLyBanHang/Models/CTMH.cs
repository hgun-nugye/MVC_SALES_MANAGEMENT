using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("CTMH")]
	public class CTMH
	{
		[Key, Column(Order = 0)]
		[Display(Name = "Mã đơn mua hàng")]
		public string? MaDMH { get; set; }

		[Key, Column(Order = 1)]
		[Display(Name = "Mã sản phẩm")]
		public string? MaSP { get; set; }

		[Required(ErrorMessage = "Số lượng mua không được để trống")]
		[Display(Name = "Số lượng mua")]
		[Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
		public int SLM { get; set; }

		[Required(ErrorMessage = "Đơn giá mua không được để trống")]
		[Display(Name = "Đơn giá mua")]
		[Column(TypeName = "money")]
		public decimal DGM { get; set; }

		[ForeignKey("MaDMH")]
		public virtual DonMuaHang? DonMuaHang { get; set; }

		[ForeignKey("MaSP")]
		public virtual SanPham? SanPham { get; set; }

		public string? TenSP { get; set; }
	}
	[Keyless]
	public class CTMHDetailDto
	{
		public string MaDMH { get; set; }
		public string MaSP { get; set; }
		public int SLM { get; set; }
		public decimal DGM { get; set; }
		public string TenSP { get; set; }
	}
}
