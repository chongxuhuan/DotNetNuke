#region Copyright

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
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;

#endregion

namespace DotNetNuke.Services.Journal
{
    public class JournalController
    {
        private readonly IJournalDataService _DataService;

        #region Constructors

        public JournalController() : this(GetDataService())
        {
        }

        public JournalController(IJournalDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region Private Shared Methods

        private static IJournalDataService GetDataService()
        {
            var ds = ComponentFactory.GetComponent<IJournalDataService>();

            if (ds == null)
            {
                ds = new JournalDataService();
                ComponentFactory.RegisterComponentInstance<IJournalDataService>(ds);
            }
            return ds;
        }

        #endregion

        #region Private Methods

        private XmlElement CreateElement(XmlDocument xDoc, string name, string value)
        {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlText xtext = xDoc.CreateTextNode(value);
            xnode.AppendChild(xtext);
            return xnode;
        }

        private XmlElement CreateCDataElement(XmlDocument xDoc, string name, string value)
        {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlCDataSection xdata = xDoc.CreateCDataSection(value);
            xnode.AppendChild(xdata);
            return xnode;
        }

        private void UpdateGroupStats(int PortalId, int GroupId)
        {
            var jds = new JournalDataService();
            RoleInfo role = TestableRoleController.Instance.GetRole(PortalId, r => r.RoleID == GroupId);
            if (role == null)
            {
                return;
            }
            using (IDataReader dr = jds.Journal_GetStatsForGroup(PortalId, GroupId))
            {
                while (dr.Read())
                {
                    string settingName = "stat_" + dr["JournalType"];
                    if (role.Settings.ContainsKey(settingName))
                    {
                        role.Settings[settingName] = dr["JournalTypeCount"].ToString();
                    }
                    else
                    {
                        role.Settings.Add(settingName, dr["JournalTypeCount"].ToString());
                    }
                }
                dr.Close();
            }
            TestableRoleController.Instance.UpdateRoleSettings(role, true);
        }

        #endregion

        #region Public Methods

        public void DeleteJournalItem(int portalId, int currentUserId, int journalId)
        {
            
            var jds = new JournalDataService();
            var ji = GetJournalItem(portalId, currentUserId, journalId);
            int groupId = ji.SocialGroupId;
            jds.Journal_Delete(journalId);
            if (groupId > 0)
            {
                UpdateGroupStats(portalId, groupId);
            }
        }

        public void DeleteJournalItemByKey(int portalId, string objectKey)
        {
            var jds = new JournalDataService();
            jds.Journal_DeleteByKey(portalId, objectKey);
        }

        public JournalItem GetJournalItem(int portalId, int currentUserId, int journalId)
        {
            return CBO.FillObject<JournalItem>(_DataService.Journal_Get(portalId, currentUserId, journalId));
        }

        public List<object> GetJournalItemLikeList(int PortalId, int JournalId)
        {
            var jds = new JournalDataService();
            var list = new List<object>();
            using (IDataReader dr = jds.Journal_LikeList(PortalId, JournalId))
            {
                while (dr.Read())
                {
                    list.Add(new { userId = dr["UserId"].ToString(), name = dr["DisplayName"].ToString() });
                }
                dr.Close();
            }
            return list;
        }

        public List<JournalItem> GetJournalItems(int portalId, int moduleId, int currentUserId, int rowIndex, int maxRows)
        {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForSummary(portalId, moduleId, currentUserId, rowIndex, maxRows));
        }

        public List<JournalItem> GetJournalItemsByGroup(int portalId, int moduleId, int currentUserId, int groupId, int rowIndex, int maxRows)
        {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForGroup(portalId, moduleId, currentUserId, groupId, rowIndex, maxRows));
        }

        public List<JournalItem> GetJournalItemsByProfile(int portalId, int moduleId, int currentUserId, int profileId, int rowIndex, int maxRows)
        {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForProfile(portalId, moduleId, currentUserId, profileId, rowIndex, maxRows));
        }

        public JournalItem GetJournalItemByKey(int portalId, string objectKey)
        {
            if (string.IsNullOrEmpty(objectKey))
            {
                return null;
            }
            return (JournalItem) CBO.FillObject(_DataService.Journal_GetByKey(portalId, objectKey), typeof (JournalItem));
        }

        public void LikeJournalItem(int journalId, int userId, string displayName)
        {
            var jds = new JournalDataService();
            jds.Journal_Like(journalId, userId, displayName);
        }

        public JournalItem SaveJournalItem(JournalItem journalItem, int tabId)
        {
            if (journalItem.UserId < 1)
            {
                return null;
            }
            UserInfo currentUser = UserController.GetUserById(journalItem.PortalId, journalItem.UserId);
            if (currentUser == null)
            {
                return null;
            }
            var jds = new JournalDataService();
            string xml = null;
            var portalSecurity = new PortalSecurity();
            if (!String.IsNullOrEmpty(journalItem.Title))
            {
                journalItem.Title = portalSecurity.InputFilter(journalItem.Title, PortalSecurity.FilterFlag.NoMarkup);
            }
            if (!String.IsNullOrEmpty(journalItem.Summary))
            {
                journalItem.Summary = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.Summary, PortalSecurity.FilterFlag.NoScripting));
            }
            if (!String.IsNullOrEmpty(journalItem.Body))
            {
                journalItem.Body = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.Body, PortalSecurity.FilterFlag.NoScripting));
            }

            if (!String.IsNullOrEmpty(journalItem.Body))
            {
                var xDoc = new XmlDocument();
                XmlElement xnode = xDoc.CreateElement("items");
                XmlElement xnode2 = xDoc.CreateElement("item");
                xnode2.AppendChild(CreateElement(xDoc, "id", "-1"));
                xnode2.AppendChild(CreateCDataElement(xDoc, "body", journalItem.Body));
                xnode.AppendChild(xnode2);
                xDoc.AppendChild(xnode);
                XmlDeclaration xDec = xDoc.CreateXmlDeclaration("1.0", null, null);
                xDec.Encoding = "UTF-16";
                xDec.Standalone = "yes";
                XmlElement root = xDoc.DocumentElement;
                xDoc.InsertBefore(xDec, root);
                journalItem.JournalXML = xDoc;
                xml = journalItem.JournalXML.OuterXml;
            }
            string journalData = string.Empty;
            if (journalItem.ItemData != null)
            {
                if (!String.IsNullOrEmpty(journalItem.ItemData.Title))
                {
                    journalItem.ItemData.Title = portalSecurity.InputFilter(journalItem.ItemData.Title, PortalSecurity.FilterFlag.NoMarkup);
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.Description))
                {
                    journalItem.ItemData.Description = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.ItemData.Description, PortalSecurity.FilterFlag.NoScripting));
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.Url))
                {
                    journalItem.ItemData.Url = portalSecurity.InputFilter(journalItem.ItemData.Url, PortalSecurity.FilterFlag.NoScripting);
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.ImageUrl))
                {
                    journalItem.ItemData.ImageUrl = portalSecurity.InputFilter(journalItem.ItemData.ImageUrl, PortalSecurity.FilterFlag.NoScripting);
                }
            }
            journalData = journalItem.ItemData.ToJson();
            if (journalData == "null")
            {
                journalData = null;
            }
            if (String.IsNullOrEmpty(journalItem.SecuritySet))
            {
                journalItem.SecuritySet = "E,";
            }
            else if (!journalItem.SecuritySet.EndsWith(","))
            {
                journalItem.SecuritySet += ",";
            }
            if (journalItem.SecuritySet == "F,")
            {
                journalItem.SecuritySet = "F" + journalItem.UserId.ToString() + ",";
                journalItem.SecuritySet += "P" + journalItem.ProfileId.ToString() + ",";
            }
            if (journalItem.SecuritySet == "U,")
            {
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString() + ",";
            }
            if (journalItem.ProfileId > 0 && journalItem.UserId != journalItem.ProfileId)
            {
                journalItem.SecuritySet += "P" + journalItem.ProfileId.ToString() + ",";
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString() + ",";
            }
            if (!journalItem.SecuritySet.Contains("U" + journalItem.UserId.ToString()))
            {
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString() + ",";
            }
            if (journalItem.SocialGroupId > 0)
            {
                RoleInfo role = TestableRoleController.Instance.GetRole(journalItem.PortalId, r => r.SecurityMode != SecurityMode.SecurityRole && r.RoleID == journalItem.SocialGroupId);
                if (role != null)
                {
                    if (currentUser.IsInRole(role.RoleName))
                    {
                        journalItem.SecuritySet += "R" + journalItem.SocialGroupId.ToString() + ",";
                        if (!role.IsPublic)
                        {
                            journalItem.SecuritySet = journalItem.SecuritySet.Replace("E,", String.Empty);
                        }
                    }
                }
            }
            journalItem.JournalId = jds.Journal_Save(journalItem.PortalId,
                                                        journalItem.UserId,
                                                        journalItem.ProfileId,
                                                        journalItem.SocialGroupId,
                                                        journalItem.JournalId,
                                                        journalItem.JournalTypeId,
                                                        journalItem.Title,
                                                        journalItem.Summary,
                                                        journalItem.Body,
                                                        journalData,
                                                        xml,
                                                        journalItem.ObjectKey,
                                                        journalItem.AccessKey,
                                                        journalItem.SecuritySet);

            journalItem = GetJournalItem(journalItem.PortalId, journalItem.UserId, journalItem.JournalId);
            var cnt = new Content();

            if (journalItem.ContentItemId > 0)
            {
                cnt.UpdateContentItem(journalItem, tabId);
            }
            else
            {
                var ci = new ContentItem();
                ci = cnt.CreateContentItem(journalItem, tabId);
                jds.Journal_UpdateContentItemId(journalItem.JournalId, ci.ContentItemId);
            }
            if (journalItem.SocialGroupId > 0)
            {
                try
                {
                    UpdateGroupStats(journalItem.PortalId, journalItem.SocialGroupId);
                }
                catch (Exception exc)
                {
                    Exceptions.Exceptions.LogException(exc);
                }
            }
            return journalItem;
        }

        #endregion

        #region Journal Types

        public void DeleteJournalType(int journalTypeId, int portalId)
        {
            _DataService.Journal_Types_Delete(journalTypeId, portalId);
        }

        public JournalTypeInfo GetJournalType(string journalType)
        {
            return CBO.FillObject<JournalTypeInfo>(_DataService.Journal_Types_Get(journalType));
        }

        public JournalTypeInfo GetJournalTypeById(int journalTypeId)
        {
            return CBO.FillObject < JournalTypeInfo>(_DataService.Journal_Types_GetById(journalTypeId));
        }

        public List<JournalTypeInfo> GetJournalTypes(int portalId)
        {
            return CBO.FillCollection<JournalTypeInfo>(_DataService.Journal_Types_List(portalId));
        }

        public void SaveJournalType(JournalTypeInfo journalType)
        {
            journalType.JournalTypeId = _DataService.Journal_Types_Save(journalType.JournalTypeId,
                                                                           journalType.JournalType,
                                                                           journalType.icon,
                                                                           journalType.PortalId,
                                                                           journalType.IsEnabled,
                                                                           journalType.AppliesToProfile,
                                                                           journalType.AppliesToGroup,
                                                                           journalType.AppliesToStream,
                                                                           journalType.Options,
                                                                           journalType.SupportsNotify);
            GetJournalTypeById(journalType.JournalTypeId);
        }

        #endregion

        #region Journal Type Filters

        public void DeleteFilters(int portalId, int moduleId)
        {
            _DataService.Journal_TypeFilters_Delete(portalId, moduleId);
        }

        public Dictionary<int, string> GetFilters(int portalId, int moduleId)
        {
            var jds = new JournalDataService();
            var filters = new Dictionary<int, string> {};
            using (IDataReader dr = jds.Journal_TypeFilters_List(portalId, moduleId))
            {
                while (dr.Read())
                {
                    filters.Add(Convert.ToInt32(dr["JournalTypeId"].ToString()), dr["JournalType"].ToString());
                }
                dr.Close();
            }
            return filters;
        }

        public void SaveFilters(int portalId, int moduleId, int journalTypeId)
        {
            _DataService.Journal_TypeFilters_Save(portalId, moduleId, journalTypeId);
        }

        #endregion

        #region Comments

        public void DeleteComment(int journalId, int commentId)
        {
            var jds = new JournalDataService();
            jds.Journal_Comment_Delete(journalId, commentId);
        }

        public CommentInfo GetComment(int commentId)
        {
            return CBO.FillObject<CommentInfo>(_DataService.Journal_Comment_Get(commentId));
        }

        public List<object> GetCommentLikeList(int portalId, int journalId, int commentId)
        {
            var jds = new JournalDataService();
            var list = new List<object>();
            using (IDataReader dr = jds.Journal_Comment_LikeList(portalId, journalId, commentId))
            {
                while (dr.Read())
                {
                    list.Add(new { userId = dr["UserId"].ToString(), name = dr["DisplayName"].ToString() });
                }
                dr.Close();
            }
            return list;
        }

        public List<CommentInfo> GetComments(int journalId)
        {
            return CBO.FillCollection<CommentInfo>(_DataService.Journal_Comment_List(journalId));
        }

        public List<CommentInfo> GetCommentsByJournalIds(string JournalIds)
        {
            return CBO.FillCollection<CommentInfo>(_DataService.Journal_Comment_ListByJournalIds(JournalIds));
        }

        public void LikeComment(int journalId, int commentId, int userId, string displayName)
        {
            var jds = new JournalDataService();
            jds.Journal_Comment_Like(journalId, commentId, userId, displayName);
        }

        public CommentInfo SaveComment(CommentInfo comment)
        {
            var jds = new JournalDataService();
            var portalSecurity = new PortalSecurity();
            if (!String.IsNullOrEmpty(comment.Comment))
            {
                comment.Comment = HttpUtility.HtmlDecode(portalSecurity.InputFilter(comment.Comment, PortalSecurity.FilterFlag.NoScripting));
            }
            //TODO: enable once the profanity filter is working properly.
            //objCommentInfo.Comment = portalSecurity.Remove(objCommentInfo.Comment, DotNetNuke.Security.PortalSecurity.ConfigType.ListController, "ProfanityFilter", DotNetNuke.Security.PortalSecurity.FilterScope.PortalList);

            if (comment.Comment.Length > 2000)
            {
                comment.Comment = comment.Comment.Substring(0, 1999);
            }
            string xml = null;
            if (comment.CommentXML != null)
            {
                xml = comment.CommentXML.OuterXml;
            }

            comment.CommentId = jds.Journal_Comment_Save(comment.JournalId, comment.CommentId, comment.UserId, comment.Comment, xml);
            return GetComment(comment.CommentId);
        }

        #endregion


    }
}