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

Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Net
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports System.Text


Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SqlUtils class provides Shared/Static methods for working with SQL Server related code
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Public Class SqlUtils
#Region "Public Methods"

#End Region

        ''' <summary>
        ''' function to translate sql exceptions to readable messages. 
        ''' It also captures cases where sql server is not available and guards against
        ''' database connection details being leaked
        ''' </summary>
        ''' <param name="exc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TranslateSQLException(ByVal exc As SqlException) As String
            Dim i As Integer
            Dim errorMessages As New StringBuilder()
            For i = 0 To exc.Errors.Count - 1
                Dim sqlError As SqlError = exc.Errors(i)
                Dim filteredMessage As String = String.Empty
                Select Case sqlError.Number
                    Case 17
                        filteredMessage = "Sql server does not exist or access denied"
                    Case 4060
                        filteredMessage = "Invalid Database"
                    Case 18456
                        filteredMessage = "Sql login failed"
                    Case 1205
                        filteredMessage = "Sql deadlock victim"
                    Case Else
                        filteredMessage = exc.ToString
                End Select

                errorMessages.Append("<b>Index #:</b> " & i.ToString() & "<br/>" _
                    & "<b>Source:</b> " & sqlError.Source & "<br/>" _
                    & "<b>Class:</b> " & sqlError.Class & "<br/>" _
                    & "<b>Number:</b> " & sqlError.Number & "<br/>" _
                    & "<b>Procedure:</b> " & sqlError.Procedure & "<br/>" _
                    & "<b>Message:</b> " & filteredMessage.ToString & "<br/>")
            Next i
            Return errorMessages.ToString
        End Function

    End Class

End Namespace
