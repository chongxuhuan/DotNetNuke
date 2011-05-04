#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.Collections;
using System.Data;
using System.Web.Security;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Services.Exceptions;

using AspNetSecurity = System.Web.Security;

#endregion

namespace DotNetNuke.Security.Membership
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      AspNetMembershipProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The AspNetMembershipProvider overrides the default MembershipProvider to provide
    /// an AspNet Membership Component (MemberRole) implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	12/09/2005	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class AspNetMembershipProvider : MembershipProvider
    {
        private readonly DataProvider _dataProvider;

        public AspNetMembershipProvider()
        {
            _dataProvider = DataProvider.Instance();
            if (_dataProvider == null)
            {
                var defaultprovider = DotNetNuke.Data.DataProvider.Instance().DefaultProviderName;
                const string dataProviderNamespace = "DotNetNuke.Security.Membership.Data";
                if (defaultprovider == "SqlDataProvider")
                {
                    _dataProvider = new SqlDataProvider();
                }
                else
                {
                    var providerType = dataProviderNamespace + "." + defaultprovider;
                    _dataProvider = (DataProvider) Reflection.CreateObject(providerType, providerType, true);
                }
                ComponentFactory.RegisterComponentInstance<DataProvider>(_dataProvider);
            }
        }

        public override bool CanEditProviderProperties
        {
            get
            {
                return false;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return System.Web.Security.Membership.MaxInvalidPasswordAttempts;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override int MinNonAlphanumericCharacters
        {
            get
            {
                return System.Web.Security.Membership.MinRequiredNonAlphanumericCharacters;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override int MinPasswordLength
        {
            get
            {
                return System.Web.Security.Membership.MinRequiredPasswordLength;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return System.Web.Security.Membership.PasswordAttemptWindow;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override PasswordFormat PasswordFormat
        {
            get
            {
                switch (System.Web.Security.Membership.Provider.PasswordFormat)
                {
                    case MembershipPasswordFormat.Encrypted:
                        return PasswordFormat.Encrypted;
                    case MembershipPasswordFormat.Hashed:
                        return PasswordFormat.Hashed;
                    default:
                        return PasswordFormat.Clear;
                }
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override bool PasswordResetEnabled
        {
            get
            {
                return System.Web.Security.Membership.EnablePasswordReset;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override bool PasswordRetrievalEnabled
        {
            get
            {
                return System.Web.Security.Membership.EnablePasswordRetrieval;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return System.Web.Security.Membership.RequiresQuestionAndAnswer;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return System.Web.Security.Membership.PasswordStrengthRegularExpression;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return System.Web.Security.Membership.Provider.RequiresUniqueEmail;
            }
            set
            {
                throw new NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config");
            }
        }

        private bool AutoUnlockUser(MembershipUser aspNetUser)
        {
            if (Host.AutoAccountUnlockDuration != 0)
            {
                if (aspNetUser.LastLockoutDate < DateTime.Now.AddMinutes(-1*Host.AutoAccountUnlockDuration))
                {
                    if (aspNetUser.UnlockUser())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CreateDNNUser persists the DNN User information to the Database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <returns>The UserId of the newly created user.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private UserCreateStatus CreateDNNUser(ref UserInfo user)
        {
            var objSecurity = new PortalSecurity();
            string userName = objSecurity.InputFilter(user.Username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string email = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string lastName = objSecurity.InputFilter(user.LastName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string firstName = objSecurity.InputFilter(user.FirstName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            UserCreateStatus createStatus = UserCreateStatus.Success;
            string displayName = objSecurity.InputFilter(user.DisplayName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            bool updatePassword = user.Membership.UpdatePassword;
            bool isApproved = user.Membership.Approved;
            try
            {
                user.UserID =
                    Convert.ToInt32(_dataProvider.AddUser(user.PortalID,
                                                         userName,
                                                         firstName,
                                                         lastName,
                                                         user.AffiliateID,
                                                         user.IsSuperUser,
                                                         email,
                                                         displayName,
                                                         updatePassword,
                                                         isApproved,
                                                         UserController.GetCurrentUserInfo().UserID));
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                user = null;
                createStatus = UserCreateStatus.ProviderError;
            }
            return createStatus;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CreateMemberhipUser persists the User as an AspNet MembershipUser to the AspNet
        /// Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <returns>A UserCreateStatus enumeration indicating success or reason for failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private UserCreateStatus CreateMemberhipUser(UserInfo user)
        {
            var objSecurity = new PortalSecurity();
            string userName = objSecurity.InputFilter(user.Username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string email = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            MembershipCreateStatus objStatus = MembershipCreateStatus.Success;
            MembershipUser objMembershipUser;
            if (MembershipProviderConfig.RequiresQuestionAndAnswer)
            {
                objMembershipUser = System.Web.Security.Membership.CreateUser(userName,
                                                                              user.Membership.Password,
                                                                              email,
                                                                              user.Membership.PasswordQuestion,
                                                                              user.Membership.PasswordAnswer,
                                                                              true,
                                                                              out objStatus);
            }
            else
            {
                objMembershipUser = System.Web.Security.Membership.CreateUser(userName, user.Membership.Password, email, null, null, true, out objStatus);
            }
            UserCreateStatus createStatus = UserCreateStatus.Success;
            switch (objStatus)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    createStatus = UserCreateStatus.DuplicateEmail;
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    createStatus = UserCreateStatus.DuplicateProviderUserKey;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    createStatus = UserCreateStatus.DuplicateUserName;
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    createStatus = UserCreateStatus.InvalidAnswer;
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    createStatus = UserCreateStatus.InvalidEmail;
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    createStatus = UserCreateStatus.InvalidPassword;
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    createStatus = UserCreateStatus.InvalidProviderUserKey;
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    createStatus = UserCreateStatus.InvalidQuestion;
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    createStatus = UserCreateStatus.InvalidUserName;
                    break;
                case MembershipCreateStatus.ProviderError:
                    createStatus = UserCreateStatus.ProviderError;
                    break;
                case MembershipCreateStatus.UserRejected:
                    createStatus = UserCreateStatus.UserRejected;
                    break;
            }
            return createStatus;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteMembershipUser deletes the User as an AspNet MembershipUser from the AspNet
        /// Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to delete from the Data Store.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/22/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool DeleteMembershipUser(UserInfo user)
        {
            bool retValue = true;
            try
            {
                System.Web.Security.Membership.DeleteUser(user.Username, true);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

                retValue = false;
            }
            return retValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Builds a UserMembership object from an AspNet MembershipUser
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="aspNetUser">The MembershipUser object to use to fill the DNN UserMembership.</param>
        /// <param name="user">The use need to fill</param>
        /// <history>
        /// 	[cnurse]	12/10/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void FillUserMembership(MembershipUser aspNetUser, UserInfo user)
        {
            if (aspNetUser != null)
            {
                if (user.Membership == null)
                {
                    user.Membership = new UserMembership(user);
                }
                user.Membership.CreatedDate = aspNetUser.CreationDate;
                user.Membership.LastActivityDate = aspNetUser.LastActivityDate;
                user.Membership.LastLockoutDate = aspNetUser.LastLockoutDate;
                user.Membership.LastLoginDate = aspNetUser.LastLoginDate;
                user.Membership.LastPasswordChangeDate = aspNetUser.LastPasswordChangedDate;
                user.Membership.LockedOut = aspNetUser.IsLockedOut;
                user.Membership.PasswordQuestion = aspNetUser.PasswordQuestion;
                user.Membership.IsDeleted = user.IsDeleted;

                if (user.IsSuperUser)
                {
                    user.Membership.Approved = aspNetUser.IsApproved;
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an AspNet MembershipUser from the DataStore
        /// </summary>
        /// <param name="user">The user to get from the Data Store.</param>
        /// <returns>The User as a AspNet MembershipUser object</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private MembershipUser GetMembershipUser(UserInfo user)
        {
            return GetMembershipUser(user.Username);
        }

        private MembershipUser GetMembershipUser(string userName)
        {
            return CBO.GetCachedObject<MembershipUser>(new CacheItemArgs(GetCacheKey(userName), DataCache.UserCacheTimeOut, DataCache.UserCachePriority, userName), GetMembershipUserCallBack);
        }

        private string GetCacheKey(string userName)
        {
            return String.Format("MembershipUser_{0}", userName);
        }

        private static object GetMembershipUserCallBack(CacheItemArgs cacheItemArgs)
        {
            string userName = cacheItemArgs.ParamList[0].ToString();

            return System.Web.Security.Membership.GetUser(userName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The GetTotalRecords method gets the number of Records returned.
        /// </summary>
        /// <param name="dr">An <see cref="IDataReader"/> containing the Total no of records</param>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	03/30/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int GetTotalRecords(ref IDataReader dr)
        {
            int total = 0;
            if (dr.Read())
            {
                try
                {
                    total = Convert.ToInt32(dr["TotalRecords"]);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                    total = -1;
                }
            }
            return total;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserByAuthToken retrieves a User from the DataStore using an Authentication Token
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userToken">The authentication token of the user being retrieved from the Data Store.</param>
        /// <param name="authType">The type of Authentication Used</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        ///     [cnurse]	07/09/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private UserInfo GetUserByAuthToken(int portalId, string userToken, string authType)
        {
            IDataReader dr = _dataProvider.GetUserByAuthToken(portalId, userToken, authType);
            UserInfo objUserInfo = UserController.FillUserInfo(portalId, dr, true);
            return objUserInfo;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateUserMembership persists a user's Membership to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateUserMembership(UserInfo user)
        {
            var objSecurity = new PortalSecurity();
            string email = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            MembershipUser objMembershipUser;
            objMembershipUser = System.Web.Security.Membership.GetUser(user.Username);
            objMembershipUser.Email = email;
            objMembershipUser.LastActivityDate = DateTime.Now;
            if (user.IsSuperUser)
            {
                objMembershipUser.IsApproved = user.Membership.Approved;
            }
            System.Web.Security.Membership.UpdateUser(objMembershipUser);
			DataCache.RemoveCache(GetCacheKey(user.Username));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Validates the users credentials against the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="username">The user name of the User attempting to log in</param>
        /// <param name="password">The password of the User attempting to log in</param>
        /// <returns>A Boolean result</returns>
        /// <history>
        ///     [cnurse]	12/12/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool ValidateUser(int portalId, string username, string password)
        {
            return System.Web.Security.Membership.ValidateUser(username, password);
        }

        public override bool ChangePassword(UserInfo user, string oldPassword, string newPassword)
        {
            bool retValue = false;
            MembershipUser aspnetUser = GetMembershipUser(user);
            if (oldPassword == Null.NullString)
            {
                aspnetUser.UnlockUser();
                oldPassword = aspnetUser.GetPassword();
            }
            retValue = aspnetUser.ChangePassword(oldPassword, newPassword);
            if (retValue && PasswordRetrievalEnabled && !RequiresQuestionAndAnswer)
            {
                string confirmPassword = aspnetUser.GetPassword();
                if (confirmPassword == newPassword)
                {
                    user.Membership.Password = confirmPassword;
                    retValue = true;
                }
                else
                {
                    retValue = false;
                }
            }
            return retValue;
        }

        public override bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer)
        {
            bool retValue = false;
            MembershipUser aspnetUser = GetMembershipUser(user);
            if (password == Null.NullString)
            {
                password = aspnetUser.GetPassword();
            }
            retValue = aspnetUser.ChangePasswordQuestionAndAnswer(password, passwordQuestion, passwordAnswer);
            return retValue;
        }

        public override UserCreateStatus CreateUser(ref UserInfo user)
        {
            UserCreateStatus createStatus;
            try
            {
                UserInfo objVerifyUser = GetUserByUserName(Null.NullInteger, user.Username);
                if (objVerifyUser != null)
                {
                    if (objVerifyUser.IsSuperUser)
                    {
                        createStatus = UserCreateStatus.UserAlreadyRegistered;
                    }
                    else
                    {
                        if (ValidateUser(objVerifyUser.PortalID, user.Username, user.Membership.Password))
                        {
                            objVerifyUser = GetUserByUserName(user.PortalID, user.Username);
                            if (objVerifyUser != null)
                            {
                                createStatus = UserCreateStatus.UserAlreadyRegistered;
                            }
                            else
                            {
                                createStatus = UserCreateStatus.AddUserToPortal;
                            }
                        }
                        else
                        {
                            createStatus = UserCreateStatus.UsernameAlreadyExists;
                        }
                    }
                }
                else
                {
                    createStatus = UserCreateStatus.AddUser;
                }
                if (createStatus == UserCreateStatus.AddUser)
                {
                    createStatus = CreateMemberhipUser(user);
                }
                if (createStatus == UserCreateStatus.Success || createStatus == UserCreateStatus.AddUserToPortal)
                {
                    createStatus = CreateDNNUser(ref user);
                    if (createStatus == UserCreateStatus.Success)
                    {
                        ProfileController.UpdateUserProfile(user);
                    }
                }
                if (createStatus == UserCreateStatus.UserAlreadyRegistered)
                {
                    if (objVerifyUser.IsDeleted)
                    {
                        objVerifyUser.IsDeleted = false;
                        objVerifyUser.Membership.Approved = user.Membership.Approved;
                        UpdateUser(objVerifyUser);
                        createStatus = UserCreateStatus.Success;
                        user.UserID = objVerifyUser.UserID;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                createStatus = UserCreateStatus.UnexpectedError;
            }
            return createStatus;
        }

        public override bool DeleteUser(UserInfo user)
        {
            bool retValue = true;
            IDataReader dr = null;
            try
            {
                dr = _dataProvider.GetRolesByUser(user.UserID, user.PortalID);
                while (dr.Read())
                {
                    _dataProvider.DeleteUserRole(user.UserID, Convert.ToInt32(dr["RoleId"]));
                }
                _dataProvider.DeleteUserPortal(user.UserID, user.PortalID);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                retValue = false;
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return retValue;
        }

        public override bool RestoreUser(UserInfo user)
        {
            var retValue = true;

            try
            {
                _dataProvider.RestoreUser(user.UserID, user.PortalID);
            }
            catch (Exception ex)
            {

                Exceptions.LogException(ex);
                retValue = false;
            }

            return retValue;
        }

        public override bool RemoveUser(UserInfo user)
        {
            bool retValue = true;

            try
            {
                _dataProvider.RemoveUser(user.UserID, user.PortalID);
                //Prior to removing membership, ensure user is not present in any other portal
                UserInfo otherUser = GetUserByUserName(Null.NullInteger, user.Username);
                if (otherUser != null)
                {
                    DeleteMembershipUser(user);
                }   
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                retValue = false;
            }

            return retValue;
        }

        public override void DeleteUsersOnline(int timeWindow)
        {
            _dataProvider.DeleteUsersOnline(timeWindow);
        }

        public override string GeneratePassword()
        {
            return GeneratePassword(MinPasswordLength + 4);
        }

        public override string GeneratePassword(int length)
        {
            return System.Web.Security.Membership.GeneratePassword(length, MinNonAlphanumericCharacters);
        }

        public override ArrayList GetOnlineUsers(int portalId)
        {
            int totalRecords = 0;
            return UserController.FillUserCollection(portalId, _dataProvider.GetOnlineUsers(portalId), ref totalRecords);
        }

        public override string GetPassword(UserInfo user, string passwordAnswer)
        {
            string retValue = "";
            bool unLocked = true;
            MembershipUser aspnetUser = GetMembershipUser(user);
            if (aspnetUser.IsLockedOut)
            {
                unLocked = AutoUnlockUser(aspnetUser);
            }
            if (RequiresQuestionAndAnswer)
            {
                retValue = aspnetUser.GetPassword(passwordAnswer);
            }
            else
            {
                retValue = aspnetUser.GetPassword();
            }
            return retValue;
        }

        public override ArrayList GetUnAuthorizedUsers(int portalId, bool includeDeleted, bool superUsersOnly)
        {
            return UserController.FillUserCollection(portalId, _dataProvider.GetUnAuthorizedUsers(portalId, includeDeleted, superUsersOnly));
        }
        
        public override ArrayList GetUnAuthorizedUsers(int portalId)
        {
            return GetUnAuthorizedUsers(portalId, false, false);
        }

        public override ArrayList GetDeletedUsers(int portalId)
        {
            return UserController.FillUserCollection(portalId, _dataProvider.GetDeletedUsers(portalId));
        }

        public override UserInfo GetUser(int portalId, int userId)
        {
            IDataReader dr = _dataProvider.GetUser(portalId, userId);
            UserInfo objUserInfo = UserController.FillUserInfo(portalId, dr, true);
            return objUserInfo;
        }

        public override UserInfo GetUserByUserName(int portalId, string username)
        {
            IDataReader dr = _dataProvider.GetUserByUsername(portalId, username);
            UserInfo objUserInfo = UserController.FillUserInfo(portalId, dr, true);
            return objUserInfo;
        }

        public override int GetUserCountByPortal(int portalId)
        {
            return _dataProvider.GetUserCountByPortal(portalId);
        }

        public override void GetUserMembership(ref UserInfo user)
        {
            MembershipUser aspnetUser = null;
            aspnetUser = GetMembershipUser(user);
            FillUserMembership(aspnetUser, user);
            user.Membership.IsOnLine = IsUserOnline(user);
        }

        public override ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return UserController.FillUserCollection(portalId, _dataProvider.GetAllUsers(portalId, pageIndex, pageSize, includeDeleted, superUsersOnly), ref totalRecords);
        }

        public override ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsers(portalId, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public override ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return UserController.FillUserCollection(portalId, _dataProvider.GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, includeDeleted, superUsersOnly), ref totalRecords);
        }

        public override ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public override ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return UserController.FillUserCollection(portalId, _dataProvider.GetUsersByUsername(portalId, userNameToMatch, pageIndex, pageSize, includeDeleted, superUsersOnly), ref totalRecords);
        }
      
        public override ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public override ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return UserController.FillUserCollection(portalId, _dataProvider.GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, includeDeleted, superUsersOnly), ref totalRecords);
        }

        public override ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public override bool IsUserOnline(UserInfo user)
        {
            bool isOnline = false;
            var objUsersOnline = new UserOnlineController();
            if (objUsersOnline.IsEnabled())
            {
                Hashtable userList = objUsersOnline.GetUserList();
                var onlineUser = (OnlineUserInfo) userList[user.UserID.ToString()];
                if (onlineUser != null)
                {
                    isOnline = true;
                }
                else
                {
                    onlineUser = (OnlineUserInfo) CBO.FillObject(_dataProvider.GetOnlineUser(user.UserID), typeof (OnlineUserInfo));
                    if (onlineUser != null)
                    {
                        isOnline = true;
                    }
                }
            }
            return isOnline;
        }

        public override string ResetPassword(UserInfo user, string passwordAnswer)
        {
            string retValue = "";
            MembershipUser aspnetUser = GetMembershipUser(user);
            if (RequiresQuestionAndAnswer)
            {
                retValue = aspnetUser.ResetPassword(passwordAnswer);
            }
            else
            {
                retValue = aspnetUser.ResetPassword();
            }
            return retValue;
        }

        public override bool UnLockUser(UserInfo user)
        {
            MembershipUser objMembershipUser;
            objMembershipUser = System.Web.Security.Membership.GetUser(user.Username);
            return objMembershipUser.UnlockUser();
        }

        public override void UpdateUser(UserInfo user)
        {
            var objSecurity = new PortalSecurity();
            string firstName = objSecurity.InputFilter(user.FirstName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string lastName = objSecurity.InputFilter(user.LastName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string email = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            string displayName = objSecurity.InputFilter(user.DisplayName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            bool updatePassword = user.Membership.UpdatePassword;
            bool isApproved = user.Membership.Approved;
            if (String.IsNullOrEmpty(displayName))
            {
                displayName = firstName + " " + lastName;
            }
            UpdateUserMembership(user);
            _dataProvider.UpdateUser(user.UserID,
                                    user.PortalID,
                                    firstName,
                                    lastName,
                                    email,
                                    displayName,
                                    updatePassword,
                                    isApproved,
                                    user.RefreshRoles,
                                    user.LastIPAddress,
                                    user.IsDeleted,
                                    UserController.GetCurrentUserInfo().UserID);
            ProfileController.UpdateUserProfile(user);
        }

        public override void UpdateUsersOnline(Hashtable userList)
        {
            _dataProvider.UpdateUsersOnline(userList);
        }

        public override UserInfo UserLogin(int portalId, string username, string password, string verificationCode, ref UserLoginStatus loginStatus)
        {
            return UserLogin(portalId, username, password, "DNN", verificationCode, ref loginStatus);
        }

        public override UserInfo UserLogin(int portalId, string username, string password, string authType, string verificationCode, ref UserLoginStatus loginStatus)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;

            DataCache.ClearUserCache(portalId, username);
            DataCache.ClearCache(GetCacheKey(username));

            UserInfo user;
            if (authType == "DNN")
            {
                user = GetUserByUserName(portalId, username);
            }
            else
            {
                user = GetUserByAuthToken(portalId, username, authType);
            }
            if (user != null && !user.IsDeleted)
            {
                MembershipUser aspnetUser = null;
                aspnetUser = GetMembershipUser(user);
                FillUserMembership(aspnetUser, user);
                if (aspnetUser.IsLockedOut)
                {
                    if (AutoUnlockUser(aspnetUser))
                    {
                        user.Membership.LockedOut = false;
                    }
                    else
                    {
                        loginStatus = UserLoginStatus.LOGIN_USERLOCKEDOUT;
                    }
                }
                if (user.Membership.Approved == false && user.IsSuperUser == false)
                {
                    if (verificationCode == (portalId + "-" + user.UserID))
                    {
                        user.Membership.Approved = true;
                        UpdateUser(user);
                    }
                    else
                    {
                        loginStatus = UserLoginStatus.LOGIN_USERNOTAPPROVED;
                    }
                }
                bool bValid = false;
                if (loginStatus != UserLoginStatus.LOGIN_USERLOCKEDOUT && loginStatus != UserLoginStatus.LOGIN_USERNOTAPPROVED)
                {
                    if (authType == "DNN")
                    {
                        if (user.IsSuperUser)
                        {
                            if (ValidateUser(Null.NullInteger, username, password))
                            {
                                loginStatus = UserLoginStatus.LOGIN_SUPERUSER;
                                bValid = true;
                            }
                        }
                        else
                        {
                            if (ValidateUser(portalId, username, password))
                            {
                                loginStatus = UserLoginStatus.LOGIN_SUCCESS;
                                bValid = true;
                            }
                        }
                    }
                    else
                    {
                        if (user.IsSuperUser)
                        {
                            loginStatus = UserLoginStatus.LOGIN_SUPERUSER;
                            bValid = true;
                        }
                        else
                        {
                            loginStatus = UserLoginStatus.LOGIN_SUCCESS;
                            bValid = true;
                        }
                    }
                }
                if (!bValid)
                {
                    user = null;
                }
            }
            else
            {
                user = null;
            }
            return user;
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override ArrayList GetUnAuthorizedUsers(int portalId, bool isHydrated)
        {
            return GetUnAuthorizedUsers(portalId);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override UserInfo GetUser(int portalId, int userId, bool isHydrated)
        {
            return GetUser(portalId, userId);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override UserInfo GetUserByUserName(int portalId, string username, bool isHydrated)
        {
            return GetUserByUserName(portalId, username);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override ArrayList GetUsers(int portalId, bool isHydrated, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsers(portalId, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override ArrayList GetUsersByEmail(int portalId, bool isHydrated, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override ArrayList GetUsersByUserName(int portalId, bool isHydrated, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")]
        public override ArrayList GetUsersByProfileProperty(int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords);
        }
    }
}
