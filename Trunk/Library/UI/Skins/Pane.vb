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

Imports System.Collections.Generic

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Skins
    ''' Class	 : Pane
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Pane class represents a Pane within the Skin
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	12/04/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Pane

#Region "Private Members"

        Private _containers As Dictionary(Of String, DotNetNuke.UI.Containers.Container)
        Private _name As String
        Private _paneControl As HtmlContainerControl
        Private Const c_PaneOutline As String = "paneOutline"

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new Pane object from the Control in the Skin
        ''' </summary>
        ''' <param name="pane">The HtmlContainerControl in the Skin.</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal pane As HtmlContainerControl)
            _paneControl = pane
            _name = pane.ID
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new Pane object from the Control in the Skin
        ''' </summary>
        ''' <param name="name">The name (ID) of the HtmlContainerControl</param>
        ''' <param name="pane">The HtmlContainerControl in the Skin.</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal name As String, ByVal pane As HtmlContainerControl)
            _paneControl = pane
            _name = name
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Containers.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Containers() As Dictionary(Of String, DotNetNuke.UI.Containers.Container)
            Get
                If _containers Is Nothing Then
                    _containers = New Dictionary(Of String, DotNetNuke.UI.Containers.Container)
                End If
                Return _containers
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the name (ID) of the Pane
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the HtmlContainerControl
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property PaneControl() As HtmlContainerControl
            Get
                Return _paneControl
            End Get
            Set(ByVal value As HtmlContainerControl)
                _paneControl = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PortalSettings of the Portal
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                PortalSettings = PortalController.GetCurrentPortalSettings
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadContainerByPath gets the Container from its Url(Path)
        ''' </summary>
        ''' <param name="ContainerPath">The Url to the Container control</param>
        ''' <returns>A Container</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadContainerByPath(ByVal ContainerPath As String) As DotNetNuke.UI.Containers.Container
            'sanity check to ensure skin not loaded accidentally
            If ContainerPath.ToLower.IndexOf("/skins/") <> -1 Or ContainerPath.ToLower.IndexOf("/skins\") <> -1 Or ContainerPath.ToLower.IndexOf("\skins\") <> -1 Or ContainerPath.ToLower.IndexOf("\skins/") <> -1 Then
                Throw New System.Exception
            End If
            Dim ctlContainer As DotNetNuke.UI.Containers.Container = Nothing

            Try
                Dim ContainerSrc As String = ContainerPath
                If ContainerPath.ToLower.IndexOf(Common.Globals.ApplicationPath.ToLower) <> -1 Then
                    ContainerPath = ContainerPath.Remove(0, Len(Common.Globals.ApplicationPath))
                End If
                ctlContainer = ControlUtilities.LoadControl(Of DotNetNuke.UI.Containers.Container)(PaneControl.Page, ContainerPath)
                ctlContainer.ContainerSrc = ContainerSrc
                ' call databind so that any server logic in the container is executed
                ctlContainer.DataBind()
            Catch exc As Exception
                ' could not load user control
                Dim lex As New ModuleLoadException(Skin.MODULELOAD_ERROR, exc)
                If TabPermissionController.CanAdminPage() Then
                    ' only display the error to administrators
                    PaneControl.Controls.Add(New ErrorContainer(PortalSettings, String.Format(Skin.CONTAINERLOAD_ERROR, ContainerPath), lex).Container)
                End If
                LogException(lex)
            End Try

            Return ctlContainer
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadModuleContainer gets the Container for a Module
        ''' </summary>
        ''' <param name="objModule">The Module</param>
        ''' <returns>A Container</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadModuleContainer(ByVal objModule As ModuleInfo) As DotNetNuke.UI.Containers.Container
            Dim ctlContainer As DotNetNuke.UI.Containers.Container = Nothing
            Dim containerSrc As String = Null.NullString
            Dim Request As HttpRequest = PaneControl.Page.Request

            ' container preview
            Dim PreviewModuleId As Integer = -1
            If Not Request.QueryString("ModuleId") Is Nothing Then
                PreviewModuleId = Integer.Parse(Request.QueryString("ModuleId"))
            End If
            If (Not Request.QueryString("ContainerSrc") Is Nothing) And (objModule.ModuleID = PreviewModuleId Or PreviewModuleId = -1) Then
                containerSrc = SkinController.FormatSkinSrc(QueryStringDecode(Request.QueryString("ContainerSrc")) & ".ascx", PortalSettings)
                ctlContainer = LoadContainerByPath(containerSrc)
            End If

            ' load user container ( based on cookie )
            If ctlContainer Is Nothing Then
                If Not Request.Cookies("_ContainerSrc" & PortalSettings.PortalId.ToString) Is Nothing Then
                    If Request.Cookies("_ContainerSrc" & PortalSettings.PortalId.ToString).Value <> "" Then
                        containerSrc = SkinController.FormatSkinSrc(Request.Cookies("_ContainerSrc" & PortalSettings.PortalId.ToString).Value & ".ascx", PortalSettings)
                        ctlContainer = LoadContainerByPath(containerSrc)
                    End If
                End If
            End If

            'Use the NoContainer container ?
            If ctlContainer Is Nothing Then
                ' if the module specifies that no container should be used
                If objModule.DisplayTitle = False Then
                    ' always display container if the current user is the administrator or the module is being used in an admin case
                    Dim blnDisplayTitle As Boolean = ModulePermissionController.CanEditModuleContent(objModule) OrElse IsAdminSkin()
                    ' unless the administrator is in view mode
                    If blnDisplayTitle = True Then
                        blnDisplayTitle = (PortalSettings.UserMode <> PortalSettings.Mode.View)
                    End If
                    If blnDisplayTitle = False Then
                        containerSrc = SkinController.FormatSkinSrc("[G]" & SkinController.RootContainer & "/_default/No Container.ascx", PortalSettings)
                        ctlContainer = LoadContainerByPath(containerSrc)
                    End If
                End If
            End If

            'Check Skin for Container
            If ctlContainer Is Nothing Then
                ' if this is not a container assigned to a module
                If objModule.ContainerSrc = PortalSettings.ActiveTab.ContainerSrc Then
                    ' look for a container specification in the skin pane
                    If TypeOf PaneControl Is HtmlControl Then
                        Dim objHtmlControl As HtmlControl = CType(PaneControl, HtmlControl)
                        If (Not objHtmlControl.Attributes("ContainerSrc") Is Nothing) Then
                            Dim validSrc As Boolean = False
                            If (Not objHtmlControl.Attributes("ContainerType") Is Nothing) And (Not objHtmlControl.Attributes("ContainerName") Is Nothing) Then
                                ' legacy container specification in skin pane
                                containerSrc = "[" & objHtmlControl.Attributes("ContainerType") & "]" & SkinController.RootContainer & "/" & objHtmlControl.Attributes("ContainerName") & "/" & objHtmlControl.Attributes("ContainerSrc")
                                validSrc = True
                            Else
                                ' 3.0 container specification in skin pane
                                containerSrc = objHtmlControl.Attributes("ContainerSrc")

                                ' The ContainerSrc should contain both a directory and filename 
                                ' i.e. "DNN-Blue/Text Header - Color Background.ascx"
                                If containerSrc.Contains("/") Then
                                    ' If container type (global or local) is not specified, then use the type from the current skin
                                    If Not (containerSrc.ToLower.StartsWith("[g]") OrElse containerSrc.ToLower.StartsWith("[l]")) Then

                                        ' This assumes that ActiveTab.SkinSrc has a valid skin path
                                        If SkinController.IsGlobalSkin(PortalSettings.ActiveTab.SkinSrc) Then
                                            containerSrc = String.Format("[G]containers/{0}", containerSrc.TrimStart("/"c))
                                        Else
                                            containerSrc = String.Format("[L]containers/{0}", containerSrc.TrimStart("/"c))
                                        End If
                                        validSrc = True
                                    End If
                                End If
                            End If
                            If validSrc Then
                                containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings)
                                ctlContainer = LoadContainerByPath(containerSrc)
                            End If
                        End If
                    End If
                End If
            End If

            ' else load assigned container
            If ctlContainer Is Nothing Then
                containerSrc = objModule.ContainerSrc

                If containerSrc <> "" Then
                    containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings)
                    ctlContainer = LoadContainerByPath(containerSrc)
                End If
            End If

            ' error loading container - load default
            If ctlContainer Is Nothing Then
                containerSrc = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalContainer, PortalSettings)
                ctlContainer = LoadContainerByPath(containerSrc)
            End If

            'Set container path
            objModule.ContainerPath = SkinController.FormatSkinPath(containerSrc)

            ' set container id to an explicit short name to reduce page payload 
            ctlContainer.ID = "ctr"
            ' make the container id unique for the page
            If objModule.ModuleID > -1 Then
                'Can't have ID with a - (dash) in it, should only be for admin modules, where they are the only container, so don't need unique name
                ctlContainer.ID += objModule.ModuleID.ToString
            End If

            Return ctlContainer
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleMoveToPanePostBack excutes when a module is moved by Drag-and-Drop
        ''' </summary>
        ''' <param name="args">A ClientAPIPostBackEventArgs object</param>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Moved from Skin.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ModuleMoveToPanePostBack(ByVal args As DotNetNuke.UI.Utilities.ClientAPIPostBackEventArgs)
            Dim PortalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If TabPermissionController.CanAdminPage() Then
                Dim intModuleID As Integer = CInt(args.EventArguments("moduleid"))
                Dim strPaneName As String = CStr(args.EventArguments("pane"))
                Dim intOrder As Integer = CInt(args.EventArguments("order"))
                Dim objModules As New ModuleController

                objModules.UpdateModuleOrder(PortalSettings.ActiveTab.TabID, intModuleID, intOrder, strPaneName)
                objModules.UpdateTabModuleOrder(PortalSettings.ActiveTab.TabID)

                ' Redirect to the same page to pick up changes
                PaneControl.Page.Response.Redirect(PaneControl.Page.Request.RawUrl, True)
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InjectModule injects a Module (and its container) into the Pane
        ''' </summary>
        ''' <param name="objModule">The Module</param>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InjectModule(ByVal objModule As ModuleInfo)
            Dim bSuccess As Boolean = True

            Try
                If Not IsAdminControl() Then
                    ' inject an anchor tag to allow navigation to the module content
                    PaneControl.Controls.Add(New LiteralControl("<a name=""" & objModule.ModuleID.ToString & """></a>"))
                End If

                'Load container control
                Dim ctlContainer As DotNetNuke.UI.Containers.Container = LoadModuleContainer(objModule)

                'Add Container to Dictionary
                Containers.Add(ctlContainer.ID, ctlContainer)

                If IsLayoutMode() AndAlso Common.Globals.IsAdminControl() = False Then
                    'provide Drag-N-Drop capabilities
                    Dim ctlDragDropContainer As Panel = New Panel
                    Dim ctlTitle As System.Web.UI.Control = ctlContainer.FindControl("dnnTitle")
                    ''Assume that the title control is named dnnTitle.  If this becomes an issue we could loop through the controls looking for the title type of skin object
                    ctlDragDropContainer.ID = ctlContainer.ID & "_DD"
                    PaneControl.Controls.Add(ctlDragDropContainer)

                    ' inject the container into the page pane - this triggers the Pre_Init() event for the user control
                    ctlDragDropContainer.Controls.Add(ctlContainer)

                    If Not ctlTitle Is Nothing Then
                        If ctlTitle.Controls.Count > 0 Then
                            ' if multiple title controls, use the first instance
                            ctlTitle = ctlTitle.Controls(0)
                        End If
                    End If

                    ' enable drag and drop
                    If Not ctlDragDropContainer Is Nothing AndAlso Not ctlTitle Is Nothing Then
                        'The title ID is actually the first child so we need to make sure at least one child exists
                        UI.Utilities.DNNClientAPI.EnableContainerDragAndDrop(ctlTitle, ctlDragDropContainer, objModule.ModuleID)
                        DotNetNuke.UI.Utilities.ClientAPI.RegisterPostBackEventHandler(PaneControl, "MoveToPane", AddressOf ModuleMoveToPanePostBack, False)
                    End If
                Else
                    PaneControl.Controls.Add(ctlContainer)
                End If

                'Attach Module to Container
                ctlContainer.SetModuleConfiguration(objModule)

                ' display collapsible page panes
                If PaneControl.Visible = False Then
                    PaneControl.Visible = True
                End If

            Catch exc As Exception
                Dim lex As ModuleLoadException
                lex = New ModuleLoadException(String.Format(Skin.MODULEADD_ERROR, PaneControl.ID.ToString), exc)
                If TabPermissionController.CanAdminPage() Then
                    ' only display the error to administrators
                    PaneControl.Controls.Add(New ErrorContainer(PortalSettings, Skin.MODULELOAD_ERROR, lex).Container)
                End If
                LogException(exc)
                bSuccess = False
                Throw lex
            End Try

            If Not bSuccess Then
                Throw New ModuleLoadException()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessPane processes the Attributes for the PaneControl
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ProcessPane()
            If Not PaneControl Is Nothing Then
                'remove excess skin non-validating attributes
                PaneControl.Attributes.Remove("ContainerType")
                PaneControl.Attributes.Remove("ContainerName")
                PaneControl.Attributes.Remove("ContainerSrc")

                If IsLayoutMode() Then
                    PaneControl.Visible = True

                    ' display pane border
                    Dim cssclass As String = PaneControl.Attributes("class")
                    If String.IsNullOrEmpty(cssclass) Then
                        PaneControl.Attributes("class") = c_PaneOutline
                    Else
                        PaneControl.Attributes("class") = cssclass.Replace(c_PaneOutline, "").Trim().Replace("  ", " ") & " " & c_PaneOutline
                    End If

                    ' display pane name
                    Dim ctlLabel As New Label
                    ctlLabel.Text = "<center>" & Name & "</center><br>"
                    ctlLabel.CssClass = "SubHead"
                    PaneControl.Controls.AddAt(0, ctlLabel)
                Else
                    If TabPermissionController.CanAddContentToPage() AndAlso PaneControl.Visible = False Then
                        PaneControl.Visible = True
                    End If

                    'This section sets the width to "0" on panes that have no modules.
                    'This preserves the integrity of the HTML syntax so we don't have to set
                    'the visiblity of a pane to false. Setting the visibility of a pane to
                    'false where there are colspans and rowspans can render the skin incorrectly.
                    Dim collapsePanes As Boolean = True
                    If Containers.Count > 0 Then
                        collapsePanes = False
                    ElseIf PaneControl.Controls.Count = 0 Then
                        collapsePanes = True
                    ElseIf PaneControl.Controls.Count = 1 Then
                        'Pane contains 1 control
                        collapsePanes = False
                        Dim literal As LiteralControl = TryCast(PaneControl.Controls(0), LiteralControl)
                        If literal IsNot Nothing Then
                            'Check  if the literal control is just whitespace - if so we can collapse panes
                            If HtmlUtils.StripWhiteSpace(literal.Text, False).Length = 0 Then
                                collapsePanes = True
                            End If
                        End If
                    Else
                        'Pane contains more than 1 control
                        collapsePanes = False
                    End If
                    If collapsePanes Then
                        'This pane has no controls so set the width to 0
                        If PaneControl.Attributes.Item("style") IsNot Nothing Then
                            PaneControl.Attributes.Remove("style")
                        End If
                        If PaneControl.Attributes.Item("class") IsNot Nothing Then
                            PaneControl.Attributes.Item("class") = PaneControl.Attributes.Item("class") + " DNNEmptyPane"
                        Else
                            PaneControl.Attributes.Item("class") = "DNNEmptyPane"
                        End If
                    End If
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
