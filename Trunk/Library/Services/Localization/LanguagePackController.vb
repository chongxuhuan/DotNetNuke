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
Imports System.Data
Imports DotNetNuke
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Services.Localization

    Public Class LanguagePackController

        Public Shared Sub DeleteLanguagePack(ByVal languagePack As LanguagePackInfo)
            If languagePack.PackageType = LanguagePackType.Core Then
                Dim language As Locale = Localization.GetLocaleByID(languagePack.LanguageID)
                If language IsNot Nothing Then
                    Localization.DeleteLanguage(language)
                End If
            End If
            DataProvider.Instance.DeleteLanguagePack(languagePack.LanguagePackID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_DELETED)
        End Sub

        Public Shared Function GetLanguagePackByPackage(ByVal packageID As Integer) As LanguagePackInfo
            Return CBO.FillObject(Of LanguagePackInfo)(DataProvider.Instance.GetLanguagePackByPackage(packageID))
        End Function


        Public Shared Sub SaveLanguagePack(ByVal languagePack As LanguagePackInfo)
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            If languagePack.LanguagePackID = Null.NullInteger Then
                'Add Language Pack
                languagePack.LanguagePackID = DataProvider.Instance.AddLanguagePack(languagePack.PackageID, languagePack.LanguageID, languagePack.DependentPackageID, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_CREATED)
            Else
                'Update LanguagePack
                DataProvider.Instance.UpdateLanguagePack(languagePack.LanguagePackID, languagePack.PackageID, languagePack.LanguageID, languagePack.DependentPackageID, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_UPDATED)
            End If
        End Sub


    End Class

End Namespace
