using System;
using System.ComponentModel.DataAnnotations;

namespace AskidaKitap.WebApp.Models
{
    public class KitapDegisim
    {
        public int Id { get; set; }

        [Required]
        public int KitapId { get; set; }
        public Kitap Kitap { get; set; }

        [Required]
        public string OgrenciId { get; set; }
        public Ogrenci Ogrenci { get; set; }

        [Required]
        public DateTime AlisTarihi { get; set; }

        public DateTime? IadeTarihi { get; set; }

        public bool AktifMi => !IadeTarihi.HasValue;
    }
} 