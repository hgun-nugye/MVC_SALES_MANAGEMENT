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
			SqlParameter pSearch = new("@Search", (object?)search ?? DBNull.Value);

			SqlParameter pMaTinh;

			if (short.TryParse(tinh, out short maTinh))
				pMaTinh = new("@MaTinh", maTinh);
			else
				pMaTinh = new("@MaTinh", DBNull.Value);

			return await _context.KhachHangDetailView
				.FromSqlRaw("EXEC KhachHang_Search @Search, @MaTinh", pSearch, pMaTinh)
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
		public async Task<string> Create(KhachHang model, IFormFile anhFile)
		{
			if (anhFile == null || anhFile.Length == 0)
				throw new ArgumentException("Vui lòng chọn ảnh minh họa!");

			// Upload ảnh
			var fileName = Guid.NewGuid() + Path.GetExtension(anhFile.FileName);
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var savePath = Path.Combine(folderPath, fileName);
			using (var stream = new FileStream(savePath, FileMode.Create))
			{
				await anhFile.CopyToAsync(stream);
			}
			model.AnhKH = fileName;

			// Call stored procedure
			await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC KhachHang_Insert
                    @TenKH = {model.TenKH},
                    @DienThoaiKH = {model.DienThoaiKH},
                    @EmailKH = {model.EmailKH},
                    @DiaChiKH = {model.DiaChiKH},
                    @AnhKH = {model.AnhKH},
                    @MaXa = {model.MaXa},
                    @TenDNKH = {model.TenDNKH},
                    @MatKhauKH = {model.MatKhauKH}
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

				var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
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
					@MaXa = {model.MaXa}
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
		
	}
}
