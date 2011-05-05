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
using System.Collections;
using System.Drawing;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Vendors;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    public partial class Banner : SkinObjectBase
    {
        private const string MyFileName = "Banner.ascx";
        public string GroupName { get; set; }

        public bool AllowNullBannerType { get; set; }

        public string BannerTypeId { get; set; }

        public string BannerCount { get; set; }

        public string Width { get; set; }

        public string Orientation { get; set; }

        public string BorderWidth { get; set; }

        public string BorderColor { get; set; }

        public string RowHeight { get; set; }

        public string ColWidth { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (PortalSettings.BannerAdvertising != 0 && Visible)
            {
                int BannerType = 0;
                if (AllowNullBannerType)
                {
                    if (!string.IsNullOrEmpty(BannerTypeId))
                    {
                        BannerType = Int32.Parse(Convert.ToString(BannerTypeId));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(BannerTypeId))
                    {
                        BannerType = PortalController.GetPortalSettingAsInteger("BannerTypeId", PortalSettings.PortalId, 1);
                    }
                }
                if (String.IsNullOrEmpty(GroupName))
                {
                    GroupName = PortalController.GetPortalSetting("BannerGroupName", PortalSettings.PortalId, "");
                }
                if (String.IsNullOrEmpty(BannerCount))
                {
                    BannerCount = "1";
                }
                int intPortalId;
                if (PortalSettings.BannerAdvertising == 1)
                {
                    intPortalId = PortalSettings.PortalId;
                }
                else
                {
                    intPortalId = Null.NullInteger;
                }
                var objBanners = new BannerController();

                ArrayList arrBanners = objBanners.LoadBanners(intPortalId, Null.NullInteger, BannerType, GroupName, int.Parse(BannerCount));

                lstBanners.DataSource = arrBanners;
                lstBanners.DataBind();
                if (lstBanners.Items.Count != 0)
                {
                    lstBanners.RepeatLayout = RepeatLayout.Table;
                    if (!String.IsNullOrEmpty(Width))
                    {
                        lstBanners.Width = Unit.Parse(Width + "px");
                    }
                    if (lstBanners.Items.Count == 1)
                    {
                        lstBanners.CellPadding = 0;
                        lstBanners.CellSpacing = 0;
                    }
                    else
                    {
                        lstBanners.CellPadding = 4;
                        lstBanners.CellSpacing = 0;
                    }
                    if (!String.IsNullOrEmpty(Orientation))
                    {
                        switch (Orientation)
                        {
                            case "H":
                                lstBanners.RepeatDirection = RepeatDirection.Horizontal;
                                break;
                            case "V":
                                lstBanners.RepeatDirection = RepeatDirection.Vertical;
                                break;
                        }
                    }
                    else
                    {
                        lstBanners.RepeatDirection = RepeatDirection.Vertical;
                    }
                    if (!String.IsNullOrEmpty(BorderWidth))
                    {
                        lstBanners.ItemStyle.BorderWidth = Unit.Parse(BorderWidth + "px");
                    }
                    if (!String.IsNullOrEmpty(BorderColor))
                    {
                        var objColorConverter = new ColorConverter();
                        lstBanners.ItemStyle.BorderColor = (Color) objColorConverter.ConvertFrom(BorderColor);
                    }
                    if (!String.IsNullOrEmpty(RowHeight))
                    {
                        lstBanners.ItemStyle.Height = Unit.Parse(RowHeight + "px");
                    }
                    if (!String.IsNullOrEmpty(ColWidth))
                    {
                        lstBanners.ItemStyle.Width = Unit.Parse(ColWidth + "px");
                    }
                }
                else
                {
                    lstBanners.Visible = false;
                }
            }
            else
            {
                lstBanners.Visible = false;
            }
        }

        public string FormatItem(int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height)
        {
            var objBanners = new BannerController();
            return objBanners.FormatBanner(VendorId,
                                           BannerId,
                                           BannerTypeId,
                                           BannerName,
                                           ImageFile,
                                           Description,
                                           URL,
                                           Width,
                                           Height,
                                           PortalSettings.BannerAdvertising == 1 ? "L" : "G",
                                           PortalSettings.HomeDirectory);
        }
    }
}