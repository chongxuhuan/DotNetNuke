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
Imports System.Globalization
Imports System.IO
Imports System.Xml
Imports System.Xml.Schema
Imports ICSharpCode.SharpZipLib.Zip

Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Entities.Modules.Definitions

    Public Enum ModuleDefinitionVersion
        VUnknown = 0
        V1 = 1
        V2 = 2
        V2_Skin = 3
        V2_Provider = 4
        V3 = 5
    End Enum

    Public Class ModuleDefinitionValidator
        Inherits XmlValidatorBase

#Region "Private Methods"

        Private Function GetDnnSchemaPath(ByVal xmlStream As Stream) As String
            Dim Version As ModuleDefinitionVersion = GetModuleDefinitionVersion(xmlStream)
            Dim schemaPath As String = ""

            Select Case Version
                Case ModuleDefinitionVersion.V2
                    schemaPath = "components\ResourceInstaller\ModuleDef_V2.xsd"
                Case ModuleDefinitionVersion.V3
                    schemaPath = "components\ResourceInstaller\ModuleDef_V3.xsd"
                Case ModuleDefinitionVersion.V2_Skin
                    schemaPath = "components\ResourceInstaller\ModuleDef_V2Skin.xsd"
                Case ModuleDefinitionVersion.V2_Provider
                    schemaPath = "components\ResourceInstaller\ModuleDef_V2Provider.xsd"
                Case ModuleDefinitionVersion.VUnknown
                    Throw New Exception(GetLocalizedString("EXCEPTION_LoadFailed"))
            End Select
            Return Path.Combine(Common.Globals.ApplicationMapPath, schemaPath)
        End Function

        Private Function GetLocalizedString(ByVal key As String) As String

            Dim objPortalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            If objPortalSettings Is Nothing Then
                Return key
            Else
                Return Localization.GetString(key, objPortalSettings)
            End If

        End Function

#End Region


#Region "Public Methods"

        Public Function GetModuleDefinitionVersion(ByVal xmlStream As Stream) As ModuleDefinitionVersion
            xmlStream.Seek(0, SeekOrigin.Begin)
            Dim xmlReader As New XmlTextReader(xmlStream)
            xmlReader.MoveToContent()

            'This test assumes provides a simple validation 
            Select Case xmlReader.LocalName.ToLower
                Case "module"
                    Return ModuleDefinitionVersion.V1
                Case "dotnetnuke"
                    Select Case xmlReader.GetAttribute("type")
                        Case "Module"
                            Select Case xmlReader.GetAttribute("version")
                                Case "2.0"
                                    Return ModuleDefinitionVersion.V2
                                Case "3.0"
                                    Return ModuleDefinitionVersion.V3
                            End Select
                        Case "SkinObject"
                            Return ModuleDefinitionVersion.V2_Skin
                        Case "Provider"
                            Return ModuleDefinitionVersion.V2_Provider
                        Case Else
                            Return ModuleDefinitionVersion.VUnknown
                    End Select
                Case Else
                    Return ModuleDefinitionVersion.VUnknown
            End Select

        End Function

        Public Overloads Overrides Function Validate(ByVal XmlStream As Stream) As Boolean

            SchemaSet.Add("", GetDnnSchemaPath(XmlStream))
            Return MyBase.Validate(XmlStream)
        End Function

#End Region

    End Class

End Namespace
