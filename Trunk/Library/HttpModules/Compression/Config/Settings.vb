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

Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web.Caching
Imports System.Xml
Imports System.Xml.XPath
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.HttpModules.Compression

    ''' <summary>
    ''' This class encapsulates the settings for an HttpCompressionModule
    ''' </summary>
    <Serializable()> _
    Public Class Settings

#Region "Private Members"

        Private _preferredAlgorithm As Algorithms
        Private _excludedPaths As StringCollection
        Private _reg As Regex
        Private _whitespace As Boolean

#End Region

#Region "Constructor"

        Private Sub New()
            _preferredAlgorithm = Algorithms.None
            _excludedPaths = New StringCollection()
            _whitespace = False
        End Sub

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' The default settings.  Deflate + normal.
        ''' </summary>
        Public Shared ReadOnly Property [Default]() As Settings
            Get
                Return New Settings()
            End Get
        End Property

        ''' <summary>
        ''' The preferred algorithm to use for compression
        ''' </summary>
        Public ReadOnly Property PreferredAlgorithm() As Algorithms
            Get
                Return _preferredAlgorithm
            End Get
        End Property

        ''' <summary>
        ''' The regular expression used for Whitespace removal
        ''' </summary>
        Public ReadOnly Property Reg() As Regex
            Get
                Return _reg
            End Get
        End Property

        ''' <summary>
        ''' Determines if Whitespace filtering is enabled
        ''' </summary>
        Public ReadOnly Property Whitespace() As Boolean
            Get
                Return _whitespace
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the current settings from the xml config file
        ''' </summary>
        Public Shared Function GetSettings() As Settings

            Dim _Settings As Settings = CType(DataCache.GetCache("CompressionConfig"), Settings)

            If _Settings Is Nothing Then
                _Settings = Settings.Default

                'Place this in a try/catch as during install the host settings will not exist
                Try
                    _Settings._preferredAlgorithm = CType(Host.HttpCompressionAlgorithm, Algorithms)
                    _Settings._whitespace = Host.WhitespaceFilter
                Catch e As Exception
                End Try

                Dim filePath As String = DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.Compression)

                'Create a FileStream for the Config file
                Dim fileReader As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim doc As New XPathDocument(fileReader)

                _Settings._reg = New Regex(doc.CreateNavigator().SelectSingleNode("compression/whitespace").Value)

                For Each nav As XPathNavigator In doc.CreateNavigator().Select("compression/excludedPaths/path")
                    _Settings._excludedPaths.Add(nav.Value.ToLower())
                Next

                If (File.Exists(filePath)) Then
                    'Set back into Cache
                    DataCache.SetCache("CompressionConfig", _Settings, New DNNCacheDependency(filePath))
                End If
            End If

            Return _Settings
        End Function


        ''' <summary>
        ''' Looks for a given path in the list of paths excluded from compression
        ''' </summary>
        ''' <param name="relUrl">the relative url to check</param>
        ''' <returns>true if excluded, false if not</returns>
        Public Function IsExcludedPath(ByVal relUrl As String) As Boolean
            Dim Match As Boolean = False

            For Each path As String In _excludedPaths
                If relUrl.ToLower().Contains(path) Then
                    Match = True
                    Exit For
                End If
            Next

            Return Match
        End Function

#End Region

    End Class

End Namespace


