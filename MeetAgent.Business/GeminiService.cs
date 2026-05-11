using GenerativeAI;


using MeetAgent.DTOs;
using MeetAgent.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MeetAgent.Business.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly string _apiKey;
        private readonly GenerativeModel _model;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"]!;
            _model = new GenerativeModel(_apiKey, "gemini-flash-latest");
        }

        public async Task<List<TaskDto>> ExtractTasksFromTranscriptAsync(string transcript)
        {
            string prompt = $@"Aşağıdaki dökümden görevleri ayıkla ve sadece JSON formatında bir liste döndür. 
            Ekstra açıklama veya markdown tırnakları (```json gibi) kullanma.
            Format: [{{""title"": ""görev başlığı"", ""description"": ""detaylı açıklama"", ""assignedTo"": ""sorumlu kişi""}}]
            Döküm: {transcript}";

            var response = await _model.GenerateContentAsync(prompt);
            string cleanJson = response.Text ?? "";

            // 1. Markdown tırnaklarını (```json veya ```) temizle
            if (cleanJson.Contains("```"))
            {
                cleanJson = cleanJson.Replace("```json", "").Replace("```", "").Trim();
            }

            try
            {
                // 2. Temizlenmiş metni Deserialize et
                return JsonSerializer.Deserialize<List<TaskDto>>(cleanJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TaskDto>();
            }
            catch (JsonException ex)
            {
                // Hata durumunda ne geldiğini görmek için hatayı fırlat
                throw new Exception($"Gemini'den geçersiz JSON geldi. Gelen metin: {cleanJson}. Hata: {ex.Message}");
            }
        }
    }
}