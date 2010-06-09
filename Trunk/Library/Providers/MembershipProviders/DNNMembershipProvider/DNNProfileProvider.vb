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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Membership.Data
Imports System.Web

Namespace DotNetNuke.Security.Profile

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Profile
    ''' Class:      DNNProfileProvider
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNProfileProvider overrides the default ProfileProvider to provide
    ''' a purely DotNetNuke implementation
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/29/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DNNProfileProvider
        Inherits ProfileProvider

#Region "Private Members"

        Private dataProvider As DotNetNuke.Security.Membership.Data.DataProvider

#End Region

#Region "Constructors"

        Public Sub New()
            dataProvider = DotNetNuke.Security.Membership.Data.DataProvider.Instance()
            If dataProvider Is Nothing Then
                ' get the provider configuration based on the type
                Dim defaultprovider As String = DotNetNuke.Data.DataProvider.Instance.DefaultProviderName
                Dim dataProviderNamespace As String = "DotNetNuke.Security.Membership.Data"
                If defaultprovider = "SqlDataProvider" Then
                    dataProvider = New SqlDataProvider
                Else
                    Dim providerType As String = dataProviderNamespace + "." + defaultprovider
                    dataProvider = CType(Framework.Reflection.CreateObject(providerType, providerType, True), DataProvider)
                End If
                ComponentFactory.RegisterComponentInstance(Of DataProvider)(dataProvider)
            End If
        End Sub

#End Region

#Region "Profile Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Provider Properties can be edited
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	03/29/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property CanEditProviderProperties() As Boolean
            Get
                Return True
            End Get
        End Property

#End Region

#Region "Profile Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserProfile retrieves the UserProfile information from the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user whose Profile information we are retrieving.</param>
        ''' <history>
        ''' 	[cnurse]	03/29/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub GetUserProfile(ByRef user As UserInfo)
            Dim portalId As Integer
            Dim definitionId As Integer
            Dim profProperty As ProfilePropertyDefinition
            Dim properties As ProfilePropertyDefinitionCollection

            If user.IsSuperUser Then
                portalId = Common.Globals.glbSuperUserAppName
            Else
                portalId = user.PortalID
            End If

            properties = ProfileController.GetPropertyDefinitionsByPortal(portalId, True)

            'Load the Profile properties
            If user.UserID > Null.NullInteger Then
                Dim dr As IDataReader = dataProvider.GetUserProfile(user.UserID)
                Try
                    While dr.Read
                        'Ensure the data reader returned is valid
                        If Not String.Equals(dr.GetName(0), "ProfileID", StringComparison.InvariantCultureIgnoreCase) Then
                            Exit While
                        End If
                        definitionId = Convert.ToInt32(dr("PropertyDefinitionId"))
                        profProperty = properties.GetById(definitionId)
                        If Not profProperty Is Nothing Then
                            profProperty.PropertyValue = Convert.ToString(dr("PropertyValue"))
                            profProperty.Visibility = CType(dr("Visibility"), UserVisibilityMode)
                        End If
                    End While
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try
            End If

            'Clear the profile
            user.Profile.ProfileProperties.Clear()

            'Add the properties to the profile
            For Each profProperty In properties
                If String.IsNullOrEmpty(profProperty.PropertyValue) AndAlso Not String.IsNullOrEmpty(profProperty.DefaultValue) Then
                    profProperty.PropertyValue = profProperty.DefaultValue
                End If
                user.Profile.ProfileProperties.Add(profProperty)
            Next

            'Clear IsDirty Flag
            user.Profile.ClearIsDirty()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateUserProfile persists a user's Profile to the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <history>
        ''' 	[cnurse]	03/29/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateUserProfile(ByVal user As UserInfo)

            Dim properties As ProfilePropertyDefinitionCollection = user.Profile.ProfileProperties

            For Each profProperty As ProfilePropertyDefinition In properties
                If (Not profProperty.PropertyValue Is Nothing) AndAlso (profProperty.IsDirty) Then
                    Dim objSecurity As New PortalSecurity
                    Dim propertyValue As String = objSecurity.InputFilter(profProperty.PropertyValue, PortalSecurity.FilterFlag.NoScripting)
                    'add additional html encoding to profile data
                    propertyValue = HttpUtility.HtmlEncode(propertyValue)
                    dataProvider.UpdateProfileProperty(Null.NullInteger, user.UserID, profProperty.PropertyDefinitionId, propertyValue, profProperty.Visibility, Now())
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog(user, Entities.Portals.PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", "USERPROFILE_UPDATED")
                End If
            Next

        End Sub

#End Region

    End Class

End Namespace
