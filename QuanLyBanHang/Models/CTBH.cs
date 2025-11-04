using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class CTBH
	{
		[Key, Column(Order = 0)]
		[Display(Name = "Mã đơn bán hàng")]
		public required string MaDBH { get; set; }

		[Key, Column(Order = 1)]
		[Display(Name = "Mã sản phẩm")]
		public required string MaSP { get; set; }

		[Display(Name = "Số lượng bán")]
		public required int SLB { get; set; }

		[Display(Name = "Đơn giá bán")]
		[DataType(DataType.Currency)]
		public required decimal DGB { get; set; }

		[ForeignKey("MaDBH")]
		public DonBanHang? DonBanHang { get; set; }

		[ForeignKey("MaSP")]
		public SanPham? SanPham { get; set; }
	}
}
