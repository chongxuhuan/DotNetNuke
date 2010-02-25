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

Imports System
Imports System.IO
Imports System.Xml
Imports System.Xml.Schema

Namespace DotNetNuke.Common

    Public Class XmlValidatorBase

#Region "Private Members"

        Dim _errs As New ArrayList
        Dim _reader As XmlTextReader
        Dim _schemaSet As New XmlSchemaSet

#End Region

#Region "Public Properties"

        Public Property Errors() As ArrayList
            Get
                Return _errs
            End Get
            Set(ByVal Value As ArrayList)
                _errs = Value
            End Set
        End Property

        Public ReadOnly Property SchemaSet() As XmlSchemaSet
            Get
                Return _schemaSet
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Sub ValidationCallBack(ByVal sender As Object, ByVal args As ValidationEventArgs)
            _errs.Add(args.Message)
        End Sub

#End Region

#Region "Public Methods"

        Public Function IsValid() As Boolean
            'There is a bug here which I haven't been able to fix.
            'If the XML Instance does not include a reference to the
            'schema, then the validation fails.  If the reference exists
            'the the validation works correctly.

            'Create a validating reader
            Dim settings As New XmlReaderSettings()
            settings.Schemas = _schemaSet
            settings.ValidationType = ValidationType.Schema

            'Set the validation event handler.
            AddHandler settings.ValidationEventHandler, New ValidationEventHandler(AddressOf ValidationCallBack)
            Dim vreader As XmlReader = XmlReader.Create(_reader, settings)

            'Read and validate the XML data.
            While vreader.Read()
            End While

            'Close the reader.
            vreader.Close()

            Return (_errs.Count = 0)
        End Function

        Public Overridable Function Validate(ByVal xmlStream As Stream) As Boolean

            xmlStream.Seek(0, SeekOrigin.Begin)
            _reader = New XmlTextReader(xmlStream)

            Return IsValid()
        End Function

        Public Overridable Function Validate(ByVal filename As String) As Boolean

            _reader = New XmlTextReader(filename)

            Return IsValid()
        End Function


#End Region

    End Class

End Namespace
