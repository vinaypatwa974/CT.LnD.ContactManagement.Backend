using CT.LnD.ContactManagement.Backend.Dto.Hosting;
namespace CT.LnD.ContactManagement.Backend.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUserResponse> GetByIdAsync(string id);
        Task CreateUserAsync(UserRegisterRequest user);

        Task<GetUserResponse> VerifyUser(UserLoginRequest userLoginRequest);

        Task VerifyEmail(string email);

        Task<List<GetUserResponse>> GetAllUsers();

        Task ChangeUserStatus(string userId, string roleId);

        Task UpdateName(UserUpdateNameRequest user, string userId);

        Task UpdateEmail(UserEmailUpdateRequest user, string userId);

    }
}