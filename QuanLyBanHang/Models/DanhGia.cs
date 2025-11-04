using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class DanhGia
	{
		[Key]
		[Display(Name = "Mã đánh giá")]
		public required int MaDG { get; set; }

		[Display(Name = "Mã sản phẩm")]
		public required string MaSP { get; set; }

		[Display(Name = "Mã khách hàng")]
		public required string MaKH { get; set; }

		[Display(Name = "Số sao")]
		[Range(1, 5)]
		public required byte SoSao { get; set; }

		[Display(Name = "Nội dung đánh giá")]
		public string? NoiDung { get; set; }

		[Display(Name = "Ngày đánh giá")]
		[DataType(DataType.DateTime)]
		public DateTime NgayDG { get; set; } = DateTime.Now;

		[ForeignKey("MaSP")]
		public SanPham? SanPham { get; set; }

		[ForeignKey("MaKH")]
		public KhachHang? KhachHang { get; set; }
	}
}
