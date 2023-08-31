using Eduplus.DTO.UserManagement;
using KS.Core.UserManagement;
using System.Collections.Generic;

namespace KS.Services.Contract
{
    public interface IUserService
    {
        string ChangePassword(string username, string oldPassword, string newPassword, string userId);
       
        List<string> FetchRoles();
        List<RoleDTO> AllRoles();
        string DeleteRole(int roleId, string userId);
        string AddRole(RoleDTO nrole, string userId);
        List<User> FetchRoleUsers(int roleid);
        List<UserDTO> FetchAllUsers();
        User FetchSingleUser(string userName);
        List<MenuItem> FetchUserMenus(string[] permissions, bool sysAdmin);
        string[] FetchUserRoles(string username);
        string[] FetchUserRolesPermissions(string username);
        List<PermissionDTO> RolePermissions(int? roleId);
        string AddPermissiontoRole(PermissionDTO perm, string userId);
        string RemovePermissionFromRole(PermissionDTO perm, string userId);
        string RemoveUserFromRole(UserDTO dto, string userId);
        string AddUserToRole(UserDTO dto, string userId);
        //User RegisterUserAsync(User user, string role);
        User RegisterUser(User user, string role, out string msg, string userId);
        User ValidateUser(string password, string username);
        void DeleteToken(int tokenId, string userId);
        bool ValidateToken(string token);
        string CreateToken(Token toke, string userIdn);
        List<Token> AllTokens();
        Token GetToken(int tokenId);
        void ChangeUserRole(string studentId, string userId, string oldRole, string newRole);
        string GenerateAndSendEmailToken(UserDTO user);
        string MaskUserEmail(string email);
        bool ValidateSimpleToken(string userId, string token);
    }
}
