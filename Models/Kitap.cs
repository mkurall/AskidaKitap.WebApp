using System.ComponentModel.DataAnnotations;

namespace AskidaKitap.WebApp.Models
{
    public class Kitap
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Yazar { get; set; } = string.Empty;

        public int KitapKategoriId { get; set; }

        public int? SayfaSayisi { get; set; }

        [Required]
        public int StokAdedi { get; set; }

        // Navigation properties
        public KitapKategori? KitapKategori { get; set; }
        public ICollection<KitapKimde> KitapKimde { get; set; } = new List<KitapKimde>();
    }
} 