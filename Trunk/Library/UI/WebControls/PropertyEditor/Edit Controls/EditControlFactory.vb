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

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Reflection

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      EditControlFactory
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditControlFactory control provides a factory for creating the 
    ''' appropriate Edit Control
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/14/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EditControlFactory

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateEditControl creates the appropriate Control based on the EditorField or
        ''' TypeDataField
        ''' </summary>
        ''' <param name="editorInfo">An EditorInfo object</param>
        ''' <history>
        '''     [cnurse]	03/06/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateEditControl(ByVal editorInfo As EditorInfo) As EditControl

            Dim propEditor As EditControl

            If editorInfo.Editor = "UseSystemType" Then
                Dim type As System.Type = System.Type.GetType(editorInfo.Type)
                'Use System Type

                Select Case type.FullName
                    Case "System.DateTime"
                        propEditor = New DateTimeEditControl
                    Case "System.Boolean"
                        propEditor = New CheckEditControl
                    Case "System.Int32", "System.Int16"
                        propEditor = New IntegerEditControl
                    Case Else
                        If type.IsEnum Then
                            propEditor = New EnumEditControl(editorInfo.Type)
                        Else
                            propEditor = New TextEditControl(editorInfo.Type)
                        End If
                End Select
            Else
                'Use Editor
                Dim editType As System.Type = System.Type.GetType(editorInfo.Editor, True, True)
                propEditor = CType(Activator.CreateInstance(editType), EditControl)
            End If

            propEditor.ID = editorInfo.Name
            propEditor.Name = editorInfo.Name

            propEditor.EditMode = editorInfo.EditMode
            propEditor.Required = editorInfo.Required

            propEditor.Value = editorInfo.Value
            propEditor.OldValue = editorInfo.Value

            propEditor.CustomAttributes = editorInfo.Attributes

            Return propEditor

        End Function

    End Class

End Namespace

