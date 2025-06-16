using AstraSoftCR.DTOs.Request;
using AstraSoftCR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AstraSoftCR.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChatController : ControllerBase
	{
		private readonly IChatService _chatService;

		public ChatController(IChatService chatService)
		{
			_chatService = chatService;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
		{
			var response = await _chatService.GetReplyAsync(request);
			return Ok(response);
		}
	}
}
