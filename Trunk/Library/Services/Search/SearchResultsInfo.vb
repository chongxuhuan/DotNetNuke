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


Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke
    ''' Class:      SearchResultsInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchResultsInfo represents a Search Result Item
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SearchResultsInfo

#Region "Private Members"

        Private m_SearchItemID As Integer
        Private m_Title As String
        Private m_Description As String
        Private m_Author As String
        Private m_PubDate As Date
        Private m_Guid As String
        Private m_ModuleId As Integer
        Private m_TabId As Integer
        Private m_SearchKey As String
        Private m_Occurrences As Integer
        Private m_Relevance As Integer
        Private m_Image As Integer
        Private m_Delete As Boolean = False
        Private m_AuthorName As String
        Private m_PortalId As Integer

#End Region

#Region "Properties"

        Public Property SearchItemID() As Integer
            Get
                Return m_SearchItemID
            End Get
            Set(ByVal Value As Integer)
                m_SearchItemID = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = Value
            End Set
        End Property

        Public Property Author() As String
            Get
                Return m_Author
            End Get
            Set(ByVal Value As String)
                m_Author = Value
            End Set
        End Property

        Public Property PubDate() As Date
            Get
                Return m_PubDate
            End Get
            Set(ByVal Value As Date)
                m_PubDate = Value
            End Set
        End Property

        Public Property Guid() As String
            Get
                Return m_Guid
            End Get
            Set(ByVal Value As String)
                m_Guid = Value
            End Set
        End Property

        Public Property Image() As Integer
            Get
                Return m_Image
            End Get
            Set(ByVal Value As Integer)
                m_Image = Value
            End Set
        End Property

        Public Property TabId() As Integer
            Get
                Return m_TabId
            End Get
            Set(ByVal Value As Integer)
                m_TabId = Value
            End Set
        End Property

        Public Property SearchKey() As String
            Get
                Return m_SearchKey
            End Get
            Set(ByVal Value As String)
                m_SearchKey = Value
            End Set
        End Property

        Public Property Occurrences() As Integer
            Get
                Return m_Occurrences
            End Get
            Set(ByVal Value As Integer)
                m_Occurrences = Value
            End Set
        End Property

        Public Property Relevance() As Integer
            Get
                Return m_Relevance
            End Get
            Set(ByVal Value As Integer)
                m_Relevance = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return m_ModuleId
            End Get
            Set(ByVal Value As Integer)
                m_ModuleId = Value
            End Set
        End Property

        Public Property Delete() As Boolean
            Get
                Return m_Delete
            End Get
            Set(ByVal Value As Boolean)
                m_Delete = Value
            End Set
        End Property

        Public Property AuthorName() As String
            Get
                Return m_AuthorName
            End Get
            Set(ByVal Value As String)
                m_AuthorName = Value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return m_PortalId
            End Get
            Set(ByVal value As Integer)
                m_PortalId = value
            End Set
        End Property

#End Region

    End Class

End Namespace