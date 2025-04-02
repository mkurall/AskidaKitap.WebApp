using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Models;

namespace AskidaKitap.WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<Ogrenci>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<KitapKimde> KitapKimde { get; set; }
        public DbSet<KitapKategori> KitapKategoriler { get; set; }
        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Ogrenci> Ogrenciler { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Sınıf-Oğrenci ilişkisi
            builder.Entity<Ogrenci>()
                .HasOne(o => o.Sinif)
                .WithMany(s => s.Ogrenciler)
                .HasForeignKey(o => o.SinifId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 