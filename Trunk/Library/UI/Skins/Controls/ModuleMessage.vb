'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
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

Namespace DotNetNuke.UI.Skins.Controls
    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleMessage
        Inherits UI.Skins.SkinObjectBase

        Public Enum ModuleMessageType
            GreenSuccess
            YellowWarning
            RedError
            BlueInfo
        End Enum

        ' private members
        Private _text As String
        Private _heading As String
        Private _iconType As ModuleMessageType
        Private _iconImage As String

        ' protected controls
        Protected WithEvents lblMessage As System.Web.UI.WebControls.Label
        Protected WithEvents lblHeading As System.Web.UI.WebControls.Label
        Protected WithEvents imgIcon As System.Web.UI.WebControls.Image
        Protected WithEvents imgLogo As System.Web.UI.WebControls.Image

#Region "Public Members"

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal Value As String)
                _text = Value
            End Set
        End Property

        Public Property Heading() As String
            Get
                Return _heading
            End Get
            Set(ByVal Value As String)
                _heading = Value
            End Set
        End Property

        Public Property IconType() As ModuleMessageType
            Get
                Return _iconType
            End Get
            Set(ByVal Value As ModuleMessageType)
                _iconType = Value
            End Set
        End Property

        Public Property IconImage() As String
            Get
                Return _iconImage
            End Get
            Set(ByVal Value As String)
                _iconImage = Value
            End Set
        End Property

#End Region

        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                Dim strMessage As String = ""

                'check to see if a url
                'was passed in for an icon
                If IconImage <> "" Then
                    strMessage += Me.Text
                    lblHeading.CssClass = "SubHead"
                    lblMessage.CssClass = "Normal"
                    imgIcon.ImageUrl = IconImage
                    imgIcon.Visible = True
                Else
                    Select Case IconType
                        Case UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess
                            strMessage += Me.Text
                            lblHeading.CssClass = "SubHead"
                            lblMessage.CssClass = "Normal"
                            imgIcon.ImageUrl = "~/images/green-ok.gif"
                            imgIcon.Visible = True
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess.ToString())
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess.ToString())
                        Case UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning
                            strMessage += Me.Text
                            lblHeading.CssClass = "Normal"
                            lblMessage.CssClass = "Normal"
                            imgIcon.ImageUrl = "~/images/yellow-warning.gif"
                            imgIcon.Visible = True
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning.ToString())
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning.ToString())
                        Case UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo

                            strMessage += Me.Text
                            lblHeading.CssClass = "Normal"
                            lblMessage.CssClass = "Normal"
                            imgIcon.ImageUrl = "~/images/blue-info.gif"
                            imgIcon.Visible = True
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo.ToString())
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo.ToString())
                        Case UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError
                            strMessage += Me.Text
                            lblHeading.CssClass = "NormalRed"
                            lblMessage.CssClass = "Normal"
                            imgIcon.ImageUrl = "~/images/red-error.gif"
                            imgIcon.Visible = True
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError.ToString())
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError.ToString())
                    End Select
                End If
                lblMessage.Text = strMessage
                If Heading <> "" Then
                    lblHeading.Visible = True
                    lblHeading.Text = Heading + "<br/>"
                End If

            Catch exc As Exception    'Control failed to load
                ProcessModuleLoadException(Me, exc, False)
            End Try
        End Sub

    End Class

End Namespace
