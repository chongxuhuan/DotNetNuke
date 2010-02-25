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
Imports DotNetNuke.Entities

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationInfo class provides the Entity Layer for the 
    ''' Authentication Systems.
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class AuthenticationInfo
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _AuthenticationID As Integer = Null.NullInteger
        Private _PackageID As Integer
        Private _IsEnabled As Boolean = False
        Private _AuthenticationType As String = Null.NullString
        Private _SettingsControlSrc As String = Null.NullString
        Private _LoginControlSrc As String = Null.NullString
        Private _LogoffControlSrc As String = Null.NullString

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the ID of the Authentication System
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AuthenticationID() As Integer
            Get
                Return _AuthenticationID
            End Get
            Set(ByVal Value As Integer)
                _AuthenticationID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the PackageID for the Authentication System
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal Value As Integer)
                _PackageID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets a flag that determines whether the Authentication System is enabled
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsEnabled() As Boolean
            Get
                Return _IsEnabled
            End Get
            Set(ByVal Value As Boolean)
                _IsEnabled = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the type (name) of the Authentication System (eg DNN, OpenID, LiveID)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AuthenticationType() As String
            Get
                Return _AuthenticationType
            End Get
            Set(ByVal Value As String)
                _AuthenticationType = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the url for the Settings Control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SettingsControlSrc() As String
            Get
                Return _SettingsControlSrc
            End Get
            Set(ByVal Value As String)
                _SettingsControlSrc = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the url for the Login Control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginControlSrc() As String
            Get
                Return _LoginControlSrc
            End Get
            Set(ByVal Value As String)
                _LoginControlSrc = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the url for the Logoff Control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/23/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LogoffControlSrc() As String
            Get
                Return _LogoffControlSrc
            End Get
            Set(ByVal Value As String)
                _LogoffControlSrc = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a RoleInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            AuthenticationID = Null.SetNullInteger(dr("AuthenticationID"))
            PackageID = Null.SetNullInteger(dr("PackageID"))
            IsEnabled = Null.SetNullBoolean(dr("IsEnabled"))
            AuthenticationType = Null.SetNullString(dr("AuthenticationType"))
            SettingsControlSrc = Null.SetNullString(dr("SettingsControlSrc"))
            LoginControlSrc = Null.SetNullString(dr("LoginControlSrc"))
            LogoffControlSrc = Null.SetNullString(dr("LogoffControlSrc"))

            'Fill base class fields
            FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return AuthenticationID
            End Get
            Set(ByVal value As Integer)
                AuthenticationID = value
            End Set
        End Property

#End Region

    End Class

End Namespace

