'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Modules.Taxonomy
    Public Class TaxonomyController
        Implements DotNetNuke.Entities.Modules.IUpgradeable

        Public Function UpgradeModule(ByVal Version As String) As String Implements DotNetNuke.Entities.Modules.IUpgradeable.UpgradeModule
            Try
                Select Case Version
                    Case "01.00.00"
                        Dim moduleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Taxonomy Manager")

                        If moduleDefinition IsNot Nothing Then
                            'Add Module to Admin Page for all Portals
                            DotNetNuke.Services.Upgrade.Upgrade.AddAdminPages("Taxonomy", _
                                                "Manage the Taxonomy for your Site", _
                                                "~/images/icon_tag_16px.gif", _
                                                "~/images/icon_tag_32px.gif", _
                                                True, _
                                                moduleDefinition.ModuleDefID, _
                                                "Taxonomy Manager", _
                                                "~/images/icon_tag_32px.gif", _
                                                True)
                        End If
                End Select
                Return "Success"
            Catch ex As Exception
                Return "Failed"
            End Try
        End Function

    End Class
End Namespace

