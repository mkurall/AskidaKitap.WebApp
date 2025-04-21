using System.Collections.Generic;

namespace AskidaKitap.WebApp.Models
{
    public class HomeViewModel
    {
        public List<EnCokOkunanKitap> EnCokOkunanKitaplar { get; set; }
        public List<EnCokKitapOkuyanOgrenci> EnCokKitapOkuyanOgrenciler { get; set; }
        public List<SonDuyuru> SonDuyurular { get; set; }
        public List<SonIslem> SonIslemler { get; set; }
    }

    public class EnCokOkunanKitap
    {
        public string KitapAdi { get; set; }
        public int OkunmaSayisi { get; set; }
    }

    public class EnCokKitapOkuyanOgrenci
    {
        public string OgrenciAdi { get; set; }
        public string OgrenciSoyadi { get; set; }
        public int OkunanKitapSayisi { get; set; }
    }

    public class SonDuyuru
    {
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }

    public class SonIslem
    {
        public string IslemTuru { get; set; }
        public string OgrenciAdi { get; set; }
        public string OgrenciSoyadi { get; set; }
        public string KitapAdi { get; set; }
        public DateTime IslemTarihi { get; set; }
    }
} 