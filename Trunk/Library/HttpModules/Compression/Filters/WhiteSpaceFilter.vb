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
Imports System.Text.RegularExpressions
Imports System.IO.Compression

Namespace DotNetNuke.HttpModules.Compression

    Class WhitespaceFilter
        Inherits HttpOutputFilter
        Private Shared _reg As Regex

        ''' <summary>
        ''' Primary constructor.
        ''' </summary>
        ''' <param name="baseStream">The stream to wrap in gzip.  Must have CanWrite.</param>
        ''' <param name="reg"></param>
        Public Sub New(ByVal baseStream As Stream, ByVal reg As Regex)
            MyBase.New(baseStream)
            _reg = reg
        End Sub

        ''' <summary>
        ''' Write out bytes to the underlying stream after removing the white space
        ''' </summary>
        ''' <param name="buf">The array of bytes to write</param>
        ''' <param name="offset">The offset into the supplied buffer to start</param>
        ''' <param name="count">The number of bytes to write</param>
        Public Overrides Sub Write(ByVal buf As Byte(), ByVal offset As Integer, ByVal count As Integer)
            Dim data() As Byte
            ReDim data(count)

            Buffer.BlockCopy(buf, offset, data, 0, count)
            Dim html As String = System.Text.Encoding.Default.GetString(buf)

            html = _reg.Replace(html, String.Empty)

            Dim outdata As Byte() = System.Text.Encoding.Default.GetBytes(html)
            BaseStream.Write(outdata, 0, outdata.GetLength(0))
        End Sub

    End Class

End Namespace
