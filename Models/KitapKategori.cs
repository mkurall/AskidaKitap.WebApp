using System.ComponentModel.DataAnnotations;

namespace AskidaKitap.WebApp.Models
{
    public class KitapKategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Ad { get; set; }

        // Navigation property
        public virtual ICollection<Kitap> Kitaplar { get; set; } = new List<Kitap>();
    }
} 