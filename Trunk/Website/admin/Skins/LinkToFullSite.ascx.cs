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
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    /// -----------------------------------------------------------------------------
    /// <summary>Skin object of portal links between desktop and mobile portals.</summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// -----------------------------------------------------------------------------
    public partial class LinkToFullSite : SkinObjectBase
    {
        private const string MyFileName = "LinkToFullSite.ascx";

    	private string _localResourcesFile;

    	private int SourcePortal
    	{
    		get
    		{
    			int sourcePortal;
    			if(Request.Cookies["SourcePortal"] != null && int.TryParse(Request.Cookies["SourcePortal"].Value, out sourcePortal))
    			{
    				return sourcePortal;
    			}
    			else
    			{
    				return Null.NullInteger;
    			}
    		}
    	}

    	private string LocalResourcesFile
    	{
    		get
    		{
    			if(string.IsNullOrEmpty(_localResourcesFile))
    			{
    				_localResourcesFile = Localization.GetResourceFile(this, MyFileName);
    			}

    			return _localResourcesFile;
    		}
    	}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(SourcePortal == Null.NullInteger || new PortalController().GetPortal(SourcePortal) == null)
			{
				this.Visible = false;
			}
			else
			{
				var lnkPortal = FindControl("lnkPortal") as HyperLink;
				var portalLink = string.Format("{0}?sp=-1", Globals.AddHTTP(new PortalSettings(SourcePortal).DefaultPortalAlias));
				lnkPortal.NavigateUrl = portalLink;
				lnkPortal.Text = Localization.GetString("lnkPortal", LocalResourcesFile);
			}
		}
    }
}