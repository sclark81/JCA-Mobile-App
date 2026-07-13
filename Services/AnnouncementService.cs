using Newtonsoft.Json;
using JCA.Mobile.Models;

namespace JCA.Mobile.Services;

public class AnnouncementService
{
    private readonly HttpClient _httpClient;
    private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
         ? "http://10.0.2.2:58564/api/mobile/announcement" // Emulator host IP and HTTP port
         : "https://localhost:58563/api/mobile/announcement"; // Standard local PC port

    public AnnouncementService()
    {
        // If we are debugging, configure HttpClient to ignore local SSL certificate mismatches
#if DEBUG
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler);
#else
            // Standard secure client for production
            _httpClient = new HttpClient();
#endif
    }

    public async Task<List<Announcement>> GetAnnouncementsAsync()
    {
        try 
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Announcement>>(content) ?? new List<Announcement>();
            }
        }
        catch (Exception ex)
        {
            // Log error here
            System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
        }
        
        return new List<Announcement>();
    }
}