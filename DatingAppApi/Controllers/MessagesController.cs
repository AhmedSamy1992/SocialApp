using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository,
        IMapper mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
        {
            var userName = User.GetUsername();

            if (userName == messageDto.RecipientUserName.ToLower())
                return BadRequest("you can not message yourself");

            var sender = await userRepository.GetUserByUserNameAsync(userName);
            var recipient = await userRepository.GetUserByUserNameAsync(messageDto.RecipientUserName);

            if (sender == null || recipient == null)
                return BadRequest("Cannot send message at this time");

            var message = new Message
            {
                RecipientUserName = recipient.UserName,
                SenderUserName = sender.UserName,
                Content = messageDto.Content,
                Sender = sender,
                Recipient = recipient,
            };

            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllAsync())
                return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Failes to save message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await messageRepository.GetMessageForUserAsync(messageParams);

            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }
    }
}