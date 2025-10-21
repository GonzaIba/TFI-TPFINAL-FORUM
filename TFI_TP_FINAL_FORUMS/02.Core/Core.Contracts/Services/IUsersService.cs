using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;

namespace Core.Contracts.Services
{
    public interface IUsersService : IGenericService<Users>
    {
        public Task<List<Users>> GetUsersAsync();
        public Task<Users> GetUserByNameAsync(string userName);
        public Task<bool> UpdateUserAsync(Users users);
        public Task<Dictionary<Users, int>> GetTopLastWeekAsync();
        public Task<(PaginatedList<Users>, Dictionary<Users, int>)> GetUsersForumAsync(int pageIndex, int pageCount, string userId);
        public Task<Users> GetDetailUserAsync(string userEmail);
        public Task<List<NotificacionesModel>> GetNotificationsAsync(string userId);
        public Task<bool> MarkNotificationAsReadAsync(MarkNotificationAsReadRequest request);
        public Task<IReadOnlyList<AlertResponse>> GetAlertsAsync(string userId);
    }
}
