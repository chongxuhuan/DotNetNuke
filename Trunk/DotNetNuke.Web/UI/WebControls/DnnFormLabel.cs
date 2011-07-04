using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using Telerik.Web.UI;

namespace DotNetNuke.Web.UI.WebControls
{
	public class DnnFormLabel : Panel
	{
		public DnnFormLabel()
		{
		}

		public string AssociatedControlID { get; set; }

		public string LocalResourceFile { get; set; }

		public string ResourceKey { get; set; }

		public string ToolTipKey { get; set; }

		protected override void CreateChildControls()
		{
			string toolTipText = LocalizeString(ToolTipKey);

			var outerLabel = new Label();
			outerLabel.AssociatedControlID = AssociatedControlID;
			Controls.Add(outerLabel);

			var link = new LinkButton { ID = "Link", CssClass = "dnnFormHelp", TabIndex = -1 };
			outerLabel.Controls.Add(link);

			var label = new Label { ID = "Label", Text = LocalizeString(ResourceKey) };
			link.Controls.Add(label);
			
			if (!String.IsNullOrEmpty(toolTipText))
			{
				CssClass += "dnnTooltip";

				var panel = new Panel { ID = "Help", CssClass = "dnnFormHelpContent dnnClear" };
				panel.Style.Clear();
				panel.Style.Value = "display:none;";
				Controls.Add(panel);
				
				var helpLabel = new Label { ID = "Text", CssClass="dnnHelpText", Text = LocalizeString(ToolTipKey) };
				panel.Controls.Add(helpLabel);

				var pinLink = new HyperLink();
				pinLink.CssClass = "pinHelp";
				pinLink.Attributes.Add("href", "#");
				panel.Controls.Add(pinLink);

				ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
				Page.ClientScript.RegisterClientScriptInclude("hoverintent", ResolveUrl("~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js"));
				jQuery.RequestDnnPluginsRegistration();
				Page.ClientScript.RegisterClientScriptBlock(typeof(DnnFormLabel), "dnnTooltip", "jQuery(document).ready(function($){ $('.dnnTooltip').dnnTooltip();Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function(){$('.dnnTooltip').dnnTooltip();}); });", true);
			}
		}

		protected string LocalizeString(string key)
		{
			return Localization.GetString(key, LocalResourceFile);
		}
	}
}
