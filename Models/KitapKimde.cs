using System.ComponentModel.DataAnnotations;

namespace AskidaKitap.WebApp.Models
{
    public class KitapKimde
    {
        public int Id { get; set; }

        public string OgrenciId { get; set; } = string.Empty;

        public int KitapId { get; set; }

        [Required]
        public DateTime AlinmaTarihi { get; set; }

        public DateTime? IadeTarihi { get; set; }

        // Navigation properties
        public Ogrenci Ogrenci { get; set; } = null!;
        public Kitap Kitap { get; set; } = null!;
    }
} 