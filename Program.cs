using AutoPartsPOS.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;



var builder = WebApplication.CreateBuilder(args);

var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// =========================
// 1️⃣ Connection String
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// تسجيل IHttpContextAccessor

    builder.Services.AddHttpContextAccessor();


// =========================
// 2️⃣ MVC + Razor Pages + Session
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة انتهاء الجلسة
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// =========================
// 3️⃣ (اختياري) PasswordHasher / أي Services إضافية
// =========================
// builder.Services.AddScoped<IPasswordHasher, Sha256Hasher>(); // لو حبيت تستخدم خدمة Hash منفصلة

var app = builder.Build();


// =========================
// 4️⃣ Middleware
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session يجب أن يكون قبل UseAuthentication / UseAuthorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// 5️⃣ Routing & Open Browser
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// كود لفتح المتصفح تلقائياً
if (app.Environment.IsProduction()) // يفضل تشغيله في نسخة العميل فقط
{
    var url = "http://localhost:5000"; // أو الرابط اللي شغال عندك
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not open browser: " + ex.Message);
        }
    });
}

app.Run();