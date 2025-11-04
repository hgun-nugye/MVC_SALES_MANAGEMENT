using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class KhuyenMai
	{
		[Key]
		[Display(Name = "Mã khuyến mãi")]
		public required string MaKM { get; set; }

		[Display(Name = "Tên khuyến mãi")]
		public required string TenKM { get; set; }

		[Display(Name = "Mô tả")]
		public string? MoTaKM { get; set; }

		[Display(Name = "Giá trị khuyến mãi (%)")]
		public required decimal GiaTriKM { get; set; }

		[Display(Name = "Ngày bắt đầu")]
		[DataType(DataType.Date)]
		public required DateTime NgayBatDau { get; set; }

		[Display(Name = "Ngày kết thúc")]
		[DataType(DataType.Date)]
		public required DateTime NgayKetThuc { get; set; }

		[Display(Name = "Điều kiện áp dụng")]
		public string? DieuKienApDung { get; set; }

		[Display(Name = "Trạng thái")]
		public required bool TrangThai { get; set; }
	}
}
