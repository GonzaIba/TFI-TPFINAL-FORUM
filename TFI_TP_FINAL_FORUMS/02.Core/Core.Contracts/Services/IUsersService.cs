using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;

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
    }
}