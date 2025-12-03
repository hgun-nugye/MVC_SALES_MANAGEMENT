using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.Models
{
	public class NhaCC
	{
		[Key]
		public string? MaNCC { get; set; }

		public string? TenNCC { get; set; }

		public string? DienThoaiNCC { get; set; }

		[EmailAddress]
		public string? EmailNCC { get; set; }

		public string? DiaChiNCC { get; set; }
		public short? MaXa { get; set; }

		public ICollection<SanPham>? SanPhams { get; set; }

	}
}
