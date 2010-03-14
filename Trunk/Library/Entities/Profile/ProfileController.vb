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

Imports System.Collections.Generic
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Entities.Modules
Imports System.Windows.Forms

Namespace DotNetNuke.Entities.Profile

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Profile
    ''' Class:      ProfileController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ProfileController class provides Business Layer methods for profiles and
    ''' for profile property Definitions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	01/31/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ProfileController


#Region "Private Shared Members"

        Private Shared provider As DataProvider = DataProvider.Instance()
        Private Shared profileProvider As DotNetNuke.Security.Profile.ProfileProvider = DotNetNuke.Security.Profile.ProfileProvider.Instance()
        Private Shared _orderCounter As Integer

#End Region

#Region "Private Shared Methods"

        Private Shared Sub AddDefaultDefinition(ByVal PortalId As Integer, ByVal category As String, ByVal name As String, ByVal strType As String, ByVal length As Integer, ByVal types As ListEntryInfoCollection)

            _orderCounter += 2

            AddDefaultDefinition(PortalId, category, name, strType, length, _orderCounter, types)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a single default property definition
        ''' </summary>
        ''' <param name="PortalId">Id of the Portal</param>
        ''' <param name="category">Category of the Property</param>
        ''' <param name="name">Name of the Property</param>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Sub AddDefaultDefinition(ByVal PortalId As Integer, ByVal category As String, ByVal name As String, ByVal strType As String, ByVal length As Integer, ByVal viewOrder As Integer, ByVal types As ListEntryInfoCollection)
            Dim typeInfo As ListEntryInfo = types.Item("DataType:" + strType)
            If typeInfo Is Nothing Then
                typeInfo = types.Item("DataType:Unknown")
            End If

            Dim propertyDefinition As ProfilePropertyDefinition = New ProfilePropertyDefinition(PortalId)
            propertyDefinition.DataType = typeInfo.EntryID
            propertyDefinition.DefaultValue = ""
            propertyDefinition.ModuleDefId = Null.NullInteger
            propertyDefinition.PropertyCategory = category
            propertyDefinition.PropertyName = name
            propertyDefinition.Required = False
            propertyDefinition.Visible = True
            propertyDefinition.Length = length

            propertyDefinition.ViewOrder = viewOrder

            AddPropertyDefinition(propertyDefinition)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ProfilePropertyDefinitionCollection from a DataReader
        ''' </summary>
        ''' <param name="dr">An IDataReader object</param>
        ''' <returns>The ProfilePropertyDefinitionCollection</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillCollection(ByVal dr As IDataReader) As ProfilePropertyDefinitionCollection
            Dim arrDefinitions As ArrayList = CBO.FillCollection(dr, GetType(ProfilePropertyDefinition))
            Dim definitionsCollection As New ProfilePropertyDefinitionCollection
            For Each definition As ProfilePropertyDefinition In arrDefinitions
                'Clear the Is Dirty Flag
                definition.ClearIsDirty()

                'Initialise the Visibility
                Dim setting As Object = UserModuleBase.GetSetting(definition.PortalId, "Profile_DefaultVisibility")
                If Not setting Is Nothing Then
                    definition.Visibility = CType(setting, UserVisibilityMode)
                End If

                'Add to collection
                definitionsCollection.Add(definition)
            Next
            Return definitionsCollection
        End Function

        Private Shared Function FillPropertyDefinitionInfo(ByVal dr As IDataReader) As ProfilePropertyDefinition
            Dim definition As ProfilePropertyDefinition = Nothing

            Try
                definition = FillPropertyDefinitionInfo(dr, False)
            Catch
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            Return definition
        End Function

        Private Shared Function FillPropertyDefinitionInfo(ByVal dr As IDataReader, ByVal checkForOpenDataReader As Boolean) As ProfilePropertyDefinition
            Dim definition As ProfilePropertyDefinition = Nothing

            ' read datareader
            Dim canContinue As Boolean = True
            If checkForOpenDataReader Then
                canContinue = False
                If dr.Read Then
                    canContinue = True
                End If
            End If
            If canContinue Then
                Dim portalid As Integer = Convert.ToInt32(Null.SetNull(dr("PortalId"), portalid))
                definition = New ProfilePropertyDefinition(portalid)
                definition.PropertyDefinitionId = Convert.ToInt32(Null.SetNull(dr("PropertyDefinitionId"), definition.PropertyDefinitionId))
                definition.ModuleDefId = Convert.ToInt32(Null.SetNull(dr("ModuleDefId"), definition.ModuleDefId))
                definition.DataType = Convert.ToInt32(Null.SetNull(dr("DataType"), definition.DataType))
                definition.DefaultValue = Convert.ToString(Null.SetNull(dr("DefaultValue"), definition.DefaultValue))
                definition.PropertyCategory = Convert.ToString(Null.SetNull(dr("PropertyCategory"), definition.PropertyCategory))
                definition.PropertyName = Convert.ToString(Null.SetNull(dr("PropertyName"), definition.PropertyName))
                definition.Length = Convert.ToInt32(Null.SetNull(dr("Length"), definition.Length))
                definition.Required = Convert.ToBoolean(Null.SetNull(dr("Required"), definition.Required))
                definition.ValidationExpression = Convert.ToString(Null.SetNull(dr("ValidationExpression"), definition.ValidationExpression))
                definition.ViewOrder = Convert.ToInt32(Null.SetNull(dr("ViewOrder"), definition.ViewOrder))
                definition.Visible = Convert.ToBoolean(Null.SetNull(dr("Visible"), definition.Visible))
            End If

            Return definition
        End Function

        Private Shared Function FillPropertyDefinitionInfoCollection(ByVal dr As IDataReader) As List(Of ProfilePropertyDefinition)
            Dim arr As New List(Of ProfilePropertyDefinition)
            Try
                Dim obj As ProfilePropertyDefinition
                While dr.Read
                    ' fill business object
                    obj = FillPropertyDefinitionInfo(dr, False)
                    ' add to collection
                    arr.Add(obj)
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try

            Return arr
        End Function

        Private Shared Function GetPropertyDefinitions(ByVal portalId As Integer) As List(Of ProfilePropertyDefinition)
            'Get the Cache Key
            Dim key As String = String.Format(DataCache.ProfileDefinitionsCacheKey, portalId)

            'Try fetching the List from the Cache
            Dim definitions As List(Of ProfilePropertyDefinition) = CType(DataCache.GetCache(key), List(Of ProfilePropertyDefinition))

            If definitions Is Nothing Then
                'definitions caching settings
                Dim timeOut As Int32 = DataCache.ProfileDefinitionsCacheTimeOut * Convert.ToInt32(Host.Host.PerformanceSetting)

                'Get the List from the database
                definitions = FillPropertyDefinitionInfoCollection(provider.GetPropertyDefinitionsByPortal(portalId))

                'Cache the List
                If timeOut > 0 Then
                    DataCache.SetCache(key, definitions, TimeSpan.FromMinutes(timeOut))
                End If
            End If

            'Return the List
            Return definitions
        End Function

#End Region

#Region "Profile Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Profile Information for the User
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="objUser">The user whose Profile information we are retrieving.</param>
        ''' <history>
        ''' 	[cnurse]	12/13/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub GetUserProfile(ByRef objUser As UserInfo)

            profileProvider.GetUserProfile(objUser)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a User's Profile
        ''' </summary>
        ''' <param name="objUser">The use to update</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/18/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateUserProfile(ByVal objUser As UserInfo)
            'Update the User Profile
            If objUser.Profile.IsDirty Then
                profileProvider.UpdateUserProfile(objUser)
            End If

            'Remove the UserInfo from the Cache, as it has been modified
            DataCache.ClearUserCache(objUser.PortalID, objUser.Username)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a User's Profile
        ''' </summary>
        ''' <param name="objUser">The use to update</param>
        ''' <param name="profileProperties">The collection of profile properties</param>
        ''' <returns>The updated User</returns>
        ''' <history>
        ''' 	[cnurse]	03/02/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UpdateUserProfile(ByVal objUser As UserInfo, ByVal profileProperties As ProfilePropertyDefinitionCollection) As UserInfo

            Dim updateUser As Boolean = Null.NullBoolean

            'Iterate through the Definitions
            If profileProperties IsNot Nothing Then
                For Each propertyDefinition As ProfilePropertyDefinition In profileProperties

                    Dim propertyName As String = propertyDefinition.PropertyName
                    Dim propertyValue As String = propertyDefinition.PropertyValue

                    If propertyDefinition.IsDirty Then
                        'Update Profile
                        objUser.Profile.SetProfileProperty(propertyName, propertyValue)

                        If propertyName.ToLower = "firstname" OrElse propertyName.ToLower = "lastname" Then
                            updateUser = True
                        End If
                    End If
                Next

                UpdateUserProfile(objUser)

                If updateUser Then
                    UserController.UpdateUser(objUser.PortalID, objUser)
                End If
            End If

            Return objUser

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Validates the Profile properties for the User (determines if all required properties
        ''' have been set)
        ''' </summary>
        ''' <param name="portalId">The Id of the portal.</param>
        ''' <param name="objProfile">The profile.</param>
        ''' <history>
        ''' 	[cnurse]	03/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ValidateProfile(ByVal portalId As Integer, ByVal objProfile As UserProfile) As Boolean

            Dim isValid As Boolean = True

            For Each propertyDefinition As ProfilePropertyDefinition In objProfile.ProfileProperties

                If propertyDefinition.Required And propertyDefinition.PropertyValue = Null.NullString Then
                    isValid = False
                    Exit For
                End If
            Next

            Return isValid

        End Function

#End Region

#Region "Property Definition Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds the default property definitions for a portal
        ''' </summary>
        ''' <param name="PortalId">Id of the Portal</param>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddDefaultDefinitions(ByVal PortalId As Integer)

            _orderCounter = 1

            Dim objListController As New ListController
            Dim dataTypes As ListEntryInfoCollection = objListController.GetListEntryInfoCollection("DataType")

            AddDefaultDefinition(PortalId, "Name", "Prefix", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Name", "FirstName", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Name", "MiddleName", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Name", "LastName", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Name", "Suffix", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "Unit", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "Street", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "City", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "Region", "Region", 0, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "Country", "Country", 0, dataTypes)
            AddDefaultDefinition(PortalId, "Address", "PostalCode", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Contact Info", "Telephone", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Contact Info", "Cell", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Contact Info", "Fax", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Contact Info", "Website", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Contact Info", "IM", "Text", 50, dataTypes)
            AddDefaultDefinition(PortalId, "Preferences", "Photo", "Image", 0, dataTypes)
            AddDefaultDefinition(PortalId, "Preferences", "Biography", "RichText", 0, dataTypes)
            AddDefaultDefinition(PortalId, "Preferences", "TimeZone", "TimeZone", 0, dataTypes)
            AddDefaultDefinition(PortalId, "Preferences", "PreferredLocale", "Locale", 0, dataTypes)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Property Defintion to the Data Store
        ''' </summary>
        ''' <param name="definition">An ProfilePropertyDefinition object</param>
        ''' <returns>The Id of the definition (or if negative the errorcode of the error)</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddPropertyDefinition(ByVal definition As ProfilePropertyDefinition) As Integer
            If definition.Required Then
                definition.Visible = True
            End If

            Dim intDefinition As Integer = provider.AddPropertyDefinition(definition.PortalId, definition.ModuleDefId, _
                definition.DataType, definition.DefaultValue, _
                definition.PropertyCategory, definition.PropertyName, definition.Required, _
                definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_CREATED)
            ClearProfileDefinitionCache(definition.PortalId)

            Return intDefinition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clears the Profile Definitions Cache
        ''' </summary>
        ''' <param name="PortalId">Id of the Portal</param>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub ClearProfileDefinitionCache(ByVal PortalId As Integer)
            DataCache.ClearDefinitionsCache(PortalId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a Property Defintion from the Data Store
        ''' </summary>
        ''' <param name="definition">The ProfilePropertyDefinition object to delete</param>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeletePropertyDefinition(ByVal definition As ProfilePropertyDefinition)
            provider.DeletePropertyDefinition(definition.PropertyDefinitionId)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_DELETED)
            ClearProfileDefinitionCache(definition.PortalId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Property Defintion from the Data Store by id
        ''' </summary>
        ''' <param name="definitionId">The id of the ProfilePropertyDefinition object to retrieve</param>
        ''' <returns>The ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetPropertyDefinition(ByVal definitionId As Integer, ByVal portalId As Integer) As ProfilePropertyDefinition
            Dim definition As ProfilePropertyDefinition = Nothing
            Dim bFound As Boolean = Null.NullBoolean

            For Each definition In GetPropertyDefinitions(portalId)
                If definition.PropertyDefinitionId = definitionId Then
                    bFound = True
                    Exit For
                End If
            Next

            If Not bFound Then
                'Try Database
                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinition(definitionId))
            End If

            Return definition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Property Defintion from the Data Store by name
        ''' </summary>
        ''' <param name="portalId">The id of the Portal</param>
        ''' <param name="name">The name of the ProfilePropertyDefinition object to retrieve</param>
        ''' <returns>The ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetPropertyDefinitionByName(ByVal portalId As Integer, ByVal name As String) As ProfilePropertyDefinition
            Dim definition As ProfilePropertyDefinition = Nothing
            Dim bFound As Boolean = Null.NullBoolean

            For Each definition In GetPropertyDefinitions(portalId)
                If definition.PropertyName = name Then
                    bFound = True
                    Exit For
                End If
            Next

            If Not bFound Then
                'Try Database
                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinitionByName(portalId, name))
            End If

            Return definition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a collection of Property Defintions from the Data Store by category
        ''' </summary>
        ''' <param name="portalId">The id of the Portal</param>
        ''' <param name="category">The category of the Property Defintions to retrieve</param>
        ''' <returns>A ProfilePropertyDefinitionCollection object</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetPropertyDefinitionsByCategory(ByVal portalId As Integer, ByVal category As String) As ProfilePropertyDefinitionCollection

            Dim definitions As New ProfilePropertyDefinitionCollection

            For Each definition As ProfilePropertyDefinition In GetPropertyDefinitions(portalId)
                If definition.PropertyCategory = category Then
                    definitions.Add(definition)
                End If
            Next

            Return definitions
        End Function

        Public Shared Function GetPropertyDefinitionsByPortal(ByVal portalId As Integer) As ProfilePropertyDefinitionCollection
            Return GetPropertyDefinitionsByPortal(portalId, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a collection of Property Defintions from the Data Store by portal
        ''' </summary>
        ''' <param name="portalId">The id of the Portal</param>
        ''' <returns>A ProfilePropertyDefinitionCollection object</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetPropertyDefinitionsByPortal(ByVal portalId As Integer, ByVal clone As Boolean) As ProfilePropertyDefinitionCollection

            Dim definitions As New ProfilePropertyDefinitionCollection

            For Each definition As ProfilePropertyDefinition In GetPropertyDefinitions(portalId)
                If clone Then
                    definitions.Add(definition.Clone)
                Else
                    definitions.Add(definition)
                End If
            Next
            Return definitions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Property Defintion in the Data Store
        ''' </summary>
        ''' <param name="definition">The ProfilePropertyDefinition object to update</param>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdatePropertyDefinition(ByVal definition As ProfilePropertyDefinition)
            If definition.Required Then
                definition.Visible = True
            End If
            provider.UpdatePropertyDefinition(definition.PropertyDefinitionId, _
                definition.DataType, definition.DefaultValue, definition.PropertyCategory, _
                definition.PropertyName, definition.Required, definition.ValidationExpression, _
                definition.ViewOrder, definition.Visible, definition.Length, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PROFILEPROPERTY_UPDATED)
            ClearProfileDefinitionCache(definition.PortalId)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("This method has been deprecated.  Please use GetPropertyDefinition(ByVal definitionId As Integer, ByVal portalId As Integer) instead")> _
        Public Shared Function GetPropertyDefinition(ByVal definitionId As Integer) As ProfilePropertyDefinition
            Return CType(CBO.FillObject(provider.GetPropertyDefinition(definitionId), GetType(ProfilePropertyDefinition)), ProfilePropertyDefinition)
        End Function

#End Region

    End Class

End Namespace

