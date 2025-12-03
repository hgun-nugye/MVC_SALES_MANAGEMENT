using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class Xa
	{
		[Key]
		public short MaXa { get; set; }

		[Required, StringLength(90)]
		public string TenXa { get; set; } = string.Empty;

		public short MaTinh { get; set; }

		//[NotMapped]
		public string? TenTinh { get; set; }

		[ForeignKey("MaTinh")]
		public Tinh? Tinh { get; set; }

	}
}
