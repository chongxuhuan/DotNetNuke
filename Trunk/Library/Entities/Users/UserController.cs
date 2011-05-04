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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Messaging.Data;

using MembershipProvider = DotNetNuke.Security.Membership.MembershipProvider;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      UserController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserController class provides Business Layer methods for Users
    /// </summary>
    /// <remarks>
	/// DotNetNuke user management is base on asp.net membership provider, but  the default implementation of these providers 
	/// do not satisfy the broad set of use cases which we need to support in DotNetNuke. so The dependency of DotNetNuke on the 
	/// MemberRole (ASP.NET 2 Membership) components will be abstracted into a DotNetNuke Membership Provider, in order to allow 
	/// developers complete flexibility in implementing alternate Membership approaches.
	/// <list type="bullet">
	/// <item>This will allow for a number of enhancements to be added</item>
	/// <item>Removal of dependence on the HttpContext</item>
	/// <item>Support for Hashed Passwords</item>
	/// <item>Support for Password Question and Answer</item>
	/// <item>Enforce Password Complexity</item>
	/// <item>Password Aging (Expiry)</item>
	/// <item>Force Password Update</item>
	/// <item>Enable/Disable Password Retrieval/Reset</item>
	/// <item>CAPTCHA Support</item>
	/// <item>Redirect after registration/login/logout</item>
	/// </list>
    /// </remarks>
    /// <history>
    ///     [cnurse]	02/18/2004	documented
    ///     [cnurse]    05/23/2005  made compatible with .NET 2.0
    /// </history>
	/// <seealso cref="DotNetNuke.Security.Membership.MembershipProvider"/>
    /// -----------------------------------------------------------------------------
    public class UserController
    {
        private static readonly MembershipProvider MemberProvider = MembershipProvider.Instance();

        public string DisplayFormat { get; set; }

        public int PortalId { get; set; }

        private static void AddEventLog(int portalId, string username, int userId, string portalName, string ip, UserLoginStatus loginStatus)
        {
            var objEventLog = new EventLogController();
            var objEventLogInfo = new LogInfo();
            var objSecurity = new PortalSecurity();
            objEventLogInfo.AddProperty("IP", ip);
            objEventLogInfo.LogPortalID = portalId;
            objEventLogInfo.LogPortalName = portalName;
            objEventLogInfo.LogUserName = objSecurity.InputFilter(username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            objEventLogInfo.LogUserID = userId;
            objEventLogInfo.LogTypeKey = loginStatus.ToString();
            objEventLog.AddLog(objEventLogInfo);
        }

        private static object GetCachedUserByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            var portalId = (int) cacheItemArgs.ParamList[0];
            var username = (string) cacheItemArgs.ParamList[1];
            return MemberProvider.GetUserByUserName(portalId, username);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        /// -----------------------------------------------------------------------------
        internal static Hashtable GetUserSettings(Hashtable settings)
        {
            var portalSettings = PortalController.GetCurrentPortalSettings();
            if (settings["Column_FirstName"] == null)
            {
                settings["Column_FirstName"] = false;
            }
            if (settings["Column_LastName"] == null)
            {
                settings["Column_LastName"] = false;
            }
            if (settings["Column_DisplayName"] == null)
            {
                settings["Column_DisplayName"] = true;
            }
            if (settings["Column_Address"] == null)
            {
                settings["Column_Address"] = true;
            }
            if (settings["Column_Telephone"] == null)
            {
                settings["Column_Telephone"] = true;
            }
            if (settings["Column_Email"] == null)
            {
                settings["Column_Email"] = false;
            }
            if (settings["Column_CreatedDate"] == null)
            {
                settings["Column_CreatedDate"] = true;
            }
            if (settings["Column_LastLogin"] == null)
            {
                settings["Column_LastLogin"] = false;
            }
            if (settings["Column_Authorized"] == null)
            {
                settings["Column_Authorized"] = true;
            }
            if (settings["Display_Mode"] == null)
            {
                settings["Display_Mode"] = DisplayMode.All;
            }
            else
            {
                settings["Display_Mode"] = (DisplayMode) settings["Display_Mode"];
            }
            if (settings["Display_SuppressPager"] == null)
            {
                settings["Display_SuppressPager"] = false;
            }
            if (settings["Records_PerPage"] == null)
            {
                settings["Records_PerPage"] = 10;
            }
            if (settings["Profile_DefaultVisibility"] == null)
            {
                settings["Profile_DefaultVisibility"] = UserVisibilityMode.AdminOnly;
            }
            else
            {
                settings["Profile_DefaultVisibility"] = (UserVisibilityMode) settings["Profile_DefaultVisibility"];
            }
            if (settings["Profile_DisplayVisibility"] == null)
            {
                settings["Profile_DisplayVisibility"] = true;
            }
            if (settings["Profile_ManageServices"] == null)
            {
                settings["Profile_ManageServices"] = true;
            }
            if (settings["Redirect_AfterLogin"] == null)
            {
                settings["Redirect_AfterLogin"] = -1;
            }
            if (settings["Redirect_AfterRegistration"] == null)
            {
                settings["Redirect_AfterRegistration"] = -1;
            }
            if (settings["Redirect_AfterLogout"] == null)
            {
                settings["Redirect_AfterLogout"] = -1;
            }
            if (settings["Security_CaptchaLogin"] == null)
            {
                settings["Security_CaptchaLogin"] = false;
            }
            if (settings["Security_CaptchaRegister"] == null)
            {
                settings["Security_CaptchaRegister"] = false;
            }
            if (settings["Security_EmailValidation"] == null)
            {
                settings["Security_EmailValidation"] = Globals.glbEmailRegEx;
            }
            if (settings["Security_RequireValidProfile"] == null)
            {
                settings["Security_RequireValidProfile"] = false;
            }
            if (settings["Security_RequireValidProfileAtLogin"] == null)
            {
                settings["Security_RequireValidProfileAtLogin"] = true;
            }
            if (settings["Security_UsersControl"] == null)
            {
                if (portalSettings != null && portalSettings.Users > 1000)
                {
                    settings["Security_UsersControl"] = UsersControl.TextBox;
                }
                else
                {
                    settings["Security_UsersControl"] = UsersControl.Combo;
                }
            }
            else
            {
                settings["Security_UsersControl"] = (UsersControl) settings["Security_UsersControl"];
            }
            if (settings["Security_DisplayNameFormat"] == null)
            {
                settings["Security_DisplayNameFormat"] = "";
            }
            return settings;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetUserCountByPortalCallBack gets the number of users associates to a portal from the the Database.
        ///   and sets the cache.
        /// </summary>
        /// <param name = "cacheItemArgs">The CacheItemArgs object that contains the parameters
        ///   needed for the database call</param>
        /// <history>
        ///   [pbeadle]	02/09/2011    
        /// </history>
        /// -----------------------------------------------------------------------------
        private static object GetUserCountByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            var portalId = (int) cacheItemArgs.ParamList[0];
            var portalUserCount = MemberProvider.GetUserCountByPortal(portalId);
            DataCache.SetCache(cacheItemArgs.CacheKey, portalUserCount);
            return portalUserCount;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ChangePassword attempts to change the users password
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to update.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool ChangePassword(UserInfo user, string oldPassword, string newPassword)
        {
            bool retValue;
            if (ValidatePassword(newPassword))
            {
                retValue = MemberProvider.ChangePassword(user, oldPassword, newPassword);
                user.Membership.UpdatePassword = false;
                UpdateUser(user.PortalID, user);
            }
            else
            {
                throw new Exception("Invalid Password");
            }
            return retValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ChangePasswordQuestionAndAnswer attempts to change the users password Question
        /// and PasswordAnswer
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to update.</param>
        /// <param name="password">The password.</param>
        /// <param name="passwordQuestion">The new password question.</param>
        /// <param name="passwordAnswer">The new password answer.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	02/08/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer)
        {
            var objEventLog = new EventLogController();
            objEventLog.AddLog(user, PortalController.GetCurrentPortalSettings(), GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_UPDATED);
            return MemberProvider.ChangePasswordQuestionAndAnswer(user, password, passwordQuestion, passwordAnswer);
        }

        public static void CheckInsecurePassword(string username, string password, ref UserLoginStatus loginStatus)
        {
            if (username == "admin" && (password == "admin" || password == "dnnadmin"))
            {
                loginStatus = UserLoginStatus.LOGIN_INSECUREADMINPASSWORD;
            }
            if (username == "host" && (password == "host" || password == "dnnhost"))
            {
                loginStatus = UserLoginStatus.LOGIN_INSECUREHOSTPASSWORD;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Creates a new User in the Data Store
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The userInfo object to persist to the Database</param>
        /// <returns>The Created status ot the User</returns>
        /// <history>
        /// 	[cnurse]	12/13/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static UserCreateStatus CreateUser(ref UserInfo objUser)
        {
            var createStatus = MemberProvider.CreateUser(ref objUser);
            if (createStatus == UserCreateStatus.Success)
            {
                var objEventLog = new EventLogController();
                objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_CREATED);
                DataCache.ClearPortalCache(objUser.PortalID, false);
                if (!objUser.IsSuperUser)
                {
                    var objRoles = new RoleController();
                    RoleInfo objRole;
                    var arrRoles = objRoles.GetPortalRoles(objUser.PortalID);
                    int i;
                    for (i = 0; i <= arrRoles.Count - 1; i++)
                    {
                        objRole = (RoleInfo) arrRoles[i];
                        if (objRole.AutoAssignment)
                        {
                            objRoles.AddUserRole(objUser.PortalID, objUser.UserID, objRole.RoleID, Null.NullDate, Null.NullDate);
                        }
                    }
                }
            }
            return createStatus;
        }

        public static bool RestoreUser(ref UserInfo objUser)
        {
            //Restore the User
            var retValue = MemberProvider.RestoreUser(objUser);

            if ((retValue))
            {
                // Obtain PortalSettings from Current Context
                var portalSettings = PortalController.GetCurrentPortalSettings();

                //Log event
                var objEventLog = new EventLogController();
                objEventLog.AddLog("Username", objUser.Username, portalSettings, objUser.UserID, EventLogController.EventLogType.USER_RESTORED);

                DataCache.ClearPortalCache(objUser.PortalID, false);
                DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
            }

            return retValue;
        }

        public static bool RemoveUser(UserInfo objUser)
        {
            //Restore the User
            var retValue = MemberProvider.RemoveUser(objUser);

            if ((retValue))
            {
                // Obtain PortalSettings from Current Context
                var portalSettings = PortalController.GetCurrentPortalSettings();

                //Log event
                var objEventLog = new EventLogController();
                objEventLog.AddLog("Username", objUser.Username, portalSettings, objUser.UserID, EventLogController.EventLogType.USER_REMOVED);

                DataCache.ClearPortalCache(objUser.PortalID, false);
                DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
            }

            return retValue;
        }
        
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes an existing User from the Data Store
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The userInfo object to delete from the Database</param>
        /// <param name="notify">A flag that indicates whether an email notification should be sent</param>
        /// <param name="deleteAdmin">A flag that indicates whether the Portal Administrator should be deleted</param>
        /// <returns>A Boolean value that indicates whether the User was successfully deleted</returns>
        /// <history>
        /// 	[cnurse]	12/13/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool DeleteUser(ref UserInfo objUser, bool notify, bool deleteAdmin)
        {
            var canDelete = true;
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetPortal(objUser.PortalID, PortalController.GetActivePortalLanguage(objUser.PortalID));
                if (dr.Read())
                {
                    if (objUser.UserID == Convert.ToInt32(dr["AdministratorId"]))
                    {
                        canDelete = deleteAdmin;
                    }
                }
                if (canDelete)
                {
                    FolderPermissionController.DeleteFolderPermissionsByUser(objUser);
                    ModulePermissionController.DeleteModulePermissionsByUser(objUser);
                    TabPermissionController.DeleteTabPermissionsByUser(objUser);
                    canDelete = MemberProvider.DeleteUser(objUser);
                }
                if (canDelete)
                {
                    var portalSettings = PortalController.GetCurrentPortalSettings();
                    var objEventLog = new EventLogController();
                    objEventLog.AddLog("Username", objUser.Username, portalSettings, objUser.UserID, EventLogController.EventLogType.USER_DELETED);
                    if (notify && !objUser.IsSuperUser)
                    {
                        var message = new Message() {};
                        message.FromUserID = portalSettings.AdministratorId;
                        message.ToUserID = portalSettings.AdministratorId;
                        message.Subject = Localization.GetSystemMessage(objUser.Profile.PreferredLocale,
                                                                         portalSettings,
                                                                         "EMAIL_USER_UNREGISTER_SUBJECT",
                                                                         objUser,
                                                                         Localization.GlobalResourceFile,
                                                                         null,
                                                                         "",
                                                                         portalSettings.AdministratorId);
                        message.Body = Localization.GetSystemMessage(objUser.Profile.PreferredLocale,
                                                                      portalSettings,
                                                                      "EMAIL_USER_UNREGISTER_BODY",
                                                                      objUser,
                                                                      Localization.GlobalResourceFile,
                                                                      null,
                                                                      "",
                                                                      portalSettings.AdministratorId);
                        message.Status = MessageStatusType.Unread;
                        Mail.SendEmail(portalSettings.Email, portalSettings.Email, message.Subject, message.Body);
                    }
                    DataCache.ClearPortalCache(objUser.PortalID, false);
                    DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                canDelete = false;
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return canDelete;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes all Users for a Portal
        /// </summary>
        /// <remarks></remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="notify">A flag that indicates whether an email notification should be sent</param>
        /// <param name="deleteAdmin">A flag that indicates whether the Portal Administrator should be deleted</param>
        /// <history>
        /// 	[cnurse]	12/14/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteUsers(int portalId, bool notify, bool deleteAdmin)
        {
            var arrUsers = GetUsers(portalId);
            for (int i = 0; i < arrUsers.Count; i++)
            {
                var objUser = arrUsers[i] as UserInfo;
                DeleteUser(ref objUser, notify, deleteAdmin);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes all Unauthorized Users for a Portal
        /// </summary>
        /// <remarks></remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <history>
        /// 	[cnurse]	12/14/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteUnauthorizedUsers(int portalId)
        {
            var arrUsers = GetUsers(portalId);
            for (int i = 0; i < arrUsers.Count; i++)
            {
                var objUser = arrUsers[i] as UserInfo;
                if (objUser.Membership.Approved == false || objUser.Membership.LastLoginDate == Null.NullDate)
                {
                    DeleteUser(ref objUser, true, false);
                }
            }
        }

        public static void RemoveDeletedUsers(int portalId)
        {
            var arrUsers = GetUsers(true, false, portalId);

            foreach (UserInfo objUser in arrUsers)
            {
                if (objUser.IsDeleted)
                {
                    RemoveUser(objUser);
                }
            }

        }

        public static ArrayList FillUserCollection(int portalId, IDataReader dr, ref int totalRecords)
        {
            var arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while (dr.Read())
                {
                    obj = FillUserInfo(portalId, dr, false);
                    arrUsers.Add(obj);
                }
                bool nextResult = dr.NextResult();
                totalRecords = Globals.GetTotalRecords(ref dr);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arrUsers;
        }

        public static ArrayList FillUserCollection(int portalId, IDataReader dr)
        {
            var arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while (dr.Read())
                {
                    obj = FillUserInfo(portalId, dr, false);
                    arrUsers.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arrUsers;
        }

        public static UserInfo FillUserInfo(int portalId, IDataReader dr, bool closeDataReader)
        {
            UserInfo objUserInfo = null;
            try
            {
                var bContinue = true;
                if (closeDataReader)
                {
                    bContinue = false;
                    if (dr.Read())
                    {
                        if (string.Equals(dr.GetName(0), "UserID", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bContinue = true;
                        }
                    }
                }
                if (bContinue)
                {
                    objUserInfo = new UserInfo();
                    objUserInfo.PortalID = portalId;
                    objUserInfo.IsSuperUser = Null.SetNullBoolean(dr["IsSuperUser"]);
                    objUserInfo.IsDeleted = Null.SetNullBoolean(dr["IsDeleted"]);
                    objUserInfo.UserID = Null.SetNullInteger(dr["UserID"]);
                    objUserInfo.FirstName = Null.SetNullString(dr["FirstName"]);
                    objUserInfo.LastName = Null.SetNullString(dr["LastName"]);
                    objUserInfo.RefreshRoles = Null.SetNullBoolean(dr["RefreshRoles"]);
                    objUserInfo.DisplayName = Null.SetNullString(dr["DisplayName"]);
                    objUserInfo.AffiliateID = Null.SetNullInteger(Null.SetNull(dr["AffiliateID"], objUserInfo.AffiliateID));
                    objUserInfo.Username = Null.SetNullString(dr["Username"]);
                    GetUserMembership(objUserInfo);
                    objUserInfo.Email = Null.SetNullString(dr["Email"]);
                    objUserInfo.Membership.UpdatePassword = Null.SetNullBoolean(dr["UpdatePassword"]);
                    if (!objUserInfo.IsSuperUser)
                    {
                        objUserInfo.Membership.Approved = Null.SetNullBoolean(dr["Authorised"]);
                    }
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, closeDataReader);
            }
            return objUserInfo;
        }

        public static string GeneratePassword()
        {
            return GeneratePassword(MembershipProviderConfig.MinPasswordLength + 4);
        }

        public static string GeneratePassword(int length)
        {
            return MemberProvider.GeneratePassword(length);
        }

        public static UserInfo GetCachedUser(int portalId, string username)
        {
            var cacheKey = string.Format(DataCache.UserCacheKey, portalId, username);
            return CBO.GetCachedObject<UserInfo>(new CacheItemArgs(cacheKey, DataCache.UserCacheTimeOut, DataCache.UserCachePriority, portalId, username), GetCachedUserByPortalCallBack);
        }

        public static UserInfo GetCurrentUserInfo()
        {
            if ((HttpContext.Current == null))
            {
                if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    return new UserInfo();
                }
                else
                {
                    PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                    if (_portalSettings != null)
                    {
                        UserInfo objUser = GetCachedUser(_portalSettings.PortalId, Thread.CurrentPrincipal.Identity.Name);
                        if (objUser != null)
                        {
                            return objUser;
                        }
                        else
                        {
                            return new UserInfo();
                        }
                    }
                    else
                    {
                        return new UserInfo();
                    }
                }
            }
            else
            {
                var objUser = (UserInfo) HttpContext.Current.Items["UserInfo"];
                if (objUser != null)
                {
                    return objUser;
                }
                else
                {
                    return new UserInfo();
                }
            }
        }

        public static ArrayList GetOnlineUsers(int portalId)
        {
            return MemberProvider.GetOnlineUsers(portalId);
        }

        public static string GetPassword(ref UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordRetrievalEnabled)
            {
                user.Membership.Password = MemberProvider.GetPassword(user, passwordAnswer);
            }
            else
            {
                throw new ConfigurationErrorsException("Password Retrieval is not enabled");
            }
            return user.Membership.Password;
        }

        public static ArrayList GetUnAuthorizedUsers(int portalId, bool includeDeleted, bool superUsersOnly)
        {
            return MemberProvider.GetUnAuthorizedUsers(portalId, includeDeleted, superUsersOnly);
        }
        public static ArrayList GetUnAuthorizedUsers(int portalId)
        {
            return GetUnAuthorizedUsers(portalId, false, false);
        }

        public static ArrayList GetDeletedUsers(int portalId)
        {
            return MemberProvider.GetDeletedUsers(portalId);
        }

        public static UserInfo GetUserById(int portalId, int userId)
        {
            return MemberProvider.GetUser(portalId, userId);
        }

        public static UserInfo GetUserByName(int portalId, string username)
        {
            return GetCachedUser(portalId, username);
        }

        public static int GetUserCountByPortal(int portalId)
        {
            var cacheKey = string.Format(DataCache.PortalUserCountCacheKey, portalId);
            return CBO.GetCachedObject<int>(new CacheItemArgs(cacheKey, DataCache.PortalUserCountCacheTimeOut, DataCache.PortalUserCountCachePriority, portalId), GetUserCountByPortalCallBack);
        }

        public static string GetUserCreateStatus(UserCreateStatus userRegistrationStatus)
        {
            switch (userRegistrationStatus)
            {
                case UserCreateStatus.DuplicateEmail:
                    return Localization.GetString("UserEmailExists");
                case UserCreateStatus.InvalidAnswer:
                    return Localization.GetString("InvalidAnswer");
                case UserCreateStatus.InvalidEmail:
                    return Localization.GetString("InvalidEmail");
                case UserCreateStatus.InvalidPassword:
                    string strInvalidPassword = Localization.GetString("InvalidPassword");
                    strInvalidPassword = strInvalidPassword.Replace("[PasswordLength]", MembershipProviderConfig.MinPasswordLength.ToString());
                    strInvalidPassword = strInvalidPassword.Replace("[NoneAlphabet]", MembershipProviderConfig.MinNonAlphanumericCharacters.ToString());
                    return strInvalidPassword;
                case UserCreateStatus.PasswordMismatch:
                    return Localization.GetString("PasswordMismatch");
                case UserCreateStatus.InvalidQuestion:
                    return Localization.GetString("InvalidQuestion");
                case UserCreateStatus.InvalidUserName:
                    return Localization.GetString("InvalidUserName");
                case UserCreateStatus.UserRejected:
                    return Localization.GetString("UserRejected");
                case UserCreateStatus.DuplicateUserName:
                case UserCreateStatus.UserAlreadyRegistered:
                case UserCreateStatus.UsernameAlreadyExists:
                    return Localization.GetString("UserNameExists");
                case UserCreateStatus.ProviderError:
                case UserCreateStatus.DuplicateProviderUserKey:
                case UserCreateStatus.InvalidProviderUserKey:
                    return Localization.GetString("RegError");
                default:
                    throw new ArgumentException("Unknown UserCreateStatus value encountered", "userRegistrationStatus");
            }
        }

        public static void GetUserMembership(UserInfo objUser)
        {
            MemberProvider.GetUserMembership(ref objUser);
        }

        public static Hashtable GetDefaultUserSettings()
        {
            return GetUserSettings(new Hashtable());
        }

        public static Hashtable GetUserSettings(int portalId)
        {
            var settings = GetDefaultUserSettings();
            Dictionary<string, string> settingsDictionary;
            if (portalId == Null.NullInteger)
            {
                settingsDictionary = HostController.Instance.GetSettingsDictionary();
            }
            else
            {
                settingsDictionary = PortalController.GetPortalSettingsDictionary(portalId);
            }
            if (settingsDictionary != null)
            {
                string prefix;
                int index;
                foreach (KeyValuePair<string, string> kvp in settingsDictionary)
                {
                    index = kvp.Key.IndexOf("_");
                    if (index > 0)
                    {
                        prefix = kvp.Key.Substring(0, index + 1);
                        switch (prefix)
                        {
                            case "Column_":
                            case "Display_":
                            case "Profile_":
                            case "Records_":
                            case "Redirect_":
                            case "Security_":
                                settings[kvp.Key] = kvp.Value;
                                break;
                        }
                    }
                }
            }
            return settings;
        }
        
        public static ArrayList GetUsers(int portalId)
        {
            return GetUsers(false, false, portalId);
        }

        public static ArrayList GetUsers(bool includeDeleted, bool superUsersOnly, int portalId)
        {
            var totalrecords = -1;
            return GetUsers(portalId, -1, -1, ref totalrecords, includeDeleted, superUsersOnly);
        }

        public static ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            return MemberProvider.GetUsers(portalId, pageIndex, pageSize, ref totalRecords, includeDeleted, superUsersOnly);
        }

        public static ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsers(portalId, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public static ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            return MemberProvider.GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords, includeDeleted, superUsersOnly);
        }

        public static ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public static ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            return MemberProvider.GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords, includeDeleted, superUsersOnly);
        }

        public static ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public static ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords, bool includeDeleted, bool superUsersOnly)
        {
            return MemberProvider.GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords, includeDeleted, superUsersOnly);
        }

        public static ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords, false, false);
        }

        public static string ResetPassword(UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordResetEnabled)
            {
                user.Membership.Password = MemberProvider.ResetPassword(user, passwordAnswer);
            }
            else
            {
                throw new ConfigurationErrorsException("Password Reset is not enabled");
            }
            return user.Membership.Password;
        }

        public static void SetAuthCookie(string username, bool createPersistentCookie)
        {
        }

        public static string SettingsKey(int portalId)
        {
            return "UserSettings|" + portalId;
        }

        public static bool UnLockUser(UserInfo user)
        {
            var retValue = MemberProvider.UnLockUser(user);
            DataCache.ClearUserCache(user.PortalID, user.Username);
            return retValue;
        }

        public static void UpdateUser(int portalId, UserInfo objUser)
        {
            UpdateUser(portalId, objUser, true);
        }

        /// <summary>
        ///   updates a user
        /// </summary>
        /// <param name = "portalId">the portalid of the user</param>
        /// <param name = "objUser">the user object</param>
        /// <param name = "loggedAction">whether or not the update calls the eventlog - the eventlogtype must still be enabled for logging to occur</param>
        /// <remarks>
        /// </remarks>
        public static void UpdateUser(int portalId, UserInfo objUser, bool loggedAction)
        {
            //Update the User
            MemberProvider.UpdateUser(objUser);
            if (loggedAction)
            {
                var objEventLog = new EventLogController();
                objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_UPDATED);
            }
            DataCache.ClearUserCache(portalId, objUser.Username);
        }

        public static UserInfo UserLogin(int portalId, string username, string password, string verificationCode, string portalName, string ip, ref UserLoginStatus loginStatus,
                                         bool createPersistentCookie)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;
            var objUser = ValidateUser(portalId, username, password, verificationCode, portalName, ip, ref loginStatus);
            if (objUser != null)
            {
                UserLogin(portalId, objUser, portalName, ip, createPersistentCookie);
            }
            else
            {
                AddEventLog(portalId, username, Null.NullInteger, portalName, ip, loginStatus);
            }
            return objUser;
        }

        public static void UserLogin(int portalId, UserInfo user, string portalName, string ip, bool createPersistentCookie)
        {
            if (user.IsSuperUser)
            {
                AddEventLog(portalId, user.Username, user.UserID, portalName, ip, UserLoginStatus.LOGIN_SUPERUSER);
            }
            else
            {
                AddEventLog(portalId, user.Username, user.UserID, portalName, ip, UserLoginStatus.LOGIN_SUCCESS);
            }
            user.LastIPAddress = ip;
            UpdateUser(portalId, user);
            FormsAuthentication.SetAuthCookie(user.Username, createPersistentCookie);
            var persistentCookieTimeout = Config.GetPersistentCookieTimeout();
            if (createPersistentCookie)
            {
                var authenticationTicket = new FormsAuthenticationTicket(user.Username, true, persistentCookieTimeout);
                var encryptedAuthTicket = FormsAuthentication.Encrypt(authenticationTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedAuthTicket);
                authCookie.Expires = authenticationTicket.Expiration;
                authCookie.Domain = FormsAuthentication.CookieDomain;
                authCookie.Path = FormsAuthentication.FormsCookiePath;
                HttpContext.Current.Response.Cookies.Set(authCookie);
            }
        }

        public static bool ValidatePassword(string password)
        {
            var isValid = true;

            if (password.Length < MembershipProviderConfig.MinPasswordLength)
            {
                isValid = false;
            }
            
            var rx = new Regex("[^0-9a-zA-Z]");
            if (rx.Matches(password).Count < MembershipProviderConfig.MinNonAlphanumericCharacters)
            {
                isValid = false;
            }
            if (!String.IsNullOrEmpty(MembershipProviderConfig.PasswordStrengthRegularExpression) && isValid)
            {
                rx = new Regex(MembershipProviderConfig.PasswordStrengthRegularExpression);
                isValid = rx.IsMatch(password);
            }
            return isValid;
        }

        public static UserInfo ValidateUser(int portalId, string username, string password, string verificationCode, string portalName, string ip, ref UserLoginStatus loginStatus)
        {
            return ValidateUser(portalId, username, password, "DNN", verificationCode, portalName, ip, ref loginStatus);
        }

        public static UserInfo ValidateUser(int portalId, string username, string password, string authType, string verificationCode, string portalName, string ip, ref UserLoginStatus loginStatus)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;
            var objUser = MemberProvider.UserLogin(portalId, username, password, authType, verificationCode, ref loginStatus);
            if (loginStatus == UserLoginStatus.LOGIN_USERLOCKEDOUT || loginStatus == UserLoginStatus.LOGIN_FAILURE)
            {
                AddEventLog(portalId, username, Null.NullInteger, portalName, ip, loginStatus);
            }
            if (loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER)
            {
                CheckInsecurePassword(username, password, ref loginStatus);
            }
            return objUser;
        }

        public static UserValidStatus ValidateUser(UserInfo objUser, int portalId, bool ignoreExpiring)
        {
            var validStatus = UserValidStatus.VALID;
            if (objUser.Membership.UpdatePassword)
            {
                validStatus = UserValidStatus.UPDATEPASSWORD;
            }
            else if (PasswordConfig.PasswordExpiry > 0)
            {
                var expiryDate = objUser.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry);
                if (expiryDate < DateTime.Now)
                {
                    validStatus = UserValidStatus.PASSWORDEXPIRED;
                }
                else if (expiryDate < DateTime.Now.AddDays(PasswordConfig.PasswordExpiryReminder) && (!ignoreExpiring))
                {
                    validStatus = UserValidStatus.PASSWORDEXPIRING;
                }
            }
            if (validStatus == UserValidStatus.VALID)
            {
                var validProfile = Convert.ToBoolean(UserModuleBase.GetSetting(portalId, "Security_RequireValidProfileAtLogin"));
                if (validProfile && (!ProfileController.ValidateProfile(portalId, objUser.Profile)))
                {
                    validStatus = UserValidStatus.UPDATEPROFILE;
                }
            }
            return validStatus;
        }

        public UserInfo GetUser(int portalId, int userId)
        {
            return GetUserById(portalId, userId);
        }

        public void UpdateDisplayNames()
        {
            var arrUsers = GetUsers(PortalId);
            foreach (UserInfo objUser in arrUsers)
            {
                objUser.UpdateDisplayName(DisplayFormat);
                UpdateUser(PortalId, objUser);
            }
        }

        #region "Obsoleted Methods, retained for Binary Compatability"

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.CreateUser")]
        public int AddUser(UserInfo objUser)
        {
            CreateUser(ref objUser);
            return objUser.UserID;
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.CreateUser")]
        public int AddUser(UserInfo objUser, bool addToMembershipProvider)
        {
            CreateUser(ref objUser);
            return objUser.UserID;
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.DeleteUsers")]
        public void DeleteAllUsers(int portalId)
        {
            DeleteUsers(portalId, false, true);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.DeleteUser")]
        public bool DeleteUser(int portalId, int userId)
        {
            var objUser = GetUser(portalId, userId);

            //Call Shared method with notify=true, deleteAdmin=false
            return DeleteUser(ref objUser, true, false);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.DeleteUsers")]
        public void DeleteUsers(int portalId)
        {
            DeleteUsers(portalId, true, false);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUserByName")]
        public UserInfo FillUserInfo(int portalID, string username)
        {
            return GetCachedUser(portalID, username);
        }

        [Obsolete("Deprecated in DNN 5.1. This function should be replaced by String.Format(DataCache.UserCacheKey, portalId, username)")]
        public string GetCacheKey(int portalID, string username)
        {
            return string.Format(DataCache.UserCacheKey, portalID, username);
        }

        [Obsolete("Deprecated in DNN 5.1. This function should be replaced by String.Format(DataCache.UserCacheKey, portalId, username)")]
        public static string CacheKey(int portalId, string username)
        {
            return string.Format(DataCache.UserCacheKey, portalId, username);
        }

        [Obsolete("Deprecated in DNN 5.1. Not needed any longer for due to autohydration")]
        public static ArrayList GetUnAuthorizedUsers(int portalId, bool isHydrated)
        {
            return GetUnAuthorizedUsers(portalId);
        }

        [Obsolete("Deprecated in DNN 5.1. Not needed any longer for due to autohydration")]
        public static UserInfo GetUser(int portalId, int userId, bool isHydrated)
        {
            return GetUserById(portalId, userId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetUser retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "portalId">The Id of the Portal</param>
        /// <param name = "userId">The Id of the user being retrieved from the Data Store.</param>
        /// <param name = "isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name = "hydrateRoles">A flag that instructs the method to automatically hydrate the roles</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        [Obsolete("Deprecated in DNN 5.1. Not needed any longer for single users due to autohydration")]
        public static UserInfo GetUser(int portalId, int userId, bool isHydrated, bool hydrateRoles)
        {
            var user = GetUserById(portalId, userId);

            if (hydrateRoles)
            {
                var controller = new RoleController();
                user.Roles = controller.GetRolesByUser(userId, portalId);
            }

            return user;
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUserByName")]
        public UserInfo GetUserByUsername(int portalID, string username)
        {
            return GetCachedUser(portalID, username);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUserByName")]
        public UserInfo GetUserByUsername(int portalID, string username, bool synchronizeUsers)
        {
            return GetCachedUser(portalID, username);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUserByName")]
        public static UserInfo GetUserByName(int portalId, string username, bool isHydrated)
        {
            return GetCachedUser(portalId, username);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsers")]
        public ArrayList GetSuperUsers()
        {
            return GetUsers(Null.NullInteger);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsers")]
        public ArrayList GetUsers(bool synchronizeUsers, bool progressiveHydration)
        {
            return GetUsers(Null.NullInteger);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsers")]
        public ArrayList GetUsers(int portalId, bool synchronizeUsers, bool progressiveHydration)
        {
            var totalRecords = -1;
            return GetUsers(portalId, -1, -1, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsers")]
        public static ArrayList GetUsers(int portalId, bool isHydrated)
        {
            var totalRecords = -1;
            return GetUsers(portalId, -1, -1, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsers")]
        public static ArrayList GetUsers(int portalId, bool isHydrated, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsers(portalId, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsersByEmail")]
        public static ArrayList GetUsersByEmail(int portalId, bool isHydrated, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsersByUserName")]
        public static ArrayList GetUsersByUserName(int portalId, bool isHydrated, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.GetUsersByProfileProperty")]
        public static ArrayList GetUsersByProfileProperty(int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.ChangePassword")]
        public bool SetPassword(UserInfo objUser, string newPassword)
        {
            return ChangePassword(objUser, Null.NullString, newPassword);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.ChangePassword")]
        public bool SetPassword(UserInfo objUser, string oldPassword, string newPassword)
        {
            return ChangePassword(objUser, oldPassword, newPassword);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.UnlockUserAccount")]
        public void UnlockUserAccount(UserInfo objUser)
        {
            UnLockUser(objUser);
        }

        [Obsolete("Deprecated in DNN 5.1. This function has been replaced by UserController.UpdateUser")]
        public void UpdateUser(UserInfo objUser)
        {
            UpdateUser(objUser.PortalID, objUser);
        }

        #endregion
    }
}
