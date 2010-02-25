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
Imports System.Windows.Forms

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Membership

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationLoginBase class provides a bas class for Authentiication 
    ''' Login controls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class AuthenticationLoginBase
        Inherits DotNetNuke.Entities.Modules.UserModuleBase

#Region "Public Delegates"

        Public Delegate Sub UserAuthenticatedEventHandler(ByVal sender As Object, ByVal e As UserAuthenticatedEventArgs)

#End Region

#Region "Public Events"

        Public Event UserAuthenticated As UserAuthenticatedEventHandler

#End Region

#Region "Private Members"

        Private _AuthenticationType As String = Null.NullString
        Private _RedirectURL As String = Null.NullString

#End Region

#Region "Public Shared Properties"

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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets whether the control is Enabled
        ''' </summary>
        ''' <remarks>This property must be overriden in the inherited class</remarks>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public MustOverride ReadOnly Property Enabled() As Boolean

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the IP address associated with the request
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IPAddress() As String
            Get
                Return GetIPAddress()
            End Get
        End Property


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Type of Authentication associated with this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property RedirectURL() As String
            Get
                Return _RedirectURL
            End Get
            Set(ByVal value As String)
                _RedirectURL = value
            End Set
        End Property

#End Region

#Region "Protected Event Methods"

        Protected Overridable Sub OnUserAuthenticated(ByVal ea As UserAuthenticatedEventArgs)
            RaiseEvent UserAuthenticated(Nothing, ea)
        End Sub

#End Region

        Public Shared Function GetIPAddress() As String
            Dim _IPAddress As String = Null.NullString
            If Not HttpContext.Current.Request.UserHostAddress Is Nothing Then
                _IPAddress = HttpContext.Current.Request.UserHostAddress
            End If
            Return _IPAddress
        End Function


    End Class

End Namespace

