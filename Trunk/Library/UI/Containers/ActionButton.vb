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

Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : ActionButton
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionButton provides a button (or group of buttons) for action(s).
    ''' </summary>
    ''' <remarks>
    ''' ActionBase inherits from UserControl, and implements the IActionControl Interface.
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/07/2004	Documented
    '''     [cnurse]    12/15/2007  Deprectaed and Refactored to use ActionButtonList 
    '''                             by Containment
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Obsolete("This class has been deprecated in favour of the new ActionCommandButton and ActionButtonList.")> _
    Public Class ActionButton
        Inherits ActionBase

#Region "Private Members"

        Private _ButtonList As ActionButtonList

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Command Name
        ''' </summary>
        ''' <remarks>Maps to ModuleActionType in DotNetNuke.Entities.Modules.Actions</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CommandName() As String
            Get
                EnsureChildControls()
                Return _ButtonList.CommandName
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                _ButtonList.CommandName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the CSS Class
        ''' </summary>
        ''' <remarks>Defaults to 'CommandButton'</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CssClass() As String
            Get
                EnsureChildControls()
                Return _ButtonList.CssClass
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                _ButtonList.CssClass = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the link is displayed
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DisplayLink() As Boolean
            Get
                EnsureChildControls()
                Return _ButtonList.DisplayLink
            End Get
            Set(ByVal Value As Boolean)
                EnsureChildControls()
                _ButtonList.DisplayLink = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the icon is displayed
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DisplayIcon() As Boolean
            Get
                EnsureChildControls()
                Return _ButtonList.DisplayIcon
            End Get
            Set(ByVal Value As Boolean)
                EnsureChildControls()
                _ButtonList.DisplayIcon = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Icon used
        ''' </summary>
        ''' <remarks>Defaults to the icon defined in Action</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IconFile() As String
            Get
                EnsureChildControls()
                Return _ButtonList.ImageURL
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                _ButtonList.ImageURL = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Separator between Buttons
        ''' </summary>
        ''' <remarks>Defaults to 2 non-breaking spaces</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	6/29/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ButtonSeparator() As String
            Get
                EnsureChildControls()
                Return _ButtonList.ButtonSeparator
            End Get
            Set(ByVal Value As String)
                EnsureChildControls()
                _ButtonList.ButtonSeparator = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Action_Click responds to an Action Event in the contained actionButtonList
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Action_Click(ByVal sender As Object, ByVal e As ActionEventArgs)
            ProcessAction(e.Action.ID.ToString)
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls builds the control tree
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            _ButtonList = New ActionButtonList
            AddHandler _ButtonList.Action, AddressOf Action_Click

            Me.Controls.Add(_ButtonList)
        End Sub

#End Region

    End Class

End Namespace