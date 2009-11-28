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

Imports System
Imports System.IO

Imports System.IO.Compression

Namespace DotNetNuke.HttpModules.Compression

    ''' <summary>
    ''' This is a little filter to support HTTP compression using GZip
    ''' </summary>
    Public Class GZipFilter
        Inherits CompressingFilter

        ''' <summary>
        ''' compression stream member
        ''' has to be a member as we can only have one instance of the
        ''' actual filter class
        ''' </summary>
        Private m_stream As GZipStream = Nothing

        ''' <summary>
        ''' Primary constructor.  Need to pass in a stream to wrap up with gzip.
        ''' </summary>
        ''' <param name="baseStream">The stream to wrap in gzip.  Must have CanWrite.</param>
        Public Sub New(ByVal baseStream As Stream)
            MyBase.New(baseStream)
            m_stream = New GZipStream(baseStream, CompressionMode.Compress)
        End Sub

        ''' <summary>
        ''' Write out bytes to the underlying stream after compressing them Imports deflate
        ''' </summary>
        ''' <param name="buffer">The array of bytes to write</param>
        ''' <param name="offset">The offset into the supplied buffer to start</param>
        ''' <param name="count">The number of bytes to write</param>
        Public Overrides Sub Write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            If Not HasWrittenHeaders Then
                WriteHeaders()
            End If
            m_stream.Write(buffer, offset, count)
        End Sub

        ''' <summary>
        ''' Return the Http name for this encoding.  Here, deflate.
        ''' </summary>
        Public Overrides ReadOnly Property ContentEncoding() As String
            Get
                Return "gzip"
            End Get
        End Property

        ''' <summary>
        ''' Closes this Filter and calls the base class implementation.
        ''' </summary>
        Public Overrides Sub Close()
            m_stream.Close()
        End Sub

        ''' <summary>
        ''' Flushes that the filter out to underlying storage
        ''' </summary>
        Public Overrides Sub Flush()
            m_stream.Flush()
        End Sub

    End Class

End Namespace
