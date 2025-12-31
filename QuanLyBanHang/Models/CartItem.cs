using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHang.Models
{
    [NotMapped]
    public class CartItem
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public string? AnhMH { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongTon { get; set; }
        public bool IsSelected { get; set; } = true; // Mặc định là được chọn
        public decimal ThanhTien => GiaBan * SoLuong;
    }
}
