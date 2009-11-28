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

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke.Search.Index
    ''' Class:      SearchContentModuleInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchContentModuleInfo class represents an extendension (by containment)
    ''' of ModuleInfo to add a parametere that determines whether a module is Searchable
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SearchContentModuleInfo

#Region "Protected Members"

        Protected m_ModInfo As ModuleInfo
        Protected m_ModControllerType As ISearchable

#End Region

#Region "Properties"

        Public Property ModControllerType() As ISearchable
            Get
                Return m_ModControllerType
            End Get
            Set(ByVal Value As ISearchable)
                m_ModControllerType = Value
            End Set
        End Property

        Public Property ModInfo() As ModuleInfo
            Get
                Return m_ModInfo
            End Get
            Set(ByVal Value As ModuleInfo)
                m_ModInfo = Value
            End Set
        End Property

#End Region

    End Class
End Namespace
