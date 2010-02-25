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

Imports System.Web
Imports System.Web.UI

Imports DotNetNuke.UI

Namespace DotNetNuke.UI.Utilities

	''' -----------------------------------------------------------------------------
	''' Project	 : DotNetNuke
	''' Class	 : ClientAPI
	''' 
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' Library responsible for interacting with DNN Client API.
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[Jon Henning]	8/3/2004	Created
	''' </history>
	''' -----------------------------------------------------------------------------
	Public Class DNNClientAPI
        Public Enum MinMaxPersistanceType
            None
            Page
            Cookie
            Personalization
        End Enum

        Public Enum PageCallBackType
            GetPersonalization = 0
            SetPersonalization = 1
        End Enum

        Private Shared m_objEnabledClientPersonalizationKeys As Hashtable = New Hashtable

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds client side body.onload event handler 
        ''' </summary>
        ''' <param name="objPage">Current page rendering content</param>
        ''' <param name="strJSFunction">Javascript function name to execute</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/3/2004	Created
        '''		[Jon Henning]	4/25/2005	registering dnn namespace when this function is utilized
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddBodyOnloadEventHandler(ByVal objPage As Page, ByVal strJSFunction As String)
            If strJSFunction.EndsWith(";") = False Then strJSFunction &= ";"
            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(objPage, ClientAPI.ClientNamespaceReferences.dnn)
            If DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(objPage, "__dnn_pageload").IndexOf(strJSFunction) = -1 Then DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objPage, "__dnn_pageload", strJSFunction, False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows any module to have drag and drop functionality enabled
        ''' </summary>
        ''' <param name="objTitle">Title element that responds to the click and dragged</param>
        ''' <param name="objContainer"></param>
        ''' <remarks>
        ''' This sub also will send down information to notify the client of the panes that have been defined in the current skin.
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub EnableContainerDragAndDrop(ByVal objTitle As Control, ByVal objContainer As Control, ByVal ModuleID As Integer)
            If DotNetNuke.UI.Utilities.ClientAPI.ClientAPIDisabled() = False AndAlso DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(DotNetNuke.UI.Utilities.ClientAPI.ClientFunctionality.Positioning) Then
                DNNClientAPI.AddBodyOnloadEventHandler(objTitle.Page, "__dnn_enableDragDrop()")
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(objTitle.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom_positioning)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_dragDrop", objContainer.ClientID & " " & objTitle.ClientID & " " & ModuleID.ToString & ";", False)

                Dim strPanes As String = ""
                Dim strPaneNames As String = ""
                Dim objPortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
                Dim strPane As String
                Dim objCtl As Control
                For Each strPane In objPortalSettings.ActiveTab.Panes
                    objCtl = DotNetNuke.Common.Globals.FindControlRecursive(objContainer.Parent, strPane)
                    If Not objCtl Is Nothing Then strPanes &= objCtl.ClientID & ";"
                    strPaneNames &= strPane & ";"
                Next
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_Panes", strPanes, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_PaneNames", strPaneNames, True)
            End If
        End Sub

        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal blnDefaultMin As Boolean, ByVal ePersistanceType As MinMaxPersistanceType)
            EnableMinMax(objButton, objContent, -1, blnDefaultMin, "", "", ePersistanceType)
        End Sub

        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal intModuleId As Integer, ByVal blnDefaultMin As Boolean, ByVal ePersistanceType As MinMaxPersistanceType)
            EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, "", "", ePersistanceType)
        End Sub

        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal blnDefaultMin As Boolean, ByVal strMinIconLoc As String, ByVal strMaxIconLoc As String, ByVal ePersistanceType As MinMaxPersistanceType)
            EnableMinMax(objButton, objContent, -1, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows a button and a content area to support client side min/max functionality
        ''' </summary>
        ''' <param name="objButton">Control that when clicked causes content area to be hidden/shown</param>
        ''' <param name="objContent">Content area that is hidden/shown</param>
        ''' <param name="intModuleId">Module id of button/content, used only for persistance type of Cookie</param>
        ''' <param name="blnDefaultMin">If content area is to be defaulted to minimized pass in true</param>
        ''' <param name="strMinIconLoc">Location of minimized icon</param>
        ''' <param name="strMaxIconLoc">Location of maximized icon</param>
        ''' <param name="ePersistanceType">How to store current state of min/max.  Cookie, Page, None</param>
        ''' <remarks>
        ''' This method's purpose is to provide a higher level of abstraction between the ClientAPI and the module developer.
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	5/6/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal intModuleId As Integer, ByVal blnDefaultMin As Boolean, ByVal strMinIconLoc As String, ByVal strMaxIconLoc As String, ByVal ePersistanceType As MinMaxPersistanceType)
            EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, 5)
        End Sub

        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal blnDefaultMin As Boolean, ByVal strMinIconLoc As String, ByVal strMaxIconLoc As String, ByVal ePersistanceType As MinMaxPersistanceType, ByVal strPersonalizationNamingCtr As String, ByVal strPersonalizationKey As String)
            EnableMinMax(objButton, objContent, -1, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, 5, strPersonalizationNamingCtr, strPersonalizationKey)
        End Sub

        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal intModuleId As Integer, ByVal blnDefaultMin As Boolean, ByVal strMinIconLoc As String, ByVal strMaxIconLoc As String, ByVal ePersistanceType As MinMaxPersistanceType, ByVal intAnimationFrames As Integer)
            EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, intAnimationFrames, Nothing, Nothing)
        End Sub


        Public Shared Sub EnableMinMax(ByVal objButton As Control, ByVal objContent As Control, ByVal intModuleId As Integer, ByVal blnDefaultMin As Boolean, ByVal strMinIconLoc As String, ByVal strMaxIconLoc As String, ByVal ePersistanceType As MinMaxPersistanceType, ByVal intAnimationFrames As Integer, ByVal strPersonalizationNamingCtr As String, ByVal strPersonalizationKey As String)
            If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(DotNetNuke.UI.Utilities.ClientAPI.ClientFunctionality.DHTML) Then
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(objButton.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom)

                Select Case ePersistanceType
                    Case MinMaxPersistanceType.None
                        AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" & objContent.ClientID & "')) return false;")

                        If Len(strMinIconLoc) > 0 Then
                            AddAttribute(objButton, "max_icon", strMaxIconLoc)
                            AddAttribute(objButton, "min_icon", strMinIconLoc)
                        End If
                    Case MinMaxPersistanceType.Page
                        AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" & objContent.ClientID & "')) return false;")

                        If Len(strMinIconLoc) > 0 Then
                            AddAttribute(objButton, "max_icon", strMaxIconLoc)
                            AddAttribute(objButton, "min_icon", strMinIconLoc)
                        End If
                    Case MinMaxPersistanceType.Cookie
                        If intModuleId <> -1 Then
                            AddAttribute(objButton, "onclick", "if (__dnn_ContainerMaxMin_OnClick(this, '" & objContent.ClientID & "')) return false;")
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "containerid_" & objContent.ClientID, intModuleId.ToString, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "cookieid_" & objContent.ClientID, "_Module" & intModuleId.ToString & "_Visible", True)

                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "min_icon_" & intModuleId.ToString, strMinIconLoc, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "max_icon_" & intModuleId.ToString, strMaxIconLoc, True)

                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "max_text", DotNetNuke.Services.Localization.Localization.GetString("Maximize"), True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "min_text", DotNetNuke.Services.Localization.Localization.GetString("Minimize"), True)

                            If blnDefaultMin Then
                                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "__dnn_" & intModuleId.ToString & ":defminimized", "true", True)
                            End If
                        End If
                    Case MinMaxPersistanceType.Personalization
                        'Regardless if we determine whether or not the browser supports client-side personalization
                        'we need to store these keys to properly display or hide the content (They are needed in MinMaxContentVisible)
                        AddAttribute(objButton, "userctr", strPersonalizationNamingCtr)
                        AddAttribute(objButton, "userkey", strPersonalizationKey)
                        If EnableClientPersonalization(strPersonalizationNamingCtr, strPersonalizationKey, objButton.Page) Then
                            AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" & objContent.ClientID & "')) return false;")

                            If Len(strMinIconLoc) > 0 Then
                                AddAttribute(objButton, "max_icon", strMaxIconLoc)
                                AddAttribute(objButton, "min_icon", strMinIconLoc)
                            End If
                        End If
                End Select
            End If

            If MinMaxContentVisibile(objButton, intModuleId, blnDefaultMin, ePersistanceType) Then
                If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(DotNetNuke.UI.Utilities.ClientAPI.ClientFunctionality.DHTML) Then
                    AddStyleAttribute(objContent, "display", "")
                Else
                    objContent.Visible = True
                End If
                If Len(strMinIconLoc) > 0 Then SetMinMaxProperties(objButton, strMinIconLoc, DotNetNuke.Services.Localization.Localization.GetString("Minimize"), DotNetNuke.Services.Localization.Localization.GetString("Minimize"))
            Else
                If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(DotNetNuke.UI.Utilities.ClientAPI.ClientFunctionality.DHTML) Then
                    AddStyleAttribute(objContent, "display", "none")
                Else
                    objContent.Visible = False
                End If
                If Len(strMaxIconLoc) > 0 Then SetMinMaxProperties(objButton, strMaxIconLoc, DotNetNuke.Services.Localization.Localization.GetString("Maximize"), DotNetNuke.Services.Localization.Localization.GetString("Maximize"))
            End If

            If intAnimationFrames <> 5 Then
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, "animf_" & objContent.ClientID, intAnimationFrames.ToString, True)
            End If

        End Sub

        Private Shared Sub SetMinMaxProperties(ByVal objButton As Control, ByVal strImage As String, ByVal strToolTip As String, ByVal strAltText As String)
            Select Case True
                Case TypeOf objButton Is LinkButton
                    Dim objLB As LinkButton = CType(objButton, LinkButton)
                    objLB.ToolTip = strToolTip
                    If objLB.Controls.Count > 0 Then
                        SetImageProperties(objLB.Controls(0), strImage, strToolTip, strAltText)
                    End If
                Case TypeOf objButton Is System.Web.UI.WebControls.Image
                    SetImageProperties(CType(objButton, System.Web.UI.WebControls.Image), strImage, strToolTip, strAltText)
                Case TypeOf objButton Is System.Web.UI.WebControls.ImageButton
                    SetImageProperties(CType(objButton, System.Web.UI.WebControls.LinkButton), strImage, strToolTip, strAltText)
            End Select

        End Sub

        Private Shared Sub SetImageProperties(ByVal objControl As Control, ByVal strImage As String, ByVal strToolTip As String, ByVal strAltText As String)
            Select Case True
                Case TypeOf objControl Is System.Web.UI.WebControls.Image
                    Dim objImage As System.Web.UI.WebControls.Image = CType(objControl, System.Web.UI.WebControls.Image)
                    objImage.ImageUrl = strImage
                    objImage.AlternateText = strAltText
                    objImage.ToolTip = strToolTip
                Case TypeOf objControl Is System.Web.UI.WebControls.ImageButton
                    Dim objImage As System.Web.UI.WebControls.ImageButton = CType(objControl, System.Web.UI.WebControls.ImageButton)
                    objImage.ImageUrl = strImage
                    objImage.AlternateText = strAltText
                    objImage.ToolTip = strToolTip
                Case TypeOf objControl Is System.Web.UI.HtmlControls.HtmlImage
                    Dim objImage As System.Web.UI.HtmlControls.HtmlImage = CType(objControl, System.Web.UI.HtmlControls.HtmlImage)
                    objImage.Src = strImage
                    objImage.Alt = strAltText
            End Select
        End Sub

        Public Shared Property MinMaxContentVisibile(ByVal objButton As Control, ByVal blnDefaultMin As Boolean, ByVal ePersistanceType As MinMaxPersistanceType) As Boolean
            Get
                Return MinMaxContentVisibile(objButton, -1, blnDefaultMin, ePersistanceType)
            End Get
            Set(ByVal Value As Boolean)
                MinMaxContentVisibile(objButton, -1, blnDefaultMin, ePersistanceType) = Value
            End Set
        End Property

        Public Shared Property MinMaxContentVisibile(ByVal objButton As Control, ByVal intModuleId As Integer, ByVal blnDefaultMin As Boolean, ByVal ePersistanceType As MinMaxPersistanceType) As Boolean
            Get
                If Not System.Web.HttpContext.Current Is Nothing Then
                    Select Case ePersistanceType
                        Case MinMaxPersistanceType.Page
                            Dim sExpanded As String = DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(objButton.Page, objButton.ClientID & ":exp")
                            If Len(sExpanded) > 0 Then
                                Return CBool(sExpanded)
                            Else
                                Return Not blnDefaultMin
                            End If
                        Case MinMaxPersistanceType.Cookie
                            If intModuleId <> -1 Then
                                Dim objModuleVisible As HttpCookie = System.Web.HttpContext.Current.Request.Cookies("_Module" & intModuleId.ToString & "_Visible")
                                If Not objModuleVisible Is Nothing Then
                                    Return objModuleVisible.Value <> "false"
                                Else
                                    Return Not blnDefaultMin
                                End If
                            Else
                                Return True
                            End If
                        Case MinMaxPersistanceType.None
                            Return Not blnDefaultMin
                        Case MinMaxPersistanceType.Personalization
                            Dim strVisible As String = Convert.ToString(Personalization.Personalization.GetProfile(Globals.GetAttribute(objButton, "userctr"), Globals.GetAttribute(objButton, "userkey")))
                            If String.IsNullOrEmpty(strVisible) Then
                                Return blnDefaultMin
                            Else
                                Return Convert.ToBoolean(strVisible)
                            End If
                    End Select
                End If
            End Get
            Set(ByVal Value As Boolean)
                If Not System.Web.HttpContext.Current Is Nothing Then
                    Select Case ePersistanceType
                        Case MinMaxPersistanceType.Page
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(objButton.Page, objButton.ClientID & ":exp", CInt(Value).ToString, True)
                        Case MinMaxPersistanceType.Cookie
                            Dim objModuleVisible As HttpCookie = New HttpCookie("_Module" & intModuleId.ToString & "_Visible")
                            If Not objModuleVisible Is Nothing Then
                                objModuleVisible.Value = Value.ToString.ToLower
                                objModuleVisible.Expires = DateTime.MaxValue                                  ' never expires
                                System.Web.HttpContext.Current.Response.AppendCookie(objModuleVisible)
                            End If
                        Case MinMaxPersistanceType.Personalization
                            Personalization.Personalization.SetProfile(Globals.GetAttribute(objButton, "userctr"), Globals.GetAttribute(objButton, "userkey"), Value.ToString)
                    End Select
                End If
            End Set
        End Property

        Private Shared Sub AddAttribute(ByVal objControl As Control, ByVal strName As String, ByVal strValue As String)
            If TypeOf objControl Is HtmlControl Then
                CType(objControl, HtmlControl).Attributes.Add(strName, strValue)
            ElseIf TypeOf objControl Is WebControl Then
                CType(objControl, WebControl).Attributes.Add(strName, strValue)
            End If
        End Sub

        Private Shared Sub AddStyleAttribute(ByVal objControl As Control, ByVal strName As String, ByVal strValue As String)
            If TypeOf objControl Is HtmlControl Then
                If Len(strValue) > 0 Then
                    CType(objControl, HtmlControl).Style.Add(strName, strValue)
                Else
                    CType(objControl, HtmlControl).Style.Remove(strName)
                End If
            ElseIf TypeOf objControl Is WebControl Then
                If Len(strValue) > 0 Then
                    CType(objControl, WebControl).Style.Add(strName, strValue)
                Else
                    CType(objControl, WebControl).Style.Remove(strName)
                End If
            End If
        End Sub

        'enables callbacks for request, and registers personalization key to be accessible from client
        'returns true when browser is capable of callbacks
        Public Shared Function EnableClientPersonalization(ByVal strNamingContainer As String, ByVal strKey As String, ByVal objPage As Page) As Boolean
            If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
                'Instead of sending the callback js function down to the client, we are hardcoding
                'it on the client.  DNN owns the interface, so there is no worry about an outside
                'entity changing it on us.  We are simply calling this here to register all the appropriate
                'js libraries
                ClientAPI.GetCallbackEventReference(objPage, "", "", "", "")

                'in order to limit the keys that can be accessed and written we are storing the enabled keys
                'in this shared hash table
                SyncLock m_objEnabledClientPersonalizationKeys.SyncRoot
                    If IsPersonalizationKeyRegistered(strNamingContainer & ClientAPI.CUSTOM_COLUMN_DELIMITER & strKey) = False Then
                        m_objEnabledClientPersonalizationKeys.Add(strNamingContainer & ClientAPI.CUSTOM_COLUMN_DELIMITER & strKey, "")
                    End If
                End SyncLock
                Return True
            End If
            Return False
        End Function

        Public Shared Function IsPersonalizationKeyRegistered(ByVal strKey As String) As Boolean
            Return m_objEnabledClientPersonalizationKeys.ContainsKey(strKey)
        End Function

    End Class
End Namespace
