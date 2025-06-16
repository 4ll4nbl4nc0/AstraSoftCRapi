using AstraSoftCR.DTOs.Request;
using AstraSoftCR.DTOs.Response;
using AstraSoftCR.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AstraSoftCR.Services
{
	public class ChatService : IChatService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _config;

		public ChatService(HttpClient httpClient, IConfiguration config)
		{
			_httpClient = httpClient;
			_config = config;
		}

		public async Task<ChatResponseDto> GetReplyAsync(ChatRequestDto request)
		{
			var apiKey = _config["DeepSeek:ApiKey"];
			var endpoint = "https://api.deepseek.com/v1/chat/completions";

			var body = new
			{
				model = "deepseek-chat",
				temperature = 0.7,
				max_tokens = 500,
				messages = new[]
				{
					new { role = "system", content = @"Eres un asistente virtual de AstraSoftCR, una empresa de desarrollo de software en Costa Rica. 
Si alguien pregunta por contacto, siempre responde incluyendo el correo contact@astrasoftcr.com y los teléfonos 7063-7308 y 8777-7093.
Responde de forma profesional y concisa sobre servicios como desarrollo web, sistemas administrativos y soluciones personalizadas."},
					new { role = "user", content = request.Message }
				}
			};

			var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
			{
				Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
			};

			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

			var response = await _httpClient.SendAsync(httpRequest);
			var json = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
				return new ChatResponseDto { Reply = "Lo siento, hubo un error al procesar tu mensaje." };

			using var doc = JsonDocument.Parse(json);
			var reply = doc.RootElement
				.GetProperty("choices")[0]
				.GetProperty("message")
				.GetProperty("content")
				.GetString();

			return new ChatResponseDto { Reply = reply };
		}
	}
}
