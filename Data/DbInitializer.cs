using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;

namespace AskidaKitap.WebApp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Ogrenci>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Veritabanını oluştur
                await context.Database.MigrateAsync();

                // Admin rolünü oluştur
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // ADMIN sınıfını oluştur
                var adminSinif = await context.Siniflar.FirstOrDefaultAsync(s => s.Ad == "ADMIN");
                if (adminSinif == null)
                {
                    adminSinif = new Sinif
                    {
                        Ad = "ADMIN",
                        KayitTarihi = DateTime.Now,
                        AktifMi = true
                    };
                    context.Siniflar.Add(adminSinif);
                    await context.SaveChangesAsync();
                }

                // Admin kullanıcısını oluştur
                var adminUser = await userManager.FindByEmailAsync("admin@askidakitap.com");
                if (adminUser == null)
                {
                    adminUser = new Ogrenci
                    {
                        UserName = "admin@askidakitap.com",
                        Email = "admin@askidakitap.com",
                        EmailConfirmed = true,
                        Ad = "Admin",
                        Soyad = "Kullanıcı",
                        OgrenciNo = "ADMIN001",
                        KayitTarihi = DateTime.Now,
                        AktifMi = true,
                        OnayDurumu = true,
                        SinifId = adminSinif.Id
                    };

                    var result = await userManager.CreateAsync(adminUser, "1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
} 