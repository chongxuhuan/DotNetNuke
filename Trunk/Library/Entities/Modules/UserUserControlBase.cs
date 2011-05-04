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

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;

#endregion

namespace DotNetNuke.Entities.Modules
{
    /// -----------------------------------------------------------------------------
    /// Project	 :  DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Modules
    /// Class	 :  UserUserControlBase
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserUserControlBase class defines a custom base class for the User Control.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	03/02/2007
    /// </history>
    /// -----------------------------------------------------------------------------
    public class UserUserControlBase : UserModuleBase
    {
        #region Delegates
        public delegate void UserCreatedEventHandler(object sender, UserCreatedEventArgs e);
        public delegate void UserDeletedEventHandler(object sender, UserDeletedEventArgs e);
        public delegate void UserRestoredEventHandler(object sender, UserRestoredEventArgs e);
        public delegate void UserRemovedEventHandler(object sender, UserRemovedEventArgs e);
        public delegate void UserUpdateErrorEventHandler(object sender, UserUpdateErrorArgs e);
        #endregion

        #region "Events"
        public event UserCreatedEventHandler UserCreated;
        public event UserCreatedEventHandler UserCreateCompleted;
        public event UserDeletedEventHandler UserDeleted;
        public event UserUpdateErrorEventHandler UserDeleteError;
        public event UserRestoredEventHandler UserRestored;
        public event UserUpdateErrorEventHandler UserRestoreError;
        public event UserRemovedEventHandler UserRemoved;
        public event UserUpdateErrorEventHandler UserRemoveError;
        public event EventHandler UserUpdated;
        public event EventHandler UserUpdateCompleted;
        public event UserUpdateErrorEventHandler UserUpdateError;
        #endregion

        #region "Event Methods"
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserCreateCompleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/13/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserCreateCompleted(UserCreatedEventArgs e)
        {
            if (UserCreateCompleted != null)
            {
                UserCreateCompleted(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserCreated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserCreated(UserCreatedEventArgs e)
        {
            if (UserCreated != null)
            {
                UserCreated(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserDeleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserDeleted(UserDeletedEventArgs e)
        {
            if (UserDeleted != null)
            {
                UserDeleted(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserDeleteError Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/30/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserDeleteError(UserUpdateErrorArgs e)
        {
            if (UserDeleteError != null)
            {
                UserDeleteError(this, e);
            }
        }
        public void OnUserRestored(UserRestoredEventArgs e)
        {
            if (UserRestored != null)
            {
                UserRestored(this, e);
            }
        }
        public void OnUserRestoreError(UserUpdateErrorArgs e)
        {
            if (UserRestoreError != null)
            {
                UserRestoreError(this, e);
            }
        }
        public void OnUserRemoved(UserRemovedEventArgs e)
        {
            if (UserRemoved != null)
            {
                UserRemoved(this, e);
            }
        }
        public void OnUserRemoveError(UserUpdateErrorArgs e)
        {
            if (UserRemoveError != null)
            {
                UserRemoveError(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserUpdated(EventArgs e)
        {
            if (UserUpdated != null)
            {
                UserUpdated(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserUpdateCompleted(EventArgs e)
        {
            if (UserUpdateCompleted != null)
            {
                UserUpdateCompleted(this, e);
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Raises the UserUpdateError Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/07/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void OnUserUpdateError(UserUpdateErrorArgs e)
        {
            if (UserUpdateError != null)
            {
                UserUpdateError(this, e);
            }
        }
        #endregion

        #region Nested type: BaseUserEventArgs

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The BaseUserEventArgs class provides a base for User EventArgs classes
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/07/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public class BaseUserEventArgs
        {
            public int UserId { get; set; }

            public string UserName { get; set; }
        }

        #endregion

        #region Nested type: UserCreatedEventArgs

        public class UserCreatedEventArgs
        {
            private UserCreateStatus _createStatus = UserCreateStatus.Success;

            public UserCreatedEventArgs(UserInfo newUser)
            {
                NewUser = newUser;
            }

            public UserCreateStatus CreateStatus
            {
                get
                {
                    return _createStatus;
                }
                set
                {
                    _createStatus = value;
                }
            }

            public UserInfo NewUser { get; set; }

            public bool Notify { get; set; }
        }

        #endregion

        #region Nested type: UserDeletedEventArgs

        public class UserDeletedEventArgs : BaseUserEventArgs
        {
            public UserDeletedEventArgs(int id, string name)
            {
                UserId = id;
                UserName = name;
            }
        }

        #endregion

        #region Nested type: UserRestoredEventArgs
        public class UserRestoredEventArgs : BaseUserEventArgs
        {
            public UserRestoredEventArgs(int id, string name)
            {
                UserId = id;
                UserName = name;
            }
        }
        #endregion

        #region Nested type: UserRemovedEventArgs
        public class UserRemovedEventArgs : BaseUserEventArgs
        {
            public UserRemovedEventArgs(int id, string name)
            {
                UserId = id;
                UserName = name;
            }
        }
        #endregion

        #region Nested type: UserUpdateErrorArgs

        public class UserUpdateErrorArgs : BaseUserEventArgs
        {
            public UserUpdateErrorArgs(int id, string name, string message)
            {
                UserId = id;
                UserName = name;
                Message = message;
            }

            public string Message { get; set; }
        }

        #endregion
    }
}
