using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
	public class SanPham
	{
		[Key]
		[Display(Name = "Mã sản phẩm")]
		public required string MaSP { get; set; }

		[Required, StringLength(50)]
		[Display(Name = "Tên sản phẩm")]
		public required string TenSP { get; set; }

		[Display(Name = "Đơn giá nhập")]
		[DataType(DataType.Currency)]
		public required decimal DonGia { get; set; }

		[Display(Name = "Giá bán")]
		[DataType(DataType.Currency)]
		public required decimal GiaBan { get; set; }

		[Display(Name = "Mô tả sản phẩm")]
		public required string MoTaSP { get; set; }

		[Display(Name = "Ảnh minh họa")]
		public required string AnhMH { get; set; }

		[Display(Name = "Trạng thái")]
		public required string TrangThai { get; set; }

		[Display(Name = "Số lượng tồn")]
		public required int SoLuongTon { get; set; }

		[Display(Name = "Mã loại sản phẩm")]
		public required string MaLoai { get; set; }

		[Display(Name = "Mã nhà cung cấp")]
		public required string MaNCC { get; set; }

		[Required(ErrorMessage = "Mã gian hàng không được để trống")]
		[StringLength(10)]
		public required string MaGH { get; set; } = null!;

		[ForeignKey("MaLoai")]
		public LoaiSP? LoaiSP { get; set; }

		[ForeignKey("MaNCC")]
		public NhaCC? NhaCC { get; set; }

		[ForeignKey(nameof(MaGH))]
		public GianHang? GianHang { get; set; }

		// Các tập hợp ngược
		public virtual ICollection<CTMH>? CTMHs { get; set; }
		public virtual ICollection<CTBH>? CTBHs { get; set; }

		// ====== Các thuộc tính không ánh xạ (chỉ dùng hiển thị) ======
		[NotMapped]
		public string? TenGH { get; set; }

		[NotMapped]
		public string? TenLoai { get; set; }

		[NotMapped] 
		public IFormFile? AnhFile { get; set; }
	}
}
