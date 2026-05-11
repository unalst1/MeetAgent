using MeetAgent.DTOs;
using MeetAgent.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MeetAgent.Business.Services;

public class TrelloService : ITrelloService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public TrelloService(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient();
        _config = config;
    }

    public async Task<(string cardId, string cardUrl)> CreateCardAsync(TaskDto dto)
    {
        var apiKey = _config["TrelloSettings:ApiKey"];
        var token = _config["TrelloSettings:Token"];
        var baseUrl = _config["TrelloSettings:BaseUrl"];

        // Not: dto.TrelloListId'nin dolu geldiðinden emin olmalýyýz. 
        // Test için geçici olarak sabit bir List ID de kullanabilirsin.
        var url = $"{baseUrl}/cards?key={apiKey}&token={token}" +
                  $"&idList={dto.TrelloListId}" +
                  $"&name={Uri.EscapeDataString(dto.Title)}" +
                  $"&desc={Uri.EscapeDataString(dto.Description + "\n\nAtanan: " + dto.AssignedTo)}";

        var response = await _httpClient.PostAsync(url, null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Trello hatasý: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var cardId = doc.RootElement.GetProperty("id").GetString()!;
        var cardUrl = doc.RootElement.GetProperty("shortUrl").GetString()!;

        return (cardId, cardUrl);
    }
}