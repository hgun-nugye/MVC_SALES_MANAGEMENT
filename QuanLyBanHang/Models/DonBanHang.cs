using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DonBanHang
	{
		[Key]
		[Display(Name = "Mã đơn bán hàng")]
		public required string MaDBH { get; set; }

		[Display(Name = "Ngày bán hàng")]
		[DataType(DataType.Date)]
		public required DateTime NgayBH { get; set; }

		[Display(Name = "Mã khách hàng")]
		public required string MaKH { get; set; }

		[ForeignKey("MaKH")]
		[Display(Name = "Khách hàng")]
		public KhachHang? KhachHang { get; set; }
	}
}
