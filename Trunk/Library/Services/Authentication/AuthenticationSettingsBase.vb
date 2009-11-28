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

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Windows.Forms

Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationSettingsBase class provides a base class for Authentiication 
    ''' Settings controls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class AuthenticationSettingsBase
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _AuthenticationType As String = Null.NullString

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Type of Authentication associated with this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AuthenticationType() As String
            Get
                Return _AuthenticationType
            End Get
            Set(ByVal value As String)
                _AuthenticationType = value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateSettings updates the settings in the Data Store
        ''' </summary>
        ''' <remarks>This method must be overriden in the inherited class</remarks>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public MustOverride Sub UpdateSettings()

#End Region

    End Class

End Namespace

