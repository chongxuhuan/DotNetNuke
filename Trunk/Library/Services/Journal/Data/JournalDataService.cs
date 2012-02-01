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

using DotNetNuke.Data;
using System.IO;
using System.Xml;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content;

#endregion

namespace DotNetNuke.Services.Journal {
    public class JournalDataService : IJournalDataService {
        private readonly DataProvider provider = DataProvider.Instance();

        #region IJournalDataService Members
        public IDataReader Journal_ListForSummary(int PortalId, int CurrentUserId, int ProfileId, int GroupId, int JournalTypeId, int RowIndex, int MaxRows) {
            return provider.ExecuteReader("Journal_ListForSummary", PortalId, CurrentUserId, ProfileId, GroupId, JournalTypeId, RowIndex, MaxRows);
        }
        public IDataReader Journal_ListForProfile(int PortalId, int CurrentUserId, int ProfileId, int RowIndex, int MaxRows) {
            return provider.ExecuteReader("Journal_ListForProfile", PortalId, CurrentUserId, ProfileId, RowIndex, MaxRows);
        }
        public IDataReader Journal_ListForGroup(int PortalId, int CurrentUserId, int SocialGroupId, int JournalTypeId, int RowIndex, int MaxRows) {
            return provider.ExecuteReader("Journal_ListForGroup", PortalId, CurrentUserId, -1, SocialGroupId, JournalTypeId, RowIndex, MaxRows);
        }
        public void Journal_Delete(int JournalId) {
            provider.ExecuteNonQuery("Journal_Delete", JournalId);
        }
        public void Journal_Like(int JournalId, int UserId, string DisplayName) {
            provider.ExecuteNonQuery("Journal_Like", JournalId, UserId, DisplayName);
        }
        public IDataReader Journal_LikeList(int PortalId, int JournalId) {
            return provider.ExecuteReader("Journal_LikeList", PortalId, JournalId);
        }
        public void Journal_UpdateContentItemId(int JournalId, int ContentItemId) {
            provider.ExecuteNonQuery("Journal_UpdateContentItemId", JournalId, ContentItemId);
        }
        public IDataReader Journal_Get(int PortalId, int CurrentUserId, int JournalId) {
            return provider.ExecuteReader("Journal_Get", PortalId, CurrentUserId, JournalId);
        }
        public int Journal_Save(int PortalId, int CurrentUserId, int ProfileId, int GroupId, int JournalId, int JournalTypeId, string Title,
                string Summary, string Body, string ItemData, string xml, string ObjectKey, Guid AccessKey) {  
            
            JournalId = (int)provider.ExecuteScalar("Journal_Save", PortalId, JournalId, JournalTypeId, CurrentUserId, ProfileId, 
                    GroupId, Title, Summary, ItemData, xml, ObjectKey, AccessKey);
            
            return JournalId;
        }

        public void Journal_Comment_Delete(int JournalId, int CommentId) {
             provider.ExecuteNonQuery("Journal_Comment_Delete", JournalId, CommentId);
        }
        public int Journal_Comment_Save(int JournalId, int CommentId,int UserId, string Comment, string xml) {
            CommentId = (int)provider.ExecuteScalar("Journal_Comment_Save",JournalId, CommentId, UserId, Comment, xml);
            return CommentId;
        }
        public IDataReader Journal_Comment_List(int JournalId) {
            return provider.ExecuteReader("Journal_Comment_List", JournalId);
        }
        public IDataReader Journal_Comment_Get(int CommentId) {
            return provider.ExecuteReader("Journal_Comment_Get", CommentId);
        }
        public IDataReader Journal_Comment_ListByJournalIds(string JournalIds) {
            return provider.ExecuteReader("Journal_Comment_ListByJournalIds", JournalIds);
        }
        public void Journal_Comment_Like(int JournalId, int CommentId, int UserId, string DisplayName) {
            provider.ExecuteNonQuery("Journal_Comment_Like", JournalId, CommentId, UserId, DisplayName);
        }
        public IDataReader Journal_Comment_LikeList(int PortalId, int JournalId, int CommentId) {
            return provider.ExecuteReader("Journal_Comment_LikeList", PortalId, JournalId, CommentId);
        }

        public IDataReader Journal_Types_List(int PortalId) {
            return provider.ExecuteReader("Journal_Types_List", PortalId);
        }
        public IDataReader Journal_Types_Get(int JournalTypeId) {
            return provider.ExecuteReader("Journal_Types_Get", JournalTypeId);
        }
        public void Journal_Types_Delete(int JournalTypeId, int PortalId) {
            provider.ExecuteNonQuery("Journal_Types_Delete", JournalTypeId, PortalId);
        }
        public int Journal_Types_Save(int JournalTypeId, string JournalType, string icon, int PortalId, bool IsEnabled, bool AppliesToProfile, bool AppliesToGroup, bool AppliesToStream, string Options, bool SupportsNotify) {
            JournalTypeId = (int)provider.ExecuteScalar("Journal_Types_Save", JournalTypeId, JournalType, icon, PortalId, IsEnabled, AppliesToProfile, AppliesToGroup, AppliesToStream, Options, SupportsNotify);
            return JournalTypeId;
        }
        #endregion
    }
}