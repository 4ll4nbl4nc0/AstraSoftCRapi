using AstraSoftCR.DTOs.Request;
using AstraSoftCR.DTOs.Response;
using System.Threading.Tasks;

namespace AstraSoftCR.Services.Interfaces
{
	public interface IChatService
	{
		Task<ChatResponseDto> GetReplyAsync(ChatRequestDto request);
	}
}
