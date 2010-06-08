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

Imports System.Xml
Imports System.Configuration
Imports System.Net
Imports System.IO
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Mail
Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Net.Mime

Namespace DotNetNuke.Modules.Admin.LogViewer

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : LogViewer
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality for viewing the Site Logs
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''   [cnurse] 17/9/2004  Updated for localization, Help and 508. Also 
    '''                       consolidated Send Exceptions into one set of 
    '''                       controls
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LogViewer

        Inherits DotNetNuke.Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region "Private Members"

        Dim arrLogTypeInfo As ArrayList
        Dim htLogTypeInfo As Hashtable
        Dim ColorCodingOn As Boolean = False
        Dim ColorCodingLegendOn As Boolean = True
        Dim PageIndex As Integer = 1
        Dim intPortalID As Integer = -1
        Dim strLogTypeKey As String

#End Region

#Region "Private Methods"

        Private Sub BindData()

            btnEmail.Attributes.Add("onclick", "return CheckExceptions();")

            If UserInfo.IsSuperUser Then
                If Not Page.IsPostBack AndAlso Not Request.QueryString("pid") Is Nothing Then
                    ddlPortalid.Items.FindByValue(Request.QueryString("pid")).Selected = True
                End If
                intPortalID = Int32.Parse(ddlPortalid.SelectedItem.Value)
            Else
                intPortalID = PortalId
            End If

            Dim TotalRecords As Integer
            Dim PageSize As Integer = Convert.ToInt32(ddlRecordsPerPage.SelectedValue)

            If Not Page.IsPostBack AndAlso Not Request.QueryString("LogTypeKey") Is Nothing Then
                Dim li As ListItem = ddlLogType.Items.FindByValue(Request.QueryString("LogTypeKey"))
                If li IsNot Nothing Then
                    li.Selected = True
                End If
            End If

            strLogTypeKey = ddlLogType.SelectedItem.Value

            Dim objLog As LogInfoArray
            Dim CurrentPage As Integer = PageIndex
            If CurrentPage > 0 Then CurrentPage = CurrentPage - 1
            Dim objLogController As New LogController
            If intPortalID = -1 And strLogTypeKey = "*" Then
                objLog = objLogController.GetLog(PageSize, CurrentPage, TotalRecords)
            ElseIf intPortalID = -1 And strLogTypeKey <> "*" Then
                objLog = objLogController.GetLog(strLogTypeKey, PageSize, CurrentPage, TotalRecords)
            ElseIf intPortalID <> -1 And strLogTypeKey = "*" Then
                objLog = objLogController.GetLog(intPortalID, PageSize, CurrentPage, TotalRecords)
            ElseIf intPortalID <> -1 And strLogTypeKey <> "*" Then
                objLog = objLogController.GetLog(intPortalID, strLogTypeKey, PageSize, CurrentPage, TotalRecords)
            Else
                objLog = objLogController.GetLog(strLogTypeKey, PageSize, CurrentPage, TotalRecords)
            End If

            If objLog.Count > 0 Then
                dlLog.Visible = True
                pnlSendExceptions.Visible = Me.IsEditable
                If ColorCodingOn Then
                    chkColorCoding.Checked = True
                Else
                    chkColorCoding.Checked = False
                End If
                If ColorCodingLegendOn Then
                    pnlLegend.Visible = True
                Else
                    pnlLegend.Visible = False
                End If
                If UserInfo.IsSuperUser Then
                    btnDelete.Visible = True
                    btnClear.Visible = True
                Else
                    btnDelete.Visible = False
                    btnClear.Visible = False
                End If
                pnlOptions.Visible = True
                tblInstructions.Visible = True
                dlLog.DataSource = objLog
                dlLog.DataBind()

                InitializePaging(ctlPagingControlBottom, TotalRecords, PageSize)
            Else
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("NoEntries", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                dlLog.Visible = False
                pnlSendExceptions.Visible = False
                btnDelete.Visible = False
                btnClear.Visible = False
                pnlLegend.Visible = False
                tblInstructions.Visible = False
                ctlPagingControlBottom.Visible = False
                chkColorCoding.Checked = False
            End If

        End Sub

        Private Sub InitializePaging(ByVal ctlPagingControl As DotNetNuke.UI.WebControls.PagingControl, ByVal TotalRecords As Integer, ByVal PageSize As Integer)
            ctlPagingControl.TotalRecords = TotalRecords
            ctlPagingControl.PageSize = PageSize
            ctlPagingControl.CurrentPage = PageIndex
            Dim strQuerystring As String = ""
            If ddlRecordsPerPage.SelectedIndex <> 0 Then
                strQuerystring += "&PageRecords=" + ddlRecordsPerPage.SelectedValue
            End If
            If intPortalID >= 0 Then
                strQuerystring += "&pid=" + intPortalID.ToString
            End If
            If strLogTypeKey <> "*" AndAlso strLogTypeKey <> "" Then
                strQuerystring += "&LogTypeKey=" + strLogTypeKey
            End If
            ctlPagingControl.QuerystringParams = strQuerystring
            ctlPagingControl.TabID = TabId
        End Sub

        Private Sub BindLogTypeDropDown()

            Dim objLogController As New LogController
            Dim arrLogTypes As New ArrayList
            arrLogTypes = objLogController.GetLogTypeInfo()
            arrLogTypes.Sort(New LogTypeSortFriendlyName)
            ddlLogType.DataTextField = "LogTypeFriendlyName"
            ddlLogType.DataValueField = "LogTypeKey"
            ddlLogType.DataSource = arrLogTypes
            ddlLogType.DataBind()
            Dim ddlAllPortals As New ListItem(Services.Localization.Localization.GetString("All"), "*")
            ddlLogType.Items.Insert(0, ddlAllPortals)

        End Sub

        Private Sub BindPortalDropDown()
            Dim objPortalSecurity As New PortalSecurity
            Dim i As Integer

            If UserInfo.IsSuperUser Then
                Dim objPortalController As New PortalController
                Dim arrPortals As New ArrayList
                arrPortals = objPortalController.GetPortals()
                arrPortals.Sort(New PortalSortTitle)
                ddlPortalid.DataTextField = "PortalName"
                ddlPortalid.DataValueField = "PortalID"
                ddlPortalid.DataSource = arrPortals
                ddlPortalid.DataBind()
                Dim ddlAllPortals As New ListItem(Services.Localization.Localization.GetString("All"), "-1")
                ddlPortalid.Items.Insert(0, ddlAllPortals)
                'check to see if any portalname is empty, otherwise set it to portalid
                For i = 0 To ddlPortalid.Items.Count - 1
                    If ddlPortalid.Items.Item(i).Text.Length = 0 Then
                        ddlPortalid.Items.Item(i).Text = "Portal: " & ddlPortalid.Items.Item(i).Value
                    End If
                Next
            Else
                plPortalID.Visible = False
                ddlPortalid.Visible = False
            End If
        End Sub

        Private Sub DeleteSelectedExceptions()
            Try
                Dim strXMLOut As New System.Text.StringBuilder
                Dim s As String = Request.Form("Exception")
                If Not s Is Nothing Then
                    Dim arrExcPositions(-1) As String
                    If s.LastIndexOf(",") > 0 Then
                        arrExcPositions = s.Split(Convert.ToChar(","))
                    ElseIf s.Length > 0 Then
                        ReDim arrExcPositions(0)
                        arrExcPositions(0) = s
                    End If

                    Dim objLoggingController As New LogController

                    Dim i As Integer
                    Dim j As Integer = arrExcPositions.Length()
                    For i = 1 To arrExcPositions.Length
                        j -= 1
                        Dim excKey() As String
                        excKey = arrExcPositions(j).Split(Convert.ToChar("|"))
                        Dim objLogInfo As New LogInfo
                        objLogInfo.LogGUID = excKey(0)
                        objLogInfo.LogFileID = excKey(1)
                        objLoggingController.DeleteLog(objLogInfo)
                    Next
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DeleteSuccess", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                End If
                BindPortalDropDown()
                BindData()
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Function GetSelectedExceptions() As XmlDocument
            Dim objXML As New XmlDocument
            Try
                Dim strXMLOut As New System.Text.StringBuilder
                Dim s As String = Request.Form("Exception")
                If Not s Is Nothing Then
                    Dim arrExcPositions(-1) As String
                    If s.LastIndexOf(",") > 0 Then
                        arrExcPositions = s.Split(Convert.ToChar(","))
                    ElseIf s.Length > 0 Then
                        ReDim arrExcPositions(0)
                        arrExcPositions(0) = s
                    End If

                    Dim objLoggingController As New LogController

                    objXML.LoadXml("<LogEntries></LogEntries>")

                    Dim i As Integer
                    Dim j As Integer = arrExcPositions.Length()
                    For i = 1 To arrExcPositions.Length
                        j -= 1
                        Dim excKey() As String
                        excKey = arrExcPositions(j).Split(Convert.ToChar("|"))
                        Dim objLogInfo As New LogInfo
                        objLogInfo.LogGUID = excKey(0)
                        objLogInfo.LogFileID = excKey(1)
                        Dim objNode As XmlNode
                        objNode = objXML.ImportNode(CType(objLoggingController.GetSingleLog(objLogInfo, LoggingProvider.ReturnType.XML), XmlNode), True)
                        objXML.DocumentElement.AppendChild(objNode)
                    Next
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
            Return objXML
        End Function

        Private Sub SendEmail()

            Dim strEmail As String = GetSelectedExceptions().OuterXml
            Dim strFromEmailAddress As String
            Dim _userController As New UserController
            If UserInfo.Email <> "" Then
                strFromEmailAddress = UserInfo.Email
            Else
                strFromEmailAddress = PortalSettings.Email
            End If
            If String.IsNullOrEmpty(txtSubject.Text) Then
                txtSubject.Text = PortalSettings.PortalName & " Exceptions"
            End If

            Dim ReturnMsg As String
            If Regex.IsMatch(strFromEmailAddress, glbEmailRegEx) Then
                Dim fileName As String = "errorlog.xml"
                Dim filePath As String = PortalSettings.HomeDirectoryMapPath + fileName
                Dim xmlDoc As XmlDocument = GetSelectedExceptions()
                xmlDoc.Save(filePath)

                Dim attachments As New List(Of Attachment)
                Dim ct As New ContentType()
                ct.MediaType = MediaTypeNames.Text.Xml
                ct.Name = fileName

                Dim attachment As New Attachment(filePath, ct)
                attachments.Add(attachment)

                ReturnMsg = Mail.SendEmail(strFromEmailAddress, strFromEmailAddress, txtEmailAddress.Text, txtSubject.Text, txtMessage.Text, attachments)

                FileSystemUtils.DeleteFile(filePath)

                If ReturnMsg = "" Then
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("EmailSuccess", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("EmailFailure", Me.LocalResourceFile) + " " + ReturnMsg, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            Else
                ReturnMsg = String.Format(Localization.GetString("InavlidEmailAddress", Me.LocalResourceFile), strFromEmailAddress)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("EmailFailure", Me.LocalResourceFile) + " " + ReturnMsg, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
            BindData()
        End Sub


#End Region

#Region "Protected Methods"

        Protected Function GetMyLogType(ByVal LogTypeKey As String) As LogTypeInfo
            Dim objLogTypeInfo As LogTypeInfo
            If Not htLogTypeInfo(LogTypeKey) Is Nothing Then
                objLogTypeInfo = CType(htLogTypeInfo(LogTypeKey), LogTypeInfo)
                If Not ColorCodingOn Then
                    objLogTypeInfo.LogTypeCSSClass = "Normal"
                End If
                Return objLogTypeInfo
            Else
                Return New LogTypeInfo
            End If
        End Function

#End Region

#Region "Public Methods"

        Public Function GetPropertiesText(ByVal obj As Object) As String

            Dim objLogInfo As LogInfo = CType(obj, LogInfo)

            Dim objLogProperties As LogProperties = objLogInfo.LogProperties
            Dim str As New System.Text.StringBuilder

            Dim i As Integer
            For i = 0 To objLogProperties.Count - 1
                ' display the values in the Panel child controls.
                Dim ldi As LogDetailInfo = CType(objLogProperties(i), LogDetailInfo)
                str.Append("<b>" + ldi.PropertyName + "</b>: " + Server.HtmlEncode(ldi.PropertyValue) + "<br>")
            Next
            str.Append(Services.Localization.Localization.GetString("ServerName", Me.LocalResourceFile) + Server.HtmlEncode(objLogInfo.LogServerName) + "<br>")
            Return str.ToString
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            If Not Request.QueryString("CurrentPage") Is Nothing Then
                PageIndex = CType(Request.QueryString("CurrentPage"), Integer)
            End If

            Dim l As New LogController
            arrLogTypeInfo = l.GetLogTypeInfo()

            htLogTypeInfo = New Hashtable

            Dim i As Integer
            For i = 0 To arrLogTypeInfo.Count - 1
                Dim objLogTypeInfo As LogTypeInfo = CType(arrLogTypeInfo(i), LogTypeInfo)
                htLogTypeInfo.Add(objLogTypeInfo.LogTypeKey, objLogTypeInfo)
            Next

            Dim ColorCoding As String
            ColorCoding = Convert.ToString(Personalization.Personalization.GetProfile("LogViewer", "ColorCoding"))
            If ColorCoding = "0" Then
                ColorCodingOn = False
            ElseIf ColorCoding = "1" Then
                ColorCodingOn = True
            Else
                ColorCodingOn = True
            End If

            Dim ColorCodingLegend As String
            ColorCodingLegend = Convert.ToString(Personalization.Personalization.GetProfile("LogViewer", "ColorCodingLegend"))
            If ColorCodingLegend = "0" Then
                ColorCodingLegendOn = False
            ElseIf ColorCodingLegend = "1" Then
                ColorCodingLegendOn = True
            Else
                ColorCodingLegendOn = True
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load runs when the page loads
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 17/9/2004  Updated for localization, Help and 508
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' Load the styles
                'DirectCast(Page, CDefault).AddStyleSheet(CreateValidID(ControlPath & "logviewer"), ControlPath & "logviewer.css")

                Dim objPortalSecurity As New PortalSecurity

                ' If this is the first visit to the page, populate the site data
                If Page.IsPostBack = False Then
                    Dim objLC As New LogController
                    objLC.PurgeLogBuffer()

                    If Not Request.QueryString("PageRecords") Is Nothing Then
                        ddlRecordsPerPage.SelectedValue = Request.QueryString("PageRecords")
                    End If

                    BindPortalDropDown()
                    BindLogTypeDropDown()
                    BindData()

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
            Dim objLoggingController As New LogController
            objLoggingController.ClearLog()
            UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("LogCleared", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            'add entry to log recording it was cleared
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(Services.Localization.Localization.GetString("LogCleared", Me.LocalResourceFile), Services.Localization.Localization.GetString("Username", Me.LocalResourceFile) & ":" & UserInfo.Username, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)

            BindPortalDropDown()
            dlLog.Visible = False
            pnlSendExceptions.Visible = False
            btnDelete.Visible = False
            btnClear.Visible = False
            pnlOptions.Visible = False
            pnlLegend.Visible = False
            tblInstructions.Visible = False
            ctlPagingControlBottom.Visible = False
        End Sub

        Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
            DeleteSelectedExceptions()
        End Sub

        Private Sub btnEmail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEmail.Click

            Select Case optEmailType.SelectedValue
                Case "Email"
                    SendEmail()
            End Select

        End Sub

        Private Sub chkColorCoding_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkColorCoding.CheckedChanged
            Dim i As Integer
            If chkColorCoding.Checked Then
                i = 1
                ColorCodingOn = True
                ColorCodingLegendOn = True
                Personalization.Personalization.SetProfile("LogViewer", "ColorCodingLegend", "1")
            Else
                i = 0
                ColorCodingOn = False
                ColorCodingLegendOn = False
            End If
            Personalization.Personalization.SetProfile("LogViewer", "ColorCoding", i)
            BindData()
        End Sub

        Private Sub ddlLogType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlLogType.SelectedIndexChanged
            PageIndex = 1
            BindData()
        End Sub

        Private Sub ddlPortalID_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlPortalid.SelectedIndexChanged
            PageIndex = 1
            BindData()
        End Sub

        Private Sub ddlRecordsPerPage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlRecordsPerPage.SelectedIndexChanged
            PageIndex = 1
            BindData()
        End Sub

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                If IsEditable Then
                    Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), False, SecurityAccessLevel.Host, True, False)
                End If
                Return Actions
            End Get
        End Property
#End Region

    End Class

End Namespace
