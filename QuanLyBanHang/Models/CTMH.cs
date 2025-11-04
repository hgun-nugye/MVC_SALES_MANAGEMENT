using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class CTMH
	{
		[Key, Column(Order = 0)]
		[Display(Name = "Mã đơn mua hàng")]
		public required string MaDMH { get; set; }

		[Key, Column(Order = 1)]
		[Display(Name = "Mã sản phẩm")]
		public required string MaSP { get; set; }

		[Display(Name = "Số lượng mua")]
		public required	int SLM { get; set; }

		[Display(Name = "Đơn giá mua")]
		[DataType(DataType.Currency)]
		public required decimal DGM { get; set; }

		[ForeignKey("MaDMH")]
		public DonMuaHang? DonMuaHang { get; set; }

		[ForeignKey("MaSP")]
		public SanPham? SanPham { get; set; }
	}
}
