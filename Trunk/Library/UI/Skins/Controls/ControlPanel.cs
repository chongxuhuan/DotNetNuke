using System;
using DotNetNuke.Entities.Host;
using DotNetNuke.UI.ControlPanels;

namespace DotNetNuke.UI.Skins.Controls
{
  public class ControlPanel : SkinObjectBase
  {
    public bool IsDockable { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request.QueryString["dnnprintmode"] != "true" && Request.QueryString["popUp"] != "true")
      {
        var objControlPanel = ControlUtilities.LoadControl<ControlPanelBase>(this, Host.ControlPanel);
        objControlPanel.IsDockable = IsDockable;
        this.Controls.Add(objControlPanel);
      }
    }
  }
}
