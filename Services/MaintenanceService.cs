using Newtonsoft.Json;
using JCA.Mobile.Models;
using System.Text;

namespace JCA.Mobile.Services
{
    public class MaintenanceService
    {
        private readonly HttpClient _httpClient = new();
        // TODO: Update this to your production Jaguar Tools URL
        private const string BaseUrl = "https://localhost:7251/api/mobile/maintenance";

        public async Task<List<MaintenanceTicket>> GetTicketsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MaintenanceTicket>>(content) ?? new List<MaintenanceTicket>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
            return new List<MaintenanceTicket>();
        }

        public async Task<MaintenanceTicket?> GetTicketByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MaintenanceTicket>(content);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> UpdateTicketStatusAsync(int id, TicketStatus status, string? notes)
        {
            try
            {
                var request = new { Id = id, Status = status, AdminNotes = notes };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{BaseUrl}/update-status", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }
    }
}
