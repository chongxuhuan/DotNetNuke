using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <summary>
    /// A TriState permission control built specifically for use in the PermissionGrid control
    /// This control is not general in any way shape of form and should NOT be used outside 
    /// of the PermissionGrid
    /// </summary>
    class PermissionTriState : HiddenField
    {
        private readonly string _grantImagePath;
        private readonly string _denyImagePath;
        private readonly string _nullImagePath;
        private readonly string _lockImagePath;
        private readonly string _grantAltText;
        private readonly string _denyAltText;
        private readonly string _nullAltText;

        public PermissionTriState()
        {
            _grantImagePath = ResolveUrl("~/images/grant.gif");
            _denyImagePath = ResolveUrl("~/images/deny.gif");
            _nullImagePath = ResolveUrl("~/images/unchecked.gif");
            _lockImagePath = ResolveUrl("~/images/lock.gif");

            _grantAltText =  Localization.GetString("PermissionTypeGrant");
            _denyAltText =  Localization.GetString("PermissionTypeDeny");
            _nullAltText = Localization.GetString("PermissionTypeNull");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if ( ! Page.ClientScript.IsClientScriptBlockRegistered(typeof(PermissionTriState), "initTriState"))
            {

                AJAX.RegisterScriptManager();
                jQuery.RequestRegistration();
                Page.ClientScript.RegisterClientScriptInclude("permissiontristateajax", ResolveUrl("~/js/MicrosoftAjax.js"));
                Page.ClientScript.RegisterClientScriptInclude("permissiontristate", ResolveUrl("~/js/dnn.permissiontristate.js"));

                string script =
                    String.Format(
                        @"<script type='text/javascript'>
                            jQuery(document).ready(
                             function() {{
                                var images = {{ 'True': '{0}', 'False': '{1}', 'Null': '{2}' }};
                                var toolTips = {{ 'True': '{3}', 'False': '{4}', 'Null': '{5}' }};
                                var tsm = dnn.controls.triStateManager(images, toolTips);
                                jQuery('.tristate').each( function(i, elem) {{
                                  tsm.initControl( elem );
                                }});
                             }});</script>",
                        _grantImagePath,
                        _denyImagePath,
                        _nullImagePath,
                        _grantAltText,
                        _denyAltText,
                        _nullAltText);

                Page.ClientScript.RegisterClientScriptBlock(typeof (PermissionTriState), "initTriState", script);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string imagePath;
            string altText;
            switch (Value)
            {
                case "True":
                    imagePath = _grantImagePath;
                    altText = _grantAltText;
                    break;

                case "False":
                    imagePath = _denyImagePath;
                    altText = _denyAltText;
                    break;

                default:
                    imagePath = _nullImagePath;
                    altText = _nullAltText;
                    break;
            }

            string cssClass = "tristate";
            if(Locked)
            {
                imagePath = _lockImagePath;
                cssClass += " lockedPerm";
                //altText is set based on Value
            }

            if(!SupportsDenyMode)
            {
                cssClass += " noDenyPerm";
            }

            writer.Write("<img src='{0}' alt='{1}' />", imagePath, altText);

            writer.AddAttribute("class", cssClass);
            base.Render(writer);
        }

        //Locked is currently not used on a post-back and therefore the 
        //value on postback is undefined at this time
        public bool Locked { get; set; }
        public bool SupportsDenyMode { get; set; }
    }
}