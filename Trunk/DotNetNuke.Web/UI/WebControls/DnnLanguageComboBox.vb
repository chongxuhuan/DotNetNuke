'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports Telerik.Web.UI
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DotNetNuke.Services.Personalization

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnLanguageComboBox
        Inherits WebControl

#Region "Private Members"

        Private _AutoPostBack As Boolean = Null.NullBoolean
        Private _FlagImageUrlFormatString As String = "~/images/Flags/{0}.gif"
        Private _HideLanguagesList As New Dictionary(Of String, Locale)
        Private _IncludeNoneSpecified As Boolean = False
        Private _LanguagesListType As LanguagesListType = LanguagesListType.Enabled
        Private _PortalId As Integer
        Private _ShowFlag As Boolean = True
        Private _ShowModeButtons As Boolean = True
        Private _ViewTypePersonalizationKey As String = "ViewType" & PortalId.ToString

        Private _ModeRadioButtonList As RadioButtonList
        Private _NativeCombo As DnnComboBox
        Private _EnglishCombo As DnnComboBox



#End Region

#Region "Public Events"

        Public Event ItemChanged As EventHandler
        Public Event ModeChanged As EventHandler

#End Region

#Region "Public Properties"

        Private ReadOnly Property DisplayMode() As String
            Get
                Dim _DisplayMode As String = Convert.ToString(Personalization.GetProfile("LanguageDisplayMode", _ViewTypePersonalizationKey))
                If _DisplayMode = "" Then _DisplayMode = "NATIVE"
                Return _DisplayMode
            End Get
        End Property

        Public Property FlagImageUrlFormatString() As String
            Get
                Return _FlagImageUrlFormatString
            End Get
            Set(ByVal value As String)
                _FlagImageUrlFormatString = value
            End Set
        End Property

        Public Property HideLanguagesList() As Dictionary(Of String, Locale)
            Get
                Return _HideLanguagesList
            End Get
            Set(ByVal value As Dictionary(Of String, Locale))
                _HideLanguagesList = value
            End Set
        End Property

        Public Property IncludeNoneSpecified() As Boolean
            Get
                Return _IncludeNoneSpecified
            End Get
            Set(ByVal value As Boolean)
                _IncludeNoneSpecified = value
            End Set
        End Property

        Public Property LanguagesListType() As LanguagesListType
            Get
                Return _LanguagesListType
            End Get
            Set(ByVal value As LanguagesListType)
                _LanguagesListType = value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property

        Public ReadOnly Property SelectedValue() As String
            Get
                Dim _SelectedValue As String = If(DisplayMode.ToUpperInvariant = "NATIVE", _NativeCombo.SelectedValue, _EnglishCombo.SelectedValue)
                If _SelectedValue = "None" Then
                    _SelectedValue = Null.NullString
                End If
                Return _SelectedValue
            End Get
        End Property

        Public Property ShowFlag() As Boolean
            Get
                Return _ShowFlag
            End Get
            Set(ByVal value As Boolean)
                _ShowFlag = value
            End Set
        End Property

        Public Property ShowModeButtons() As Boolean
            Get
                Return _ShowModeButtons
            End Get
            Set(ByVal value As Boolean)
                _ShowModeButtons = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the List Auto Posts Back
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Property AutoPostBack() As Boolean
            Get
                Return _AutoPostBack
            End Get
            Set(ByVal Value As Boolean)
                _AutoPostBack = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Overloads Sub DataBind(ByVal refresh As Boolean)
            If refresh Then
                Dim cultures As List(Of CultureInfo)
                Select Case LanguagesListType
                    Case LanguagesListType.Supported
                        cultures = LocaleController.Instance().GetCultures(LocaleController.Instance().GetLocales(Null.NullInteger))
                    Case LanguagesListType.Enabled
                        cultures = LocaleController.Instance().GetCultures(LocaleController.Instance().GetLocales(PortalId))
                    Case Else
                        cultures = New List(Of CultureInfo)(CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                End Select

                For Each lang In HideLanguagesList
                    Dim cultureCode As String = lang.Value.Code
                    Dim culture As CultureInfo = cultures.Where(Function(c As CultureInfo) c.Name = cultureCode).SingleOrDefault
                    If culture IsNot Nothing Then
                        cultures.Remove(culture)
                    End If
                Next

                _NativeCombo.DataSource = cultures.OrderBy(Function(c As CultureInfo) c.NativeName)
                _EnglishCombo.DataSource = cultures.OrderBy(Function(c As CultureInfo) c.EnglishName)
            End If


            _NativeCombo.DataBind()
            _EnglishCombo.DataBind()

            If IncludeNoneSpecified AndAlso refresh Then
                _EnglishCombo.Items.Insert(0, New RadComboBoxItem(Localization.GetString("System_Default", Localization.SharedResourceFile), "None"))
                _NativeCombo.Items.Insert(0, New RadComboBoxItem(Localization.GetString("System_Default", Localization.SharedResourceFile), "None"))
            End If
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            _NativeCombo = New DnnComboBox()
            _NativeCombo.DataValueField = "Name"
            _NativeCombo.DataTextField = "NativeName"
            AddHandler _NativeCombo.SelectedIndexChanged, AddressOf Item_Changed
            Controls.Add(_NativeCombo)

            _EnglishCombo = New DnnComboBox()
            _EnglishCombo.DataValueField = "Name"
            _EnglishCombo.DataTextField = "EnglishName"
            AddHandler _EnglishCombo.SelectedIndexChanged, AddressOf Item_Changed
            Controls.Add(_EnglishCombo)

            _ModeRadioButtonList = New RadioButtonList()
            _ModeRadioButtonList.AutoPostBack = True
            _ModeRadioButtonList.RepeatDirection = RepeatDirection.Horizontal
            _ModeRadioButtonList.Items.Add(New ListItem(Localization.GetString("NativeName", Localization.GlobalResourceFile), "NATIVE"))
            _ModeRadioButtonList.Items.Add(New ListItem(Localization.GetString("EnglishName", Localization.GlobalResourceFile), "ENGLISH"))
            AddHandler _ModeRadioButtonList.SelectedIndexChanged, AddressOf Mode_Changed
            Controls.Add(_ModeRadioButtonList)
        End Sub

        Protected Overridable Sub OnItemChanged()
            RaiseEvent ItemChanged(Me, New EventArgs)
        End Sub

        Protected Sub OnModeChanged(ByVal e As EventArgs)
            RaiseEvent ModeChanged(Me, e)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            _ModeRadioButtonList.Items.FindByValue(DisplayMode).Selected = True

            For Each item As RadComboBoxItem In _EnglishCombo.Items
                item.ImageUrl = String.Format(FlagImageUrlFormatString, item.Value)
            Next
            For Each item As RadComboBoxItem In _NativeCombo.Items
                item.ImageUrl = String.Format(FlagImageUrlFormatString, item.Value)
            Next

            _EnglishCombo.AutoPostBack = AutoPostBack
            _EnglishCombo.Visible = (DisplayMode.ToUpperInvariant = "ENGLISH")

            _NativeCombo.AutoPostBack = AutoPostBack
            _NativeCombo.Visible = (DisplayMode.ToUpperInvariant = "NATIVE")

            _ModeRadioButtonList.Visible = ShowModeButtons

            _EnglishCombo.Width = Width
            _NativeCombo.Width = Width

            MyBase.OnPreRender(e)
        End Sub

#End Region

#Region "Public Methods"

        Public Sub SetLanguage(ByVal code As String)
            If String.IsNullOrEmpty(code) Then
                _NativeCombo.SelectedIndex = _NativeCombo.FindItemIndexByValue("None")
                _EnglishCombo.SelectedIndex = _EnglishCombo.FindItemIndexByValue("None")
            Else
                _NativeCombo.SelectedIndex = _NativeCombo.FindItemIndexByValue(code)
                _EnglishCombo.SelectedIndex = _EnglishCombo.FindItemIndexByValue(code)
            End If
        End Sub

        Public Overrides Sub DataBind()
            DataBind(Not Page.IsPostBack)
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Mode_Changed(ByVal sender As Object, ByVal e As EventArgs)
            Personalization.SetProfile("LanguageDisplayMode", _ViewTypePersonalizationKey, _ModeRadioButtonList.SelectedValue)

            'Resort
            DataBind(True)

            OnModeChanged(EventArgs.Empty)
        End Sub

        Private Sub Item_Changed(ByVal sender As Object, ByVal e As EventArgs)
            OnItemChanged()
        End Sub

#End Region


    End Class

End Namespace
