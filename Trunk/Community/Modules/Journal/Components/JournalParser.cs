﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Services.Journal;
using DotNetNuke.Entities.Users;
using System.Text;
using DotNetNuke.Entities.Portals;
using System.Text.RegularExpressions;

using DotNetNuke.Services.Journal.Internal;
using DotNetNuke.Services.Localization;
using System.Xml;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Modules.Journal.Components {
	public class JournalParser {
		List<JournalItem> journalList { get; set; }
		PortalSettings PortalSettings { get; set; }
		int ProfileId { get; set; }
		int SocialGroupId { get; set; }
        int ModuleId { get; set; }
		UserInfo CurrentUser { get; set; }
        private string url = "";
        private bool isAdmin = false;
        string resxPath = "~/DesktopModules/Journal/App_LocalResources/SharedResources.resx";
		public JournalParser(PortalSettings portalSettings, int moduleId, int profileId, int socialGroupId, UserInfo userInfo) {
			PortalSettings = portalSettings;
            ModuleId = moduleId;
			ProfileId = profileId;
			SocialGroupId = socialGroupId;
			CurrentUser = userInfo;
            url = PortalSettings.DefaultPortalAlias;
            if (string.IsNullOrEmpty(url)) {
                url = HttpContext.Current.Request.Url.Host;
            }
		    url = string.Format("{0}://{1}{2}",
		                        HttpContext.Current.Request.IsSecureConnection ? "https" : "http",
		                        url,
		                        !HttpContext.Current.Request.Url.IsDefaultPort ? ":" + HttpContext.Current.Request.Url.Port : string.Empty);
		}
		public string GetList(int currentIndex, int rows) {
            if (CurrentUser.UserID > 0) {
                isAdmin = CurrentUser.IsInRole(PortalSettings.AdministratorRoleName);
            }
			var jc = InternalJournalController.Instance;
			var sb = new StringBuilder();
			
            string statusTemplate = Services.Localization.Localization.GetString("journal_status", resxPath);
            string linkTemplate = Services.Localization.Localization.GetString("journal_link", resxPath);
            string photoTemplate = Services.Localization.Localization.GetString("journal_photo", resxPath);
            string fileTemplate = Services.Localization.Localization.GetString("journal_file", resxPath);

            string rowTemplate = string.Empty;
			

            statusTemplate = Regex.Replace(statusTemplate, "\\[BaseUrl\\]", url, RegexOptions.IgnoreCase);
            linkTemplate = Regex.Replace(linkTemplate, "\\[BaseUrl\\]", url, RegexOptions.IgnoreCase);
            photoTemplate = Regex.Replace(photoTemplate, "\\[BaseUrl\\]", url, RegexOptions.IgnoreCase);
            fileTemplate = Regex.Replace(fileTemplate, "\\[BaseUrl\\]", url, RegexOptions.IgnoreCase);

            
			string Comment = Localization.GetString("comment", resxPath);
			string Comments = Localization.GetString("Comments", resxPath);
            IList<JournalItem> journalList;
            if (ProfileId > 0) {
                journalList = jc.GetJournalItemsByProfile(PortalSettings.PortalId, ModuleId, CurrentUser.UserID, ProfileId, currentIndex, rows);
            } else if (SocialGroupId > 0) {
                journalList = jc.GetJournalItemsByGroup(PortalSettings.PortalId, ModuleId, CurrentUser.UserID, SocialGroupId, currentIndex, rows);
            } else {
                journalList = jc.GetJournalItems(PortalSettings.PortalId, ModuleId, CurrentUser.UserID, currentIndex, rows);
            }
            

            string journalIds = "";
            foreach(JournalItem ji in journalList){
                journalIds +=ji.JournalId.ToString() + ';';
            }

            IList<CommentInfo> comments = jc.GetCommentsByJournalIds(journalIds);

			foreach (JournalItem ji in journalList) {
                string pattern = "{CanComment}(.*?){/CanComment}";
                string replacement = string.Empty;
                if (CurrentUser.UserID > 0 && SocialGroupId <= 0) {
                   replacement = "$1";
                }
                if (CurrentUser.UserID > 0 && ji.SocialGroupId > 0) {
                    if (CurrentUser.IsInRole(ji.JournalOwner.Name)) {
                        replacement = "$1";
                    } else {
                        replacement = string.Empty;
                    }
                }
                if (ji.JournalType == "status") {
                    rowTemplate = statusTemplate;
                    rowTemplate = Regex.Replace(rowTemplate, pattern, replacement, RegexOptions.IgnoreCase);
                } else if (ji.JournalType == "link") {
                    rowTemplate = linkTemplate;
                    rowTemplate = Regex.Replace(rowTemplate, pattern, replacement, RegexOptions.IgnoreCase);
                } else if (ji.JournalType == "photo") {
                    rowTemplate = photoTemplate;
                    rowTemplate = Regex.Replace(rowTemplate, pattern, replacement, RegexOptions.IgnoreCase);
                } else if (ji.JournalType == "file") {
                    rowTemplate = fileTemplate;
                    rowTemplate = Regex.Replace(rowTemplate, pattern, replacement, RegexOptions.IgnoreCase);
                } else {
                    rowTemplate = GetJournalTemplate(ji.JournalType, ji);
                }

                
                
				Components.JournalControl ctl = new Components.JournalControl();
				
                bool isLiked = false;
                ctl.LikeList = GetLikeListHTML(ji, ref isLiked);
                ctl.LikeLink = String.Empty;
                ctl.CommentLink = String.Empty;
                
                ctl.AuthorNameLink = "<a href=\"" + DotNetNuke.Common.Globals.NavigateURL(PortalSettings.UserTabId, string.Empty, new String[] {"userId=" + ji.JournalAuthor.Id.ToString()}) + "\">" + ji.JournalAuthor.Name + "</a>";
                if (CurrentUser.UserID > 0) {
                    ctl.CommentLink = "<a href=\"#\" id=\"cmtbtn-" + ji.JournalId + "\">" + Comment + "</a>";
                    if (isLiked) {
                        ctl.LikeLink = "<a href=\"#\" id=\"like-" + ji.JournalId + "\">{resx:unlike}</a>";
                    } else {
                        ctl.LikeLink = "<a href=\"#\" id=\"like-" + ji.JournalId + "\">{resx:like}</a>";
                    }
                }
                
                ctl.CommentArea = GetCommentAreaHTML(ji.JournalId, comments);
				ji.TimeFrame = Common.Utilities.DateUtils.CalculateDateForDisplay(ji.DateCreated);
                ji.DateCreated = CurrentUser.LocalTime(ji.DateCreated);
 
                if (ji.Summary != null)
                {
                    ji.Summary = ji.Summary.Replace("\n", "<br />");
                }
                
                if (ji.Body != null)
                {
                    ji.Body = ji.Body.Replace(Environment.NewLine, "<br />");
                }
				Components.JournalItemTokenReplace tokenReplace = new Components.JournalItemTokenReplace(ji, ctl);
				string tmp = tokenReplace.ReplaceJournalItemTokens(rowTemplate);
                tmp = tmp.Replace("<br>", "<br />");
                sb.Append("<div class=\"journalrow\" id=\"jid-" + ji.JournalId + "\">");
                if (isAdmin || CurrentUser.UserID == ji.UserId) {
                    sb.Append("<div class=\"minidel\" onclick=\"journalDelete(this);\"></div>");
                }
				sb.Append(tmp);
				sb.Append("</div>");
			}
          
			return Utilities.LocalizeControl(sb.ToString());
		}
        internal string GetJournalTemplate(string journalType, JournalItem ji) {
            string template = Services.Localization.Localization.GetString("journal_" + journalType, resxPath);
            if (String.IsNullOrEmpty(template))
            {
                template = Services.Localization.Localization.GetString("journal_generic", resxPath);
            }

            template = Regex.Replace(template, "\\[BaseUrl\\]", url, RegexOptions.IgnoreCase);
            template = template.Replace("[journalitem:action]", Services.Localization.Localization.GetString(journalType + ".Action", resxPath));
            string pattern = "{CanComment}(.*?){/CanComment}";
            string replacement = string.Empty;
            if (CurrentUser.UserID > 0 && SocialGroupId <= 0) {
                replacement = "$1";
            }
            if (CurrentUser.UserID > 0 && ji.SocialGroupId > 0) {
                if (CurrentUser.IsInRole(ji.JournalOwner.Name)) {
                    replacement = "$1";
                } else {
                    replacement = string.Empty;
                }
            }
            template = Regex.Replace(template, pattern, replacement, RegexOptions.IgnoreCase);

            return template;
        }
		internal string GetLikeListHTML(JournalItem ji, ref bool isLiked) {
			StringBuilder sb = new StringBuilder();
            isLiked = false;
			if (ji.JournalXML == null) {
				return string.Empty;
			}
			XmlNodeList xLikes = ji.JournalXML.DocumentElement.SelectNodes("//likes/u");
			if (xLikes == null){
				return string.Empty;
			}
			 foreach(XmlNode xLike in xLikes) {
				if (Convert.ToInt32(xLike.Attributes["uid"].Value.ToString()) == CurrentUser.UserID){
					ji.CurrentUserLikes = true;
                    isLiked = true;
					break;
				}

			}
			 int xc = 0;
			sb.Append("<div class=\"likes\">");
			if (xLikes.Count == 1 && ji.CurrentUserLikes) {
				sb.Append("{resx:youlikethis}");
			} else if (xLikes.Count > 1) {
				if (ji.CurrentUserLikes) {
					sb.Append("{resx:you}");
					xc += 1;
				}
				foreach (XmlNode l in xLikes) {
					int UserId = Convert.ToInt32(l.Attributes["uid"].Value.ToString());
					string Name = l.Attributes["un"].Value.ToString();
					if (!(UserId == CurrentUser.UserID)) {
						if (xc < xLikes.Count - 1 & xc > 0) {
							sb.Append(", ");
						} else if (xc > 0 & xc < xLikes.Count & xc < 3) {
							sb.Append(" {resx:and} ");
						} else if (xc >= 3) {
							int diff = (xLikes.Count - xc);
							sb.Append(" {resx:and} " + (xLikes.Count - xc).ToString());
							if (diff > 1) {
								sb.Append(" {resx:others}");
							} else {
								sb.Append(" {resx:other}");
							}
							break; // TODO: might not be correct. Was : Exit For
						}
						sb.AppendFormat("<span id=\"user-{0}\" class=\"juser\">{1}</span>", UserId.ToString(), Name);
						xc += 1;
					}
				}
				if (xc == 1) {
					sb.Append(" {resx:likesthis}");
				} else if (xc>1 && isLiked) {
					sb.Append(" {resx:likethis}");
				}

		} else {
			foreach (XmlNode l in xLikes) {
				int UserId = Convert.ToInt32(l.Attributes["uid"].Value.ToString());
				string Name = l.Attributes["un"].Value.ToString();
				sb.AppendFormat("<span id=\"user-{0}\" class=\"juser\">{1}</span>", UserId.ToString(), Name);
				xc += 1;
				if (xc == xLikes.Count - 1) {
					sb.Append(" {resx:and} ");
				} else if (xc < xLikes.Count - 1) {
					sb.Append(", ");
				}
			}
			if (xc == 1) {
				sb.Append(" {resx:likesthis}");
			} else if (xc>1 && isLiked) {
				sb.Append(" {resx:likethis}");
			}
		}

		   
			sb.Append("</div>");
			return sb.ToString();
		}
        internal string GetCommentAreaHTML(int JournalId, IList<CommentInfo> comments) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ul class=\"jcmt\" id=\"jcmt-{0}\">", JournalId);
            foreach(CommentInfo ci in comments) {
                if (ci.JournalId == JournalId) {
                    sb.Append(GetCommentRow(ci));
                }
            }
            if (CurrentUser.UserID > 0) {
                sb.AppendFormat("<li id=\"jcmt-{0}-txtrow\" class=\"cmteditarea\">", JournalId);
                sb.AppendFormat("<textarea id=\"jcmt-{0}-txt\" class=\"cmteditor\"></textarea>", JournalId);
                sb.Append("<div class=\"editorPlaceholder\">{resx:leavecomment}</div></li>");
                sb.AppendFormat("<li class=\"cmtbtn\">", JournalId);
                sb.Append("<a href=\"#\">{resx:comment}</a></li>");
            }
            
            sb.Append("</ul>");
            return sb.ToString();
        }
        internal string GetCommentRow(CommentInfo comment) {
            StringBuilder sb = new StringBuilder();
            string pic = string.Format(Globals.UserProfilePicFormattedUrl(), comment.UserId, 32, 32);
            sb.AppendFormat("<li id=\"cmt-{0}\">", comment.CommentId);
            if (comment.UserId == CurrentUser.UserID || isAdmin) {
                sb.Append("<div class=\"miniclose\"></div>");
            }
            sb.AppendFormat("<img src=\"{0}\" />", pic);
            sb.Append("<p>");
            string userUrl = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.UserTabId, string.Empty, new String[] { "userId=" + comment.UserId });
            sb.AppendFormat("<a href=\"{1}\">{0}</a>", comment.DisplayName, userUrl);
            sb.Append(comment.Comment.Replace("\n","<br />"));
            var timeFrame = Common.Utilities.DateUtils.CalculateDateForDisplay(comment.DateCreated);
            comment.DateCreated = CurrentUser.LocalTime(comment.DateCreated);
            sb.AppendFormat("<abbr title=\"{0}\">{1}</abbr>", comment.DateCreated, timeFrame);
  
            sb.Append("</p>");
            sb.Append("</li>");
            return sb.ToString();
        }
	}
}