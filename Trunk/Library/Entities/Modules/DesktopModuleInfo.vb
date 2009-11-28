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

Imports DotNetNuke.Entities.Modules.Definitions
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : DesktopModuleInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' DesktopModuleInfo provides the Entity Layer for Desktop Modules
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/11/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class DesktopModuleInfo
        Inherits BaseEntityInfo
        Implements IXmlSerializable
        Implements IHydratable

#Region "Private Members"

        Private _DesktopModuleID As Integer = Null.NullInteger
        Private _PackageID As Integer = Null.NullInteger
        Private _CodeSubDirectory As String = Null.NullString
        Private _ModuleName As String
        Private _FolderName As String
        Private _FriendlyName As String
        Private _Description As String
        Private _Version As String
        Private _IsPremium As Boolean = Null.NullBoolean
        Private _IsAdmin As Boolean = Null.NullBoolean
        Private _SupportedFeatures As Integer = Null.NullInteger
        Private _BusinessControllerClass As String
        Private _CompatibleVersions As String
        Private _Dependencies As String
        Private _Permissions As String

        Private _ModuleDefinitions As Dictionary(Of String, ModuleDefinitionInfo)

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ID of the Desktop Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DesktopModuleID() As Integer
            Get
                Return _DesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _DesktopModuleID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ID of the Package for this Desktop Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
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
        ''' Gets and sets the AppCode Folder Name of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	02/20/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CodeSubDirectory() As String
            Get
                Return _CodeSubDirectory
            End Get
            Set(ByVal Value As String)
                _CodeSubDirectory = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the BusinessControllerClass of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property BusinessControllerClass() As String
            Get
                Return _BusinessControllerClass
            End Get
            Set(ByVal Value As String)
                _BusinessControllerClass = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a Regular Expression that matches the versions of the core
        ''' that this module is compatible with
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CompatibleVersions() As String
            Get
                Return _CompatibleVersions
            End Get
            Set(ByVal Value As String)
                _CompatibleVersions = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a list of Dependencies for the module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Dependencies() As String
            Get
                Return _Dependencies
            End Get
            Set(ByVal Value As String)
                _Dependencies = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the  Description of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Folder Name of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FolderName() As String
            Get
                Return _FolderName
            End Get
            Set(ByVal Value As String)
                _FolderName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Friendly Name of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FriendlyName() As String
            Get
                Return _FriendlyName
            End Get
            Set(ByVal Value As String)
                _FriendlyName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Module is an Admin Module
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsAdmin() As Boolean
            Get
                Return _IsAdmin
            End Get
            Set(ByVal Value As Boolean)
                _IsAdmin = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Module is Portable
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsPortable() As Boolean
            Get
                Return GetFeature(DesktopModuleSupportedFeature.IsPortable)
            End Get
            Set(ByVal Value As Boolean)
                UpdateFeature(DesktopModuleSupportedFeature.IsPortable, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Module is a Premium Module
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsPremium() As Boolean
            Get
                Return _IsPremium
            End Get
            Set(ByVal Value As Boolean)
                _IsPremium = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Module is Searchable
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsSearchable() As Boolean
            Get
                Return GetFeature(DesktopModuleSupportedFeature.IsSearchable)
            End Get
            Set(ByVal Value As Boolean)
                UpdateFeature(DesktopModuleSupportedFeature.IsSearchable, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Module is Upgradable
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsUpgradeable() As Boolean
            Get
                Return GetFeature(DesktopModuleSupportedFeature.IsUpgradeable)
            End Get
            Set(ByVal Value As Boolean)
                UpdateFeature(DesktopModuleSupportedFeature.IsUpgradeable, Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Module Definitions for this Desktop Module
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleDefinitions() As Dictionary(Of String, ModuleDefinitionInfo)
            Get
                If _ModuleDefinitions Is Nothing Then
                    If DesktopModuleID > Null.NullInteger Then
                        _ModuleDefinitions = ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(DesktopModuleID)
                    Else
                        _ModuleDefinitions = New Dictionary(Of String, ModuleDefinitionInfo)
                    End If
                End If
                Return _ModuleDefinitions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the  Name of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleName() As String
            Get
                Return _ModuleName
            End Get
            Set(ByVal Value As String)
                _ModuleName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a list of Permissions for the module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Permissions() As String
            Get
                Return _Permissions
            End Get
            Set(ByVal Value As String)
                _Permissions = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Supported Features of the Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SupportedFeatures() As Integer
            Get
                Return (_SupportedFeatures)
            End Get
            Set(ByVal Value As Integer)
                _SupportedFeatures = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Version of the Desktop Module
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Version() As String
            Get
                Return _Version
            End Get
            Set(ByVal Value As String)
                _Version = Value
            End Set
        End Property

#End Region

#Region "Private Helper Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clears a Feature from the Features
        ''' </summary>
        ''' <param name="Feature">The feature to Clear</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ClearFeature(ByVal Feature As DesktopModuleSupportedFeature)
            'And with the 1's complement of Feature to Clear the Feature flag
            SupportedFeatures = SupportedFeatures And (Not Feature)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Feature from the Features
        ''' </summary>
        ''' <param name="Feature">The feature to Get</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFeature(ByVal Feature As DesktopModuleSupportedFeature) As Boolean
            Dim isSet As Boolean = False
            'And with the Feature to see if the flag is set
            If SupportedFeatures > Null.NullInteger AndAlso (SupportedFeatures And Feature) = Feature Then
                isSet = True
            End If
            Return isSet
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a Feature in the Features
        ''' </summary>
        ''' <param name="Feature">The feature to Set</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetFeature(ByVal Feature As DesktopModuleSupportedFeature)
            'Or with the Feature to Set the Feature flag
            SupportedFeatures = SupportedFeatures Or Feature
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Feature in the Features
        ''' </summary>
        ''' <param name="Feature">The feature to Set</param>
        ''' <param name="IsSet">A Boolean indicating whether to set or clear the feature</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateFeature(ByVal Feature As DesktopModuleSupportedFeature, ByVal IsSet As Boolean)
            If IsSet Then
                SetFeature(Feature)
            Else
                ClearFeature(Feature)
            End If
        End Sub

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a DesktopModuleInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill

            DesktopModuleID = Null.SetNullInteger(dr("DesktopModuleID"))
            PackageID = Null.SetNullInteger(dr("PackageID"))
            ModuleName = Null.SetNullString(dr("ModuleName"))
            FriendlyName = Null.SetNullString(dr("FriendlyName"))
            Description = Null.SetNullString(dr("Description"))
            FolderName = Null.SetNullString(dr("FolderName"))
            Version = Null.SetNullString(dr("Version"))
            Description = Null.SetNullString(dr("Description"))
            IsPremium = Null.SetNullBoolean(dr("IsPremium"))
            IsAdmin = Null.SetNullBoolean(dr("IsAdmin"))
            BusinessControllerClass = Null.SetNullString(dr("BusinessControllerClass"))
            SupportedFeatures = Null.SetNullInteger(dr("SupportedFeatures"))
            CompatibleVersions = Null.SetNullString(dr("CompatibleVersions"))
            Dependencies = Null.SetNullString(dr("Dependencies"))
            Permissions = Null.SetNullString(dr("Permissions"))
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return DesktopModuleID
            End Get
            Set(ByVal value As Integer)
                DesktopModuleID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the DesktopModule
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a Supported Features from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadSupportedFeatures(ByVal reader As XmlReader)
            SupportedFeatures = 0
            reader.ReadStartElement("supportedFeatures")
            Do
                If reader.HasAttributes Then
                    reader.MoveToFirstAttribute()
                    Select Case reader.ReadContentAsString
                        Case "Portable"
                            IsPortable = True
                        Case "Searchable"
                            IsSearchable = True
                        Case "Upgradeable"
                            IsUpgradeable = True
                    End Select
                End If
            Loop While reader.ReadToNextSibling("supportedFeature")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a Module Definitions from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadModuleDefinitions(ByVal reader As XmlReader)
            reader.ReadStartElement("moduleDefinitions")
            Do
                reader.ReadStartElement("moduleDefinition")

                'Create new ModuleDefinition object
                Dim moduleDefinition As New ModuleDefinitionInfo

                'Load it from the Xml
                moduleDefinition.ReadXml(reader)

                'Add to the collection
                ModuleDefinitions.Add(moduleDefinition.FriendlyName, moduleDefinition)

            Loop While reader.ReadToNextSibling("moduleDefinition")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a DesktopModuleInfo from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            While reader.Read()
                If reader.NodeType = XmlNodeType.EndElement Then
                    Exit While
                ElseIf reader.NodeType = XmlNodeType.Whitespace Then
                    Continue While
                ElseIf reader.NodeType = XmlNodeType.Element AndAlso reader.Name = "moduleDefinitions" AndAlso Not reader.IsEmptyElement Then
                    ReadModuleDefinitions(reader)
                ElseIf reader.NodeType = XmlNodeType.Element AndAlso reader.Name = "supportedFeatures" AndAlso Not reader.IsEmptyElement Then
                    ReadSupportedFeatures(reader)
                Else
                    Select Case reader.Name
                        Case "moduleName"
                            ModuleName = reader.ReadElementContentAsString()
                        Case "foldername"
                            FolderName = reader.ReadElementContentAsString()
                        Case "businessControllerClass"
                            BusinessControllerClass = reader.ReadElementContentAsString()
                        Case "codeSubDirectory"
                            CodeSubDirectory = reader.ReadElementContentAsString()
                    End Select
                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a DesktopModuleInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("desktopModule")

            'write out properties
            writer.WriteElementString("moduleName", ModuleName)
            writer.WriteElementString("foldername", FolderName)
            writer.WriteElementString("businessControllerClass", BusinessControllerClass)
            If Not String.IsNullOrEmpty(CodeSubDirectory) Then
                writer.WriteElementString("codeSubDirectory", CodeSubDirectory)
            End If

            'Write out Supported Features
            writer.WriteStartElement("supportedFeatures")
            If IsPortable Then
                writer.WriteStartElement("supportedFeature")
                writer.WriteAttributeString("type", "Portable")
                writer.WriteEndElement()
            End If
            If IsSearchable Then
                writer.WriteStartElement("supportedFeature")
                writer.WriteAttributeString("type", "Searchable")
                writer.WriteEndElement()
            End If
            If IsUpgradeable Then
                writer.WriteStartElement("supportedFeature")
                writer.WriteAttributeString("type", "Upgradeable")
                writer.WriteEndElement()
            End If

            'Write end of Supported Features
            writer.WriteEndElement()

            'Write start of Module Definitions
            writer.WriteStartElement("moduleDefinitions")

            'Iterate through definitions
            For Each definition As ModuleDefinitionInfo In ModuleDefinitions.Values
                definition.WriteXml(writer)
            Next

            'Write end of Module Definitions
            writer.WriteEndElement()

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

