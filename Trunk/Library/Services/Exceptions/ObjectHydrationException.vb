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

Imports System.Collections.Generic
Imports System.Security.Permissions
Imports System.Runtime.Serialization

Namespace DotNetNuke.Services.Exceptions

    <Serializable()> Public Class ObjectHydrationException
        Inherits BasePortalException

        Private _Type As System.Type
        Private _Columns As List(Of String)

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception, ByVal type As Type, ByVal dr As IDataReader)
            MyBase.New(message, innerException)
            _Type = type

            _Columns = New List(Of String)()
            For Each row As DataRow In dr.GetSchemaTable.Rows
                _Columns.Add(row("ColumnName").ToString())
            Next
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Property Columns() As List(Of String)
            Get
                Return _Columns
            End Get
            Set(ByVal value As List(Of String))
                _Columns = value
            End Set
        End Property

        Public Property Type() As System.Type
            Get
                Return _Type
            End Get
            Set(ByVal value As System.Type)
                _Type = value
            End Set
        End Property

        <SecurityPermission(SecurityAction.Demand, SerializationFormatter:=True)> _
        Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            ' Serialize this class' state and then call the base class GetObjectData
            MyBase.GetObjectData(info, context)
        End Sub

        Public Overrides ReadOnly Property Message() As String
            Get
                Dim _Message As String = MyBase.Message
                _Message += " Expecting - " + Type.ToString() + "."
                _Message += " Returned - "
                For Each columnName As String In Columns
                    _Message += columnName + ", "
                Next

                Return _Message
            End Get
        End Property

    End Class

End Namespace
