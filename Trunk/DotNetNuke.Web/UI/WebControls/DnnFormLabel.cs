using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using Telerik.Web.UI;

namespace DotNetNuke.Web.UI.WebControls
{
    public class DnnFormLabel : Label
    {
        public DnnFormLabel()
        {
        }

        public string LocalResourceFile { get; set; }

        public string ResourceKey { get; set; }

        public string ToolTipKey { get; set; }

        protected override void CreateChildControls()
        {
            string toolTipText = LocalizeString(ToolTipKey);
            if (!String.IsNullOrEmpty(toolTipText))
            {
                var link = new LinkButton { ID = "Link", CssClass = "dnnFormHelp", TabIndex = -1};
                Controls.Add(link);

                var label = new Label { ID = "Label", Text = LocalizeString(ResourceKey) };
                link.Controls.Add(label);

                var panel = new Panel { ID = "Help", CssClass = "dnnFormHelpContent dnnClear" };
                Controls.Add(panel);
                
                var helpLabel = new Label { ID = "Text", Text = LocalizeString(ToolTipKey) };
                panel.Controls.Add(helpLabel);

                DNNClientAPI.EnableMinMax(link, panel, true, DNNClientAPI.MinMaxPersistanceType.None);
            }
            else
            {
                Text = LocalizeString(ResourceKey);                
            }
        }

        protected string LocalizeString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }
    }
}
