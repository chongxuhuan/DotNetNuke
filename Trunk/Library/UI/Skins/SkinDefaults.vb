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

Imports System.Xml
Imports System.Xml.XPath

Imports System.IO

Namespace DotNetNuke.UI.Skins

    Public Enum SkinDefaultType
        SkinInfo
        ContainerInfo
    End Enum

    <Serializable()> _
    Public Class SkinDefaults

#Region "Fields"

        Private _adminDefaultName As String
        Private _defaultName As String
        Private _folder As String

#End Region

#Region "Properties"

        Public Property AdminDefaultName() As String
            Get
                Return _adminDefaultName
            End Get
            Set(ByVal Value As String)
                _adminDefaultName = Value
            End Set
        End Property

        Public Property DefaultName() As String
            Get
                Return _defaultName
            End Get
            Set(ByVal Value As String)
                _defaultName = Value
            End Set
        End Property

        Public Property Folder() As String
            Get
                Return _folder
            End Get
            Set(ByVal Value As String)
                _folder = Value
            End Set
        End Property

#End Region

#Region "Constructors"

        Private Sub New(ByVal DefaultType As SkinDefaultType)
            Dim nodename As String = [Enum].GetName(DefaultType.GetType(), DefaultType).ToLower
            Dim filePath As String = Config.GetPathToFile(ConfigFileType.DotNetNuke)

            Dim dnndoc As New XmlDocument()
            dnndoc.Load(filePath)
            Dim defaultElement As XmlNode = dnndoc.SelectSingleNode("/configuration/skinningdefaults/" & nodename)

            _folder = defaultElement.Attributes("folder").Value
            _defaultName = defaultElement.Attributes("default").Value
            _adminDefaultName = defaultElement.Attributes("admindefault").Value
        End Sub

#End Region

        Private Shared Function GetSkinDefaultsCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim defaultType As SkinDefaultType = CType(cacheItemArgs.ParamList(0), SkinDefaultType)
            Return New SkinDefaults(defaultType)
        End Function

        Public Shared Function GetSkinDefaults(ByVal DefaultType As SkinDefaultType) As SkinDefaults
            Return CBO.GetCachedObject(Of SkinDefaults)(New CacheItemArgs(String.Format(DataCache.SkinDefaultsCacheKey, DefaultType), DataCache.SkinDefaultsCacheTimeOut, DataCache.SkinDefaultsCachePriority, DefaultType), AddressOf GetSkinDefaultsCallback)
        End Function

    End Class

End Namespace