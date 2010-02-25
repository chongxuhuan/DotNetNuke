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

Imports DotNetNuke.HttpModules.Config
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Localization
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

Namespace DotNetNuke.Modules.Admin.Host

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The FriendlyUrls PortalModuleBase is used to edit the friendly urls
    ''' for the application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/06/2006 Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class FriendlyUrls
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase
#Region "Private Methods"
        Private _Rules As RewriterRuleCollection
#End Region

#Region "Private Methods"

        Private Sub BindRules()
            grdRules.DataSource = Rules
            grdRules.DataBind()
        End Sub

#End Region

#Region "Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the mode of the control
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property AddMode() As Boolean
            Get
                Dim _Mode As Boolean = Null.NullBoolean
                If Not ViewState("Mode") Is Nothing Then
                    _Mode = CType(ViewState("Mode"), Boolean)
                End If
                Return _Mode
            End Get
            Set(ByVal value As Boolean)
                ViewState("Mode") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of rewriter rules
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property Rules() As RewriterRuleCollection
            Get
                If _Rules Is Nothing Then
                    _Rules = RewriterConfiguration.GetConfig().Rules
                End If
                Return _Rules
            End Get
            Set(ByVal Value As RewriterRuleCollection)
                _Rules = Value
            End Set
        End Property

#End Region

#Region "Protected methods"

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            Dim myState As Object() = CType(savedState, Object())
            If Not (myState(0) Is Nothing) Then
                MyBase.LoadViewState(myState(0))
            End If
            If Not (myState(1) Is Nothing) Then
                Dim config As New RewriterConfiguration

                ' Deserialize into RewriterConfiguration
                config = DirectCast(XmlUtils.Deserialize(CStr(myState(1)), config.GetType), RewriterConfiguration)
                Rules = config.Rules
            End If
        End Sub

        Protected Overrides Function SaveViewState() As Object
            Dim config As RewriterConfiguration = New RewriterConfiguration
            config.Rules = Rules

            Dim baseState As Object = MyBase.SaveViewState()
            Dim allStates(2) As Object
            allStates(0) = baseState
            allStates(1) = XmlUtils.Serialize(config)

            Return allStates
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            For Each column As DataGridColumn In grdRules.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName = "Delete" Then
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                    End If

                    'Localize Image Column Text
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            'Bind the rules (as long as not postback)
            If Not Page.IsPostBack Then
                'Localize the Data Grid
                Localization.LocalizeDataGrid(grdRules, LocalResourceFile)
                BindRules()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddRule runs when the Add button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddRule(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddRule.Click

            'Add a new empty rule and set the editrow to the new row
            Rules.Add(New RewriterRule)
            grdRules.EditItemIndex = Rules.Count - 1

            'Set the AddMode to true
            AddMode = True

            'Rebind the collection
            BindRules()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteRule runs when a delete button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	7/06/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteRule(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdRules.DeleteCommand

            'Get the index of the row to delete
            Dim index As Integer = e.Item.ItemIndex

            'Remove the rule from the rules collection
            Rules.RemoveAt(index)

            'Save the new collection
            RewriterConfiguration.SaveConfig(Rules)

            'Rebind the collection
            BindRules()

        End Sub

        Private Sub EditRule(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdRules.EditCommand

            'Set the AddMode to false
            AddMode = False

            'Set the editrow
            grdRules.EditItemIndex = e.Item.ItemIndex

            'Rebind the collection
            BindRules()

        End Sub

        Protected Sub SaveRule(ByVal source As Object, ByVal e As CommandEventArgs)

            'Get the index of the row to save
            Dim index As Integer = grdRules.EditItemIndex

            Dim rule As RewriterRule = Rules(index)
            Dim ctlMatch As TextBox = CType(grdRules.Items(index).Cells(2).FindControl("txtMatch"), TextBox)
            Dim ctlReplace As TextBox = CType(grdRules.Items(index).Cells(2).FindControl("txtReplace"), TextBox)

            If ctlMatch.Text <> "" And ctlReplace.Text <> "" Then
                rule.LookFor = ctlMatch.Text
                rule.SendTo = ctlReplace.Text
                'Save the modified collection
                RewriterConfiguration.SaveConfig(Rules)
            Else
                If AddMode Then
                    'Remove the temporary added row
                    Rules.RemoveAt(Rules.Count - 1)
                    AddMode = False
                End If
            End If

            'Reset Edit Index
            grdRules.EditItemIndex = -1
            BindRules()

        End Sub

        Protected Sub CancelEdit(ByVal source As Object, ByVal e As CommandEventArgs)

            If AddMode Then
                'Remove the temporary added row
                Rules.RemoveAt(Rules.Count - 1)
                AddMode = False
            End If

            'Clear editrow
            grdRules.EditItemIndex = -1

            'Rebind the collection
            BindRules()

        End Sub

#End Region

    End Class

End Namespace

