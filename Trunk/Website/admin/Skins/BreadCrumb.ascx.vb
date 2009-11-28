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
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.UI.Skins.Controls
    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------


    Partial  Class BreadCrumb

        Inherits UI.Skins.SkinObjectBase

        ' private members
        Private _separator As String
        Private _cssClass As String
        Private _rootLevel As String
        Private _useTitle As Boolean = False

        Const MyFileName As String = "Breadcrumb.ascx"

        ' protected controls

#Region "Public Members"
        Public Property Separator() As String
            Get
                Return _separator
            End Get
            Set(ByVal Value As String)
                _separator = Value
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

        Public Property RootLevel() As String
            Get
                Return _rootLevel
            End Get
            Set(ByVal Value As String)
                _rootLevel = Value
            End Set
        End Property

        Public Property UseTitle() As Boolean
            Get
                Return _useTitle
            End Get
            Set(ByVal Value As Boolean)
                _useTitle = Value
            End Set
        End Property
#End Region

#Region " Web Form Designer Generated Code "


        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' public attributes
            Dim strSeparator As String
            If Separator <> "" Then
                If Separator.IndexOf("src=") <> -1 Then
                    Separator = Replace(Separator, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                End If
                strSeparator = Separator
            Else
                strSeparator = "&nbsp;<img alt=""*"" src=""" & Common.Globals.ApplicationPath & "/images/breadcrumb.gif"">&nbsp;"
            End If

            Dim strCssClass As String
            If CssClass <> "" Then
                strCssClass = CssClass
            Else
                strCssClass = "SkinObject"
            End If

            Dim intRootLevel As Integer
            If RootLevel <> "" Then
                intRootLevel = Integer.Parse(RootLevel)
            Else
                intRootLevel = 1
            End If

            Dim strBreadCrumbs As String = ""

            If intRootLevel = -1 Then
                strBreadCrumbs += String.Format(Services.Localization.Localization.GetString("Root", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request), strCssClass)
                strBreadCrumbs += strSeparator
                intRootLevel = 0
            End If

            ' process bread crumbs
            Dim intTab As Integer
            For intTab = intRootLevel To PortalSettings.ActiveTab.BreadCrumbs.Count - 1
                If intTab <> intRootLevel Then
                    strBreadCrumbs += strSeparator
                End If
                Dim objTab As TabInfo = CType(PortalSettings.ActiveTab.BreadCrumbs(intTab), TabInfo)
                Dim strLabel As String = objTab.LocalizedTabName
                If UseTitle And objTab.Title <> "" Then
                    strLabel = objTab.Title
                End If
                If objTab.DisableLink Then
                    strBreadCrumbs += "<span class=""" & strCssClass & """>" & strLabel & "</span>"
                Else
                    strBreadCrumbs += "<a href=""" & objTab.FullUrl & """ class=""" & strCssClass & """>" & strLabel & "</a>"
                End If
            Next
            lblBreadCrumb.Text = Convert.ToString(strBreadCrumbs)

        End Sub

    End Class

End Namespace
