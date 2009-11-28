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

Imports System


Namespace DotNetNuke.Services.Installer.Log

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LogEntry class provides a single entry for the Installer Log
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class LogEntry

#Region "Private Members"

        Private m_Type As LogType
        Private m_Description As String

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor builds a LogEntry from its type and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="description">The description (detail) of the entry</param>
        ''' <param name="type">The type of LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal type As LogType, ByVal description As String)
            m_Type = type
            m_Description = description
        End Sub       'New

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the type of LogEntry
        ''' </summary>
        ''' <value>A LogType</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Type() As LogType
            Get
                Return m_Type
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the description of LogEntry
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Description() As String
            Get
                If m_Description Is Nothing Then
                    Return "..."
                Else
                    Return m_Description
                End If
            End Get
        End Property

#End Region

    End Class

End Namespace