using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DonMuaHang
	{
		[Key]
		[Display(Name = "Mã đơn mua hàng")]
		public required string MaDMH { get; set; }

		[Display(Name = "Ngày mua hàng")]
		[DataType(DataType.Date)]
		public required DateTime NgayMH { get; set; }

		[Display(Name = "Mã nhà cung cấp")]
		public required string MaNCC { get; set; }

		[ForeignKey("MaNCC")]
		[Display(Name = "Nhà cung cấp")]
		public NhaCC? NhaCC { get; set; }
	}
}
