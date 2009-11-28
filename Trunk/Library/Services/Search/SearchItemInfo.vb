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

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke
    ''' Class:      SearchItemInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchItemInfo represents a Search Item
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SearchItemInfo

#Region "Private Members"

        Private _SearchItemId As Integer
        Private _Title As String
        Private _Description As String
        Private _Author As Integer
        Private _PubDate As Date
        Private _ModuleId As Integer
        Private _SearchKey As String
        Private _Content As String
        Private _GUID As String
        Private _ImageFileId As Integer
        Private _HitCount As Integer

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal Title As String, ByVal Description As String, ByVal Author As Integer, ByVal PubDate As Date, ByVal ModuleID As Integer, ByVal SearchKey As String, ByVal Content As String)
            Me.New(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Null.NullInteger)
        End Sub

        Public Sub New(ByVal Title As String, ByVal Description As String, ByVal Author As Integer, ByVal PubDate As Date, ByVal ModuleID As Integer, ByVal SearchKey As String, ByVal Content As String, ByVal Guid As String)
            Me.New(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, Guid, Null.NullInteger)
        End Sub

        Public Sub New(ByVal Title As String, ByVal Description As String, ByVal Author As Integer, ByVal PubDate As Date, ByVal ModuleID As Integer, ByVal SearchKey As String, ByVal Content As String, ByVal Image As Integer)
            Me.New(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Image)
        End Sub

        Public Sub New(ByVal Title As String, ByVal Description As String, ByVal Author As Integer, ByVal PubDate As Date, ByVal ModuleID As Integer, ByVal SearchKey As String, ByVal Content As String, ByVal Guid As String, ByVal Image As Integer)
            _Title = Title
            _Description = Description
            _Author = Author
            _PubDate = PubDate
            _ModuleId = ModuleID
            _SearchKey = SearchKey
            _Content = Content
            _GUID = Guid
            _ImageFileId = Image
            _HitCount = 0
        End Sub

#End Region

#Region "Properties"

        Public Property SearchItemId() As Integer
            Get
                Return _SearchItemId
            End Get
            Set(ByVal Value As Integer)
                _SearchItemId = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property Author() As Integer
            Get
                Return _Author
            End Get
            Set(ByVal Value As Integer)
                _Author = Value
            End Set
        End Property

        Public Property PubDate() As Date
            Get
                Return _PubDate
            End Get
            Set(ByVal Value As Date)
                _PubDate = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property SearchKey() As String
            Get
                Return _SearchKey
            End Get
            Set(ByVal Value As String)
                _SearchKey = Value
            End Set
        End Property

        Public Property Content() As String
            Get
                Return _Content
            End Get
            Set(ByVal Value As String)
                _Content = Value
            End Set
        End Property

        Public Property GUID() As String
            Get
                Return _GUID
            End Get
            Set(ByVal Value As String)
                _GUID = Value
            End Set
        End Property

        Public Property ImageFileId() As Integer
            Get
                Return _ImageFileId
            End Get
            Set(ByVal Value As Integer)
                _ImageFileId = Value
            End Set
        End Property

        Public Property HitCount() As Integer
            Get
                Return _HitCount
            End Get
            Set(ByVal Value As Integer)
                _HitCount = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
