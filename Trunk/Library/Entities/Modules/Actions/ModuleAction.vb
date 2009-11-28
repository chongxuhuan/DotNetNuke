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

Namespace DotNetNuke.Entities.Modules.Actions

    '''-----------------------------------------------------------------------------
    ''' Project		: DotNetNuke
    ''' Class		: ModuleAction
    '''-----------------------------------------------------------------------------
    ''' <summary>
    ''' Each Module Action represents a separate functional action as defined by the
    ''' associated module.
    ''' </summary>
    ''' <remarks>A module action is used to define a specific function for a given module.
    ''' Each module can define one or more actions which the portal will present to the
    ''' user.  These actions may be presented as a menu, a dropdown list or even a group
    ''' of linkbuttons.
    ''' <seealso cref="T:DotNetNuke.ModuleActionCollection" /></remarks>
    ''' <history>
    ''' 	[Joe] 	10/9/2003	Created
    ''' </history>
    '''-----------------------------------------------------------------------------
	Public Class ModuleAction

#Region "Private Members"

        Private _actions As ModuleActionCollection
        Private _id As Integer
        Private _commandName As String
        Private _commandArgument As String
        Private _title As String
        Private _icon As String
        Private _url As String
        Private _clientScript As String
        Private _useActionEvent As Boolean
        Private _secure As SecurityAccessLevel
        Private _visible As Boolean
        Private _newwindow As Boolean

#End Region

#Region "Constructors"

        Public Sub New(ByVal ID As Integer)
            Me.New(ID, "", "", "", "", "", "", False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String)
            Me.New(ID, Title, CmdName, "", "", "", "", False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String)
            Me.New(ID, Title, CmdName, CmdArg, "", "", "", False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String)
            Me.New(ID, Title, CmdName, CmdArg, Icon, "", "", False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String)
            Me.New(ID, Title, CmdName, CmdArg, Icon, Url, "", False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String)
            Me.New(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, False, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String, ByVal UseActionEvent As Boolean)
            Me.New(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, SecurityAccessLevel.Anonymous, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel)
            Me.New(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, True, False)
        End Sub

        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel, ByVal Visible As Boolean)
            Me.New(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible, False)
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="T:DotNetNuke.ModuleAction"/> class
        ''' using the specified parameters
        ''' </summary>
        ''' <param name="ID">This is the identifier to use for this action.</param>
        ''' <param name="Title">This is the title that will be displayed for this action</param>
        ''' <param name="CmdName">The command name passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="CmdArg">The command argument passed to the client when this action is 
        ''' clicked.</param>
        ''' <param name="Icon">The URL of the Icon to place next to this action</param>
        ''' <param name="Url">The destination URL to redirect the client browser when this 
        ''' action is clicked.</param>
        ''' <param name="UseActionEvent">Determines whether client will receive an event
        ''' notification</param>
        ''' <param name="Secure">The security access level required for access to this action</param>
        ''' <param name="Visible">Whether this action will be displayed</param>
        ''' <remarks>The moduleaction constructor is used to set the various properties of 
        ''' the <see cref="T:DotNetNuke.ModuleAction" /> class at the time the instance is created.
        ''' </remarks>
        ''' <history>
        ''' 	[Joe] 	        10/26/2003	Created
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel, ByVal Visible As Boolean, ByVal NewWindow As Boolean)
            _id = ID
            _title = Title
            _commandName = CmdName
            _commandArgument = CmdArg
            _icon = Icon
            _url = Url
            _clientScript = ClientScript
            _useActionEvent = UseActionEvent
            _secure = Secure
            _visible = Visible
            _newwindow = NewWindow
        End Sub

#End Region

#Region "Public Properties"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the action node contains any child actions.
        ''' </summary>
        ''' <returns>True if child actions exist, false if child actions do not exist.</returns>
        ''' <remarks>Each action may contain one or more child actions in the
        ''' <see cref="P:DotNetNuke.ModuleAction.Actions"/> property.  When displayed via
        ''' the <see cref="T:DotNetNuke.Containers.Actions"/> control, these subactions are
        ''' shown as sub-menus.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/26/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function HasChildren() As Boolean
            Return (Actions.Count > 0)
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' The Actions property allows the user to create a heirarchy of actions, with
        ''' each action having sub-actions.
        ''' </summary>
        ''' <value>Returns a collection of ModuleActions.</value>
        ''' <remarks>Each action may contain one or more child actions.  When displayed via
        ''' the <see cref="T:DotNetNuke.Containers.Actions"/> control, these subactions are
        ''' shown as sub-menus.  If other Action controls are implemented, then
        ''' sub-actions may or may not be supported for that control type.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/26/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Actions() As ModuleActionCollection
            Get
                If _actions Is Nothing Then
                    _actions = New ModuleActionCollection
                End If
                Return _actions
            End Get
            Set(ByVal Value As ModuleActionCollection)
                _actions = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' A Module Action ID is a identifier that can be used in a Module Action Collection
        ''' to find a specific Action. 
        ''' </summary>
        ''' <value>The integer ID of the current <see cref="T:DotNetNuke.ModuleAction"/>.</value>
        ''' <remarks>When building a heirarchy of <see cref="T:DotNetNuke.ModuleAction">ModuleActions</see>, 
        ''' the ID is used to link the child and parent actions.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/18/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property ID() As Integer
            Get
                Return _id
            End Get
            Set(ByVal Value As Integer)
                _id = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the current action should be displayed.
        ''' </summary>
        ''' <value>A boolean value that determines if the current action should be displayed</value>
        ''' <remarks>If Visible is false, then the action is always hidden.  If Visible
        ''' is true then the action may be visible depending on the security access rights
        ''' specified by the <see cref="P:DotNetNuke.ModuleAction.Secure"/> property.  By
        ''' utilizing a custom method in your module, you can encapsulate specific business
        ''' rules to determine if the Action should be visible.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/26/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Visible() As Boolean
            Get
                Return _visible
            End Get
            Set(ByVal Value As Boolean)
                _visible = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the value indicating the <see cref="T:DotnetNuke.SecurityAccessLevel" /> that is required
        ''' to access this <see cref="T:DotNetNuke.ModuleAction" />.
        ''' </summary>
        ''' <value>The value indicating the <see cref="T:DotnetNuke.SecurityAccessLevel" /> that is required
        ''' to access this <see cref="T:DotNetNuke.ModuleAction" /></value>
        ''' <remarks>The security access level determines the roles required by the current user in
        ''' order to access this module action.</remarks>
        ''' <history>
        ''' 	[jbrinkman] 	12/27/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Secure() As SecurityAccessLevel
            Get
                Return _secure
            End Get
            Set(ByVal Value As SecurityAccessLevel)
                _secure = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' A Module Action CommandName represents a string used by the ModuleTitle to notify
        ''' the parent module that a given Module Action was selected in the Module Menu.
        ''' </summary>
        ''' <value>The name of the command to perform.</value>
        ''' <remarks>
        ''' Use the CommandName property to determine the command to perform. The CommandName 
        ''' property can contain any string set by the programmer. The programmer can then 
        ''' identify the command name in code and perform the appropriate tasks.
        ''' </remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CommandName() As String
            Get
                Return _commandName
            End Get
            Set(ByVal Value As String)
                _commandName = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' A Module Action CommandArgument provides additional information and 
        ''' complements the CommandName.
        ''' </summary>
        ''' <value>A string that contains the argument for the command.</value>
        ''' <remarks>
        ''' The CommandArgument can contain any string set by the programmer. The 
        ''' CommandArgument property complements the <see cref="P:DotNetNuke.ModuleAction.CommandName" /> 
        '''  property by allowing you to provide any additional information for the command. 
        ''' For example, you can set the CommandName property to "Sort" and set the 
        ''' CommandArgument property to "Ascending" to specify a command to sort in ascending 
        ''' order.
        ''' </remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CommandArgument() As String
            Get
                Return _commandArgument
            End Get
            Set(ByVal Value As String)
                _commandArgument = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the string that is displayed in the Module Menu
        ''' that represents a given menu action.
        ''' </summary>
        ''' <value>The string value that is displayed to represent the module action.</value>
        ''' <remarks>The title property is displayed by the Actions control for each module
        ''' action.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal Value As String)
                _title = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the URL for the icon file that is displayed for the given 
        ''' <see cref="T:DotNetNuke.ModuleAction" />.
        ''' </summary>
        ''' <value>The URL for the icon that is displayed with the module action.</value>
        ''' <remarks>The URL for the icon is a simple string and is not checked for formatting.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Icon() As String
            Get
                Return _icon
            End Get
            Set(ByVal Value As String)
                _icon = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the URL to which the user is redirected when the 
        ''' associated Module Menu Action is selected.  
        ''' </summary>
        ''' <value>The URL to which the user is redirected when the 
        ''' associated Module Menu Action is selected.</value>
        ''' <remarks>If the URL is present then the Module Action Event is not fired.  
        ''' If the URL is empty then the Action Event is fired and is passed the value 
        ''' of the associated Command property.</remarks>
        ''' <history>
        ''' 	[Joe] 	10/9/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal Value As String)
                _url = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets javascript which will be run in the clients browser
        ''' when the associated Module menu Action is selected. prior to a postback.
        ''' </summary>
        ''' <value>The Javascript which will be run during the menuClick event</value>
        ''' <remarks>If the ClientScript property is present then it is called prior
        ''' to the postback occuring. If the ClientScript returns false then the postback
        ''' is canceled.  If the ClientScript is empty then the Action Event is fired and 
        ''' is passed the value of the associated Command property.</remarks>
        ''' <history>
        ''' 	[jbrinkman]	5/21/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ClientScript() As String
            Get
                Return _clientScript
            End Get
            Set(ByVal Value As String)
                _clientScript = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value that determines if a local ActionEvent is fired when the 
        ''' <see cref="T:DotNetNuke.ModuleAction" /> contains a URL. 
        ''' </summary>
        ''' <value>A boolean indicating whether to fire the ActionEvent.</value>
        ''' <remarks>When a MenuAction is clicked, an event is fired within the Actions 
        ''' control.  If the UseActionEvent is true then the Actions control will forward
        ''' the event to the parent skin which will then attempt to raise the event to
        ''' the appropriate module.  If the UseActionEvent is false, and the URL property
        ''' is set, then the Actions control will redirect the response to the URL.  In
        ''' all cases, an ActionEvent is raised if the URL is not set.</remarks>
        ''' <history>
        ''' 	[jbrinkman] 	12/22/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property UseActionEvent() As Boolean
            Get
                Return _useActionEvent
            End Get
            Set(ByVal Value As Boolean)
                _useActionEvent = Value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets a value that determines if a new window is opened when the 
        ''' DoAction() method is called. 
        ''' </summary>
        ''' <value>A boolean indicating whether to open a new window.</value>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[jbrinkman] 	12/22/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property NewWindow() As Boolean
            Get
                Return _newwindow
            End Get
            Set(ByVal Value As Boolean)
                _newwindow = Value
            End Set
        End Property
#End Region

    End Class

End Namespace
