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

Namespace DotNetNuke.HttpModules.Compression

    ''' <summary>
    ''' The base of anything you want to latch onto the Filter property of a <see cref="System.Web.HttpResponse"/>
    ''' object.
    ''' </summary>
    ''' <remarks>
    ''' These are generally used with HttpModule but you could really use them in
    ''' other HttpModules.  This is a general, write-only stream that writes to some underlying stream.  When implementing
    ''' a real class, you have to override void Write(byte[], int offset, int count).  Your work will be performed there.
    ''' </remarks>
    Public MustInherit Class HttpOutputFilter
        Inherits Stream

        Private _sink As Stream

        ''' <summary>
        ''' Subclasses need to call this on contruction to setup the underlying stream
        ''' </summary>
        ''' <param name="baseStream">The stream we're wrapping up in a filter</param>
        Protected Sub New(ByVal baseStream As Stream)
            _sink = baseStream
        End Sub

        ''' <summary>
        ''' Allow subclasses access to the underlying stream
        ''' </summary>
        Protected ReadOnly Property BaseStream() As Stream
            Get
                Return _sink
            End Get
        End Property

        ''' <summary>
        ''' False.  These are write-only streams
        ''' </summary>
        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' False.  These are write-only streams
        ''' </summary>
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' True.  You can write to the stream.  May change if you call Close or Dispose
        ''' </summary>
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return _sink.CanWrite
            End Get
        End Property

        ''' <summary>
        ''' Not supported.  Throws an exception saying so.
        ''' </summary>
        ''' <exception cref="NotSupportedException">Thrown.  Always.</exception>
        Public Overrides ReadOnly Property Length() As Long
            Get
                Throw New NotSupportedException
            End Get
        End Property

        ''' <summary>
        ''' Not supported.  Throws an exception saying so.
        ''' </summary>
        ''' <exception cref="NotSupportedException">Thrown.  Always.</exception>
        Public Overrides Property Position() As Long
            Get
                Throw New NotSupportedException
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException
            End Set
        End Property

        ''' <summary>
        ''' Not supported.  Throws an exception saying so.
        ''' </summary>
        ''' <exception cref="NotSupportedException">Thrown.  Always.</exception>
        Public Overrides Function Seek(ByVal offset As Long, ByVal direction As SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Not supported.  Throws an exception saying so.
        ''' </summary>
        ''' <exception cref="NotSupportedException">Thrown.  Always.</exception>
        Public Overrides Sub SetLength(ByVal length As Long)
            Throw New NotSupportedException()
        End Sub

        ''' <summary>
        ''' Closes this Filter and the underlying stream.
        ''' </summary>
        ''' <remarks>
        ''' If you override, call up to this method in your implementation.
        ''' </remarks>
        Public Overrides Sub Close()
            _sink.Close()
        End Sub

        ''' <summary>
        ''' Fluses this Filter and the underlying stream.
        ''' </summary>
        ''' <remarks>
        ''' If you override, call up to this method in your implementation.
        ''' </remarks>
        Public Overrides Sub Flush()
            _sink.Flush()
        End Sub

        ''' <summary>
        ''' Not supported.
        ''' </summary>
        ''' <param name="buffer">The buffer to write into.</param>
        ''' <param name="offset">The offset on the buffer to write into</param>
        ''' <param name="count">The number of bytes to write.  Must be less than buffer.Length</param>
        ''' <returns>An int telling you how many bytes were written</returns>
        Public Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw New NotSupportedException()
        End Function

    End Class

End Namespace
