using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class LoaiSP
	{
		[Key]
		public string? MaLoai { get; set; } = null!;

		public string? TenLoai { get; set; } = null!;

		// Khóa ngoại đến NhomSP
		public string? MaNhom { get; set; } = null!;

		[NotMapped]
		public string? TenNhom { get; set; }

		[ForeignKey(nameof(MaNhom))]
		public NhomSP? NhomSP { get; set; } = null;
		public ICollection<SanPham>? SanPhams { get; set; }

	}

	[Keyless]
	public class LoaiSPDto
	{		
		public string? MaLoai { get; set; } = null!;

		public string? TenLoai { get; set; } = null!;

		public string? MaNhom { get; set; } = null!;

		public string? TenNhom { get; set; }

	}

	[Keyless]
	public class LoaiSPCountDto
	{
		public int TotalRecords { get; set; }
	}
}
