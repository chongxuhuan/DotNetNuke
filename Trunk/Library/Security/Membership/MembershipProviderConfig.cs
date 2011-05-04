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

using System.ComponentModel;

using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Security.Membership
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      MembershipProviderConfig
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The MembershipProviderConfig class provides a wrapper to the Membership providers
    /// configuration
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/02/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class MembershipProviderConfig
    {
        private static readonly MembershipProvider memberProvider = MembershipProvider.Instance();

        [Browsable(false)]
        public static bool CanEditProviderProperties
        {
            get
            {
                return memberProvider.CanEditProviderProperties;
            }
        }

        [SortOrder(8), Category("Password")]
        public static int MaxInvalidPasswordAttempts
        {
            get
            {
                return memberProvider.MaxInvalidPasswordAttempts;
            }
            set
            {
                memberProvider.MaxInvalidPasswordAttempts = value;
            }
        }

        [SortOrder(5), Category("Password")]
        public static int MinNonAlphanumericCharacters
        {
            get
            {
                return memberProvider.MinNonAlphanumericCharacters;
            }
            set
            {
                memberProvider.MinNonAlphanumericCharacters = value;
            }
        }

        [SortOrder(4), Category("Password")]
        public static int MinPasswordLength
        {
            get
            {
                return memberProvider.MinPasswordLength;
            }
            set
            {
                memberProvider.MinPasswordLength = value;
            }
        }

        [SortOrder(9), Category("Password")]
        public static int PasswordAttemptWindow
        {
            get
            {
                return memberProvider.PasswordAttemptWindow;
            }
            set
            {
                memberProvider.PasswordAttemptWindow = value;
            }
        }

        [SortOrder(1), Category("Password")]
        public static PasswordFormat PasswordFormat
        {
            get
            {
                return memberProvider.PasswordFormat;
            }
            set
            {
                memberProvider.PasswordFormat = value;
            }
        }

        [SortOrder(3), Category("Password")]
        public static bool PasswordResetEnabled
        {
            get
            {
                return memberProvider.PasswordResetEnabled;
            }
            set
            {
                memberProvider.PasswordResetEnabled = value;
            }
        }

        [SortOrder(2), Category("Password")]
        public static bool PasswordRetrievalEnabled
        {
            get
            {
                bool enabled = memberProvider.PasswordRetrievalEnabled;
                if (memberProvider.PasswordFormat == PasswordFormat.Hashed)
                {
                    enabled = false;
                }
                return enabled;
            }
            set
            {
                memberProvider.PasswordRetrievalEnabled = value;
            }
        }

        [SortOrder(7), Category("Password")]
        public static string PasswordStrengthRegularExpression
        {
            get
            {
                return memberProvider.PasswordStrengthRegularExpression;
            }
            set
            {
                memberProvider.PasswordStrengthRegularExpression = value;
            }
        }

        [SortOrder(6), Category("Password")]
        public static bool RequiresQuestionAndAnswer
        {
            get
            {
                return memberProvider.RequiresQuestionAndAnswer;
            }
            set
            {
                memberProvider.RequiresQuestionAndAnswer = value;
            }
        }

        [SortOrder(0), Category("User")]
        public static bool RequiresUniqueEmail
        {
            get
            {
                return memberProvider.RequiresUniqueEmail;
            }
            set
            {
                memberProvider.RequiresUniqueEmail = value;
            }
        }
    }
}
