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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using Image = System.Web.UI.WebControls.Image;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      CaptchaControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The CaptchaControl control provides a Captcha Challenge control
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/17/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:CaptchaControl Runat=\"server\" CaptchaHeight=\"100px\" CaptchaWidth=\"300px\" />")]
    public class CaptchaControl : WebControl, INamingContainer, IPostBackDataHandler
    {
        private const int EXPIRATION_DEFAULT = 120;
        private const int LENGTH_DEFAULT = 6;
        private const string RENDERURL_DEFAULT = "ImageChallenge.captcha.aspx";
        private const string CHARS_DEFAULT = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        internal const string KEY = "captcha";

        private static readonly string[] _FontFamilies = {"Arial", "Comic Sans MS", "Courier New", "Georgia", "Lucida Console", "MS Sans Serif", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana"};

        private static readonly Random _Rand = new Random();
        private static string _Separator = ":-:";
        private readonly Style _ErrorStyle = new Style();
        private readonly Style _TextBoxStyle = new Style();
        private Color _BackGroundColor = Color.Transparent;
        private string _BackGroundImage = "";
        private string _CaptchaChars = CHARS_DEFAULT;
        private Unit _CaptchaHeight = Unit.Pixel(100);
        private int _CaptchaLength = LENGTH_DEFAULT;
        private string _CaptchaText;
        private Unit _CaptchaWidth = Unit.Pixel(300);
        private int _Expiration = EXPIRATION_DEFAULT;
        private bool _IsValid;
        private string _RenderUrl = RENDERURL_DEFAULT;
        private string _UserText = "";
        private Image _image;

        public CaptchaControl()
        {
            ErrorMessage = Localization.GetString("InvalidCaptcha", Localization.SharedResourceFile);
            Text = Localization.GetString("CaptchaText.Text", Localization.SharedResourceFile);
        }

        private bool IsDesignMode
        {
            get
            {
                return HttpContext.Current == null;
            }
        }

        [Category("Appearance"), Description("The Background Color to use for the Captcha Image.")]
        public Color BackGroundColor
        {
            get
            {
                return _BackGroundColor;
            }
            set
            {
                _BackGroundColor = value;
            }
        }

        [Category("Appearance"), Description("A Background Image to use for the Captcha Image.")]
        public string BackGroundImage
        {
            get
            {
                return _BackGroundImage;
            }
            set
            {
                _BackGroundImage = value;
            }
        }

        [Category("Behavior"), DefaultValue(CHARS_DEFAULT), Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string.")]
        public string CaptchaChars
        {
            get
            {
                return _CaptchaChars;
            }
            set
            {
                _CaptchaChars = value;
            }
        }

        [Category("Appearance"), Description("Height of Captcha Image.")]
        public Unit CaptchaHeight
        {
            get
            {
                return _CaptchaHeight;
            }
            set
            {
                _CaptchaHeight = value;
            }
        }

        [Category("Behavior"), DefaultValue(LENGTH_DEFAULT), Description("Number of CaptchaChars used in the CAPTCHA text")]
        public int CaptchaLength
        {
            get
            {
                return _CaptchaLength;
            }
            set
            {
                _CaptchaLength = value;
            }
        }

        [Category("Appearance"), Description("Width of Captcha Image.")]
        public Unit CaptchaWidth
        {
            get
            {
                return _CaptchaWidth;
            }
            set
            {
                _CaptchaWidth = value;
            }
        }

        [Browsable(false)]
        public override bool EnableViewState
        {
            get
            {
                return base.EnableViewState;
            }
            set
            {
                base.EnableViewState = value;
            }
        }

        [Category("Behavior"), Description("The Error Message to display if invalid."), DefaultValue("")]
        public string ErrorMessage { get; set; }

         /// -----------------------------------------------------------------------------
         /// <summary>
         /// Gets and sets the BackGroundColor
         /// </summary>
         /// <history>
         /// 	[cnurse]	03/20/2006	Created
         /// </history>
         /// -----------------------------------------------------------------------------
        [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         Description("Set the Style for the Error Message Control.")]
        public Style ErrorStyle
        {
            get
            {
                return _ErrorStyle;
            }
        }

        [Category("Behavior"), Description("The duration of time (seconds) a user has before the challenge expires."), DefaultValue(EXPIRATION_DEFAULT)]
        public int Expiration
        {
            get
            {
                return _Expiration;
            }
            set
            {
                _Expiration = value;
            }
        }

        [Category("Validation"), Description("Returns True if the user was CAPTCHA validated after a postback.")]
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
        }

        [Category("Behavior"), Description("The URL used to render the image to the client."), DefaultValue(RENDERURL_DEFAULT)]
        public string RenderUrl
        {
            get
            {
                return _RenderUrl;
            }
            set
            {
                _RenderUrl = value;
            }
        }

        [Category("Captcha"), DefaultValue("Enter the code shown above:"), Description("Instructional text displayed next to CAPTCHA image.")]
        public string Text { get; set; }

         /// -----------------------------------------------------------------------------
         /// <summary>
         /// Gets and sets the BackGround Image
         /// </summary>
         /// <history>
         /// 	[cnurse]	03/20/2006	Created
         /// </history>
         /// -----------------------------------------------------------------------------
        [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         Description("Set the Style for the Text Box Control.")]
        public Style TextBoxStyle
        {
            get
            {
                return _TextBoxStyle;
            }
        }

        #region IPostBackDataHandler Members

        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            _UserText = postCollection[postDataKey];
            Validate(_UserText);
            if (!_IsValid && !string.IsNullOrEmpty(_UserText))
            {
                _CaptchaText = GetNextCaptcha();
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        #endregion

        public event ServerValidateEventHandler UserValidated;

        private string GetUrl()
        {
            string url = ResolveUrl(RenderUrl);
            url += "?" + KEY + "=" + Encrypt(EncodeTicket(), DateTime.Now.AddSeconds(Expiration));
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            url += "&alias=" + _portalSettings.PortalAlias.HTTPAlias;
            return url;
        }

        private string EncodeTicket()
        {
            var sb = new StringBuilder();
            sb.Append(CaptchaWidth.Value.ToString());
            sb.Append(_Separator + CaptchaHeight.Value);
            sb.Append(_Separator + _CaptchaText);
            sb.Append(_Separator + BackGroundImage);
            return sb.ToString();
        }

        private static Bitmap CreateImage(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            Graphics g;
            var rect = new Rectangle(0, 0, width, height);
            var rectF = new RectangleF(0, 0, width, height);
            g = Graphics.FromImage(bmp);
            Brush b = new LinearGradientBrush(rect,
                                              Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)),
                                              Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)),
                                              Convert.ToSingle(_Rand.NextDouble())*360,
                                              false);
            g.FillRectangle(b, rectF);
            if (_Rand.Next(2) == 1)
            {
                DistortImage(ref bmp, _Rand.Next(5, 10));
            }
            else
            {
                DistortImage(ref bmp, -_Rand.Next(5, 10));
            }
            return bmp;
        }

        private static GraphicsPath CreateText(string text, int width, int height, Graphics g)
        {
            var textPath = new GraphicsPath();
            FontFamily ff = GetFont();
            int emSize = Convert.ToInt32(width*2/text.Length);
            Font f = null;
            try
            {
                var measured = new SizeF(0, 0);
                var workingSize = new SizeF(width, height);
                while ((emSize > 2))
                {
                    f = new Font(ff, emSize);
                    measured = g.MeasureString(text, f);
                    if (!(measured.Width > workingSize.Width || measured.Height > workingSize.Height))
                    {
                        break;
                    }
                    f.Dispose();
                    emSize -= 2;
                }
                emSize += 8;
                f = new Font(ff, emSize);
                var fmt = new StringFormat();
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;
                textPath.AddString(text, f.FontFamily, Convert.ToInt32(f.Style), f.Size, new RectangleF(0, 0, width, height), fmt);
                WarpText(ref textPath, new Rectangle(0, 0, width, height));
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

            }
            finally
            {
                f.Dispose();
            }
            return textPath;
        }

        private static string Decrypt(string encryptedContent)
        {
            string decryptedText = string.Empty;
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedContent);
                if ((!ticket.Expired))
                {
                    decryptedText = ticket.UserData;
                }
            }
            catch (ArgumentException exc)
            {
                DnnLog.Debug(exc);

            }
            return decryptedText;
        }

        private static void DistortImage(ref Bitmap b, double distortion)
        {
            int width = b.Width;
            int height = b.Height;
            var copy = (Bitmap) b.Clone();
            for (int y = 0; y <= height - 1; y++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    int newX = Convert.ToInt32(x + (distortion*Math.Sin(Math.PI*y/64.0)));
                    int newY = Convert.ToInt32(y + (distortion*Math.Cos(Math.PI*x/64.0)));
                    if ((newX < 0 || newX >= width))
                    {
                        newX = 0;
                    }
                    if ((newY < 0 || newY >= height))
                    {
                        newY = 0;
                    }
                    b.SetPixel(x, y, copy.GetPixel(newX, newY));
                }
            }
        }

        private static string Encrypt(string content, DateTime expiration)
        {
            var ticket = new FormsAuthenticationTicket(1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, expiration, false, content);
            return FormsAuthentication.Encrypt(ticket);
        }

        internal static Bitmap GenerateImage(string encryptedText)
        {
            string encodedText = Decrypt(encryptedText);
            Bitmap bmp = null;
            string[] Settings = Regex.Split(encodedText, _Separator);
            try
            {
                int width = int.Parse(Settings[0]);
                int height = int.Parse(Settings[1]);
                string text = Settings[2];
                string backgroundImage = Settings[3];
                Graphics g;
                Brush b = new SolidBrush(Color.LightGray);
                Brush b1 = new SolidBrush(Color.Black);
                if (String.IsNullOrEmpty(backgroundImage))
                {
                    bmp = CreateImage(width, height);
                }
                else
                {
                    bmp = (Bitmap) System.Drawing.Image.FromFile(HttpContext.Current.Request.MapPath(backgroundImage));
                }
                g = Graphics.FromImage(bmp);
                GraphicsPath textPath = CreateText(text, width, height, g);
                if (String.IsNullOrEmpty(backgroundImage))
                {
                    g.FillPath(b, textPath);
                }
                else
                {
                    g.FillPath(b1, textPath);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            return bmp;
        }

        private static FontFamily GetFont()
        {
            FontFamily _font = null;
            while (_font == null)
            {
                try
                {
                    _font = new FontFamily(_FontFamilies[_Rand.Next(_FontFamilies.Length)]);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                    _font = null;
                }
            }
            return _font;
        }

        private static PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF(_Rand.Next(xmin, xmax), _Rand.Next(ymin, ymax));
        }

        private static void WarpText(ref GraphicsPath textPath, Rectangle rect)
        {
            int intWarpDivisor;
            var rectF = new RectangleF(0, 0, rect.Width, rect.Height);
            intWarpDivisor = _Rand.Next(4, 8);
            int intHrange = Convert.ToInt32(rect.Height/intWarpDivisor);
            int intWrange = Convert.ToInt32(rect.Width/intWarpDivisor);
            PointF p1 = RandomPoint(0, intWrange, 0, intHrange);
            PointF p2 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p1.X)), rect.Width, 0, intHrange);
            PointF p3 = RandomPoint(0, intWrange, rect.Height - (intHrange - Convert.ToInt32(p1.Y)), rect.Height);
            PointF p4 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p3.X)), rect.Width, rect.Height - (intHrange - Convert.ToInt32(p2.Y)), rect.Height);
            var points = new[] {p1, p2, p3, p4};
            var m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if ((CaptchaWidth.IsEmpty || CaptchaWidth.Type != UnitType.Pixel || CaptchaHeight.IsEmpty || CaptchaHeight.Type != UnitType.Pixel))
            {
                throw new InvalidOperationException("Must specify size of control in pixels.");
            }
            _image = new Image();
            _image.BorderColor = BorderColor;
            _image.BorderStyle = BorderStyle;
            _image.BorderWidth = BorderWidth;
            _image.ToolTip = ToolTip;
            _image.EnableViewState = false;
            Controls.Add(_image);
        }

        protected virtual string GetNextCaptcha()
        {
            var sb = new StringBuilder();
            var _rand = new Random();
            int n;
            int intMaxLength = CaptchaChars.Length;
            for (n = 0; n <= CaptchaLength - 1; n++)
            {
                sb.Append(CaptchaChars.Substring(_rand.Next(intMaxLength), 1));
            }
            return sb.ToString();
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                var myState = (object[]) savedState;
                if (myState[0] != null)
                {
                    base.LoadViewState(myState[0]);
                }
                if (myState[1] != null)
                {
                    _CaptchaText = Convert.ToString(myState[1]);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            _CaptchaText = GetNextCaptcha();

            Page.RegisterRequiresViewStateEncryption();
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, GetUrl());
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            if (!String.IsNullOrEmpty(ToolTip))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, ToolTip);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, Localization.GetString("CaptchaAlt.Text", Localization.SharedResourceFile));
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            if (!String.IsNullOrEmpty(Text))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(Text);
                writer.RenderEndTag();
            }
            TextBoxStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + Width);
            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, _CaptchaText.Length.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            if (!String.IsNullOrEmpty(AccessKey))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);
            }
            if (!Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            if (TabIndex > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());
            }
            if (_UserText == _CaptchaText)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Value, _UserText);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            if (!IsValid && Page.IsPostBack && !string.IsNullOrEmpty(_UserText))
            {
                ErrorStyle.AddAttributesToRender(writer);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(ErrorMessage);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            var allStates = new object[2];
            allStates[0] = baseState;
            if (string.IsNullOrEmpty(_CaptchaText))
            {
                _CaptchaText = GetNextCaptcha();
            }
            allStates[1] = _CaptchaText;
            return allStates;
        }

        public bool Validate(string userData)
        {
            if (string.Compare(userData, _CaptchaText, false, CultureInfo.InvariantCulture) == 0)
            {
                _IsValid = true;
            }
            else
            {
                _IsValid = false;
            }
            UserValidated(this, new ServerValidateEventArgs(_CaptchaText, _IsValid));
            return _IsValid;
        }
    }
}
