using Newtonsoft.Json;
using JCA.Mobile.Models;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using Microsoft.Maui.Storage;

namespace JCA.Mobile.Services
{
    public class MaintenanceService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // TODO: Update this to your production Jaguar Tools URL
        private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
         ? "http://10.0.2.2:58564/api/mobile/maintenance" // Emulator host IP and HTTP port
         : "https://localhost:58563/api/mobile/maintenance"; // Standard local PC port

        public MaintenanceService()
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

        public async Task<List<MaintenanceTicket>> GetTicketsAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
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
                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
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
                string json = JsonConvert.SerializeObject(ticket);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);
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
                string json = JsonConvert.SerializeObject(ticket);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);
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
                using (Stream stream = await file.OpenReadAsync())
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    StreamContent imageContent = new StreamContent(stream);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(imageContent, "file", file.FileName);

                    HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/upload-image", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        dynamic? result = JsonConvert.DeserializeObject<dynamic>(responseString);
                        return (string?)result?.ImagePath;
                    }
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
