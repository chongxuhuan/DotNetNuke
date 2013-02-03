using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using DotNetNuke.Professional.SearchInput;

namespace DotNetNuke.Professional.SearchInput
{
    using Modules.SearchInput;

    /// <summary>
	/// 
	/// Xepient Solutions ® - http://www.xepient.com
	/// Copyright (c) 2005
	///
	/// </summary>
	public abstract class XSSearchInputSkinObject : DotNetNuke.UI.Skins.SkinObjectBase 
	{
		#region UI Controls
		protected System.Web.UI.WebControls.Button cmdGo; 
		protected System.Web.UI.WebControls.ImageButton imgGo; 
		protected System.Web.UI.WebControls.TextBox txtSearch; 
		#endregion

		#region Constants
		private const string c_sLocalResourceFile  = "XSSearchInputSkinObject.ascx";
		#endregion

		#region Public properties
		public bool ShowGoImage = true;
		public string ResultsTabName = "";
		public int ResultsTabId = int.MinValue;
		public bool ApplyCustomCSS = true;
		public bool SearchOnEnter = false;
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.imgGo.Click += new System.Web.UI.ImageClickEventHandler(this.imgGo_Click);
			this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Page Load
		private void Page_Load(object sender, System.EventArgs e)
		{
			try 
			{
				if (!Page.IsPostBack) 
				{
					if (Page.Request.Params["xsq"] != null) 
					{
						txtSearch.Text = Page.Request.Params["xsq"];
					}

					SetupUI();			
				}
			} 
			catch (Exception exc) 
			{
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		private void SetupUI() 
		{
			if (this.SearchOnEnter) 
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(this.txtSearch, this.cmdGo, 13);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(this.txtSearch, this.imgGo, 13);
			}

			if (ApplyCustomCSS) 
			{
				imgGo.CssClass = "xsImgSearch";
				cmdGo.CssClass = "xsCmdSearch";
				txtSearch.CssClass = "xsTxtSearch";
			}

			string sGoUrl = DotNetNuke.Services.Localization.Localization.GetString("imgGo.ImageUrl", DotNetNuke.Services.Localization.Localization.GetResourceFile(this, c_sLocalResourceFile));
						
			imgGo.AlternateText = DotNetNuke.Services.Localization.Localization.GetString("imgGo.Text", DotNetNuke.Services.Localization.Localization.GetResourceFile(this, c_sLocalResourceFile));
			if (sGoUrl.StartsWith("~")) 
			{ 
				imgGo.ImageUrl = sGoUrl; 
			} 
			else 
			{ 
				imgGo.ImageUrl = Path.Combine(PortalSettings.HomeDirectory, sGoUrl); 
			} 
			
			cmdGo.Text = DotNetNuke.Services.Localization.Localization.GetString("cmdGo.Text", DotNetNuke.Services.Localization.Localization.GetResourceFile(this, c_sLocalResourceFile));
			
			ShowHideImages(); 
		}

		private void ShowHideImages() 
		{ 
			imgGo.Visible = ShowGoImage; 
			cmdGo.Visible = !(ShowGoImage); 
		} 
		#endregion

		#region  Events
		private void imgGo_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{ 
			SearchExecute(); 
		} 

		private void cmdGo_Click(object sender, System.EventArgs e) 
		{ 
			SearchExecute(); 
		} 

		private void SearchExecute() 
		{ 
			try 
			{
				if (this.ResultsTabId != int.MinValue)
				{
					if (DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") == "Y") 
					{ 
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.ResultsTabId) + "?xsq=" + Server.UrlEncode(FilterStrings(txtSearch.Text))); 
					} 
					else 
					{ 
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.ResultsTabId) + "&xsq=" + Server.UrlEncode(FilterStrings(txtSearch.Text))); 
					} 
				}
				else if (this.ResultsTabName != "") 
				{
					SearchInputController oController = new SearchInputController(); 
					ArrayList oResults = oController.GetSearchResultModules(this.PortalSettings.PortalId);
					foreach(XSSearchInputInfo oResult in oResults) 
					{
						if (oResult.SearchTabName.Trim().ToLower() == this.ResultsTabName.Trim().ToLower()) 
						{
				
							if (DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") == "Y") 
							{ 
								Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(oResult.TabID) + "?xsq=" + Server.UrlEncode(FilterStrings(txtSearch.Text))); 
							} 
							else 
							{ 
								Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(oResult.TabID) + "&xsq=" + Server.UrlEncode(FilterStrings(txtSearch.Text))); 
							} 
						}
					}
				}
				else 
				{
					if (FilterStrings(txtSearch.Text) != "") 
					{
						//if it gets here it is because the parameter was not set in the skin
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.PortalSettings.ActiveTab.TabID, "OpenSearchResults", new string[]{"xsq=" + Server.UrlEncode(FilterStrings(txtSearch.Text))})); 						
					}
				}
			}
			catch 
			{
				//do nothing
			}
		} 

		private string FilterStrings(string strInput) 
		{ 
			string TempInput = strInput; 
    
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline; 
			string strReplacement = " "; 
    
			TempInput = Regex.Replace(TempInput, "<script[^>]*>.*?</script[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "%3cscript.*?script%3e", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<input[^>]*>.*?</input[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<object[^>]*>.*?</object[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<embed[^>]*>.*?</embed[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<applet[^>]*>.*?</applet[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<form[^>]*>.*?</form[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<option[^>]*>.*?</option[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<select[^>]*>.*?</select[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<iframe[^>]*>.*?</iframe[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<ilayer[^>]*>.*?</ilayer[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "<form[^>]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "</form[^><]*>", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "javascript:", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "vbscript:", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "alert.*\\(?'", strReplacement, options); 
			TempInput = Regex.Replace(TempInput, "alert.*\\(?\"", strReplacement, options); 
    
			return TempInput.Trim(); 
		} 
		#endregion
	}
}
