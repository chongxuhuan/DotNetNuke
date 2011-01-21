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

Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Profile

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      UserProfile
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserProfile class provides a Business Layer entity for the Users Profile
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	01/31/2006	documented
    '''     [cnurse]    02/10/2006  updated with extensible profile enhancment
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class UserProfile

#Region "Private Constants"

        'Name properties
        Private Const cPrefix As String = "Prefix"
        Private Const cFirstName As String = "FirstName"
        Private Const cMiddleName As String = "MiddleName"
        Private Const cLastName As String = "LastName"
        Private Const cSuffix As String = "Suffix"

        'Address Properties
        Private Const cUnit As String = "Unit"
        Private Const cStreet As String = "Street"
        Private Const cCity As String = "City"
        Private Const cRegion As String = "Region"
        Private Const cCountry As String = "Country"
        Private Const cPostalCode As String = "PostalCode"

        'Phone contact
        Private Const cTelephone As String = "Telephone"
        Private Const cCell As String = "Cell"
        Private Const cFax As String = "Fax"

        'Online contact
        Private Const cWebsite As String = "Website"
        Private Const cIM As String = "IM"

        'Preferences
        Private Const cPhoto As String = "Photo"
        Private Const cTimeZone As String = "TimeZone"
        Private Const cPreferredLocale As String = "PreferredLocale"
        Private Const cPreferredTimeZone As String = "PreferredTimeZone"

#End Region

#Region "Private Members"

        Private _IsDirty As Boolean
        Private _ObjectHydrated As Boolean
        Private UserID As Integer

        ' collection to store all profile properties.
        Private _profileProperties As ProfilePropertyDefinitionCollection

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Cell/Mobile Phone
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Cell() As String
            Get
                Return GetPropertyValue(cCell)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cCell, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the City part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property City() As String
            Get
                Return GetPropertyValue(cCity)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cCity, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Country part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Country() As String
            Get
                Return GetPropertyValue(cCountry)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cCountry, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Fax Phone
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Fax() As String
            Get
                Return GetPropertyValue(cFax)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cFax, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the First Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FirstName() As String
            Get
                Return GetPropertyValue(cFirstName)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cFirstName, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Full Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property FullName() As String
            Get
                Return FirstName & " " & LastName
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Instant Messenger Handle
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IM() As String
            Get
                Return GetPropertyValue(cIM)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cIM, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the property has been changed
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsDirty() As Boolean
            Get
                Return _IsDirty
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LastName() As String
            Get
                Return GetPropertyValue(cLastName)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cLastName, Value)
            End Set
        End Property

        Public Property Photo() As String
            Get
                Return GetPropertyValue(cPhoto)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cPhoto, Value)
            End Set
        End Property

        Public ReadOnly Property PhotoURL() As String
            Get
                Dim strPhotoURL As String = ApplicationPath & "/images/no_avatar.gif"
                Dim objProperty As ProfilePropertyDefinition = GetProperty(cPhoto)
                If Not objProperty Is Nothing Then
                    If objProperty.PropertyValue <> "" AndAlso objProperty.Visibility = UserVisibilityMode.AllUsers Then
                        Dim objFiles As New DotNetNuke.Services.FileSystem.FileController
                        Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = objFiles.GetFileById(Integer.Parse(objProperty.PropertyValue), objProperty.PortalId)
                        If Not objFile Is Nothing Then
                            Dim objPortal As PortalInfo = New PortalController().GetPortal(objFile.PortalId)
                            If Not objPortal Is Nothing Then
                                strPhotoURL = ApplicationPath & "/" & objFile.RelativePath
                            End If

                        End If
                    End If
                End If
                Return strPhotoURL
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PostalCode part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PostalCode() As String
            Get
                Return GetPropertyValue(cPostalCode)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cPostalCode, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Preferred Locale
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PreferredLocale() As String
            Get
                Return GetPropertyValue(cPreferredLocale)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cPreferredLocale, Value)
            End Set
        End Property

        Public Property PreferredTimeZone() As TimeZoneInfo
            Get
                'First set to Server
                Dim _TimeZone As TimeZoneInfo = TimeZoneInfo.Local

                'Next check if there is a Portal Setting
                Dim _PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
                If _PortalSettings IsNot Nothing Then
                    _TimeZone = _PortalSettings.TimeZone
                End If

                'Next check if there is a Property Setting
                Dim _TimeZoneId As String = GetPropertyValue(cPreferredTimeZone)
                If Not String.IsNullOrEmpty(_TimeZoneId) Then
                    _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeZoneId)
                End If

                'return timezone
                Return _TimeZone
            End Get
            Set(ByVal Value As TimeZoneInfo)
                SetProfileProperty(cPreferredTimeZone, Value.Id)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Collection of Profile Properties
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        '''     [cnurse]    03/28/2006  Converted to a ProfilePropertyDefinitionCollection
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ProfileProperties() As ProfilePropertyDefinitionCollection
            Get
                If _profileProperties Is Nothing Then
                    _profileProperties = New ProfilePropertyDefinitionCollection
                End If
                Return _profileProperties
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Region part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Region() As String
            Get
                Return GetPropertyValue(cRegion)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cRegion, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Street part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Street() As String
            Get
                Return GetPropertyValue(cStreet)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cStreet, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Telephone
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Telephone() As String
            Get
                Return GetPropertyValue(cTelephone)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cTelephone, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Unit part of the Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Unit() As String
            Get
                Return GetPropertyValue(cUnit)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cUnit, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Website
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/10/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Website() As String
            Get
                Return GetPropertyValue(cWebsite)
            End Get
            Set(ByVal Value As String)
                SetProfileProperty(cWebsite, Value)
            End Set
        End Property


#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clears the IsDirty Flag
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/29/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ClearIsDirty()
            _IsDirty = False
            For Each profProperty As ProfilePropertyDefinition In ProfileProperties
                profProperty.ClearIsDirty()
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Profile Property from the Profile
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="propName">The name of the property to retrieve.</param>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetProperty(ByVal propName As String) As ProfilePropertyDefinition
            Return ProfileProperties(propName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Profile Property Value from the Profile
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="propName">The name of the propoerty to retrieve.</param>
        ''' <history>
        ''' 	[cnurse]	02/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPropertyValue(ByVal propName As String) As String
            'Declare ProfileProperty
            Dim propValue As String = Null.NullString
            Dim profileProp As ProfilePropertyDefinition = GetProperty(propName)

            If Not profileProp Is Nothing Then
                propValue = profileProp.PropertyValue
            End If

            Return propValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialises the Profile with an empty collection of profile properties
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="portalId">The name of the property to retrieve.</param>
        ''' <history>
        ''' 	[cnurse]	05/18/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InitialiseProfile(ByVal portalId As Integer)
            InitialiseProfile(portalId, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialises the Profile with an empty collection of profile properties
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="portalId">The name of the property to retrieve.</param>
        ''' <param name="useDefaults">A flag that indicates whether the profile default values should be
        ''' copied to the Profile.</param>
        ''' <history>
        ''' 	[cnurse]	08/04/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InitialiseProfile(ByVal portalId As Integer, ByVal useDefaults As Boolean)

            _profileProperties = ProfileController.GetPropertyDefinitionsByPortal(portalId, True)
            If useDefaults Then
                For Each ProfileProperty As ProfilePropertyDefinition In _profileProperties
                    ProfileProperty.PropertyValue = ProfileProperty.DefaultValue
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a Profile Property Value in the Profile
        ''' </summary>
        ''' <remarks></remarks>
        ''' <param name="propName">The name of the propoerty to set.</param>
        ''' <param name="propValue">The value of the propoerty to set.</param>
        ''' <history>
        ''' 	[cnurse]	02/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetProfileProperty(ByVal propName As String, ByVal propValue As String)
            'Declare ProfileProperty
            Dim profileProp As ProfilePropertyDefinition = GetProperty(propName)

            If Not profileProp Is Nothing Then
                profileProp.PropertyValue = propValue

                'Set the IsDirty flag
                If profileProp.IsDirty Then _IsDirty = True
            End If
        End Sub

#End Region

#Region "Obsolete"

        <Obsolete("Deprecated in DNN 5.1")> _
        <Browsable(False)> Public Property ObjectHydrated() As Boolean
            Get
                Return _ObjectHydrated
            End Get
            Set(ByVal Value As Boolean)
                _ObjectHydrated = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.6.2. Replaced by PreferredTimeZone.")> _
        <Browsable(False)> Public Property TimeZone() As Integer
            Get
                Dim retValue As Int32 = Null.NullInteger
                Dim propValue As String = GetPropertyValue(cTimeZone)
                If Not propValue Is Nothing Then
                    retValue = Integer.Parse(propValue)
                End If
                Return retValue
            End Get
            Set(ByVal Value As Integer)
                SetProfileProperty(cTimeZone, Value.ToString)
            End Set
        End Property


#End Region
    End Class

End Namespace
