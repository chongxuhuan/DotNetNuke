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

Imports DotNetNuke.UI
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Modules
Imports System.Xml.Serialization

Namespace DotNetNuke.Entities.Profile

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Profile
    ''' Class:      ProfilePropertyDefinition
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ProfilePropertyDefinition class provides a Business Layer entity for 
    ''' property Definitions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	01/31/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <XmlRoot("profiledefinition", IsNullable:=False)> <Serializable()> Public Class ProfilePropertyDefinition
        Inherits BaseEntityInfo

#Region "Private Members"

        Private _DataType As Integer = Null.NullInteger
        Private _DefaultValue As String
        Private _IsDirty As Boolean
        Private _Length As Integer
        Private _ModuleDefId As Integer = Null.NullInteger
        Private _PortalId As Integer
        Private _PropertyCategory As String
        Private _PropertyDefinitionId As Integer = Null.NullInteger
        Private _PropertyName As String
        Private _PropertyValue As String
        Private _Required As Boolean
        Private _ValidationExpression As String
        Private _ViewOrder As Integer
        Private _Visible As Boolean
        Private _DefaultVisibility As UserVisibilityMode = UserVisibilityMode.AdminOnly
        Private _Visibility As UserVisibilityMode = UserVisibilityMode.AdminOnly

#End Region

#Region "Constructors"

        Public Sub New()
            'Get the default PortalSettings
            Dim _Settings As PortalSettings = PortalController.GetCurrentPortalSettings()
            Me.PortalId = _Settings.PortalId
        End Sub

        Public Sub New(ByVal portalId As Integer)
            Me.PortalId = portalId
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Data Type of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Editor("DotNetNuke.UI.WebControls.DNNListEditControl, DotNetNuke", GetType(DotNetNuke.UI.WebControls.EditControl)), _
         List("DataType", "", ListBoundField.Id, ListBoundField.Value), _
         IsReadOnly(True), Required(True), SortOrder(1)> _
         <XmlIgnore()> Public Property DataType() As Integer
            Get
                Return _DataType
            End Get
            Set(ByVal Value As Integer)
                If _DataType <> Value Then _IsDirty = True
                _DataType = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Default Value of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(4)> <XmlIgnore()> Public Property DefaultValue() As String
            Get
                Return _DefaultValue
            End Get
            Set(ByVal Value As String)
                If _DefaultValue <> Value Then _IsDirty = True
                _DefaultValue = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Definition has been modified since it has been retrieved
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public ReadOnly Property IsDirty() As Boolean
            Get
                Return _IsDirty
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Length of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(3)> <XmlElement("length")> Public Property Length() As Integer
            Get
                Return _Length
            End Get
            Set(ByVal Value As Integer)
                If _Length <> Value Then _IsDirty = True
                _Length = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleDefId
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public Property ModuleDefId() As Integer
            Get
                Return _ModuleDefId
            End Get
            Set(ByVal Value As Integer)
                _ModuleDefId = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PortalId
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Category of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Required(True), SortOrder(2)> <XmlElement("propertycategory")> Public Property PropertyCategory() As String
            Get
                Return _PropertyCategory
            End Get
            Set(ByVal Value As String)
                If _PropertyCategory <> Value Then _IsDirty = True
                _PropertyCategory = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Id of the ProfilePropertyDefinition
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public Property PropertyDefinitionId() As Integer
            Get
                Return _PropertyDefinitionId
            End Get
            Set(ByVal Value As Integer)
                _PropertyDefinitionId = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Name of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Required(True), IsReadOnly(True), SortOrder(0), RegularExpressionValidator("^[a-zA-Z0-9._%\-+']+$")> <XmlElement("propertyname")> Public Property PropertyName() As String
            Get
                Return _PropertyName
            End Get
            Set(ByVal Value As String)
                If _PropertyName <> Value Then _IsDirty = True
                _PropertyName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Value of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public Property PropertyValue() As String
            Get
                Return _PropertyValue
            End Get
            Set(ByVal Value As String)
                If _PropertyValue <> Value Then _IsDirty = True
                _PropertyValue = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is required
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(6)> <XmlIgnore()> Public Property Required() As Boolean
            Get
                Return _Required
            End Get
            Set(ByVal Value As Boolean)
                If _Required <> Value Then _IsDirty = True
                _Required = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a Validation Expression (RegEx) for the Profile Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(5)> <XmlIgnore()> Public Property ValidationExpression() As String
            Get
                Return _ValidationExpression
            End Get
            Set(ByVal Value As String)
                If _ValidationExpression <> Value Then _IsDirty = True
                _ValidationExpression = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the View Order of the Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <IsReadOnly(True), SortOrder(8)> <XmlIgnore()> Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal Value As Integer)
                If _ViewOrder <> Value Then _IsDirty = True
                _ViewOrder = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is visible
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(7)> <XmlIgnore()> Public Property Visible() As Boolean
            Get
                Return _Visible
            End Get
            Set(ByVal Value As Boolean)
                If _Visible <> Value Then _IsDirty = True
                _Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Default Visibility of the Profile Property
        ''' </summary>
        ''' <history>
        '''     [sbwalker]	06/28/2010	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(9)> <XmlIgnore()> Public Property DefaultVisibility() As UserVisibilityMode
            Get
                Return _DefaultVisibility
            End Get
            Set(ByVal Value As UserVisibilityMode)
                If _DefaultVisibility <> Value Then _IsDirty = True
                _DefaultVisibility = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the property is visible
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> <XmlIgnore()> Public Property Visibility() As UserVisibilityMode
            Get
                Return _Visibility
            End Get
            Set(ByVal Value As UserVisibilityMode)
                If _Visibility <> Value Then _IsDirty = True
                _Visibility = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clears the IsDirty Flag
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ClearIsDirty()
            _IsDirty = False
        End Sub

        Public Function Clone() As ProfilePropertyDefinition
            Dim objClone As New ProfilePropertyDefinition(Me.PortalId)
            objClone.DataType = Me.DataType
            objClone.DefaultValue = Me.DefaultValue
            objClone.Length = Me.Length
            objClone.ModuleDefId = Me.ModuleDefId
            objClone.PropertyCategory = Me.PropertyCategory
            objClone.PropertyDefinitionId = Me.PropertyDefinitionId
            objClone.PropertyName = Me.PropertyName
            objClone.PropertyValue = Me.PropertyValue
            objClone.Required = Me.Required
            objClone.ValidationExpression = Me.ValidationExpression
            objClone.ViewOrder = Me.ViewOrder
            objClone.Visibility = Me.Visibility
            objClone.DefaultVisibility = Me.DefaultVisibility
            objClone.Visible = Me.Visible
            objClone.ClearIsDirty()

            Return objClone
        End Function

#End Region

    End Class

End Namespace
