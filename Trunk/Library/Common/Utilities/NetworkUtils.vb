'
' DotNetNuke® - http:'www.dotnetnuke.com
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

Imports System.Net

Namespace DotNetNuke.Common.Utils
    Public Class NetworkUtils
        Public Shared Function GetAddress(ByVal Host As String, ByVal AddressFormat As AddressType) As String
            Dim IPAddress As String = String.Empty
            Dim addrFamily As System.Net.Sockets.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork
            Select Case AddressFormat
                Case AddressType.IPv4
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetwork
                Case AddressType.IPv6
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetworkV6
            End Select

            Dim IPE As IPHostEntry = Dns.GetHostEntry(Host)
            If Host <> IPE.HostName Then
                IPE = Dns.GetHostEntry(IPE.HostName)
            End If
            For Each IPA As IPAddress In IPE.AddressList
                If IPA.AddressFamily = addrFamily Then
                    Return IPA.ToString()
                End If
            Next

            Return String.Empty
        End Function
    End Class

    Public Enum AddressType
        IPv4
        IPv6
    End Enum
End Namespace
