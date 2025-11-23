using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class GianHang
	{
		[Key]
		[StringLength(10)]
		public string? MaGH { get; set; } = null!;

		[Required(ErrorMessage = "Tên gian hàng không được để trống!")]
		[StringLength(100)]
		public string TenGH { get; set; } = null!;

		[StringLength(255)]
		public string? MoTaGH { get; set; }

		[DataType(DataType.Date)]
		public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.Now);

		[Required(ErrorMessage = "Điện thoại không được để trống!")]
		[StringLength(15)]
		public string DienThoaiGH { get; set; } = null!;

		[EmailAddress(ErrorMessage = "Email không hợp lệ!")]
		[StringLength(100)]
		public string? EmailGH { get; set; }

		[Required(ErrorMessage = "Địa chỉ không được để trống!")]
		[StringLength(200)]
		public string DiaChiGH { get; set; } = null!;

		// Quan hệ 1-n: GianHang có nhiều SảnPhẩm
		public ICollection<SanPham>? DsSanPham { get; set; }		
	}

	
	[Keyless]
	public class GianHangCountDto
	{
		public int TotalRecords { get; set; }
	}

}
