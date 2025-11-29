using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class TaiKhoan
	{
		[Key]
		public string? TenUser { get; set; }
				
		[DataType(DataType.Password)]
		public string? MatKhau { get; set; }
	}
}
