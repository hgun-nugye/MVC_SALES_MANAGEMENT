using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class GianHang
	{
		[Key]
		public string? MaGH { get; set; } = null!;

		public string TenGH { get; set; } = null!;

		public string? MoTaGH { get; set; }

		[DataType(DataType.Date)]
		public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.Now);

		public string DienThoaiGH { get; set; } = null!;

		public string? EmailGH { get; set; }

		public string DiaChiGH { get; set; } = null!;
		public short MaXa { get; set; }
		[NotMapped]
		public short MaTinh { get; set; }
		public string? TenXa { get; set; }
		public string? TenTinh { get; set; }

		public ICollection<SanPham>? DsSanPham { get; set; }		
	}

}
