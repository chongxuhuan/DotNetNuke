using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content;
using System.Xml;
using DotNetNuke.ComponentModel;
using DotNetNuke.Common.Utilities;
using System.Data;
using DotNetNuke.Security;
using System.Web;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.Journal {
    public class JournalController {
        private readonly IJournalDataService _DataService;
        #region "Constructors"
        public JournalController() : this(GetDataService()) {
        }
        public JournalController(IJournalDataService dataService) {
            _DataService = dataService;
        }

        #endregion
        #region "Private Shared Methods"

        private static IJournalDataService GetDataService() {
            var ds = ComponentFactory.GetComponent<IJournalDataService>();

            if (ds == null) {
                ds = new JournalDataService();
                ComponentFactory.RegisterComponentInstance<IJournalDataService>(ds);
            }
            return ds;
        }

        #endregion
        #region "Public Methods"
        public List<JournalItem> ListForProfile(int PortalId, int ModuleId, int CurrentUserId, int ProfileId, int RowIndex, int MaxRows) {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForProfile(PortalId, ModuleId, CurrentUserId, ProfileId, RowIndex, MaxRows));
        }
        public List<JournalItem> ListForSummary(int PortalId, int ModuleId, int CurrentUserId, int RowIndex, int MaxRows) {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForSummary(PortalId, ModuleId, CurrentUserId, RowIndex, MaxRows));
        }
        public List<JournalItem> ListForGroup(int PortalId, int ModuleId, int CurrentUserId, int GroupId, int RowIndex, int MaxRows) {
            return CBO.FillCollection<JournalItem>(_DataService.Journal_ListForGroup(PortalId, ModuleId, CurrentUserId, GroupId, RowIndex, MaxRows));
        }
        public JournalItem Journal_Get(int PortalId, int CurrentUserId, int JournalId) {
            return (JournalItem)CBO.FillObject(_DataService.Journal_Get(PortalId, CurrentUserId, JournalId), typeof(JournalItem));
        }
        public JournalItem Journal_GetByKey(int PortalId, string ObjectKey) {
            if (string.IsNullOrEmpty(ObjectKey)) {
                return null;
            }
            return (JournalItem)CBO.FillObject(_DataService.Journal_GetByKey(PortalId, ObjectKey), typeof(JournalItem));
        }
        public JournalItem CreateStatus(int PortalId, int TabId, int UserId, string Text, string SecuritySet) {
            var ji = new Services.Journal.JournalItem() {
                JournalId = -1,
                JournalTypeId = 1,
                PortalId =PortalId,
                UserId = UserId,
                SocialGroupId = -1,
                ProfileId = UserId,
                Summary = Text,
                SecuritySet = SecuritySet
            };
            return Journal_Save(ji, TabId);
        }
        public JournalItem CreateLink(int PortalId, int TabId, int UserId, string Text, string Url, string Title, string Description, string ImageUrl, string SecuritySet) {
            var ji = new Services.Journal.JournalItem() {
                JournalId = -1,
                JournalTypeId = 2,
                PortalId = PortalId,
                UserId = UserId,
                SocialGroupId = -1,
                ProfileId = UserId,
                Summary = Text,
                SecuritySet = SecuritySet
            };
            ji.ItemData = new ItemData();
            ji.ItemData.Url = Url;
            ji.ItemData.Title = Title;
            ji.ItemData.Description = Description;
            ji.ItemData.ImageUrl = ImageUrl;
            return Journal_Save(ji, TabId);
        }
        public JournalItem Journal_Save(JournalItem objJournalItem, int TabId) {
            if (objJournalItem.UserId < 1) {
                return null;
            }
            UserInfo currentUser = UserController.GetUserById(objJournalItem.PortalId, objJournalItem.UserId);
            if (currentUser == null) {
                return null;
            }
            JournalDataService jds = new JournalDataService();
            string xml = null;
            PortalSecurity portalSecurity = new PortalSecurity();
            if (!String.IsNullOrEmpty(objJournalItem.Title)) {
                objJournalItem.Title = portalSecurity.InputFilter(objJournalItem.Title, PortalSecurity.FilterFlag.NoMarkup);
            }
            if (!String.IsNullOrEmpty(objJournalItem.Summary)) {
                objJournalItem.Summary = HttpUtility.HtmlDecode(portalSecurity.InputFilter(objJournalItem.Summary, PortalSecurity.FilterFlag.NoScripting));
            }
            if (!String.IsNullOrEmpty(objJournalItem.Body)) {
                objJournalItem.Body = HttpUtility.HtmlDecode(portalSecurity.InputFilter(objJournalItem.Body, PortalSecurity.FilterFlag.NoScripting));
            }

            if (!String.IsNullOrEmpty(objJournalItem.Body)) {
                System.Xml.XmlDocument xDoc = new XmlDocument();
                XmlElement xnode = xDoc.CreateElement("items");
                XmlElement xnode2 = xDoc.CreateElement("item");
                xnode2.AppendChild(CreateElement(xDoc, "id", "-1"));
                xnode2.AppendChild(CreateCDataElement(xDoc, "body", objJournalItem.Body));
                xnode.AppendChild(xnode2);
                xDoc.AppendChild(xnode);
                XmlDeclaration xDec = xDoc.CreateXmlDeclaration("1.0", null, null);
                xDec.Encoding = "UTF-16";
                xDec.Standalone = "yes";
                XmlElement root = xDoc.DocumentElement;
                xDoc.InsertBefore(xDec, root);
                objJournalItem.JournalXML = xDoc;
                xml = objJournalItem.JournalXML.OuterXml;
            }
            string journalData = string.Empty;
            if (objJournalItem.ItemData != null) {
                if (!String.IsNullOrEmpty(objJournalItem.ItemData.Title)) {
                    objJournalItem.ItemData.Title = portalSecurity.InputFilter(objJournalItem.ItemData.Title, PortalSecurity.FilterFlag.NoMarkup);
                }
                if (!String.IsNullOrEmpty(objJournalItem.ItemData.Description)) {
                    objJournalItem.ItemData.Description = HttpUtility.HtmlDecode(portalSecurity.InputFilter(objJournalItem.ItemData.Description, PortalSecurity.FilterFlag.NoScripting));
                }
                if (!String.IsNullOrEmpty(objJournalItem.ItemData.Url)) {
                    objJournalItem.ItemData.Url = portalSecurity.InputFilter(objJournalItem.ItemData.Url, PortalSecurity.FilterFlag.NoScripting);
                }
                if (!String.IsNullOrEmpty(objJournalItem.ItemData.ImageUrl)) {
                    objJournalItem.ItemData.ImageUrl = portalSecurity.InputFilter(objJournalItem.ItemData.ImageUrl, PortalSecurity.FilterFlag.NoScripting);
                }

            }
            journalData = objJournalItem.ItemData.ToJson();
            if (journalData == "null") {
                journalData = null;
            }
            if (String.IsNullOrEmpty(objJournalItem.SecuritySet)) {
                objJournalItem.SecuritySet = "E,";
            } else if(!objJournalItem.SecuritySet.EndsWith(",")) {
                objJournalItem.SecuritySet += ",";
               
            }
            if (objJournalItem.SecuritySet == "F,") {
                objJournalItem.SecuritySet = "F" + objJournalItem.UserId.ToString() + ",";
                objJournalItem.SecuritySet += "P" + objJournalItem.ProfileId.ToString() + ",";
            }
            if (objJournalItem.SecuritySet == "U,") {
                objJournalItem.SecuritySet += "U" + objJournalItem.UserId.ToString() + ",";
            }
            if (objJournalItem.ProfileId > 0 && objJournalItem.UserId != objJournalItem.ProfileId) {
                objJournalItem.SecuritySet += "P" + objJournalItem.ProfileId.ToString() + ",";
                objJournalItem.SecuritySet += "U" + objJournalItem.UserId.ToString() + ",";
            }
            if (!objJournalItem.SecuritySet.Contains("U" + objJournalItem.UserId.ToString())) {
                objJournalItem.SecuritySet += "U" + objJournalItem.UserId.ToString() + ",";
            }
            if (objJournalItem.SocialGroupId > 0) {
                RoleInfo role = TestableRoleController.Instance.GetRole(objJournalItem.PortalId, r => r.SecurityMode != SecurityMode.SecurityRole && r.RoleID == objJournalItem.SocialGroupId);
                if (role != null) {
                    if (currentUser.IsInRole(role.RoleName)) {
                        objJournalItem.SecuritySet += "R" + objJournalItem.SocialGroupId.ToString() + ",";
                        if (!role.IsPublic) {
                            objJournalItem.SecuritySet = objJournalItem.SecuritySet.Replace("E,", String.Empty);
                        }
                    }
                }
            }
            objJournalItem.JournalId = jds.Journal_Save(objJournalItem.PortalId, objJournalItem.UserId, objJournalItem.ProfileId, objJournalItem.SocialGroupId,
                    objJournalItem.JournalId, objJournalItem.JournalTypeId, objJournalItem.Title, objJournalItem.Summary, objJournalItem.Body, journalData, xml, 
                    objJournalItem.ObjectKey, objJournalItem.AccessKey, objJournalItem.SecuritySet);

            objJournalItem = Journal_Get(objJournalItem.PortalId, objJournalItem.UserId, objJournalItem.JournalId);
            Content cnt = new Content();

            if (objJournalItem.ContentItemId > 0) {
                cnt.UpdateContentItem(objJournalItem, TabId);
            } else {
                ContentItem ci = new ContentItem();
                ci = cnt.CreateContentItem(objJournalItem, TabId);
                jds.Journal_UpdateContentItemId(objJournalItem.JournalId, ci.ContentItemId);
            }
            if (objJournalItem.SocialGroupId > 0) {
                try {
                    Journal_UpdateGroupStats(objJournalItem.PortalId, objJournalItem.SocialGroupId);
                } catch (Exception exc) {
                    Exceptions.Exceptions.LogException(exc);
                }
            }
            return objJournalItem;
        }
        public void CreateFriend() {
        }
        public void RemoveFriend() {
        }
        public void CreateGroup(RoleInfo roleInfo, UserInfo createdBy) {
            JournalItem ji = new JournalItem();
            string url = "";
            if (roleInfo.Settings.ContainsKey("URL")) {
                url = roleInfo.Settings["URL"];
            }
            ji.PortalId = roleInfo.PortalID;
            ji.ProfileId = createdBy.UserID;
            ji.UserId = createdBy.UserID;
            ji.Title = roleInfo.RoleName;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.SocialGroupId = roleInfo.RoleID;
            ji.Summary = roleInfo.Description;
            ji.Body = null;
            ji.JournalTypeId = JournalTypeGet("groupcreate").JournalTypeId;
            ji.ObjectKey = string.Format("groupcreate:{0}:{1}", roleInfo.RoleID.ToString(), createdBy.UserID.ToString());
            if ((Journal_GetByKey(roleInfo.PortalID, ji.ObjectKey) != null)) {
                Journal_DeleteByKey(roleInfo.PortalID, ji.ObjectKey);
            }
            ji.SecuritySet = string.Empty;
            if (roleInfo.IsPublic) {
                ji.SecuritySet += "E,";
            }
            Journal_Save(ji, -1);
        }
        public void RemoveGroup(RoleInfo roleInfo) {
        }
        public void CreateGroupMember(RoleInfo roleInfo, UserInfo userInfo) {
        }
        public void RemoveGroupMember(RoleInfo roleInfo, UserInfo userInfo) {
        }

        public void Journal_Delete(int JournalId) {
            JournalDataService jds = new JournalDataService();
            jds.Journal_Delete(JournalId);

        }
        public void Journal_DeleteByKey(int PortalId, string ObjectKey) {
            JournalDataService jds = new JournalDataService();
            jds.Journal_DeleteByKey(PortalId, ObjectKey);

        }
        public void Journal_Like(int JournalId, int UserId, string DisplayName) {
            JournalDataService jds = new JournalDataService();
            jds.Journal_Like(JournalId, UserId, DisplayName);

        }
        public List<object> Journal_LikeList(int PortalId, int JournalId) {
            JournalDataService jds = new JournalDataService();
            List<object> list = new List<object>();
            using (IDataReader dr = jds.Journal_LikeList(PortalId, JournalId)) {
                while (dr.Read()) {
                    list.Add(new { userId = dr["UserId"].ToString(), name = dr["DisplayName"].ToString() });
                }
                dr.Close();
            }
            return list;
        }
        public void Journal_UpdateGroupStats(int PortalId, int GroupId) {
      
                JournalDataService jds = new JournalDataService();
                RoleInfo role = TestableRoleController.Instance.GetRole(PortalId, r => r.RoleID == GroupId);
                if (role == null) {
                    return;
                }
                using (IDataReader dr = jds.Journal_GetStatsForGroup(PortalId, GroupId)) {
                    while (dr.Read()) {
                        string settingName = "stat_" + dr["JournalType"].ToString();
                        if (role.Settings.ContainsKey(settingName)) {
                            role.Settings[settingName] = dr["JournalTypeCount"].ToString();
                        } else {
                            role.Settings.Add(settingName, dr["JournalTypeCount"].ToString());
                        }
                        
                        
                    }
                    dr.Close();
                }
                TestableRoleController.Instance.UpdateRoleSettings(role, true);
           
            
        }
    #endregion
        #region Journal Types
        public JournalTypeInfo JournalTypeGetById(int JournalTypeId) {
            return (JournalTypeInfo)CBO.FillObject(_DataService.Journal_Types_GetById(JournalTypeId), typeof(JournalTypeInfo));
        }
        public JournalTypeInfo JournalTypeGet(string JournalType) {
            return (JournalTypeInfo)CBO.FillObject(_DataService.Journal_Types_Get(JournalType), typeof(JournalTypeInfo));
        }
        public void JournalTypeDelete(int JournalTypeId, int PortalId) {
            _DataService.Journal_Types_Delete(JournalTypeId, PortalId);
        }
        public List<JournalTypeInfo> JournalTypeList(int PortalId) {
            return CBO.FillCollection<JournalTypeInfo>(_DataService.Journal_Types_List(PortalId));
        }
        public JournalTypeInfo JournalTypeSave(JournalTypeInfo objJournalType) {
            objJournalType.JournalTypeId = _DataService.Journal_Types_Save(objJournalType.JournalTypeId, objJournalType.JournalType, objJournalType.icon,
                objJournalType.PortalId, objJournalType.IsEnabled, objJournalType.AppliesToProfile, objJournalType.AppliesToGroup, objJournalType.AppliesToStream,
                objJournalType.Options, objJournalType.SupportsNotify);
                return JournalTypeGetById(objJournalType.JournalTypeId);
        }
        #endregion
        #region Journal Type Filters
        public Dictionary<int, string> FiltersList(int PortalId, int ModuleId) {
            JournalDataService jds = new JournalDataService();
            var filters = new Dictionary<int, string>{};
            using (IDataReader dr = jds.Journal_TypeFilters_List(PortalId, ModuleId)) {
                while (dr.Read()) {
                    filters.Add(Convert.ToInt32(dr["JournalTypeId"].ToString()), dr["JournalType"].ToString());
                }
                dr.Close();
            }
            return filters;
        }
        public void FiltersDelete(int PortalId, int ModuleId) {
            _DataService.Journal_TypeFilters_Delete(PortalId, ModuleId);
        }
        public void FiltersSave(int PortalId, int ModuleId, int JournalTypeId) {
            _DataService.Journal_TypeFilters_Save(PortalId, ModuleId, JournalTypeId);
        }

        #endregion
        #region Comments
        public CommentInfo CommentSave(CommentInfo objCommentInfo) {
            JournalDataService jds = new JournalDataService();
            PortalSecurity portalSecurity = new PortalSecurity();
            if (!String.IsNullOrEmpty(objCommentInfo.Comment)) {
                objCommentInfo.Comment = HttpUtility.HtmlDecode(portalSecurity.InputFilter(objCommentInfo.Comment, PortalSecurity.FilterFlag.NoScripting));
            }
            //TODO: enable once the profanity filter is working properly.
            //objCommentInfo.Comment = portalSecurity.Remove(objCommentInfo.Comment, DotNetNuke.Security.PortalSecurity.ConfigType.ListController, "ProfanityFilter", DotNetNuke.Security.PortalSecurity.FilterScope.PortalList);

            if (objCommentInfo.Comment.Length > 2000) {
                objCommentInfo.Comment = objCommentInfo.Comment.Substring(0, 1999);
            }
            string xml = null;
            if (objCommentInfo.CommentXML != null) {
                xml = objCommentInfo.CommentXML.OuterXml;
            }

            objCommentInfo.CommentId = jds.Journal_Comment_Save(objCommentInfo.JournalId, objCommentInfo.CommentId, objCommentInfo.UserId, objCommentInfo.Comment, xml);
            objCommentInfo = CommentGet(objCommentInfo.CommentId);
            return objCommentInfo;
        }
        public void CommentDelete(int JournalId, int CommentId) {
            JournalDataService jds = new JournalDataService();
            jds.Journal_Comment_Delete(JournalId, CommentId);
        }
        public List<CommentInfo> CommentList(int JournalId) {
            return CBO.FillCollection<CommentInfo>(_DataService.Journal_Comment_List(JournalId));
        }
        public List<CommentInfo> CommentListByJournalIds(string JournalIds) {
            return CBO.FillCollection<CommentInfo>(_DataService.Journal_Comment_ListByJournalIds(JournalIds));
        }
        public void CommentLike(int JournalId, int CommentId, int UserId, string DisplayName) {
            JournalDataService jds = new JournalDataService();
            jds.Journal_Comment_Like(JournalId, CommentId, UserId, DisplayName);

        }
        public List<object> Journal_Comment_LikeList(int PortalId, int JournalId, int CommentId) {
            JournalDataService jds = new JournalDataService();
            List<object> list = new List<object>();
            using (IDataReader dr = jds.Journal_Comment_LikeList(PortalId, JournalId, CommentId)) {
                while (dr.Read()) {
                    list.Add(new { userId = dr["UserId"].ToString(), name = dr["DisplayName"].ToString() });
                }
                dr.Close();
            }
            return list;
        }
        public CommentInfo CommentGet(int CommentId) {
            return (CommentInfo)CBO.FillObject(_DataService.Journal_Comment_Get(CommentId), typeof(CommentInfo));
        }
        #endregion


        private XmlElement CreateElement(XmlDocument xDoc, string name, string value) {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlText xtext = xDoc.CreateTextNode(value);
            xnode.AppendChild(xtext);
            return xnode;
        }
        private XmlElement CreateCDataElement(XmlDocument xDoc, string name, string value) {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlCDataSection xdata = xDoc.CreateCDataSection(value);
            xnode.AppendChild(xdata);
            return xnode;

        }
    }
}
