using System.Collections.Generic;

namespace AskidaKitap.WebApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<dynamic> EnCokOkunanKitaplar { get; set; }
        public IEnumerable<dynamic> EnCokKitapOkuyanOgrenciler { get; set; }
    }
} 