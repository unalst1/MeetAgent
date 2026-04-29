using GenerativeAI;


using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MeetingAssistantAPI.Services
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
            string prompt = $@"Aşağıdaki dökümden görevleri ayıkla ve sadece JSON döndür: 
            [{{""title"": ""görev"", ""description"": ""detay"", ""assignedTo"": ""kişi""}}]
            Döküm: {transcript}";

            var response = await _model.GenerateContentAsync(prompt);
            return JsonSerializer.Deserialize<List<TaskDto>>(response.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}