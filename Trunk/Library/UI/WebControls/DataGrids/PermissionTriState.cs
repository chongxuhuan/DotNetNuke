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
            //kind of ugly to lookup this data each time, but doesn't seem worth the effort to 
            //maintain statics for the paths but require a control instance to initialize them
            //and lazy load the text bits when the page instance (or localization) changes 
            LookupScriptValues(this, out _grantImagePath, out _denyImagePath, out _nullImagePath, out _lockImagePath, out _grantAltText, out _denyAltText, out _nullAltText);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RegisterScripts(Page, this);
        }

        public static void RegisterScripts(Page page, Control ctl)
        {
            const string scriptKey = "initTriState";
            if( ! ClientAPI.IsClientScriptBlockRegistered(page, scriptKey))
            {
                AJAX.RegisterScriptManager();
                jQuery.RequestRegistration();
                ClientAPI.RegisterClientScriptBlock(page, "dnn.permissiontristate.js");
                
                string grantImagePath, denyImagePath, nullImagePath, lockImagePath, grantAltText, denyAltText, nullAltText;

                LookupScriptValues(ctl, out grantImagePath, out denyImagePath, out nullImagePath, out lockImagePath, out grantAltText, out denyAltText, out nullAltText);
                
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
                        grantImagePath,
                        denyImagePath,
                        nullImagePath,
                        grantAltText,
                        denyAltText,
                        nullAltText);

                ClientAPI.RegisterStartUpScript(page, scriptKey, script);
            }
        }

        private static void LookupScriptValues(Control ctl, out string grantImagePath, out string denyImagePath, out string nullImagePath, out string lockImagePath, out string grantAltText, out string denyAltText, out string nullAltText)
        {
            grantImagePath = ctl.ResolveUrl("~/images/grant.gif");
            denyImagePath = ctl.ResolveUrl("~/images/deny.gif");
            nullImagePath = ctl.ResolveUrl("~/images/unchecked.gif");
            lockImagePath = ctl.ResolveUrl("~/images/lock.gif");

            grantAltText = Localization.GetString("PermissionTypeGrant");
            denyAltText = Localization.GetString("PermissionTypeDeny");
            nullAltText = Localization.GetString("PermissionTypeNull");
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