using CT.LnD.ContactManagement.Backend.Dto.Hosting;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<GetUserResponse> GetByIdAsync(string id);
        Task CreateUserAsync(UserRegisterRequest user);

        Task<GetUserResponse> VerifyUser(UserLoginRequest user);

        Task VerifyEmail(string email);

        Task<List<GetUserResponse>> GetAllUsers();


        Task ChangeUserStatus(string userId, string taskId);

        Task UpdateName(UserUpdateNameRequest user, string userId);

        Task UpdateEmail(UserEmailUpdateRequest user, string userId);
    }
}