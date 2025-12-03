using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class Tinh
	{
		[Key]
		public short MaTinh { get; set; }

		[StringLength(90)]
		public string TenTinh { get; set; } = string.Empty;

		[InverseProperty("Tinh")]
		public ICollection<Xa> DsXa { get; set; } = new List<Xa>();
	}
}
