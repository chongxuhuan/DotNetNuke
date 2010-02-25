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

Namespace DotNetNuke.Entities.Modules.Actions

    '''-----------------------------------------------------------------------------
    ''' Project		: DotNetNuke
    ''' Namespace   : DotNetNuke.Entities.Modules.Actions
    ''' Class		: ActionEventArgs
    '''-----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionEventArgs provides a custom EventARgs class for Action Events
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[Joe] 	10/26/2003	Created
    ''' </history>
    '''-----------------------------------------------------------------------------
    Public Class ActionEventArgs
        Inherits EventArgs

#Region "Private Members"

        Private _action As ModuleAction
        Private _moduleConfiguration As ModuleInfo

#End Region

#Region "Constructors"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="Action"></param>
        ''' <param name="ModuleConfiguration"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[Joe] 	10/26/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal Action As ModuleAction, ByVal ModuleConfiguration As ModuleInfo)
            _action = Action
            _moduleConfiguration = ModuleConfiguration
        End Sub

#End Region

#Region "Public Properties"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[Joe] 	10/26/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public ReadOnly Property Action() As ModuleAction
            Get
                Return _action
            End Get
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[jbrinkman] 	12/27/2003	Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public ReadOnly Property ModuleConfiguration() As ModuleInfo
            Get
                Return _moduleConfiguration
            End Get
        End Property

#End Region

    End Class

End Namespace
