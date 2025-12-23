using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class NhanVienService
	{
		private readonly AppDbContext _context;

		public NhanVienService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<NhanVienDetailView>> Search(string? search)
		{
			var parameter = new SqlParameter("@Search", (object?)search ?? DBNull.Value);

			return await _context.NhanVienDetailView
				.FromSqlRaw("EXEC NhanVien_Search @Search", parameter)
				.ToListAsync();
		}

		// Lấy danh sách tất cả Nhân Viên
		public async Task<List<NhanVien>> GetAll()
		{
			return await _context.NhanVien
				.FromSqlRaw("EXEC NhanVien_GetAll")
				.ToListAsync();
		}

		// Lấy Nhân Viên theo ID
		public async Task<NhanVienDetailView?> GetByID(string id)
		{
			return (await _context.NhanVienDetailView
				.FromSqlInterpolated($"EXEC NhanVien_GetByID @MaNV = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Lấy Nhân Viên theo ID
		public async Task<NhanVien?> GetByIDEdit(string id)
		{
			return (await _context.NhanVien
				.FromSqlInterpolated($"EXEC NhanVien_GetByID @MaNV = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Thêm Nhân Viên
		public async Task Create(NhanVien model, string maVT)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Insert 
                    @CCCD = {model.CCCD},
                    @TenNV = {model.TenNV},
                    @GioiTinh = {model.GioiTinh},
                    @NgaySinh = {model.NgaySinh},
                    @SDT = {model.SDT},
                    @Email = {model.Email},
                    @DiaChiNV = {model.DiaChiNV},
                    @MaXa = {model.MaXa},
                    @TenDNNV = {model.TenDNNV},
                    @MatKhauNV = {model.MatKhauNV},
                    @MaVT = {maVT}");
		}

		// Cập nhật Nhân Viên
		public async Task Update(NhanVien model, string maVT, string? newPassword)
		{
			string finalPassword;

			if (!string.IsNullOrWhiteSpace(newPassword))
			{
				// Nếu có mật khẩu mới thì mã hóa
				finalPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
			}
			else
			{
				finalPassword = model.MatKhauNV;
			}

			await _context.Database.ExecuteSqlInterpolatedAsync($@"
			EXEC NhanVien_Update 
				@MaNV = {model.MaNV},
				@CCCD = {model.CCCD},
				@TenNV = {model.TenNV},
				@GioiTinh = {model.GioiTinh},
				@NgaySinh = {model.NgaySinh},
				@SDT = {model.SDT},
				@Email = {model.Email},
				@DiaChiNV = {model.DiaChiNV},
				@MaXa = {model.MaXa},
				@TenDNNV = {model.TenDNNV},
				@MatKhauNV = {finalPassword}, 
				@AnhNV = {model.AnhNV},
				@MaVT = {maVT}");
		}

		// Xóa Nhân Viên
		public async Task Delete(string id)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Delete @MaNV = {id}");
		}
	}
}

