using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AskidaKitap.WebApp.Models
{
    public class Ogrenci : IdentityUser
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Öğrenci numarası zorunludur.")]
        [StringLength(20, ErrorMessage = "Öğrenci numarası en fazla 20 karakter olabilir.")]
        [Display(Name = "Öğrenci Numarası")]
        public string OgrenciNo { get; set; } = string.Empty;

        [Display(Name = "Sınıf")]
        public int? SinifId { get; set; }

        [ForeignKey("SinifId")]
        public virtual Sinif? Sinif { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; }

        [Display(Name = "Onay Durumu")]
        public bool OnayDurumu { get; set; }

        public virtual ICollection<KitapKimde> Kitaplar { get; set; } = new List<KitapKimde>();

        public ICollection<KitapDegisim> KitapDegisimler { get; set; } = new List<KitapDegisim>();
    }
} 