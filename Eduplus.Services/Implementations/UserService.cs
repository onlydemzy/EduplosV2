using Eduplos.Domain.CoreModule;
using Eduplos.DTO.UserManagement;
using Eduplos.Services.Contracts;
using Eduplos.Services.Implementations;
using KS.AES256Encryption;
using KS.Core;
using KS.Core.UserManagement;
using KS.Domain.HRModule;
using KS.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KS.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommunicationService _comService;
        
       public UserService(IUnitOfWork unitOfWork,ICommunicationService communication)
        {
              
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            _comService = communication;
        }
        public User ValidateUser(string password, string username)
        {
            bool isValid;
            
            User user =_unitOfWork.UserRepository.GetFiltered(u=>u.UserName==username&&u.IsActive==true).SingleOrDefault();
            if (user == null)
            { return user = null; }

            isValid = PasswordMaker.ValidateUserPassword(password, user.Password);
            if (isValid == false)//password is not correct
            {
                return user = null;
            }

            if (user.IsActive == false)
            { return user = null; }
            return user;
        }

        
        public User RegisterUser(User user, string role,out string msg,string userId)
        {
            //Check if user already exist
            
            var _dbuser = _unitOfWork.UserRepository.GetFiltered(u => u.UserName == user.UserName).FirstOrDefault();
            if(_dbuser!=null)
            {
                msg = "Exist";
                return new User();
            }

            var srole =  SingleRole(role);
            
                string passwordHased = PasswordMaker.HashPassword(user.Password);

                user.Password = passwordHased;
                user.IsActive = true;
                user.LastActivityDate = DateTime.UtcNow;
                user.CreateDate = DateTime.UtcNow;
                user.LoginCounter = 0;
                user.UserRoles.Add(srole);
                var _user=_unitOfWork.UserRepository.Add(user);
                _unitOfWork.Commit(userId);
            msg = "Successful";
                return _user;
          
        }

        public void ChangeUserRole(string studentId,string userId,string oldRole,string newRole)
        {
            var user = _unitOfWork.UserRepository.GetFiltered(a => a.UserCode==studentId).SingleOrDefault();
            var userRole = _unitOfWork.RoleRepository.GetAll();
            var pros = userRole.Where(a => a.RoleName == oldRole).SingleOrDefault();
            var stu = userRole.Where(a => a.RoleName == newRole).SingleOrDefault();
            if(newRole=="Student")
            {
                var student = _unitOfWork.StudentRepository.Get(studentId);
                if(!string.IsNullOrEmpty(student.MatricNumber))
                {
                    user.UserName = student.MatricNumber;
                }
                
            }

            user.UserRoles.Remove(pros);
            user.UserRoles.Add(stu);
            _unitOfWork.Commit(userId);

        }
        
        /// <summary>
        /// Change password
        /// Condition1: Administrator wants to change for user
        /// Condition2: User has logged in and want to change
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public string ChangePassword(string username, string oldPassword, string newPassword,string userId)//user knows his password
        {

            bool isValid;
            
            User user =_unitOfWork.UserRepository.GetFiltered(u=>u.UserName==username).SingleOrDefault();
            if (user == null)
            { return "User does not exist"; }//User does not exist

            var userrole = user.UserRoles.Select(r => r.IsSystemAdmin == true);
            if (string.IsNullOrEmpty(oldPassword)&&userrole.Count()>0)//admin wants to reset the password
            {
                user.Password = PasswordMaker.HashPassword(newPassword);
                 //_unitOfWork.SetModified<User>(user);
                _unitOfWork.Commit(userId);
                return "Ok";
            }
            
            //Valid old user password
            isValid = PasswordMaker.ValidateUserPassword(oldPassword, user.Password);//check if password is valid
            if (isValid == false)//password is not correct
            {
                return "invalid old password inputted";
            }

            //UpDate User Password

            user.Password = PasswordMaker.HashPassword(newPassword);
            user.LastActivityDate = DateTime.UtcNow;
            _unitOfWork.SetModified<User>(user);
            _unitOfWork.Commit(userId);
            return "Password successfully changed";
        }
        
        public string MaskUserEmail(string email)
        {
            string first2X = email.Substring(0, 2);
            int indexAt = email.IndexOf("@");
            string[] re = email.Split('@');

            string last2B4At = email.Substring(indexAt - 2, 2);
            return first2X + "***" + last2B4At + "@" + re[1];
        }
        #region TOKEN OPERATIONS
        public string CreateToken(Token token, string userId)
        {
            try
            {
                
                
                token.ClientId = DataEncryption.GetUniqueKey();
                token.ClientSecret = DataEncryption.GetUniqueKey();
                token.IssuedDate = DateTime.UtcNow;
                string uniqueKey= DataEncryption.GetUniqueKey();
                string tokenencrypt= token.ClientId+"~" +uniqueKey+ "~"+ token.ClientSecret  + "~"+token.Company+ "~" + token.Url;
                token.AuthToken = DataEncryption.AESEncryptData(tokenencrypt);
                _unitOfWork.TokenRepository.Add(token);
                _unitOfWork.Commit(userId);
                return "Token successfully created";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public bool ValidateSimpleToken(string userId,string token)
        {
            var to = _unitOfWork.TokenRepository.GetFiltered(a => a.UserId == userId && a.AuthToken == token
            && a.ExpiryDate>=DateTime.UtcNow).FirstOrDefault();
            if (to == null)
                return false;
            else return true;
        }
        public bool ValidateToken(string token)
        {
           
                bool isValid = false;
                if (string.IsNullOrEmpty(token))
                    return isValid;
                string decryptText = DataEncryption.AESDecryptData(token);
                string[] tokenParts = decryptText.Split('~');
                var clientId = tokenParts[0];
                var uniqueKey = tokenParts[1];
                var secret = tokenParts[2];
                
                string company = tokenParts[3];
                string url = tokenParts[4];
                var dbtoken = _unitOfWork.TokenRepository.GetSingle(a => a.AuthToken == token
                &&a.ClientId==clientId&&a.ClientSecret==secret&&a.Company==company && a.Url==url);
                if (dbtoken == null)
                    return isValid;

                
                {
                    isValid = true;

                    return isValid;
                    
                }
            
        }
        public string GenerateAndSendEmailToken(UserDTO user)
        {
            var tk= _unitOfWork.TokenRepository.GetFiltered(t => t.UserId == user.UserId).FirstOrDefault();
            string tk1 = GenerateSimpleToken();
            _unitOfWork.TokenRepository.Add(new Token
            {
                UserId = user.UserId,
                AuthToken = tk1,
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10)
            });
            _unitOfWork.Commit(user.UserId);
            var st = _unitOfWork.StudentRepository.Get(user.UserId);
             
            //send token
            string msg = $@"Hello {st.Name},
                         Your password reset code={tk1}.
                            Please note that this code will expire in 10minutes time. 
                            If you are not the one that initiated this action please ignore this mail and change ypur password immediately";
            _comService.SendMail(user.FullName, user.Email, msg, "Password change verification");
            return "Ok";
        }
        string GenerateSimpleToken()
        {
            Random ran = new Random();
            return ran.Next(100000, 999999).ToString();
        }
        public void DeleteToken(int tokenId, string userId)
        {
            var dbtoken = _unitOfWork.TokenRepository.Get(tokenId);
            _unitOfWork.TokenRepository.Remove(dbtoken);
            _unitOfWork.Commit(userId);
            
        }

        public List<Token> AllTokens()
        {
            return _unitOfWork.TokenRepository.GetAll().ToList();
        }
        public Token GetToken(int tokenId)
        {
            return _unitOfWork.TokenRepository.Get(tokenId);
        }
        #endregion
        #region //Roles Management
        public List<string> FetchRoles()
        {
            List<string> _roles = new List<string>();
            var roles = _unitOfWork.RoleRepository.GetAll();
            foreach (var r in roles)
            {
                _roles.Add(r.RoleName);
            }
            return _roles;
        }
        public List<RoleDTO> AllRoles()
        {
            List<RoleDTO> dto = new List<RoleDTO>();
            var roles = _unitOfWork.RoleRepository.GetAll().ToList();
            foreach (var r in roles)
            {
                dto.Add(new RoleDTO
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                });
            }
            return dto;
        }
        public string DeleteRole(int roleId, string userId)
        {
            var role = _unitOfWork.RoleRepository.Get(roleId);
            try
            {
                _unitOfWork.RoleRepository.Remove(role);
                _unitOfWork.Commit(userId);
                return "Delete was successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AddRole(RoleDTO nrole, string userId)
        {
            var role = _unitOfWork.RoleRepository.GetFiltered(a => a.RoleName == nrole.RoleName).FirstOrDefault();
            if (role != null)
            {
                return "Role already exist";
            }

            _unitOfWork.RoleRepository.Add(new Role
            {
                RoleName = nrole.RoleName
            });
            _unitOfWork.Commit(userId);
            return "Role added successfully";
        }
        public string[] FetchUserRoles(string username)
        {

            List<string> roles = new List<string>();
            var user = _unitOfWork.UserRepository.GetFiltered(r => r.UserName == username).SingleOrDefault();

            foreach(var r in user.UserRoles)
            {
                roles.Add(r.RoleName);
            }
            return roles.ToArray();          
        }

        public string[] FetchUserRolesPermissions(string username)
        {
            List<string> perms = new List<string>();
            
            var user= _unitOfWork.UserRepository.GetFiltered(r => r.UserName == username).SingleOrDefault();
            foreach(var r in user.UserRoles)
            {
                foreach(var p in r.Permissions)
                {
                    perms.Add(p.Activity);
                    
                }
            }
            return perms.ToArray();
        }

        public List<MenuItem> FetchUserMenus(string[] permissions, bool sysAdmin)
        {
            
            //return uow.UserRepository.GetUserMenus(permissions, sysAdmin);
            List<MenuItem> menus = new List<MenuItem>();
            List<MenuItem> children = new List<MenuItem>();
            
            if (sysAdmin == true)//return all menu items
            {
                var mem = _unitOfWork.MenuItemRepository.GetFiltered(m => m.ParentMenuItem == null && m.AlwaysEnable==true).ToList();
                           

                foreach (var m in mem)
                {
                    MenuItem mI = new MenuItem();
                    mI.Action = m.Action;
                    mI.AlwaysEnable = m.AlwaysEnable;
                    mI.Controller = m.Controller;
                    mI.Description = m.Description;
                    mI.MenuItemId = m.MenuItemId;
                    mI.ParentMenuItemId = m.ParentMenuItemId;
                    mI.MenuItemName = m.MenuItemName;
                    mI.Heading = m.Heading;
                    mI.fawsome = m.fawsome;
                    mI.Collapse = m.Collapse;

                    if (m.ChildrenMenus.Count>0)
                    {
                        mI.ChildrenMenus = m.ChildrenMenus.ToList();
                    }
                    menus.Add(mI);
                }

                return menus;
            }

            foreach (string perm in permissions)
            {
                var menuItems = _unitOfWork.MenuItemRepository.GetFiltered(m => m.ParentMenuItem == null&&m.Permission.Activity==perm).ToList();

                foreach (var m in menuItems)
                {
                    MenuItem newMen = new MenuItem();
                    newMen.MenuItemName = m.MenuItemName;
                    newMen.Controller = m.Controller;
                    newMen.Action = m.Action;
                    newMen.fawsome = m.fawsome;
                    newMen.Heading = m.Heading;
                    newMen.Collapse = m.Collapse;
                    if (m.ChildrenMenus.Count > 0)
                    {
                        foreach (var mn in m.ChildrenMenus)
                        {
                            MenuItem cmen = new MenuItem();
                            cmen.MenuItemName = mn.MenuItemName;
                            cmen.Controller = mn.Controller;
                            cmen.Action = mn.Action;
                            newMen.ChildrenMenus.Add(cmen);
                        }

                    }
                    menus.Add(newMen);
                }
            }


            return menus;
        }
        public List<PermissionDTO> RolePermissions(int? roleId)
        {
            List<PermissionDTO> pem = new List<PermissionDTO>();
            if (roleId>0)
            {
                var role = _unitOfWork.RoleRepository.Get(Convert.ToInt32(roleId));
                
                foreach (var p in role.Permissions)
                {

                    string[] chk = p.Activity.Split('-');
                    PermissionDTO rp = new PermissionDTO();
                    rp.PermissionId = p.PermissionId;
                    rp.RoleId = role.RoleId;
                    rp.Activity = p.Activity;
                    if (chk.Length == 2)
                    {                        
                        rp.Controller = p.Activity.Split('-')[0];
                        rp.Action = p.Activity.Split('-')[1];                    
                    }
                    else
                    {
                        rp.Controller = p.Activity;
                        rp.Action = p.Activity;
                    }
                    pem.Add(rp);

                }
            }
            else
            {
                var role = _unitOfWork.PermissionRepository.GetAll();

                foreach (var p in role)
                {

                    string[] chk = p.Activity.Split('-');
                    PermissionDTO rp = new PermissionDTO();
                    rp.PermissionId = p.PermissionId;
                    rp.Activity = p.Activity;
                    if (chk.Length == 2)
                    {
                        rp.Controller = p.Activity.Split('-')[0];
                        rp.Action = p.Activity.Split('-')[1];    
                    }
                    else
                    {
                        rp.Controller = p.Activity;
                        rp.Action = p.Activity;
                    }
                    
                    pem.Add(rp);

                }
            }
            
            return pem.OrderBy(a=>a.Controller).ToList();
        }

        public string AddPermissiontoRole(PermissionDTO perm,string userId)
        {
            var role = _unitOfWork.RoleRepository.Get(perm.RoleId);
            
            //check for existing permissions
            var roleperm = role.Permissions.Where(a => a.PermissionId == perm.PermissionId).FirstOrDefault();
            if (roleperm != null)
                return "Error: this permission already exist in role";

            var dbperm = _unitOfWork.PermissionRepository.Get(perm.PermissionId);
            role.Permissions.Add(dbperm);
            
            _unitOfWork.Commit(userId);

            return "Permission successfully add to role";
           
        }
        public string RemovePermissionFromRole(PermissionDTO perm, string userId)
        {
            var role = _unitOfWork.RoleRepository.Get(perm.RoleId);

            //check for existing permissions
            var roleperm = role.Permissions.Where(a => a.PermissionId == perm.PermissionId).FirstOrDefault();
            if (roleperm == null)
                return "Error: permission does not exist in role";
            
            role.Permissions.Remove(roleperm);

            _unitOfWork.Commit(userId);

            return "Permission successfully removed from role";

        }
        public string AddPermission(PermissionDTO perm, string userId)
        {
            var perms = _unitOfWork.PermissionRepository.GetAll().OrderBy(a => a.PermissionId).ToList().Last();
            int no = Convert.ToInt32(perms);
            var role = _unitOfWork.RoleRepository.Get(perm.RoleId);
            no++;
            string permno = "";
            string sn = no.ToString();
            switch (sn.Length)
            {
                case 1:
                    permno = "00" + sn;
                    break;
                case 2:
                    permno = "0" + sn;
                    break;
                case 3:
                    permno = sn;
                    break;
            }
            Permission nperm = new Permission();
            nperm.PermissionId = permno;
            nperm.Activity = perm.Activity;
            nperm.Module = perm.Controller;
            nperm.UserRoles.Add(role);
            _unitOfWork.PermissionRepository.Add(nperm);
            _unitOfWork.Commit(userId);

            return "Permission Added Successfully";

        }
        public User FetchSingleUser(string userName)
        {

            string email = "";
            User user =_unitOfWork.UserRepository.GetFiltered(u=>u.UserName==userName).SingleOrDefault();
            if(user != null )
            {
                var stUser = _unitOfWork.StudentRepository.GetFiltered(s=>s.PersonId==user.UserId).SingleOrDefault();
                if(stUser!=null)
                {
                    email = stUser.Email;
                }
                else
                {
                    var staffUser = _unitOfWork.StaffRepository.GetFiltered(s => s.PersonId == user.UserId).SingleOrDefault();
                    if(staffUser!=null)
                    {
                        email = staffUser.Email;
                    }
                }
                user.UserCode = email;

            }
       
            return user;
        }
        public List<User>FetchRoleUsers(int roleid)
        {
            var users = _unitOfWork.RoleRepository.Get(roleid);
            List<User> ur = new List<User>();
            foreach(var u in users.Users)
            {
                ur.Add(new User
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FullName = u.FullName
                });
            }
            return ur;
        }
        public List<UserDTO> FetchAllUsers()
        {
            var users = _unitOfWork.UserRepository.GetFiltered(a=>a.IsActive==true).ToList();
            List<UserDTO> ur = new List<UserDTO>();
            foreach (var u in users)
            {
                var dto = new UserDTO();

                dto.UserId = u.UserId;
                dto.UserName = u.UserName;
                dto.FullName = u.FullName;
                if(u.UserRoles.Count()>0)
                {
                    dto.RoleIDs = u.UserRoles.Select(a=>a.RoleId).ToArray();
                    dto.Roles = u.UserRoles.Select(a => a.RoleName).ToArray();
                }
                ur.Add(dto);
            }
            return ur;
        }
        public string AddUserToRole(UserDTO dto,string userId)
        {
            var dbrole = _unitOfWork.RoleRepository.Get(dto.NRoleID);
            var userroles = dbrole.Users.Where(a => a.UserId == dto.UserId).FirstOrDefault();
            if (userroles != null)
                return "Error, user already in selected role";

            var user = _unitOfWork.UserRepository.Get(dto.UserId);
            dbrole.Users.Add(user);
            _unitOfWork.Commit(userId);
            return "User added to role";

        }
        public string RemoveUserFromRole(UserDTO dto, string userId)
        {
            var dbrole = _unitOfWork.RoleRepository.Get(dto.NRoleID);
            var userroles = dbrole.Users.Where(a => a.UserId == dto.UserId).FirstOrDefault();
            if (string.IsNullOrEmpty(dto.UserId))
                return "Error, user cannot be removed from role";

            var user = _unitOfWork.UserRepository.Get(dto.UserId);
            dbrole.Users.Remove(user);
            _unitOfWork.Commit(userId);
            return "User removed from role";

        }
        
        private Role SingleRole(string title)
        {
            return _unitOfWork.RoleRepository.GetFiltered(r => r.RoleName == title).FirstOrDefault();
        }
        #endregion

        #region Private helper methods For password hashing

        //Validating user inputted password
        //parameters:password, correctHash stored in db
        //returns true if both passwords matches and false if otherwise


       
        

        

        
        #endregion
    }
}
