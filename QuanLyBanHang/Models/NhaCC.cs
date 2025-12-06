using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public short MaXa { get; set; }
		[NotMapped]
		public string? TenXa { get; set; }
		[NotMapped]
		public short? MaTinh { get; set; }
		[NotMapped]
		public string? TenTinh { get; set; }	
		public ICollection<SanPham>? SanPhams { get; set; }

	}

	[Keyless]
	public class NhaCCDetailView
	{
		public string? MaNCC { get; set; }
		public string? TenNCC { get; set; }
		public string? DienThoaiNCC { get; set; }
		public string? EmailNCC { get; set; }
		public string? DiaChiNCC { get; set; }
		public short MaXa { get; set; }
		public string? TenXa { get; set; }
		public string? TenTinh { get; set; }
	}
}
