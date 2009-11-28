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
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Collections
Imports System.Data

Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls
Imports System.Globalization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      LanguageSelector
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Language Selector control
    ''' </summary>
    ''' <history>
    '''     [sleupold]	2007-11-10 created
    '''     [sleupold]  2007-12-08 Support for Languages ("de", "en") and Locales ("de-DE", "en-US") added
    ''' </history>

    Public Class LanguageSelector
        Inherits Control
        Implements INamingContainer

        Private pnlControl As Panel

#Region "Enums"
        ''' <summary>
        ''' Language Selection mode, offered to the user: single select or multiple select.
        ''' </summary>
        Public Enum LanguageSelectionMode
            Multiple = 1
            [Single] = 2
        End Enum

        ''' <summary>
        ''' Selection object: Language ("de", "en") or Locale ("de-DE", "en-US")
        ''' </summary>
        Public Enum LanguageSelectionObject
            NeutralCulture = 1
            SpecificCulture = 2
        End Enum

        ''' <summary>
        ''' orientation, how elements (text or flags) are arranged
        ''' </summary>
        Public Enum LanguageListDirection
            Horizontal = 1
            Vertical = 2
        End Enum

        ''' <summary>
        ''' display style for each item: Text, Flag, Both
        ''' </summary>
        Public Enum LanguageItemStyle
            FlagOnly = 1
            FlagAndCaption = 2
            CaptionOnly = 3
        End Enum
#End Region

#Region "Public Properties"

        ''' <summary>
        ''' Gets or sets selection mode (single, multiple)
        ''' </summary>
        Public Property SelectionMode() As LanguageSelectionMode
            Get
                If ViewState("SelectionMode") Is Nothing Then
                    Return LanguageSelectionMode.Single
                Else
                    Return CType(ViewState("SelectionMode"), LanguageSelectionMode)
                End If
            End Get
            Set(ByVal value As LanguageSelectionMode)
                If SelectionMode <> value Then
                    ViewState("SelectionMode") = value
                    If Me.Controls.Count > 0 Then CreateChildControls() 'Recreate if already created
                End If
            End Set
        End Property


        '' <summary>
        '' Gets or sets the type of objects to be selectable: NeutralCulture ("de") or SpecificCulture ("de-DE")
        '' </summary>
        Public Property SelectionObject() As LanguageSelectionObject
            Get
                If viewstate("SelectionObject") Is Nothing Then
                    Return LanguageSelectionObject.SpecificCulture
                Else
                    Return CType(viewstate("SelectionObject"), LanguageSelectionObject)
                End If
            End Get
            Set(ByVal Value As LanguageSelectionObject)
                If SelectionMode <> Value Then
                    viewstate("SelectionObject") = Value
                    If Me.Controls.Count > 0 Then CreateChildControls() 'Recreate if already created 
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the style of the language items
        ''' </summary>
        Public Property ItemStyle() As LanguageItemStyle
            Get
                If ViewState("ItemStyle") Is Nothing Then
                    Return LanguageItemStyle.FlagAndCaption
                Else
                    Return CType(ViewState("ItemStyle"), LanguageItemStyle)
                End If
            End Get
            Set(ByVal value As LanguageItemStyle)
                If ItemStyle <> value Then
                    ViewState("ItemStyle") = value
                    If Me.Controls.Count > 0 Then CreateChildControls() 'Recreate if already created
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the direction of the language list
        ''' </summary>
        Public Property ListDirection() As LanguageListDirection
            Get
                If ViewState("ListDirection") Is Nothing Then
                    Return LanguageListDirection.Vertical
                Else
                    Return CType(ViewState("ListDirection"), LanguageListDirection)
                End If
            End Get
            Set(ByVal value As LanguageListDirection)
                If ListDirection <> value Then
                    ViewState("ListDirection") = value
                    If Me.Controls.Count > 0 Then CreateChildControls() 'Recreate if already created
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the list of selected languages
        ''' </summary>
        Public Property SelectedLanguages() As String()
            Get
                EnsureChildControls()
                Dim a As New ArrayList
                If GetCultures(SelectionObject = LanguageSelectionObject.SpecificCulture).Length < 2 Then
                    'return single language
                    Dim _Settings As PortalSettings = PortalController.GetCurrentPortalSettings
                    For Each strLocale As String In Localization.GetLocales(_Settings.PortalID).Keys
                        a.Add(strLocale)
                    Next
                Else
                    'create list of selected languages
                    For Each c As CultureInfo In GetCultures(SelectionObject = LanguageSelectionObject.SpecificCulture)
                        If SelectionMode = LanguageSelectionMode.Single Then
                            If CType(pnlControl.FindControl("opt" & c.Name), RadioButton).Checked Then a.Add(c.Name)
                        Else
                            If CType(pnlControl.FindControl("chk" & c.Name), CheckBox).Checked Then a.Add(c.Name)
                        End If
                    Next
                End If
                Return CType(a.ToArray(GetType(String)), String())
            End Get
            Set(ByVal value As String())
                EnsureChildControls()
                If SelectionMode = LanguageSelectionMode.Single And value.Length > 1 Then Throw New ArgumentException("Selection mode 'single' cannot have more than one selected item.")
                For Each c As CultureInfo In GetCultures(SelectionObject = LanguageSelectionObject.SpecificCulture)
                    If SelectionMode = LanguageSelectionMode.Single Then
                        CType(pnlControl.FindControl("opt" & c.Name), RadioButton).Checked = False
                    Else
                        CType(pnlControl.FindControl("chk" & c.Name), CheckBox).Checked = False
                    End If
                Next
                For Each strLocale As String In value
                    If SelectionMode = LanguageSelectionMode.Single Then
                        Dim ctl As Control = pnlControl.FindControl("opt" & strLocale)
                        If Not ctl Is Nothing Then CType(ctl, RadioButton).Checked = True
                    Else
                        Dim ctl As Control = pnlControl.FindControl("chk" & strLocale)
                        If Not ctl Is Nothing Then CType(ctl, CheckBox).Checked = True
                    End If
                Next
            End Set
        End Property
#End Region

#Region "Protected Methods"

        ''' <summary>
        ''' Create Child Controls
        ''' </summary>
        Protected Overrides Sub CreateChildControls()
            Me.Controls.Clear()
            pnlControl = New Panel
            Me.Controls.Add(pnlControl)

            For Each c As CultureInfo In GetCultures(SelectionObject = LanguageSelectionObject.SpecificCulture)
                Dim lblLocale As New HtmlControls.HtmlGenericControl("label")
                If SelectionMode = LanguageSelectionMode.Single Then
                    Dim optLocale As New RadioButton
                    optLocale.ID = "opt" & c.Name
                    optLocale.GroupName = pnlControl.ID & "_Locale"
                    If c.Name = Localization.SystemLocale Then optLocale.Checked = True
                    pnlControl.Controls.Add(optLocale)
                    lblLocale.Attributes("for") = optLocale.ClientID
                Else
                    Dim chkLocale As New CheckBox
                    chkLocale.ID = "chk" & c.Name
                    pnlControl.Controls.Add(chkLocale)
                    lblLocale.Attributes("for") = chkLocale.ClientID
                End If
                pnlControl.Controls.Add(lblLocale)
                If ItemStyle <> LanguageItemStyle.CaptionOnly Then
                    Dim imgLocale As New System.Web.UI.WebControls.Image
                    imgLocale.ImageUrl = ResolveUrl("~/images/Flags/" & c.Name & ".gif")
                    imgLocale.AlternateText = c.DisplayName
                    imgLocale.Style("vertical-align") = "middle"
                    lblLocale.Controls.Add(imgLocale)
                End If
                If ItemStyle <> LanguageItemStyle.FlagOnly Then
                    lblLocale.Attributes("class") = "Normal"
                    lblLocale.Controls.Add(New LiteralControl("&nbsp;" & c.DisplayName))
                End If
                If ListDirection = LanguageListDirection.Vertical Then
                    pnlControl.Controls.Add(New LiteralControl("<br />"))
                Else
                    pnlControl.Controls.Add(New LiteralControl(" "))
                End If
            Next
            'Hide if not more than one language
            If GetCultures(SelectionObject = LanguageSelectionObject.SpecificCulture).Length < 2 Then
                Me.Visible = False
            End If
        End Sub

#End Region

#Region " Private Methods "
        ''' <summary>
        ''' retrieve the cultures, currently supported by the portal
        ''' </summary>
        ''' <param name="specific">true: locales, false: neutral languages</param>
        ''' <returns>Array of cultures</returns>
        Private Function GetCultures(ByVal specific As Boolean) As System.Globalization.CultureInfo()
            Dim a As New ArrayList
            Dim _Settings As PortalSettings = PortalController.GetCurrentPortalSettings
            For Each strLocale As String In Localization.GetLocales(_Settings.PortalId).Keys
                Dim c As New System.Globalization.CultureInfo(strLocale)
                If specific Then
                    a.Add(c)
                Else
                    Dim p As System.Globalization.CultureInfo = c.Parent
                    If Not a.Contains(p) Then a.Add(p)
                End If
            Next
            Return CType(a.ToArray(GetType(CultureInfo)), System.Globalization.CultureInfo())
        End Function
#End Region

    End Class

End Namespace