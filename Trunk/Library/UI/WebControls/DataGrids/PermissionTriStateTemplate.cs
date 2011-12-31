using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls.Internal
{
    class PermissionTriStateTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            var triState = new PermissionTriState();
            triState.DataBinding += BindToTriState;
            container.Controls.Add(triState);
        }
        
        public void BindToTriState(object sender, EventArgs e)
        {
            var triState = (PermissionTriState) sender;
            var dataRowView = ((DataRowView) ((DataGridItem)triState.NamingContainer).DataItem);

            triState.Value = dataRowView[DataField].ToString();
            triState.Locked = !bool.Parse(dataRowView[EnabledField].ToString());
            triState.SupportsDenyMode = SupportDenyMode;
        }

        public string DataField { get; set; }
        public string EnabledField { get; set; }
        public bool SupportDenyMode { get; set; }
    }
}