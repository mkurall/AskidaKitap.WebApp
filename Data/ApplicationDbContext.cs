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
        public DbSet<Duyuru> Duyurular { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Sınıf-Oğrenci ilişkisi
            builder.Entity<Ogrenci>()
                .HasOne(o => o.Sinif)
                .WithMany(s => s.Ogrenciler)
                .HasForeignKey(o => o.SinifId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Kitap>()
                .HasOne(k => k.KitapKategori)
                .WithMany(kk => kk.Kitaplar)
                .HasForeignKey(k => k.KitapKategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<KitapDegisim>()
                .HasOne(kd => kd.Kitap)
                .WithMany(k => k.KitapDegisimler)
                .HasForeignKey(kd => kd.KitapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<KitapDegisim>()
                .HasOne(kd => kd.Ogrenci)
                .WithMany(o => o.KitapDegisimler)
                .HasForeignKey(kd => kd.OgrenciId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 