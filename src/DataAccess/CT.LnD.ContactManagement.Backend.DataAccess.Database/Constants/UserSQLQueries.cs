namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Constants
{
    public static class UserSqlQueries
    {
        public const string GetUserById = "SELECT * FROM Users WHERE Id = {0}";
        public const string GetRoleById = "SELECT * FROM Roles WHERE Id = {0}";
        public const string GetStatusById = "SELECT * FROM Statuses WHERE Id = {0}";
        public const string GetUserByEmail = "SELECT * FROM Users WHERE Email = {0}";
        public const string InsertUser = "INSERT INTO Users (FirstName, LastName , Email , Password , StatusId , RoleId , CreatedAt) VALUES ({0}, {1}, {2} , {3} , {4} , {5} , {6})";
        public const string UpdateUserStatusByEmail = "UPDATE Users SET StatusId = {0} WHERE Email = {1}";
        public const string GetAllUsers = "SELECT * FROM Users";
        public const string ChangeUserStatus = "UPDATE Users SET StatusId = {1} WHERE Id = {0}";

        public const string UpdateName = "UPDATE Users Set FirstName = {0} , LastName = {1} WHERE Id = {2}";

        public const string UpdateEmail = "UPDATE Users Set Email = {0} WHERE Id = {1}";


    }


}
