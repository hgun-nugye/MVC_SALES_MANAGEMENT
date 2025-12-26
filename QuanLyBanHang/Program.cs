using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));
builder.Services.AddControllersWithViews();

// Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	// Thời gian session tồn tại khi không hoạt động (30 phút)
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	
	// Cookie settings cho session
	options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập từ HTTP, không cho JavaScript
	options.Cookie.IsEssential = true; // Cookie cần thiết cho ứng dụng
	options.Cookie.Name = ".QuanLyBanHang.Session"; // Tên cookie
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ gửi qua HTTPS
	options.Cookie.SameSite = SameSiteMode.Lax; // Bảo vệ khỏi CSRF
});

// Cấu hình Authentication Cookie (nếu muốn dùng Cookie Auth thay vì Session)
builder.Services.AddAuthentication("CookieAuth")
	.AddCookie("CookieAuth", options =>
	{
		options.LoginPath = "/Home/Login";
		options.AccessDeniedPath = "/Home/AccessDenied";
		options.ExpireTimeSpan = TimeSpan.FromHours(8);
		options.SlidingExpiration = true;
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.SameSite = SameSiteMode.Strict;
		options.Cookie.Name = ".QuanLyBanHang.Auth";
	});

builder.Services.AddScoped<CTMHService>();
builder.Services.AddScoped<CTBHService>();
builder.Services.AddScoped<DonMuaHangService>();
builder.Services.AddScoped<DonBanHangService>();
builder.Services.AddScoped<KhachHangService>();
builder.Services.AddScoped<LoaiSPService>();
builder.Services.AddScoped<NhaCCService>();
builder.Services.AddScoped<NhomSPService>();
builder.Services.AddScoped<SanPhamService>();
builder.Services.AddScoped<TinhService>();
builder.Services.AddScoped<XaService>();
builder.Services.AddScoped<NuocService>();
builder.Services.AddScoped<HangSXService>();
builder.Services.AddScoped<NuocService>();
builder.Services.AddScoped<NhanVienService>();
builder.Services.AddScoped<TrangThaiBHService>();
builder.Services.AddScoped<TrangThaiService>();
builder.Services.AddScoped<TrangThaiMHService>();
builder.Services.AddScoped<VaiTroService>();
builder.Services.AddScoped<PhanQuyenService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ExportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thứ tự middleware quan trọng: Session -> Authentication -> Authorization
app.UseSession();
app.UseAuthentication(); // Phải đặt trước UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
