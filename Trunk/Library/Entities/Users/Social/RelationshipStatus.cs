﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Enum:      RelationshipStatus
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The RelationshipStatus enum describes various UserRelationship statuses. E.g. Accepted, Blocked, Initiated.
    /// </summary>
    /// -----------------------------------------------------------------------------
	public enum RelationshipStatus
	{
        /// <summary>
        /// Relationship Request is not present (lack of any other status)
        /// </summary>
        None = 0,

		/// <summary>
		/// Relationship Request is Initiated. E.g. User 1 sent a friend request to User 2.
		/// </summary>
		Initiated = 1,

		/// <summary>
        /// Relationship Request is Accepted. E.g. User 2 has accepted User 1's friend request.
		/// </summary>
		Accepted = 2,

		/// <summary>
        /// Relationship Request is Rejected. E.g. User 2 has rejected User 1's friend request.
		/// </summary>
		Rejected = 3,

        /// <summary>
        /// Relationship Request is Ignored. E.g. User 2 has ignored User 1's friend request.
        /// </summary>
        Ignored = 4,

        /// <summary>
        /// Relationship Request is Reported (for spam). E.g. User 2 has reported User 1's friend request for spam.
        /// </summary>
        Reported = 5,

		/// <summary>
        /// Relationship Request is Blocked. E.g. User 2 has blocked User 1's ability to send any request.
		/// </summary>
		Blocked = 6        
	}
}
