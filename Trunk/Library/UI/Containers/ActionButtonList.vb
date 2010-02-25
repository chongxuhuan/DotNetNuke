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

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Modules

Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : ActionButtonList
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionButtonList provides a list of buttons for a group of actions of the same type.
    ''' </summary>
    ''' <remarks>
    ''' ActionButtonList inherits from CompositeControl, and implements the IActionControl 
    ''' Interface.  It uses a single ActionCommandButton for each Action.
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	12/23/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ActionButtonList
        Inherits CompositeControl
        Implements IActionControl

#Region "Private Members"

        Private _buttonSeparator As String = "&nbsp;&nbsp;"
        Private _commandName As String = ""
        Private _displayLink As Boolean = True
        Private _displayIcon As Boolean = False
        Private _imageURL As String

        Private _ActionManager As ActionManager
        Private _ModuleActions As ModuleActionCollection
        Private _ModuleControl As IModuleControl

#End Region

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleActionCollection to bind to the list
        ''' </summary>
        ''' <value>A ModuleActionCollection</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ModuleActions() As ModuleActionCollection
            Get
                If _ModuleActions Is Nothing Then
                    _ModuleActions = ModuleControl.ModuleContext.Actions.GetActionsByCommandName(CommandName)
                End If
                Return _ModuleActions
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Separator between Buttons
        ''' </summary>
        ''' <remarks>Defaults to 2 non-breaking spaces</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ButtonSeparator() As String
            Get
                Return _buttonSeparator
            End Get
            Set(ByVal Value As String)
                _buttonSeparator = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Command Name
        ''' </summary>
        ''' <remarks>Maps to ModuleActionType in DotNetNuke.Entities.Modules.Actions</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CommandName() As String
            Get
                Return _commandName
            End Get
            Set(ByVal Value As String)
                _commandName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the icon is displayed
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DisplayIcon() As Boolean
            Get
                Return _displayIcon
            End Get
            Set(ByVal Value As Boolean)
                _displayIcon = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the link is displayed
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DisplayLink() As Boolean
            Get
                Return _displayLink
            End Get
            Set(ByVal Value As Boolean)
                _displayLink = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Icon used
        ''' </summary>
        ''' <remarks>Defaults to the icon defined in Action</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	12/23/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ImageURL() As String
            Get
                Return _imageURL
            End Get
            Set(ByVal Value As String)
                _imageURL = Value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnAction raises the Action Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnAction(ByVal e As ActionEventArgs)
            RaiseEvent Action(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs when the control is loaded into the Control Tree
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            For Each action As ModuleAction In ModuleActions
                If action IsNot Nothing AndAlso ActionManager.IsVisible(action) Then
                    'Create a new ActionCommandButton
                    Dim actionButton As New ActionCommandButton()

                    'Set all the properties
                    actionButton.ModuleAction = action
                    actionButton.ModuleControl = ModuleControl
                    actionButton.CommandName = CommandName
                    actionButton.CssClass = CssClass
                    actionButton.DisplayLink = DisplayLink
                    actionButton.DisplayIcon = DisplayIcon
                    actionButton.ImageUrl = ImageURL

                    'Add a handler for the Action Event
                    AddHandler actionButton.Action, AddressOf ActionButtonClick

                    Me.Controls.Add(actionButton)

                    Me.Controls.Add(New LiteralControl(ButtonSeparator))
                End If
            Next
            Me.Visible = (Me.Controls.Count > 0)
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ActionButtonClick handles the Action event of the contained ActionCommandButton(s)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ActionButtonClick(ByVal sender As Object, ByVal e As ActionEventArgs)
            OnAction(e)
        End Sub

#End Region

#Region "IActionControl Members"

        Public Event Action As ActionEventHandler Implements IActionControl.Action

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ActionManager instance for this Action control
        ''' </summary>
        ''' <returns>An ActionManager object</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ActionManager() As ActionManager Implements IActionControl.ActionManager
            Get
                If _ActionManager Is Nothing Then
                    _ActionManager = New ActionManager(Me)
                End If
                Return _ActionManager
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleControl instance for this Action control
        ''' </summary>
        ''' <returns>An IModuleControl object</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleControl() As IModuleControl Implements IActionControl.ModuleControl
            Get
                Return _ModuleControl
            End Get
            Set(ByVal Value As IModuleControl)
                _ModuleControl = Value
            End Set
        End Property

#End Region

    End Class

End Namespace