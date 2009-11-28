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
Imports System.Collections.Specialized
Imports System.IO

Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Tokens

Namespace DotNetNuke.UI.Skins.Controls

    ''' <summary>
    ''' The Language skinobject allows the visitor to select the language of the page
    ''' </summary>
    ''' <remarks></remarks>
    Partial Class Language
        Inherits UI.Skins.SkinObjectBase

#Region " private members "
        Private _cssClass As String
        Private _itemTemplate As String
        Private _headerTemplate As String
        Private _footerTemplate As String
        Private _separatorTemplate As String
        Private _alternateTemplate As String
        Private _commonHeaderTemplate As String
        Private _commonFooterTemplate As String
        Private _showMenu As Boolean = True
        Private _showLinks As Boolean = False
        Private _localResourceFile As String
        Private _localTokenReplace As LanguageTokenReplace
#End Region

#Region " constants "
        Const MyFileName As String = "Language.ascx"
#End Region

#Region " Public Properties"

        Public Property AlternateTemplate() As String
            Get
                If String.IsNullOrEmpty(_alternateTemplate) Then
                    _alternateTemplate = Localization.GetString("AlternateTemplate.Default", LocalResourceFile)
                End If
                Return _alternateTemplate
            End Get
            Set(ByVal value As String)
                _alternateTemplate = value
            End Set
        End Property

        Public Property CommonFooterTemplate() As String
            Get
                If String.IsNullOrEmpty(_commonFooterTemplate) Then
                    _commonFooterTemplate = Localization.GetString("CommonFooterTemplate.Default", LocalResourceFile)
                End If
                Return _commonFooterTemplate
            End Get
            Set(ByVal value As String)
                _commonFooterTemplate = value
            End Set
        End Property

        Public Property CommonHeaderTemplate() As String
            Get
                If String.IsNullOrEmpty(_commonHeaderTemplate) Then
                    _commonHeaderTemplate = Localization.GetString("CommonHeaderTemplate.Default", LocalResourceFile)
                End If
                Return _commonHeaderTemplate
            End Get
            Set(ByVal value As String)
                _commonHeaderTemplate = value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

        Public Property FooterTemplate() As String
            Get
                If String.IsNullOrEmpty(_footerTemplate) Then
                    _footerTemplate = Localization.GetString("FooterTemplate.Default", LocalResourceFile)
                End If
                Return _footerTemplate
            End Get
            Set(ByVal value As String)
                _footerTemplate = value
            End Set
        End Property

        Public Property HeaderTemplate() As String
            Get
                If String.IsNullOrEmpty(_headerTemplate) Then
                    _headerTemplate = Localization.GetString("HeaderTemplate.Default", LocalResourceFile)
                End If
                Return _headerTemplate
            End Get
            Set(ByVal value As String)
                _headerTemplate = value
            End Set
        End Property

        Public Property ItemTemplate() As String
            Get
                If String.IsNullOrEmpty(_itemTemplate) Then
                    _itemTemplate = Localization.GetString("ItemTemplate.Default", LocalResourceFile)
                End If
                Return _itemTemplate
            End Get
            Set(ByVal value As String)
                _itemTemplate = value
            End Set
        End Property

        Public Property SeparatorTemplate() As String
            Get
                If String.IsNullOrEmpty(_separatorTemplate) Then
                    _separatorTemplate = Localization.GetString("SeparatorTemplate.Default", LocalResourceFile)
                End If
                Return _separatorTemplate
            End Get
            Set(ByVal value As String)
                _separatorTemplate = value
            End Set
        End Property

        Public Property ShowLinks() As Boolean
            Get
                Return _showLinks
            End Get
            Set(ByVal value As Boolean)
                _showLinks = value
            End Set
        End Property

        Public Property ShowMenu() As Boolean
            Get
                If (_showMenu = False) AndAlso (ShowLinks = False) Then
                    ' this is to make sure that at least one type of selector will be visible if multiple languages are enabled
                    _showMenu = True
                End If
                Return _showMenu
            End Get
            Set(ByVal value As Boolean)
                _showMenu = value
            End Set
        End Property

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property CurrentCulture() As String
            Get
                Return CultureInfo.CurrentCulture.ToString()
            End Get
        End Property

        Protected ReadOnly Property LocalResourceFile() As String
            Get
                If String.IsNullOrEmpty(_localResourceFile) Then
                    _localResourceFile = Localization.GetResourceFile(Me, MyFileName)
                End If
                Return _localResourceFile
            End Get
        End Property

        Protected ReadOnly Property LocalTokenReplace() As LanguageTokenReplace
            Get
                If _localTokenReplace Is Nothing Then
                    _localTokenReplace = New LanguageTokenReplace()
                    _localTokenReplace.resourceFile = LocalResourceFile
                End If
                Return _localTokenReplace
            End Get
        End Property

#End Region

#Region " Private Methods "

        Private Function parseTemplate(ByVal template As String, ByVal locale As String) As String
            Dim strReturnValue As String = template

            Try
                If Not String.IsNullOrEmpty(locale) Then
                    'for non data items use locale
                    LocalTokenReplace.Language = locale
                Else
                    'for non data items use page culture
                    LocalTokenReplace.Language = currentCulture
                End If

                'perform token replacements
                strReturnValue = LocalTokenReplace.ReplaceEnvironmentTokens(strReturnValue)
            Catch ex As Exception
                ProcessPageLoadException(ex, Request.RawUrl)
            End Try

            Return strReturnValue

        End Function

        Private Sub handleCommonTemplates()
            If String.IsNullOrEmpty(CommonHeaderTemplate) Then
                litCommonHeaderTemplate.Visible = False
            Else
                litCommonHeaderTemplate.Text = parseTemplate(CommonHeaderTemplate, currentCulture)
            End If

            If String.IsNullOrEmpty(CommonFooterTemplate) Then
                litCommonFooterTemplate.Visible = False
            Else
                litCommonFooterTemplate.Text = parseTemplate(CommonFooterTemplate, currentCulture)
            End If
        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If ShowLinks Then
                    If Localization.GetLocales(PortalSettings.PortalId).Count > 1 Then
                        rptLanguages.DataSource = Localization.GetLocales(PortalSettings.PortalId).Values
                        rptLanguages.DataBind()
                    Else
                        rptLanguages.Visible = False
                    End If

                End If

                If Not Page.IsPostBack Then

                    If ShowMenu Then
                        ' public attributes
                        If CssClass <> "" Then
                            selectCulture.CssClass = CssClass
                        End If

                        Localization.LoadCultureDropDownList(selectCulture, CultureDropDownTypes.NativeName, CType(Page, PageBase).PageCulture.Name)

                        'only show language selector if more than one language
                        If Not selectCulture.Items.Count > 1 Then
                            selectCulture.Visible = False
                        End If
                    Else
                        selectCulture.Visible = False
                    End If

                    handleCommonTemplates()
                End If

            Catch ex As Exception
                ProcessPageLoadException(ex, Request.RawUrl)
            End Try
        End Sub

        Private Sub selectCulture_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles selectCulture.SelectedIndexChanged

            ' Store selected language in cookie
            ' Localization.SetLanguage(selectCulture.SelectedItem.Value)

            'Redirect to same page to update all controls for newly selected culture
            LocalTokenReplace.Language = selectCulture.SelectedItem.Value
            Response.Redirect(LocalTokenReplace.ReplaceEnvironmentTokens("[URL]"))
        End Sub

        ''' <summary>
        ''' Binds data to repeater. a template is used to render the items
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <history>
        '''      [erikvb]  20070814	  added
        ''' </history>
        Protected Sub rtpLanguages_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLanguages.ItemDataBound
            Try
                Dim litTemplate As Literal = e.Item.FindControl("litItemTemplate")
                If Not litTemplate Is Nothing Then

                    'load proper template for this Item
                    Dim strTemplate As String = ""
                    Select Case e.Item.ItemType
                        Case ListItemType.Item
                            strTemplate = ItemTemplate
                        Case ListItemType.AlternatingItem
                            If Not String.IsNullOrEmpty(Me.AlternateTemplate) Then
                                strTemplate = AlternateTemplate
                            Else
                                strTemplate = ItemTemplate
                            End If
                        Case ListItemType.Header
                            strTemplate = HeaderTemplate
                        Case ListItemType.Footer
                            strTemplate = FooterTemplate
                        Case ListItemType.Separator
                            strTemplate = SeparatorTemplate
                    End Select

                    If String.IsNullOrEmpty(strTemplate) Then
                        litTemplate.Visible = False
                    Else
                        If e.Item.DataItem IsNot Nothing Then
                            Dim locale As Locale = TryCast(e.Item.DataItem, Locale)
                            If locale IsNot Nothing Then
                                litTemplate.Text = parseTemplate(strTemplate, locale.Code)
                            End If
                        Else
                            'for non data items use page culture
                            litTemplate.Text = parseTemplate(strTemplate, currentCulture)
                        End If
                    End If

                End If
            Catch ex As Exception
                ProcessPageLoadException(ex, Request.RawUrl)
            End Try

        End Sub
#End Region

    End Class

End Namespace
