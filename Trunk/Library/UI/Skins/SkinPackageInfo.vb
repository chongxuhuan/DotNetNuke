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
Imports DotNetNuke.Entities.Modules
Imports System.Collections.Generic
Imports DotNetNuke.Entities
Imports System.Xml.Serialization

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : SkinPackageInfo
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles the Business Object for Skins
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SkinPackageInfo
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _SkinPackageID As Integer = Null.NullInteger
        Private _PackageID As Integer = Null.NullInteger
        Private _PortalID As Integer = Null.NullInteger
        Private _SkinName As String
        Private _Skins As New Dictionary(Of Integer, String)
        Private _SkinType As String

#End Region

#Region "Public Properties"

        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal Value As Integer)
                _PackageID = Value
            End Set
        End Property

        Public Property SkinPackageID() As Integer
            Get
                Return _SkinPackageID
            End Get
            Set(ByVal Value As Integer)
                _SkinPackageID = Value
            End Set
        End Property

        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        Public Property SkinName() As String
            Get
                Return _SkinName
            End Get
            Set(ByVal Value As String)
                _SkinName = Value
            End Set
        End Property

        <XmlIgnore()> Public Property Skins() As Dictionary(Of Integer, String)
            Get
                Return _Skins
            End Get
            Set(ByVal value As Dictionary(Of Integer, String))
                _Skins = value
            End Set
        End Property

        Public Property SkinType() As String
            Get
                Return _SkinType
            End Get
            Set(ByVal Value As String)
                _SkinType = Value
            End Set
        End Property


#End Region

#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            SkinPackageID = Null.SetNullInteger(dr("SkinPackageID"))
            PackageID = Null.SetNullInteger(dr("PackageID"))
            SkinName = Null.SetNullString(dr("SkinName"))
            SkinType = Null.SetNullString(dr("SkinType"))
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)

            If dr.NextResult Then
                While dr.Read()
                    Dim skinID As Integer = Null.SetNullInteger(dr("SkinID"))
                    If skinID > Null.NullInteger Then
                        _Skins.Item(skinID) = Null.SetNullString(dr("SkinSrc"))
                    End If
                End While
            End If
        End Sub

        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return SkinPackageID
            End Get
            Set(ByVal value As Integer)
                SkinPackageID = value
            End Set
        End Property

#End Region

    End Class

End Namespace