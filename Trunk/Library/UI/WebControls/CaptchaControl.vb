'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports System.Web
Imports System.Web.Security
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.Collections.Specialized

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      CaptchaControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CaptchaControl control provides a Captcha Challenge control
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/17/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:CaptchaControl Runat=""server"" CaptchaHeight=""100px"" CaptchaWidth=""300px"" />")> _
    Public Class CaptchaControl
        Inherits WebControl
        Implements INamingContainer
        Implements IPostBackDataHandler

#Region "Controls"

        Private _image As System.Web.UI.WebControls.Image

#End Region

#Region "Events"

        Public Event UserValidated As ServerValidateEventHandler

#End Region

#Region "Private Constants"

        Private Const EXPIRATION_DEFAULT As Integer = 120
        Private Const LENGTH_DEFAULT As Integer = 6
        Private Const RENDERURL_DEFAULT As String = "ImageChallenge.captcha.aspx"
        Private Const CHARS_DEFAULT As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789"

#End Region

#Region "Friend Constants"

        Friend Const KEY As String = "captcha"

#End Region

#Region "Private Members"

        Private _Authenticated As Boolean
        Private _BackGroundColor As Color = Color.Transparent
        Private _BackGroundImage As String = ""
        Private _CaptchaChars As String = CHARS_DEFAULT
        Private _CaptchaHeight As Unit = Unit.Pixel(100)
        Private _CaptchaLength As Integer = LENGTH_DEFAULT
        Private _CaptchaText As String
        Private _CaptchaWidth As Unit = Unit.Pixel(300)
        Private _ErrorMessage As String
        Private _ErrorStyle As Style = New Style
        Private _Expiration As Integer = EXPIRATION_DEFAULT
        Private _IsValid As Boolean = False
        Private _RenderUrl As String = RENDERURL_DEFAULT
        Private _Text As String
        Private _TextBoxStyle As Style = New Style
        Private _UserText As String = ""

        Private Shared _FontFamilies As String() = {"Arial", "Comic Sans MS", "Courier New", _
                "Georgia", "Lucida Console", "MS Sans Serif", "Stencil", "Tahoma", _
                "Times New Roman", "Trebuchet MS", "Verdana"}
        Private Shared _Rand As New Random
        Private Shared _Separator As String = ":-:"

        Private ReadOnly Property IsDesignMode() As Boolean
            Get
                Return HttpContext.Current Is Nothing
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            _ErrorMessage = Localization.GetString("InvalidCaptcha", Localization.SharedResourceFile)
            _Text = Localization.GetString("CaptchaText.Text", Localization.SharedResourceFile)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the BackGroundColor
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance"), _
        Description("The Background Color to use for the Captcha Image.")> _
        Public Property BackGroundColor() As Color
            Get
                Return _BackGroundColor
            End Get
            Set(ByVal Value As Color)
                _BackGroundColor = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the BackGround Image
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance"), _
        Description("A Background Image to use for the Captcha Image.")> _
        Public Property BackGroundImage() As String
            Get
                Return _BackGroundImage
            End Get
            Set(ByVal Value As String)
                _BackGroundImage = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the list of characters
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        DefaultValue(CHARS_DEFAULT), _
        Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string.")> _
        Public Property CaptchaChars() As String
            Get
                Return _CaptchaChars
            End Get
            Set(ByVal Value As String)
                _CaptchaChars = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the height of the Captcha image
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance"), _
        Description("Height of Captcha Image.")> _
        Public Property CaptchaHeight() As Unit
            Get
                Return _CaptchaHeight
            End Get
            Set(ByVal Value As Unit)
                _CaptchaHeight = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the length of the Captcha string
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        DefaultValue(LENGTH_DEFAULT), _
        Description("Number of CaptchaChars used in the CAPTCHA text")> _
        Public Property CaptchaLength() As Integer
            Get
                Return _CaptchaLength
            End Get
            Set(ByVal Value As Integer)
                _CaptchaLength = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the width of the Captcha image
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance"), _
        Description("Width of Captcha Image.")> _
        Public Property CaptchaWidth() As Unit
            Get
                Return _CaptchaWidth
            End Get
            Set(ByVal Value As Unit)
                _CaptchaWidth = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Viewstate is enabled
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> Public Overrides Property EnableViewState() As Boolean
            Get
                Return MyBase.EnableViewState
            End Get
            Set(ByVal Value As Boolean)
                MyBase.EnableViewState = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ErrorMessage to display if the control is invalid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        Description("The Error Message to display if invalid."), _
        DefaultValue("")> _
        Public Property ErrorMessage() As String
            Get
                Return _ErrorMessage
            End Get
            Set(ByVal Value As String)
                _ErrorMessage = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Style to use for the ErrorMessage
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Error Message Control.")> _
        Public ReadOnly Property ErrorStyle() As Style
            Get
                Return _ErrorStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Expiration time in seconds
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        Description("The duration of time (seconds) a user has before the challenge expires."), _
        DefaultValue(EXPIRATION_DEFAULT)> _
        Public Property Expiration() As Integer
            Get
                Return _Expiration
            End Get
            Set(ByVal Value As Integer)
                _Expiration = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the control is valid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Validation"), _
        Description("Returns True if the user was CAPTCHA validated after a postback.")> _
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return _IsValid
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Url to use to render the control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        Description("The URL used to render the image to the client."), _
        DefaultValue(RENDERURL_DEFAULT)> _
        Public Property RenderUrl() As String
            Get
                Return _RenderUrl
            End Get
            Set(ByVal Value As String)
                _RenderUrl = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Help Text to use
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Captcha"), _
        DefaultValue("Enter the code shown above:"), _
        Description("Instructional text displayed next to CAPTCHA image.")> _
        Public Property Text() As String
            Get
                Return _Text
            End Get
            Set(ByVal Value As String)
                _Text = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Style to use for the Text Box
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	09/02/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Text Box Control.")> _
        Public ReadOnly Property TextBoxStyle() As Style
            Get
                Return _TextBoxStyle
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the url for the Handler
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetUrl() As String
            Dim url As String = ResolveUrl(RenderUrl)
            url += "?" + KEY + "=" + Encrypt(EncodeTicket(), DateTime.Now.AddSeconds(Expiration))

            'Append the Alias to the url so that it doesn't lose track of the alias it's currently on

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            url += "&alias=" & _portalSettings.PortalAlias.HTTPAlias()
            Return url
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Encodes the querystring to pass to the Handler
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function EncodeTicket() As String

            Dim sb As New System.Text.StringBuilder

            sb.Append(CaptchaWidth.Value.ToString)
            sb.Append(_Separator + CaptchaHeight.Value.ToString)
            sb.Append(_Separator + _CaptchaText)
            sb.Append(_Separator + BackGroundImage)

            Return sb.ToString

        End Function

#End Region

#Region "Shared/Static Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the Image
        ''' </summary>
        ''' <param name="width">The width of the image</param>
        ''' <param name="height">The height of the image</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function CreateImage(ByVal width As Integer, ByVal height As Integer) As Bitmap

            Dim bmp As Bitmap = New Bitmap(width, height)
            Dim g As Graphics
            Dim rect As Rectangle = New Rectangle(0, 0, width, height)
            Dim rectF As RectangleF = New RectangleF(0, 0, width, height)

            g = Graphics.FromImage(bmp)

            Dim b As Brush = New LinearGradientBrush(rect, _
                        Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), _
                        Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), _
                        CSng(_Rand.NextDouble) * 360, False)
            g.FillRectangle(b, rectF)

            If _Rand.Next(2) = 1 Then
                DistortImage(bmp, _Rand.Next(5, 10))
            Else
                DistortImage(bmp, -_Rand.Next(5, 10))
            End If

            Return bmp

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the Text
        ''' </summary>
        ''' <param name="text">The text to display</param>
        ''' <param name="width">The width of the image</param>
        ''' <param name="height">The height of the image</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function CreateText(ByVal text As String, ByVal width As Integer, ByVal height As Integer, ByVal g As Graphics) As GraphicsPath

            Dim textPath As GraphicsPath = New GraphicsPath
            Dim ff As FontFamily = GetFont()
            Dim emSize As Integer = CInt(width * 2 / text.Length)
            Dim f As Font = Nothing
            Try
                Dim measured As New SizeF(0, 0)
                Dim workingSize As New SizeF(width, height)
                While (emSize > 2)
                    f = New Font(ff, emSize)
                    measured = g.MeasureString(text, f)
                    If Not (measured.Width > workingSize.Width Or measured.Height > workingSize.Height) Then
                        Exit While
                    End If
                    f.Dispose()
                    emSize -= 2
                End While
                emSize += 8
                f = New Font(ff, emSize)

                Dim fmt As New StringFormat
                fmt.Alignment = StringAlignment.Center
                fmt.LineAlignment = StringAlignment.Center

                textPath.AddString(text, f.FontFamily, CType(f.Style, Integer), f.Size, New RectangleF(0, 0, width, height), fmt)
                WarpText(textPath, New Rectangle(0, 0, width, height))

            Catch ex As Exception
            Finally
                f.Dispose()
            End Try

            Return textPath

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Decrypts the CAPTCHA Text
        ''' </summary>
        ''' <param name="encryptedContent">The encrypted text</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function Decrypt(ByVal encryptedContent As String) As String

            Dim decryptedText As String = String.Empty
            Try
                Dim ticket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(encryptedContent)
                If (Not ticket.Expired) Then
                    decryptedText = ticket.UserData
                End If
            Catch exc As ArgumentException

            End Try

            Return decryptedText

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DistortImage distorts the captcha image
        ''' </summary>
        ''' <param name="b">The Image to distort</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub DistortImage(ByRef b As Bitmap, ByVal distortion As Double)

            Dim width As Integer = b.Width
            Dim height As Integer = b.Height

            Dim copy As Bitmap = CType(b.Clone(), Bitmap)

            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    Dim newX As Integer = CInt(x + (distortion * Math.Sin(Math.PI * y / 64.0)))
                    Dim newY As Integer = CInt(y + (distortion * Math.Cos(Math.PI * x / 64.0)))
                    If (newX < 0 Or newX >= width) Then newX = 0
                    If (newY < 0 Or newY >= height) Then newY = 0
                    b.SetPixel(x, y, copy.GetPixel(newX, newY))
                Next
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Encrypts the CAPTCHA Text
        ''' </summary>
        ''' <param name="content">The text to encrypt</param>
        ''' <param name="expiration">The time the ticket expires</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function Encrypt(ByVal content As String, ByVal expiration As DateTime) As String
            Dim ticket As New FormsAuthenticationTicket( _
                1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, _
                expiration, False, content)
            Return FormsAuthentication.Encrypt(ticket)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GenerateImage generates the Captch Image
        ''' </summary>
        ''' <param name="encryptedText">The Encrypted Text to display</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function GenerateImage(ByVal encryptedText As String) As Bitmap
            Dim encodedText As String = Decrypt(encryptedText)
            Dim bmp As Bitmap = Nothing
            Dim Settings As String() = Split(encodedText, _Separator)

            Try
                Dim width As Integer = Integer.Parse(Settings(0))
                Dim height As Integer = Integer.Parse(Settings(1))
                Dim text As String = Settings(2)
                Dim backgroundImage As String = Settings(3)

                Dim g As Graphics
                Dim b As Brush = New SolidBrush(Color.LightGray)
                Dim b1 As Brush = New SolidBrush(Color.Black)

                If backgroundImage = "" Then
                    bmp = CreateImage(width, height)
                Else
                    bmp = CType(Bitmap.FromFile(HttpContext.Current.Request.MapPath(backgroundImage)), Bitmap)
                End If
                g = Graphics.FromImage(bmp)

                'Create Text
                Dim textPath As GraphicsPath = CreateText(text, width, height, g)
                If backgroundImage = "" Then
                    g.FillPath(b, textPath)
                Else
                    g.FillPath(b1, textPath)
                End If

            Catch exc As Exception
                LogException(exc)
            End Try

            Return bmp

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFont gets a random font to use for the Captcha Text
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetFont() As FontFamily

            Dim _font As FontFamily = Nothing
            While _font Is Nothing
                Try
                    _font = New FontFamily(_FontFamilies(_Rand.Next(_FontFamilies.Length)))
                Catch ex As Exception
                    _font = Nothing
                End Try
            End While
            Return _font

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates a random point
        ''' </summary>
        ''' <param name="xmin">The minimum x value</param>
        ''' <param name="xmax">The maximum x value</param>
        ''' <param name="ymin">The minimum y value</param>
        ''' <param name="ymax">The maximum y value</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function RandomPoint(ByVal xmin As Integer, ByVal xmax As Integer, ByRef ymin As Integer, ByRef ymax As Integer) As PointF
            Return New PointF(_Rand.Next(xmin, xmax), _Rand.Next(ymin, ymax))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Warps the Text
        ''' </summary>
        ''' <param name="textPath">The Graphics Path for the text</param>
        ''' <param name="rect">a rectangle which defines the image</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub WarpText(ByRef textPath As GraphicsPath, ByVal rect As Rectangle)

            Dim intWarpDivisor As Integer
            Dim rectF As RectangleF = New RectangleF(0, 0, rect.Width, rect.Height)

            intWarpDivisor = _Rand.Next(4, 8)

            Dim intHrange As Integer = Convert.ToInt32(rect.Height / intWarpDivisor)
            Dim intWrange As Integer = Convert.ToInt32(rect.Width / intWarpDivisor)

            Dim p1 As PointF = RandomPoint(0, intWrange, 0, intHrange)
            Dim p2 As PointF = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p1.X)), rect.Width, 0, intHrange)
            Dim p3 As PointF = RandomPoint(0, intWrange, rect.Height - (intHrange - Convert.ToInt32(p1.Y)), rect.Height)
            Dim p4 As PointF = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p3.X)), rect.Width, rect.Height - (intHrange - Convert.ToInt32(p2.Y)), rect.Height)

            Dim points As PointF() = New PointF() {p1, p2, p3, p4}
            Dim m As New Matrix
            m.Translate(0, 0)
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0)

        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the child controls
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            If (Me.CaptchaWidth.IsEmpty Or Me.CaptchaWidth.Type <> UnitType.Pixel Or _
                    Me.CaptchaHeight.IsEmpty Or Me.CaptchaHeight.Type <> UnitType.Pixel) Then
                Throw New InvalidOperationException("Must specify size of control in pixels.")
            End If

            _image = New System.Web.UI.WebControls.Image
            _image.BorderColor = Me.BorderColor
            _image.BorderStyle = Me.BorderStyle
            _image.BorderWidth = Me.BorderWidth
            _image.ToolTip = Me.ToolTip
            _image.EnableViewState = False
            Controls.Add(_image)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the next Captcha
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetNextCaptcha() As String
            Dim sb As New System.Text.StringBuilder
            Dim _rand As New Random
            Dim n As Integer

            Dim intMaxLength As Integer = CaptchaChars.Length

            For n = 0 To CaptchaLength - 1
                sb.Append(CaptchaChars.Substring(_rand.Next(intMaxLength), 1))
            Next
            Return sb.ToString

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads the previously saved Viewstate
        ''' </summary>
        ''' <param name="savedState">The saved state</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If Not (savedState Is Nothing) Then
                ' Load State from the array of objects that was saved at SaveViewState.
                Dim myState As Object() = CType(savedState, Object())

                'Load the ViewState of the Base Control
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If

                'Load the CAPTCHA Text from the ViewState
                If Not (myState(1) Is Nothing) Then
                    _CaptchaText = CStr(myState(1))
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs just before the control is to be rendered
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            'Generate Random Challenge Text
            _CaptchaText = GetNextCaptcha()

            'Enable Viewstate Encryption
            Page.RegisterRequiresViewStateEncryption()

            'Call Base Class method
            MyBase.OnPreRender(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Render the  control
        ''' </summary>
        ''' <param name="writer">An Html Text Writer</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)

            ControlStyle.AddAttributesToRender(writer)

            'Render outer <div> Tag
            writer.RenderBeginTag(HtmlTextWriterTag.Div)

            'Render image <img> Tag
            writer.AddAttribute(HtmlTextWriterAttribute.Src, GetUrl)
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
            If ToolTip.Length > 0 Then
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, ToolTip)
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, Localization.GetString("CaptchaAlt.Text", Localization.SharedResourceFile))
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Img)
            writer.RenderEndTag()

            'Render Help Text
            If Text.Length > 0 Then
                writer.RenderBeginTag(HtmlTextWriterTag.Div)
                writer.Write(Text)
                writer.RenderEndTag()
            End If

            'Render text box <input> Tag
            TextBoxStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + Width.ToString)
            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, _CaptchaText.Length.ToString)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            If AccessKey.Length > 0 Then
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey)
            End If
            If Not Enabled Then
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled")
            End If
            If TabIndex > 0 Then
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString)
            End If
            If _UserText = _CaptchaText Then
                writer.AddAttribute(HtmlTextWriterAttribute.Value, _UserText)
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "")
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            'Render error message
            If Not IsValid AndAlso Page.IsPostBack AndAlso Not String.IsNullOrEmpty(_UserText) Then
                ErrorStyle.AddAttributesToRender(writer)
                writer.RenderBeginTag(HtmlTextWriterTag.Div)
                writer.Write(ErrorMessage)
                writer.RenderEndTag()
            End If

            'Render </div>
            writer.RenderEndTag()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Save the controls Voewstate
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SaveViewState() As Object
            Dim baseState As Object = MyBase.SaveViewState()
            Dim allStates(2) As Object
            allStates(0) = baseState
            If String.IsNullOrEmpty(_CaptchaText) Then
                _CaptchaText = GetNextCaptcha()
            End If
            allStates(1) = _CaptchaText
            Return allStates
        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Validates the posted back data
        ''' </summary>
        ''' <param name="userData">The user entered data</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Validate(ByVal userData As String) As Boolean
            If String.Compare(userData, Me._CaptchaText, False, CultureInfo.InvariantCulture) = 0 Then
                _IsValid = True
            Else
                _IsValid = False
            End If
            RaiseEvent UserValidated(Me, New ServerValidateEventArgs(_CaptchaText, _IsValid))
        End Function

#End Region

#Region "IPostBackDataHandler Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadPostData loads the Post Back Data and determines whether the value has change
        ''' </summary>
        ''' <param name="postDataKey">A key to the PostBack Data to load</param>
        ''' <param name="postCollection">A name value collection of postback data</param>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData

            _UserText = postCollection(postDataKey)
            Validate(_UserText)

            If Not _IsValid AndAlso Not String.IsNullOrEmpty(_UserText) Then
                'Generate Random Challenge Text
                _CaptchaText = GetNextCaptcha()
            End If

            Return False

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RaisePostDataChangedEvent runs when the PostBackData has changed. 
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub

#End Region

    End Class

End Namespace

