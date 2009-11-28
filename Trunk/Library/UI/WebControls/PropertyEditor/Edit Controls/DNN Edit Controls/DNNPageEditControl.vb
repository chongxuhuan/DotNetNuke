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

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework
Imports System.Collections.Generic

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNPageEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNPageEditControl control provides a standard UI component for selecting
    ''' a DNN Page
    ''' </summary>
    ''' <history>
    '''     [cnurse]	03/22/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DNNPageEditControl runat=server></{0}:DNNPageEditControl>")> _
    Public Class DNNPageEditControl
        Inherits IntegerEditControl

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a DNNPageEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	03/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim _portalSettings As PortalSettings = GetPortalSettings()

            'Get the Pages
            Dim listTabs As List(Of TabInfo) = TabController.GetPortalTabs(_portalSettings.PortalId, Null.NullInteger, True, True)

            'Render the Select Tag
            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            For tabIndex As Integer = 0 To listTabs.Count - 1
                Dim tab As TabInfo = listTabs(tabIndex)

                'Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, tab.TabID.ToString())

                If tab.TabID = IntegerValue Then
                    'Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If

                'Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write(tab.IndentedTabName)
                writer.RenderEndTag()
            Next

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	10/11/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            'try to find a tab with the specified ID
            Dim tabController As New DotNetNuke.Entities.Tabs.TabController()
            Dim linkedTabInfo As TabInfo = tabController.GetTab(IntegerValue, DotNetNuke.Common.GetPortalSettings.PortalId, False)

            'don't render anything if we didn't find the tab
            If Not (linkedTabInfo Is Nothing) Then
                'Not really sure how to get a good TabID and ModuleID but it's only for tracking so not to concerned
                Dim tabID As Integer = 0
                Dim moduleID As Integer = 0
                Int32.TryParse(Me.Page.Request.QueryString("tabid"), tabID)
                Int32.TryParse(Me.Page.Request.QueryString("mid"), moduleID)

                Dim url As String = DotNetNuke.Common.Globals.LinkClick(StringValue, tabID, moduleID, True)
                ControlStyle.AddAttributesToRender(writer)
                writer.AddAttribute(HtmlTextWriterAttribute.Href, url)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "Normal")
                writer.RenderBeginTag(HtmlTextWriterTag.A)
                writer.Write(linkedTabInfo.LocalizedTabName)
                writer.RenderEndTag()
            End If
        End Sub

#End Region

    End Class

End Namespace

