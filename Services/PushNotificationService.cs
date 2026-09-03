using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;

namespace JCA.Mobile.Services
{
    internal class PushNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
             ? "https://10.0.2.2:58563/api/mobile/announcement" // Emulator host IP and HTTP port
            : "https://localhost:58563/api/mobile/announcement"; // Standard local PC port

        //private readonly string BaseUrl = "https://tools.jcadm.org/api/mobile/announcement";
        public PushNotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
