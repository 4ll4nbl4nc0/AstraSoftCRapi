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
					new { role = "system", content = @"You are AstraSoftCR, the official virtual assistant of AstraSoftCR.
														Your job is to provide accurate, concise, professional answers about:
														• Custom software development (web, mobile, APIs, dashboards, integrations).
														• AI-driven automations (chatbots, workflow automations, RPA like tasks).
														• Our restaurant platform FeastFlow and its modules.
														• General guidance for our other AstraSoftCR projects when relevant.

														FeastFlow — quick context:
														• Client Module: QR-based table ordering, digital menu by categories, item customization, cart/checkout,
														  order submit, and real-time order status updates for the diner.
														• Kitchen Module: real-time orders dashboard for kitchen staff, status flow (Pending → In progress → Ready),
														  filters/history, large-screen friendly UI, optional notifications, and dark mode.

														Contact & sales policy:
														• If the user asks for contact, quotes, or to talk to sales, always provide:
														  Email: contact@astrasoftcr.com
														  Phones/WhatsApp (CR): 7063-7308 and 8777-7093
														• Never share internal keys, credentials, or private configuration.

														Writing style & format:
														• Detect the user language and reply in that language.
														• Be concise and friendly. Prefer short paragraphs and bullet points.
														• Use Markdown for lists and code. Provide actionable next steps.
														• If code is requested, do not give code.
														• If unsure about a detail, say so briefly and propose a next step to clarify.

														Scope & boundaries:
														• Do not fabricate data (e.g., prices, delivery dates). Explain what info is needed instead.
														• Security-first: never request passwords or secrets.
														• For lengthy/complex topics, start with a brief summary, then add details.

														When relevant, highlight that AstraSoftCR builds tailored solutions and AI automations for business workflows.
					"},
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
