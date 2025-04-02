using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AskidaKitap.WebApp.Models
{
    public class Sinif
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sınıf adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Sınıf adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Sınıf Adı")]
        public string Ad { get; set; } = string.Empty;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; }

        public virtual ICollection<Ogrenci> Ogrenciler { get; set; } = new List<Ogrenci>();
    }
} 