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
			// Hash password
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MatKhauNV);

			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC NhanVien_Insert 
                    @CCCD = {model.CCCD},
                    @TenNV = {model.TenNV},
                    @GioiTinh = {model.GioiTinh},
                    @NgaySinh = {model.NgaySinh},
                    @SDT = {model.SDT},
                    @Email = {model.Email},
					@AnhNV = {model.AnhNV},
                    @DiaChiNV = {model.DiaChiNV},
                    @MaXa = {model.MaXa},
                    @TenDNNV = {model.TenDNNV},
                    @MatKhauNV = {hashedPassword},
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

		public async Task ResetPassword(string id, string newPassword)
		{
			var nv = await GetByIDEdit(id);
			if (nv == null) throw new Exception("Nhân viên không tìm thấy!");

			// Lấy Vai trò hiện tại bằng Proc (Thay vì LINQ)
			var phanQuyenList = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_GetByNhanVien @MaNV", new SqlParameter("@MaNV", id))
				.ToListAsync();
			
			string maVT = phanQuyenList.FirstOrDefault()?.MaVT ?? ""; 

			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

			await _context.Database.ExecuteSqlInterpolatedAsync($@"
			EXEC NhanVien_Update 
				@MaNV = {nv.MaNV},
				@CCCD = {nv.CCCD},
				@TenNV = {nv.TenNV},
				@GioiTinh = {nv.GioiTinh},
				@NgaySinh = {nv.NgaySinh},
				@SDT = {nv.SDT},
				@Email = {nv.Email},
				@DiaChiNV = {nv.DiaChiNV},
				@MaXa = {nv.MaXa},
				@TenDNNV = {nv.TenDNNV},
				@MatKhauNV = {hashedPassword}, 
				@AnhNV = {nv.AnhNV},
				@MaVT = {maVT}");
		}

		public async Task<NhanVien?> GetByUsername(string username)
		{
			return (await _context.NhanVien
				.FromSqlInterpolated($"EXEC NhanVien_GetByUsername @Username = {username}")
				.ToListAsync())
				.FirstOrDefault();
		}

		public async Task<string> GetRole(string maNV)
		{
			var phanQuyenList = await _context.PhanQuyenDto
				.FromSqlRaw("EXEC PhanQuyen_GetByNhanVien @MaNV", new SqlParameter("@MaNV", maNV))
				.ToListAsync();
			
			return phanQuyenList.FirstOrDefault()?.TenVT ?? "Employee";
		}
	}
}

