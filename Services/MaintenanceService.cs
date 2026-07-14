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

        public async Task<bool> UpdateTicketAsync(MaintenanceTicket ticket)
        {
            try
            {
                var json = JsonConvert.SerializeObject(ticket);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(BaseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateTicketAsync(MaintenanceTicket ticket)
        {
            try
            {
                var json = JsonConvert.SerializeObject(ticket);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BaseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> UploadImageAsync(FileResult file)
        {
            try
            {
                using var stream = await file.OpenReadAsync();
                var content = new MultipartFormDataContent();
                var imageContent = new StreamContent(stream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(imageContent, "file", file.FileName);

                var response = await _httpClient.PostAsync($"{BaseUrl}/upload-image", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return result?.ImagePath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload Error: {ex.Message}");
            }
            return null;
        }
    }
}
