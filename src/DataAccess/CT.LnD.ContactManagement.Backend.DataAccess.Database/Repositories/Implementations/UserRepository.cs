using CT.LnD.ContactManagement.Backend.DataAccess.Database.Configurations;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Constants;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Implementations
{
    public class UserRepository(ContactManagementDbContext context, IPasswordHasher hasher) : IUserRepository
    {
        private readonly ContactManagementDbContext _context = context;

        public async Task<GetUserResponse> GetByIdAsync(string id)
        {
            User user = await _context.Users.FromSqlRaw(UserSqlQueries.GetUserById, id).FirstOrDefaultAsync();

            Role role = await _context.Roles.FromSqlRaw(UserSqlQueries.GetRoleById, user.RoleId).FirstOrDefaultAsync();
            Status status = await _context.Statuses.FromSqlRaw(UserSqlQueries.GetStatusById, user.StatusId).FirstOrDefaultAsync();

            GetUserResponse response = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                Role = role.RoleName,
                Status = status.StatusName
            };
            return response;
        }

        public async Task CreateUserAsync(UserRegisterRequest user)
        {
            User userDetails = await _context.Users.FromSqlRaw(UserSqlQueries.GetUserByEmail, user.Email).FirstOrDefaultAsync();

            if (userDetails != null)
            {
                throw new Exception("User already exists with this email");
            }

            string hashedPassword = hasher.HashPassword(user.Password);
            DateTime createdAt = DateTime.Now;
            _ = await _context.Database.ExecuteSqlRawAsync(
                UserSqlQueries.InsertUser,
                user.FirstName, user.LastName, user.Email, hashedPassword, 4, 1, createdAt);
        }

        public async Task<GetUserResponse> VerifyUser(UserLoginRequest user)
        {
            string userEmail = user.Email;

            User userDetails = await _context.Users.FromSqlRaw(UserSqlQueries.GetUserByEmail, userEmail).FirstOrDefaultAsync()
                ?? throw new Exception("No user found !");

            string hashedPassword = userDetails.Password;
            string userPassword = user.Password;

            if (userDetails.StatusId == 4)
            {
                throw new Exception("User email verifiction pending. Verify the email to login");
            }

            PasswordVerificationResult res = hasher.VerifyHashedPassword(hashedPassword, userPassword);

            if (res == PasswordVerificationResult.Success)
            {
                Role role = await _context.Roles.FromSqlRaw(UserSqlQueries.GetRoleById, userDetails.RoleId).FirstOrDefaultAsync();
                Status status = await _context.Statuses.FromSqlRaw(UserSqlQueries.GetStatusById, userDetails.StatusId).FirstOrDefaultAsync();

                DateTime lastLogin = DateTime.Now;

                _ = await _context.Database.ExecuteSqlRawAsync("UPDATE Users SET LastLogin = {0} WHERE Id = {1}", lastLogin, userDetails.Id);

                GetUserResponse response = new()
                {
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    Id = userDetails.Id,
                    LastLogin = lastLogin,
                    CreatedAt = userDetails.CreatedAt,
                    Role = role.RoleName,
                    Status = status.StatusName
                };

                return response;
            }

            return null;
        }

        public async Task VerifyEmail(string email)
        {
            _ = await _context.Database.ExecuteSqlRawAsync(UserSqlQueries.UpdateUserStatusByEmail, 1, email);
        }




        public async Task<List<GetUserResponse>> GetAllUsers()
        {
            List<User> users = await _context.Users.FromSqlRaw(UserSqlQueries.GetAllUsers).ToListAsync();


            List<GetUserResponse> allUsers = [];

            foreach (User user in users)
            {
                Role role = await _context.Roles.FromSqlRaw(UserSqlQueries.GetRoleById, user.RoleId).FirstOrDefaultAsync();
                Status status = await _context.Statuses.FromSqlRaw(UserSqlQueries.GetStatusById, user.StatusId).FirstOrDefaultAsync();

                GetUserResponse response = new()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Id = user.Id,
                    CreatedAt = user.CreatedAt,
                    Role = role.RoleName,
                    Status = status.StatusName
                };


                allUsers.Add(response);
            }



            return allUsers;
        }


        public async Task ChangeUserStatus(string userId, string statusId)
        {
            _ = await _context.Database.ExecuteSqlRawAsync(UserSqlQueries.ChangeUserStatus, userId, statusId);
        }


        public async Task UpdateName(UserUpdateNameRequest user, string userId)
        {
            _ = await _context.Database.ExecuteSqlRawAsync(UserSqlQueries.UpdateName, user.FirstName, user.LastName, userId);
        }


        public async Task UpdateEmail(UserEmailUpdateRequest user, string userId)
        {
            User userExists = await _context.Users.FromSqlRaw(UserSqlQueries.GetUserById, userId).FirstOrDefaultAsync();

            User userDetails = await _context.Users.FromSqlRaw(UserSqlQueries.GetUserByEmail, user.Email).FirstOrDefaultAsync();

            if (userExists == null)
            {
                throw new Exception("No user exists with this Id");
            }


            if (userDetails != null)
            {
                throw new Exception("Another account exists with same email Id");
            }

            _ = await _context.Database.ExecuteSqlRawAsync(UserSqlQueries.UpdateEmail, user.Email, userId);
        }
    }
}