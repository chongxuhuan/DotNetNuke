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
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Text.RegularExpressions

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : SkinInfo
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles the Business Object for Skins
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[willhsc]	3/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SkinInfo

#Region "Private Members"

        Private _SkinId As Integer
        Private _PortalId As Integer
        Private _SkinRoot As String
        Private _SkinType As UI.Skins.SkinType
        Private _SkinSrc As String

#End Region

#Region "Public Properties"

        Public Property SkinId() As Integer
            Get
                Return _SkinId
            End Get
            Set(ByVal Value As Integer)
                _SkinId = Value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        Public Property SkinRoot() As String
            Get
                Return _SkinRoot
            End Get
            Set(ByVal Value As String)
                _SkinRoot = Value
            End Set
        End Property

        Public Property SkinType() As UI.Skins.SkinType
            Get
                Return _SkinType
            End Get
            Set(ByVal Value As UI.Skins.SkinType)
                _SkinType = Value
            End Set
        End Property

        Public Property SkinSrc() As String
            Get
                Return _SkinSrc
            End Get
            Set(ByVal Value As String)
                _SkinSrc = Value
            End Set
        End Property

#End Region

#Region "Public Shared Properties"

        <Obsolete("Replaced in DNN 5.0 by SkinController.RootSkin")> _
            Public Shared ReadOnly Property RootSkin() As String
            Get
                Return "Skins"
            End Get
        End Property

        <Obsolete("Replaced in DNN 5.0 by SkinController.RootContainer")> _
            Public Shared ReadOnly Property RootContainer() As String
            Get
                Return "Containers"
            End Get
        End Property

#End Region

    End Class

End Namespace