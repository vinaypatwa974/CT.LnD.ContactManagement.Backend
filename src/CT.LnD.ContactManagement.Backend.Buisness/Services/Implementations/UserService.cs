using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;

namespace CT.LnD.ContactManagement.Backend.Business.Services.Implementations
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<GetUserResponse> GetByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
        public async Task CreateUserAsync(UserRegisterRequest user)
        {
            await _userRepository.CreateUserAsync(user);
        }

        public async Task<GetUserResponse> VerifyUser(UserLoginRequest userLoginRequest)
        {
            return await _userRepository.VerifyUser(userLoginRequest);
        }


        public async Task VerifyEmail(string email)
        {
            await _userRepository.VerifyEmail(email);
        }



        public async Task<List<GetUserResponse>> GetAllUsers()
        {
            return await userRepository.GetAllUsers();
        }


        public async Task ChangeUserStatus(string userId, string statusId)
        {
            await _userRepository.ChangeUserStatus(userId, statusId);
        }


        public async Task UpdateName(UserUpdateNameRequest user, string userId)
        {
            await _userRepository.UpdateName(user, userId);
        }

        public async Task UpdateEmail(UserEmailUpdateRequest user, string userId)
        {
            await _userRepository.UpdateEmail(user, userId);
        }
    }
}