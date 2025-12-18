using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	[Table("TrangThaiMH")]
	public class TrangThaiMH
	{
		[Key]
		[Required(ErrorMessage = "Mã trạng thái mua hàng không được để trống.")]
		[StringLength(3, ErrorMessage = "Mã trạng thái mua hàng tối đa 3 ký tự.")]
		[Display(Name = "Mã Trạng Thái Mua Hàng")]
		public string MaTTMH { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên trạng thái mua hàng không được để trống.")]
		[StringLength(50, ErrorMessage = "Tên trạng thái mua hàng tối đa 50 ký tự.")]
		[Display(Name = "Tên Trạng Thái Mua Hàng")]
		public string TenTTMH { get; set; } = string.Empty;
	}
}

