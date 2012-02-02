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
using System.Data;

#endregion

namespace DotNetNuke.Services.Journal {
    public interface IJournalDataService {

        IDataReader Journal_ListForSummary(int PortalId, int CurrentUserId, int RowIndex, int MaxRows);
        IDataReader Journal_ListForProfile(int PortalId, int CurrentUserId, int ProfileId, int RowIndex, int MaxRows);
        IDataReader Journal_ListForGroup(int PortalId, int CurrentUserId, int SocialGroupId, int JournalTypeId, int RowIndex, int MaxRows);
        void Journal_Delete(int JournalId);
        IDataReader Journal_Get(int PortalId, int CurrentUserId, int JournalId);
        int Journal_Save(int PortalId, int CurrentUserId, int ProfileId, int GroupId, int JournalId, int JournalTypeId, string Title, string Summary,
            string Body, string ItemData, string xml, string ObjectKey, Guid AccessKey, string SecuritySet);
        void Journal_UpdateContentItemId(int JournalId, int ContentItemId);
        void Journal_Like(int JournalId, int UserId, string DisplayName);
        IDataReader Journal_LikeList(int PortalId, int JournalId);

        void Journal_Comment_Delete(int JournalId, int CommentId);
        int Journal_Comment_Save(int JournalId, int CommentId, int UserId, string Comment, string xml);
        IDataReader Journal_Comment_List(int JournalId);
        IDataReader Journal_Comment_Get(int CommentId);
        IDataReader Journal_Comment_ListByJournalIds(string JournalIds);
        void Journal_Comment_Like(int JournalId, int CommentId, int UserId, string DisplayName);
        IDataReader Journal_Comment_LikeList(int PortalId, int JournalId, int CommentId);
        IDataReader Journal_Types_List(int PortalId);
        IDataReader Journal_Types_Get(int JournalTypeId);
        void Journal_Types_Delete(int JournalTypeId, int PortalId);
        int Journal_Types_Save(int JournalTypeId, string JournalType, string icon, int PortalId, bool IsEnabled, bool AppliesToProfile, bool AppliesToGroup, bool AppliesToStream, string Options, bool SupportsNotify);

    }
}