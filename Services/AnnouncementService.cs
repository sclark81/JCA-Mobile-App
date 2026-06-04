using Newtonsoft.Json;
using JCA.Mobile.Models;

namespace JCA.Mobile.Services;

public class AnnouncementService
{
    private readonly HttpClient _httpClient = new();
    // TODO: Update this to your production Jaguar Tools URL
    private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
         ? "http://10.0.2.2:58564/api/mobile/announcements" // Emulator host IP and HTTP port
         : "https://localhost:58564/api/mobile/announcements"; // Standard local PC port

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