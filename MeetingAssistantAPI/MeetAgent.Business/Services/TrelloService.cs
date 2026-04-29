using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MeetingAssistantAPI.Services;

public class TrelloService : ITrelloService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public TrelloService(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient();
        _config = config;
    }

    public async Task<(string cardId, string cardUrl)> CreateCard(TaskDto dto)
    {
        var apiKey = _config["TrelloSettings:ApiKey"];
        var token = _config["TrelloSettings:Token"];
        var baseUrl = _config["TrelloSettings:BaseUrl"];

        var url = $"{baseUrl}/cards?key={apiKey}&token={token}" +
                  $"&idList={dto.TrelloListId}" +
                  $"&name={Uri.EscapeDataString(dto.Title)}" +
                  $"&desc={Uri.EscapeDataString(dto.Description + "\n\nAtanan: " + dto.AssignedTo)}";

        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var cardId = doc.RootElement.GetProperty("id").GetString()!;
        var cardUrl = doc.RootElement.GetProperty("shortUrl").GetString()!;

        return (cardId, cardUrl);
    }
}