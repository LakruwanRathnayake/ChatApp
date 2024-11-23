using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.Services;
using API.DTOs;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BaseApiController
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            if (messageDto == null || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                return BadRequest("Invalid message data.");
            }

            var message = new Message
            {
                SenderId = messageDto.SenderId,
                RecipientId = messageDto.RecipientId,
                Content = messageDto.Content
            };

            await _messageService.SendMessageAsync(message);

            return Ok(new { Success = true, Message = "Message sent successfully." });
        }

        [HttpGet("user/{userId}/recipient/{recipientId}")]
        public async Task<IActionResult> GetMessages(string userId, string recipientId)
        {
            var messages = await _messageService.GetMessagesAsync(userId, recipientId);
            return Ok(messages);
        }

        [HttpGet("conversations/{userId}")]
            public async Task<IActionResult> GetPreviousConversations(string userId)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is required.");
                }

                var conversations = await _messageService.GetPreviousConversationsAsync(userId);

                if (conversations == null || !conversations.Any())
                {
                    return NotFound("No conversations found.");
                }

                return Ok(conversations);
            }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation([FromQuery] string currentUserId, [FromQuery] string selectedUserId)
        {
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(selectedUserId))
            {
                return BadRequest("Both currentUserId and selectedUserId are required.");
            }

            var messages = await _messageService.GetMessagesAsync(currentUserId, selectedUserId);

            if (messages == null || !messages.Any())
            {
                return NotFound("No messages found for this conversation.");
            }

            return Ok(messages);
        }

    }
}
