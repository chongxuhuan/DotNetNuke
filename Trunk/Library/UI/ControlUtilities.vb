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

Imports System.Web.UI

Namespace DotNetNuke.UI

    Public Class ControlUtilities

        Public Shared Function FindParentControl(Of T As Control)(ByVal control As Control) As T
            Dim parent As T
            If control.Parent Is Nothing Then
                parent = Nothing
            Else
                Dim parentT As T = TryCast(control.Parent, T)
                If parentT IsNot Nothing Then
                    parent = parentT
                Else
                    parent = FindParentControl(Of T)(control.Parent)
                End If
            End If
            Return parent
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadControl loads a control and returns a reference to the control
        ''' </summary>
        ''' <typeparam name="T">The type of control to Load</typeparam>
        ''' <param name="containerControl">The parent Container Control</param>
        ''' <param name="ControlSrc">The source for the control.  This can either be a User Control (.ascx) or a compiled
        ''' control.</param>
        ''' <returns>A Control of type T</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function LoadControl(Of T As Control)(ByVal containerControl As TemplateControl, ByVal ControlSrc As String) As T
            Dim ctrl As T

            ' load the control dynamically
            If ControlSrc.ToLower.EndsWith(".ascx") Then
                ' load from a user control on the file system
                ctrl = CType(containerControl.LoadControl("~/" & ControlSrc), T)
            Else
                ' load from a typename in an assembly ( ie. server control )
                Dim objType As System.Type = Framework.Reflection.CreateType(ControlSrc)
                ctrl = CType(containerControl.LoadControl(objType, Nothing), T)
            End If

            Return ctrl
        End Function

    End Class
End Namespace
