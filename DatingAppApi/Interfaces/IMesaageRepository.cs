using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;

namespace DatingAppApi.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message?> GetMessageAsync(int id);
        Task<PagedList<MessageDto>> GetMessageForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessgeThreadAsync(string currentUsername, string RecipientUserName);
        Task<bool> SaveAllAsync();
    }
}
