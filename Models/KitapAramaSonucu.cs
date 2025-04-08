namespace AskidaKitap.WebApp.Models
{
    public class KitapAramaSonucu
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Yazar { get; set; }
        public string Kategori { get; set; }
        public int StokAdedi { get; set; }
        public bool StokDurumu { get; set; }
    }
} 