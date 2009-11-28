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
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.Containers
    Partial Class LinkActions
        Inherits UI.Containers.ActionBase

        Protected _itemSeparator As String = ""

        Public Property ItemSeparator() As String
            Get
                Return _itemSeparator
            End Get
            Set(ByVal Value As String)
                _itemSeparator = Value
            End Set
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Put user code to initialize the page here
            Try
                With ActionRoot
                    ' Is Root Menu visible?
                    If .Visible Then
                        If Me.Controls.Count > 0 Then
                            Me.Controls.Clear()
                        End If

                        Dim PreSpacer As New LiteralControl(ItemSeparator)
                        Me.Controls.Add(PreSpacer)

                        ' Add Menu Items
                        Dim action As ModuleAction
                        For Each action In ActionRoot.Actions
                            If action.Title = "~" Then
                                ' not supported in this Action object
                            Else
                                If action.Visible Then
                                    If (ModuleControl.ModuleContext.EditMode = True AndAlso IsAdminControl() = False) OrElse _
                                            (action.Secure <> SecurityAccessLevel.Anonymous AndAlso _
                                             action.Secure <> SecurityAccessLevel.View) Then
                                        Dim ModuleActionLink As New LinkButton
                                        ModuleActionLink.Text = action.Title
                                        ModuleActionLink.CssClass = "CommandButton"
                                        ModuleActionLink.ID = "lnk" & action.ID.ToString

                                        AddHandler ModuleActionLink.Click, AddressOf LinkAction_Click

                                        Me.Controls.Add(ModuleActionLink)
                                        Dim Spacer As New LiteralControl(ItemSeparator)
                                        Me.Controls.Add(Spacer)
                                    End If
                                End If
                            End If
                        Next
                    End If
                End With

                'Need to determine if this action list actually has any items.
                If Me.Controls.Count > 0 Then
                    Me.Visible = True
                Else
                    Me.Visible = False
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If _itemSeparator.Length = 0 Then
                _itemSeparator = "&nbsp;&nbsp;"
            End If
        End Sub

        Private Sub LinkAction_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Try
                ProcessAction(DirectCast(sender, LinkButton).ID.Substring(3))
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

    End Class

End Namespace