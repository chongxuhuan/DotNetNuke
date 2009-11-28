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

Imports System.Collections.Generic
Imports System.Reflection


Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Common.Utilities
    ''' Class:      ObjectMappingInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ObjectMappingInfo class is a helper class that holds the mapping information
    ''' for a particular type.  This information is in two parts:
    '''     - Information about the Database Table that the object is mapped to
    '''     - Information about how the object is cached.
    ''' For each object, when it is first accessed, reflection is used on the class and
    ''' an instance of ObjectMappingInfo is created, which is cached for performance.
    ''' </summary>
    ''' <history>
    '''     [cnurse]	12/01/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class ObjectMappingInfo

#Region "Private Members"

        Private _CacheByProperty As String
        Private _CacheTimeOutMultiplier As Integer
        Private _ColumnNames As Dictionary(Of String, String)
        Private _Properties As Dictionary(Of String, PropertyInfo)
        Private _ObjectType As String
        Private _TableName As String
        Private _PrimaryKey As String

        Private Const RootCacheKey As String = "ObjectCache_"

#End Region

#Region "Constructors"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ObjectMappingInfo Object
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New()
            _Properties = New Dictionary(Of String, PropertyInfo)
            _ColumnNames = New Dictionary(Of String, String)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CacheKey gets the root value of the key used to identify the cached collection 
        ''' in the ASP.NET Cache.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property CacheKey() As String
            Get
                Dim _CacheKey As String = RootCacheKey + TableName + "_"
                If Not String.IsNullOrEmpty(CacheByProperty) Then
                    _CacheKey &= CacheByProperty + "_"
                End If
                Return _CacheKey
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CacheByProperty gets and sets the property that is used to cache collections
        ''' of the object.  For example: Modules are cached by the "TabId" proeprty.  Tabs 
        ''' are cached by the PortalId property.
        ''' </summary>
        ''' <remarks>If empty, a collection of all the instances of the object is cached.</remarks>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CacheByProperty() As String
            Get
                Return _CacheByProperty
            End Get
            Set(ByVal value As String)
                _CacheByProperty = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CacheTimeOutMultiplier gets and sets the multiplier used to determine how long
        ''' the cached collection should be cached.  It is multiplied by the Performance
        ''' Setting - which in turn can be modified by the Host Account.
        ''' </summary>
        ''' <remarks>Defaults to 20.</remarks>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CacheTimeOutMultiplier() As Integer
            Get
                Return _CacheTimeOutMultiplier
            End Get
            Set(ByVal value As Integer)
                _CacheTimeOutMultiplier = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ColumnNames gets a dictionary of Database Column Names for the Object
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/02/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ColumnNames() As Dictionary(Of String, String)
            Get
                Return _ColumnNames
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ObjectType gets and sets the type of the object
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ObjectType() As String
            Get
                Return _ObjectType
            End Get
            Set(ByVal value As String)
                _ObjectType = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' PrimaryKey gets and sets the property of the object that corresponds to the
        ''' primary key in the database
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PrimaryKey() As String
            Get
                Return _PrimaryKey
            End Get
            Set(ByVal value As String)
                _PrimaryKey = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Properties gets a dictionary of Properties for the Object
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Properties() As Dictionary(Of String, PropertyInfo)
            Get
                Return _Properties
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TableName gets and sets the name of the database table that is used to
        ''' persist the object.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/01/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property TableName() As String
            Get
                Return _TableName
            End Get
            Set(ByVal value As String)
                _TableName = value
            End Set
        End Property

#End Region

    End Class

End Namespace
