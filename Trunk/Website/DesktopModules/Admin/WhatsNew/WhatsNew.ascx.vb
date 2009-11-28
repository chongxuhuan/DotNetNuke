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
Imports System.Xml.XPath
Imports System.Collections.Generic
Imports System.IO

Namespace DotNetNuke.Modules.Admin.Host

    Partial Class WhatsNew
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim resourcefile As String = Server.MapPath(LocalResourceFile & ".ascx.resx")
            If File.Exists(resourcefile) Then
                Dim document As XPathDocument = New XPathDocument(resourcefile)
                Dim navigator As XPathNavigator = document.CreateNavigator()

                Dim nodes As XPathNodeIterator = navigator.Select("/root/data[starts-with(@name, 'WhatsNew')]/@name")

                Dim releasenotes As New List(Of ReleaseInfo)

                While nodes.MoveNext()
                    Dim key As String = nodes.Current.Value
                    Dim version As String = String.Format(Localization.GetString("notestitle.text", LocalResourceFile), key.Replace("WhatsNew.", String.Empty))
                    releasenotes.Add(New ReleaseInfo(Localization.GetString(key, LocalResourceFile), version))

                End While

                releasenotes.Sort(AddressOf CompareReleaseInfo)

                WhatsNewList.DataSource = releasenotes
                WhatsNewList.DataBind()

                header.InnerHtml = Localization.GetString("header.text", LocalResourceFile)
                footer.InnerHtml = Localization.GetString("footer.text", LocalResourceFile)

            End If

        End Sub

        Private Function CompareReleaseInfo(ByVal notes1 As ReleaseInfo, ByVal notes2 As ReleaseInfo) As Integer
            'We do this in reverse order so that we have the latest release at the top of the list
            Return notes2.Version.CompareTo(notes1.Version)
        End Function

        Friend Class ReleaseInfo

#Region "Constructors"

            ''' <summary>
            ''' Initializes a new instance of the ReleaseInfo class.
            ''' </summary>
            ''' <param name="notes"></param>
            ''' <param name="version"></param>
            Public Sub New(ByVal notes As String, ByVal version As String)
                _notes = notes
                _version = version
            End Sub

#End Region

#Region "Fields"

            Private _notes As String
            Private _version As String

#End Region

#Region "Properties"

            Public Property Notes() As String
                Get
                    Return _notes
                End Get
                Set(ByVal Value As String)
                    _notes = Value
                End Set
            End Property

            Public Property Version() As String
                Get
                    Return _version
                End Get
                Set(ByVal Value As String)
                    _version = Value
                End Set
            End Property

#End Region

        End Class
    End Class
End Namespace