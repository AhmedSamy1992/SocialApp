using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using System.Linq;

namespace DatingAppApi.Data
{
    public class MessageRepository(DataContext dataContext, IMapper mapper) : IMessageRepository
    {
        public void AddMessage(Message message)
        {
            dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            dataContext.Messages.Remove(message);
        }

        public async Task<Message?> GetMessageAsync(int id)
        {
            return await dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUserAsync(MessageParams messageParams)
        {
            var query = dataContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null)
            };

            var message = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
            
            return await PagedList<MessageDto>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);

        }

        public Task<IEnumerable<MessageDto>> GetMessgeThreadAsync(string currentUsername, string RecipientUserName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
