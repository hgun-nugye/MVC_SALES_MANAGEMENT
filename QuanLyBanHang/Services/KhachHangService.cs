using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Services
{
	public class KhachHangService
	{
		private readonly AppDbContext _context;

		public KhachHangService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<KhachHangDetailView>> Search(string? search, string? tinh)
		{
			var parameters = new SqlParameter[]
			{
				new SqlParameter("@Search", (object?)search ?? DBNull.Value),
				new SqlParameter("@MaTinh", (object?)tinh ?? DBNull.Value)
			};

			return await _context.KhachHangDetailView
				.FromSqlRaw("EXEC KhachHang_Search @Search, @MaTinh",parameters)
				.ToListAsync();
		}

		// Get all KhachHang
		public async Task<List<KhachHangDetailView>> GetAllWithXa()
		{
			return await _context.KhachHangDetailView
				.FromSqlRaw("EXEC KhachHang_GetAllWithXa")
				.ToListAsync();
		}

		// Get KhachHang by ID
		public async Task<KhachHang?> GetByID(string id)
		{
			var result = await _context.KhachHang
				.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH={id}")
				.AsNoTracking()
				.ToListAsync();

			return result.SingleOrDefault();
		}

		public async Task<KhachHangDetailView?> GetByIDWithXa(string id)
		{
			return (await _context.KhachHangDetailView
				.FromSqlInterpolated($"EXEC KhachHang_GetByIDWithXa @MaKH = {id}")
				.ToListAsync())
				.FirstOrDefault();
		}

		// Create KhachHang
		public async Task<string> Create(KhachHang model, IFormFile? anhFile)
		{			
			// Upload ảnh
			string fileName = "";
			if (anhFile != null && anhFile.Length > 0)
			{
				fileName = Guid.NewGuid() + Path.GetExtension(anhFile.FileName);
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/customers");
				if (!Directory.Exists(folderPath))
					Directory.CreateDirectory(folderPath);

				var savePath = Path.Combine(folderPath, fileName);
				using (var stream = new FileStream(savePath, FileMode.Create))
				{
					await anhFile.CopyToAsync(stream);
				}
			}
			model.AnhKH = fileName;

			// Hash password
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MatKhauKH);

			// Call stored procedure
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC KhachHang_Insert
                    @TenKH = {model.TenKH},
                    @DienThoaiKH = {model.DienThoaiKH},
                    @EmailKH = {model.EmailKH},
                    @DiaChiKH = {model.DiaChiKH},
					@GioiTinh = {model.GioiTinh},
                    @AnhKH = {model.AnhKH},
                    @MaXa = {model.MaXa},
                    @TenDNKH = {model.TenDNKH},
                    @MatKhauKH = {hashedPassword}
            ");

			return fileName;
		}

		// Update KhachHang
		public async Task<string> Update(KhachHang model, IFormFile? anhFile)
		{
			var oldKH = (await _context.KhachHang
				.FromSqlInterpolated($"EXEC KhachHang_GetByID @MaKH = {model.MaKH}")
				.AsNoTracking()
				.ToListAsync())
				.SingleOrDefault();

			if (oldKH == null)
				throw new KeyNotFoundException("Khách hàng không tồn tại!");

			// Giữ ảnh cũ mặc định
			string anhPath = oldKH.AnhKH;

			// Có upload ảnh mới
			if (anhFile != null && anhFile.Length > 0)
			{
				var fileName = Guid.NewGuid() + Path.GetExtension(anhFile.FileName);

				var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "customers");
				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

				var savePath = Path.Combine(folder, fileName);

				using var stream = new FileStream(savePath, FileMode.Create);
				await anhFile.CopyToAsync(stream);

				anhPath = fileName;

				Console.WriteLine("FileName: " + anhPath);
				Console.WriteLine("SavePath: " + savePath);

			}

			await _context.Database.ExecuteSqlInterpolatedAsync($@"
				EXEC KhachHang_Update
					@MaKH = {model.MaKH},
					@TenKH = {model.TenKH},
					@DienThoaiKH = {model.DienThoaiKH},
					@EmailKH = {model.EmailKH},
					@DiaChiKH = {model.DiaChiKH},
					@AnhKH = {anhPath},
					@MaXa = {model.MaXa},
					@GioiTinh = {model.GioiTinh},
					@TenDNKH = {model.TenDNKH},
					@MatKhauKH = {oldKH.MatKhauKH}
			");

			return anhPath;
		}


		// Delete KhachHang
		public async Task Delete(string id)
		{
			var kh = await GetByID(id);
			if (kh == null)
				throw new KeyNotFoundException("Khách hàng không tồn tại!");

			await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC KhachHang_Delete @MaKH = {id}");
		}

		public async Task ResetPassword(string id, string newPassword)
		{
			var kh = await GetByID(id);
			if (kh == null) throw new KeyNotFoundException("Khách hàng không tồn tại!");

			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

			await _context.Database.ExecuteSqlInterpolatedAsync($@"
				EXEC KhachHang_Update
					@MaKH = {kh.MaKH},
					@TenKH = {kh.TenKH},
					@DienThoaiKH = {kh.DienThoaiKH},
					@EmailKH = {kh.EmailKH},
					@DiaChiKH = {kh.DiaChiKH},
					@AnhKH = {kh.AnhKH},
					@MaXa = {kh.MaXa},
					@GioiTinh = {kh.GioiTinh},
					@TenDNKH = {kh.TenDNKH},
					@MatKhauKH = {hashedPassword}
			");
		}

		public async Task<KhachHang?> GetByUsername(string username)
		{
			return (await _context.KhachHang
				.FromSqlInterpolated($"EXEC KhachHang_GetByUsername @Username = {username}")
				.ToListAsync())
				.FirstOrDefault();
		}
	}
}
