using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class KhachHang
	{
		[Key]
		public string? MaKH { get; set; }

		public string? TenKH { get; set; }

		public string? DienThoaiKH { get; set; }
		public string? AnhKH { get; set; }

		[EmailAddress]
		public string? EmailKH { get; set; }

		public string? DiaChiKH { get; set; }
		public short MaXa { get; set; }

		[NotMapped]
		public string? TenXa { get; set; }
		[NotMapped]
		public string? TenTinh { get; set; }

		[NotMapped]
		public IFormFile? AnhFile { get; set; }
	}

	[Keyless]
	public class KhachHangDetailView
	{

		public string? MaKH { get; set; }

		public string? TenKH { get; set; }

		public string? DienThoaiKH { get; set; }

		public string? EmailKH { get; set; }

		public string? DiaChiKH { get; set; }

		public string? AnhKH { get; set; }
		public short MaXa { get; set; }
		public string? TenXa { get; set; }
		public string? TenTinh { get; set; }

	}
}
