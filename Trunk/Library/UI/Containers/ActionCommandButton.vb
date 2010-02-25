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
    ''' Class	 : ActionCommandButton
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionCommandButton provides a button for a single action.
    ''' </summary>
    ''' <remarks>
    ''' ActionBase inherits from CommandButton, and implements the IActionControl Interface.
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	12/23/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ActionCommandButton
        Inherits CommandButton
        Implements IActionControl

#Region "Private Members"

        Private _ActionManager As ActionManager
        Private _ModuleAction As ModuleAction
        Private _ModuleControl As IModuleControl

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleAction for this Action control
        ''' </summary>
        ''' <returns>A ModuleAction object</returns>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleAction() As ModuleAction
            Get
                If _ModuleAction Is Nothing Then
                    _ModuleAction = ModuleControl.ModuleContext.Actions.GetActionByCommandName(CommandName)
                End If
                Return _ModuleAction
            End Get
            Set(ByVal value As ModuleAction)
                _ModuleAction = value
            End Set
        End Property

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
            'Call base class method to ensure Control Tree is built
            MyBase.CreateChildControls()

            'Set Causes Validation and Enables ViewState to false
            CausesValidation = False
            EnableViewState = False
        End Sub

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
        ''' OnButtonClick runs when the underlying CommandButton is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnButtonClick(ByVal e As System.EventArgs)
            MyBase.OnButtonClick(e)
            If Not ActionManager.ProcessAction(ModuleAction) Then
                OnAction(New ActionEventArgs(ModuleAction, ModuleControl.ModuleContext.Configuration))
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs when just before the Render phase of the Page Lifecycle
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            If ModuleAction IsNot Nothing AndAlso ActionManager.IsVisible(ModuleAction) Then
                Text = ModuleAction.Title
                CommandArgument = ModuleAction.ID.ToString()

                If DisplayIcon AndAlso (Not String.IsNullOrEmpty(ModuleAction.Icon) OrElse Not String.IsNullOrEmpty(ImageUrl)) Then
                    If Not String.IsNullOrEmpty(ImageUrl) Then
                        ImageUrl = ModuleControl.ModuleContext.Configuration.ContainerPath.Substring(0, ModuleControl.ModuleContext.Configuration.ContainerPath.LastIndexOf("/") + 1) & ImageUrl
                    Else
                        If ModuleAction.Icon.IndexOf("/") > 0 Then
                            ImageUrl = ModuleAction.Icon
                        Else
                            ImageUrl = "~/images/" & ModuleAction.Icon
                        End If
                    End If
                End If
                ActionManager.GetClientScriptURL(ModuleAction, Me)
            Else
                Visible = False
            End If

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