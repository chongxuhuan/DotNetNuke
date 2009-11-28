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

Imports System.Reflection
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke.Search.Index
    ''' Class:      ModuleIndexer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ModuleIndexer is an implementation of the abstract IndexingProvider
    ''' class
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleIndexer
        Inherits IndexingProvider

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSearchIndexItems gets the SearchInfo Items for the Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetSearchIndexItems(ByVal PortalID As Integer) As SearchItemInfoCollection

            Dim SearchItems As New SearchItemInfoCollection
            Dim SearchCollection As SearchContentModuleInfoCollection = GetModuleList(PortalID)

            For Each ScModInfo As SearchContentModuleInfo In SearchCollection
                Try
                    Dim myCollection As SearchItemInfoCollection
                    myCollection = ScModInfo.ModControllerType.GetSearchItems(ScModInfo.ModInfo)
                    If Not myCollection Is Nothing Then
                        SearchItems.AddRange(myCollection)
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try
            Next

            Return SearchItems

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleList gets a collection of SearchContentModuleInfo Items for the Portal
        ''' </summary>
        ''' <remarks>
        ''' Parses the Modules of the Portal, determining whetehr they are searchable.
        ''' </remarks>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function GetModuleList(ByVal PortalID As Integer) As SearchContentModuleInfoCollection

            Dim Results As New SearchContentModuleInfoCollection

            Dim objModules As New ModuleController
            Dim arrModules As ArrayList = objModules.GetSearchModules(PortalID)
            Dim businessControllers As New Hashtable
            Dim htModules As New Hashtable

            Dim objModule As ModuleInfo
            For Each objModule In arrModules
                If Not htModules.ContainsKey(objModule.ModuleID) Then
                    Try
                        'Check if the business controller is in the Hashtable
                        Dim objController As Object = businessControllers(objModule.DesktopModule.BusinessControllerClass)

                        If objModule.DesktopModule.BusinessControllerClass <> "" Then
                            'If nothing create a new instance
                            If objController Is Nothing Then
                                objController = Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass)

                                'Add to hashtable
                                businessControllers.Add(objModule.DesktopModule.BusinessControllerClass, objController)
                            End If

                            'Double-Check that module supports ISearchable
                            If TypeOf objController Is ISearchable Then
                                Dim ContentInfo As New SearchContentModuleInfo
                                ContentInfo.ModControllerType = CType(objController, ISearchable)
                                ContentInfo.ModInfo = objModule
                                Results.Add(ContentInfo)
                            End If
                        End If
                    Catch ex As Exception
                        Try
                            Dim strMessage As String = _
                                String.Format("Error Creating BusinessControllerClass '{0}' of module({1}) id=({2}) in tab({3}) and portal({4}) ", _
                                objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.ModuleName, objModule.ModuleID, objModule.TabID, objModule.PortalID)
                            Throw New Exception(strMessage, ex)
                        Catch ex1 As Exception
                            LogException(ex1)
                        End Try
                    Finally
                        htModules.Add(objModule.ModuleID, objModule.ModuleID)
                    End Try
                End If
            Next

            Return Results

        End Function

    End Class

End Namespace
