using Newtonsoft.Json;
using JCA.Mobile.Models;

namespace JCA.Mobile.Services;

public class AnnouncementService
{
    private readonly HttpClient _httpClient = new();
    // TODO: Update this to your production Jaguar Tools URL
    private const string BaseUrl = "https://localhost:7251/api/mobile/announcements";

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