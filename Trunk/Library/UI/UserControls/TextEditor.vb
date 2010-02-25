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
Imports DotNetNuke.Services.Personalization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.HTMLEditorProvider

Namespace DotNetNuke.UI.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Class:  TextEditor
    ''' Project: DotNetNuke
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' TextEditor is a user control that provides a wrapper for the HtmlEditor providers
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	12/13/2004	Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ValidationPropertyAttribute("Text")> Public Class TextEditor
        Inherits System.Web.UI.UserControl

#Region "Controls"

        Protected WithEvents tblTextEditor As System.Web.UI.HtmlControls.HtmlTable
        Protected WithEvents celTextEditor As System.Web.UI.HtmlControls.HtmlTableCell
        Protected WithEvents optView As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents txtDesktopHTML As System.Web.UI.WebControls.TextBox
        Protected WithEvents optRender As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents pnlBasicTextBox As System.Web.UI.WebControls.Panel
        Protected WithEvents pnlBasicRender As System.Web.UI.WebControls.Panel
        Protected WithEvents plcEditor As System.Web.UI.WebControls.PlaceHolder
        Protected WithEvents pnlRichTextBox As System.Web.UI.WebControls.Panel
        Protected WithEvents trView As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents plView As DotNetNuke.UI.UserControls.LabelControl
#End Region

#Region "Private Members"

        Private _ChooseMode As Boolean = True
        Private _ChooseRender As Boolean = True
        Private _HtmlEncode As Boolean = True
        Private _Width As System.Web.UI.WebControls.Unit
        Private _Height As System.Web.UI.WebControls.Unit
        Private RichTextEditor As HtmlEditorProvider

        Private MyFileName As String = "TextEditor.ascx"

#End Region

#Region "Properties"

        'Enables/Disables the option to allow the user to select between Rich/Basic Mode
        'Default is true.
        Public Property ChooseMode() As Boolean
            Get
                ChooseMode = _ChooseMode
            End Get
            Set(ByVal Value As Boolean)
                _ChooseMode = Value
            End Set
        End Property

        'Determines wether or not the Text/Html button is rendered for Basic mode
        'Default is True
        Public Property ChooseRender() As Boolean
            Get
                ChooseRender = _ChooseRender
            End Get
            Set(ByVal Value As Boolean)
                _ChooseRender = Value
            End Set
        End Property

        'Gets/Sets the Default mode of the control, either "RICH" or "BASIC"
        'Defaults to Rich
        Public Property DefaultMode() As String
            Get
                If CType(Me.ViewState.Item("DefaultMode"), String) = "" Then
                    Return "RICH"     ' default to rich
                Else
                    Return CType(Me.ViewState.Item("DefaultMode"), String)
                End If
            End Get
            Set(ByVal Value As String)
                If Value.ToUpper <> "BASIC" Then
                    Me.ViewState.Item("DefaultMode") = "RICH"
                Else
                    Me.ViewState.Item("DefaultMode") = "BASIC"
                End If
            End Set
        End Property

        'Gets/Sets the Height of the control
        Public Property Height() As System.Web.UI.WebControls.Unit
            Get
                Return _Height
            End Get
            Set(ByVal Value As System.Web.UI.WebControls.Unit)
                _Height = Value
            End Set
        End Property

        'Turns on HtmlEncoding of text.  If this option is on the control will assume
        'it is being passed encoded text and will decode.
        Public Property HtmlEncode() As Boolean
            Get
                HtmlEncode = _HtmlEncode
            End Get
            Set(ByVal Value As Boolean)
                _HtmlEncode = Value
            End Set
        End Property

        'The current mode of the control "RICH",  "BASIC"
        Public Property Mode() As String
            Get
                Dim strMode As String = ""
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

                'Check if Personal Preference is set
                If objUserInfo.UserID >= 0 Then
                    If Not Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor") Is Nothing Then
                        strMode = CType(Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor"), String)
                    End If
                End If

                'If no Preference Check if Viewstate has been saved
                If strMode Is Nothing Or strMode = "" Then
                    If CType(Me.ViewState.Item("DesktopMode"), String) <> "" Then
                        strMode = CType(Me.ViewState.Item("DesktopMode"), String)
                    End If
                End If

                'Finally if still no value Use default
                If strMode Is Nothing Or strMode = "" Then
                    strMode = DefaultMode
                End If

                Return strMode
            End Get
            Set(ByVal Value As String)
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

                If Value.ToUpper <> "BASIC" Then
                    Me.ViewState.Item("DesktopMode") = "RICH"

                    If objUserInfo.UserID >= 0 Then
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "RICH")
                    End If
                Else
                    Me.ViewState.Item("DesktopMode") = "BASIC"

                    If objUserInfo.UserID >= 0 Then
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "BASIC")
                    End If
                End If
            End Set
        End Property

        'Gets/Sets the Text of the control
        Public Property Text() As String
            Get
                If optView.SelectedItem.Value = "BASIC" Then
                    Select Case optRender.SelectedItem.Value
                        Case "T"
                            Text = Encode(FormatHtml(txtDesktopHTML.Text))
                        Case "R"
                            Text = txtDesktopHTML.Text
                        Case Else
                            Text = Encode(txtDesktopHTML.Text)
                    End Select
                Else
                    Text = Encode(RichTextEditor.Text)
                End If
            End Get
            Set(ByVal Value As String)
                If Value <> "" Then
                    txtDesktopHTML.Text = Decode(FormatText(Value))
                    RichTextEditor.Text = Decode(Value)
                End If
            End Set
        End Property

        'Sets the render mode for Basic mode.  {Raw | HTML | Text}
        Public Property TextRenderMode() As String
            Get
                TextRenderMode = CType(Me.ViewState.Item("textrender"), String)
            End Get
            Set(ByVal Value As String)
                Dim strMode As String

                strMode = Value.ToUpper.Substring(0, 1)
                If strMode <> "R" And strMode <> "H" And strMode <> "T" Then
                    strMode = "H"
                End If

                Me.ViewState.Item("textrender") = strMode
            End Set
        End Property

        'Gets/Sets the Width of the control
        Public Property Width() As System.Web.UI.WebControls.Unit
            Get
                Return _Width
            End Get
            Set(ByVal Value As System.Web.UI.WebControls.Unit)
                _Width = Value
            End Set
        End Property

        'Allows public access ot the HtmlEditorProvider
        Public ReadOnly Property RichText() As HtmlEditorProvider
            Get
                Return RichTextEditor
            End Get
        End Property

        Public ReadOnly Property LocalResourceFile() As String
            Get
                Return Me.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/" & MyFileName
            End Get
        End Property
#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Decodes the html
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strHtml">Html to decode</param>
        ''' <returns>The decoded html</returns>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function Decode(ByVal strHtml As String) As String
            If Me.HtmlEncode Then
                Decode = Server.HtmlDecode(strHtml)
            Else
                Decode = strHtml
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Encodes the html
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strHtml">Html to encode</param>
        ''' <returns>The encoded html</returns>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function Encode(ByVal strHtml As String) As String
            If Me.HtmlEncode Then
                Encode = Server.HtmlEncode(strHtml)
            Else
                Encode = strHtml
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Formats String as Html by replacing linefeeds by <br/>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strText">Text to format</param>
        ''' <returns>The formatted html</returns>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FormatHtml(ByVal strText As String) As String

            Dim strHtml As String = strText
            Try
                If strHtml <> "" Then
                    strHtml = Replace(strHtml, Chr(13), "")
                    strHtml = Replace(strHtml, ControlChars.Lf, "<br />")
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

            Return strHtml
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Formats Html as text by removing <br/> tags and replacing by linefeeds
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strHtml">Html to format</param>
        ''' <returns>The formatted text</returns>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented and modified to use HtmlUtils methods
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FormatText(ByVal strHtml As String) As String

            Dim strText As String = strHtml
            Try
                If strText <> "" Then
                    'First remove white space (html does not render white space anyway and it screws up the conversion to text)
                    'Replace it by a single space
                    strText = HtmlUtils.StripWhiteSpace(strText, True)

                    'Replace all variants of <br> by Linefeeds
                    strText = HtmlUtils.FormatText(strText, False)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

            Return strText
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the radio button lists
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PopulateLists()
            If optRender.Items.Count = 0 Then
                optRender.Items.Add(New ListItem(Services.Localization.Localization.GetString("Text", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "T"))
                optRender.Items.Add(New ListItem(Services.Localization.Localization.GetString("Html", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "H"))
                optRender.Items.Add(New ListItem(Services.Localization.Localization.GetString("Raw", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "R"))
            End If
            If optView.Items.Count = 0 Then
                optView.Items.Add(New ListItem(Services.Localization.Localization.GetString("BasicTextBox", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "BASIC"))
                optView.Items.Add(New ListItem(Services.Localization.Localization.GetString("RichTextBox", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "RICH"))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the Mode displayed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/10/2005	created (extracted from Page_load)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetPanels()
            If optView.SelectedIndex <> -1 Then
                Mode = optView.SelectedItem.Value
            End If
            If Mode <> "" Then
                optView.Items.FindByValue(Mode).Selected = True
            Else
                optView.SelectedIndex = 0
            End If

            'Set the text render mode for basic mode
            If optRender.SelectedIndex <> -1 Then
                TextRenderMode = optRender.SelectedItem.Value
            End If
            If TextRenderMode <> "" Then
                optRender.Items.FindByValue(TextRenderMode).Selected = True
            Else
                optRender.SelectedIndex = 0
            End If

            If optView.SelectedItem.Value = "BASIC" Then
                pnlBasicTextBox.Visible = True
                pnlRichTextBox.Visible = False
            Else
                pnlBasicTextBox.Visible = False
                pnlRichTextBox.Visible = True
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            RichTextEditor = HtmlEditorProvider.Instance
            RichTextEditor.ControlID = Me.ID
            RichTextEditor.Initialize()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'Populate Radio Button Lists
                PopulateLists()

                'Get the current user
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

                'Set the width and height of the controls
                RichTextEditor.Width = Width
                RichTextEditor.Height = Height
                txtDesktopHTML.Height = Height
                txtDesktopHTML.Width = Width
                tblTextEditor.Width = Width.ToString()
                celTextEditor.Width = Width.ToString()

                'Optionally display the radio button lists
                If Not ChooseMode Then
                    trView.Visible = False
                End If
                If Not ChooseRender Then
                    pnlBasicRender.Visible = False
                End If

                'Load the editor
                plcEditor.Controls.Add(RichTextEditor.HtmlEditorControl)

                SetPanels()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' optRender_SelectedIndexChanged runs when Basic Text Box mode is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub optRender_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optRender.SelectedIndexChanged

            If optRender.SelectedIndex <> -1 Then
                TextRenderMode = optRender.SelectedItem.Value
            End If

            If Mode = "BASIC" Then
                If TextRenderMode = "H" Then
                    txtDesktopHTML.Text = FormatHtml(txtDesktopHTML.Text)
                Else
                    txtDesktopHTML.Text = FormatText(txtDesktopHTML.Text)
                End If
            End If
            SetPanels()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' optView_SelectedIndexChanged runs when Editor Mode is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub optView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optView.SelectedIndexChanged

            If optView.SelectedIndex <> -1 Then
                Mode = optView.SelectedItem.Value
            End If

            If Mode = "BASIC" Then
                If TextRenderMode = "T" Then
                    txtDesktopHTML.Text = FormatText(RichTextEditor.Text)
                Else
                    txtDesktopHTML.Text = RichTextEditor.Text
                End If
            Else
                If TextRenderMode = "T" Then
                    RichTextEditor.Text = FormatHtml(txtDesktopHTML.Text)
                Else
                    RichTextEditor.Text = txtDesktopHTML.Text
                End If
            End If
            SetPanels()
        End Sub

#End Region

    End Class

End Namespace
