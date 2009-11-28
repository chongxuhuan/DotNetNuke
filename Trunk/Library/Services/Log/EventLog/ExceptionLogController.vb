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

Namespace DotNetNuke.Services.Log.EventLog


	Public Class ExceptionLogController
		Inherits LogController

		Public Enum ExceptionLogType
			GENERAL_EXCEPTION
			MODULE_LOAD_EXCEPTION
			PAGE_LOAD_EXCEPTION
            SCHEDULER_EXCEPTION
            SECURITY_EXCEPTION
            SEARCH_INDEXER_EXCEPTION
            DATA_EXCEPTION
		End Enum

        Public Overloads Sub AddLog(ByVal objException As Exception)
            AddLog(objException, ExceptionLogType.GENERAL_EXCEPTION)
        End Sub

        Public Overloads Sub AddLog(ByVal objBasePortalException As BasePortalException)
            If objBasePortalException.GetType.Name = "ModuleLoadException" Then
                AddLog(objBasePortalException, ExceptionLogType.MODULE_LOAD_EXCEPTION)
            ElseIf objBasePortalException.GetType.Name = "PageLoadException" Then
                AddLog(objBasePortalException, ExceptionLogType.PAGE_LOAD_EXCEPTION)
            ElseIf objBasePortalException.GetType.Name = "SchedulerException" Then
                AddLog(objBasePortalException, ExceptionLogType.SCHEDULER_EXCEPTION)
            ElseIf objBasePortalException.GetType.Name = "SecurityException" Then
                AddLog(objBasePortalException, ExceptionLogType.SECURITY_EXCEPTION)
            ElseIf objBasePortalException.GetType.Name = "SearchException" Then
                AddLog(objBasePortalException, ExceptionLogType.SEARCH_INDEXER_EXCEPTION)
            Else
                AddLog(objBasePortalException, ExceptionLogType.GENERAL_EXCEPTION)
            End If
        End Sub

        Public Overloads Sub AddLog(ByVal objException As Exception, ByVal LogType As ExceptionLogType)

            Dim objLogController As New LogController
            Dim objLogInfo As New LogInfo
            objLogInfo.LogTypeKey = LogType.ToString
            If LogType = ExceptionLogType.SEARCH_INDEXER_EXCEPTION Then
                'Add SearchException Properties
                Dim objSearchException As SearchException = CType(objException, SearchException)
                objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleId", objSearchException.SearchItem.ModuleId.ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("SearchItemId", objSearchException.SearchItem.SearchItemId.ToString()))
                objLogInfo.LogProperties.Add(New LogDetailInfo("Title", objSearchException.SearchItem.Title))
                objLogInfo.LogProperties.Add(New LogDetailInfo("SearchKey", objSearchException.SearchItem.SearchKey))
                objLogInfo.LogProperties.Add(New LogDetailInfo("GUID", objSearchException.SearchItem.GUID))
            ElseIf LogType = ExceptionLogType.MODULE_LOAD_EXCEPTION Then
                'Add ModuleLoadException Properties
                Dim objModuleLoadException As ModuleLoadException = CType(objException, ModuleLoadException)
                objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleId", objModuleLoadException.ModuleId.ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleDefId", objModuleLoadException.ModuleDefId.ToString))
                objLogInfo.LogProperties.Add(New LogDetailInfo("FriendlyName", objModuleLoadException.FriendlyName))
                objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleControlSource", objModuleLoadException.ModuleControlSource))
            ElseIf LogType = ExceptionLogType.SECURITY_EXCEPTION Then
                'Add SecurityException Properties
                Dim objSecurityException As SecurityException = CType(objException, SecurityException)
                objLogInfo.LogProperties.Add(New LogDetailInfo("Querystring", objSecurityException.Querystring))
                objLogInfo.LogProperties.Add(New LogDetailInfo("IP", objSecurityException.IP.ToString))
            End If

            'Add BasePortalException Properties
            Dim objBasePortalException As BasePortalException = New BasePortalException(objException.ToString, objException)
            objLogInfo.LogProperties.Add(New LogDetailInfo("AssemblyVersion", objBasePortalException.AssemblyVersion))
            objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", objBasePortalException.PortalID.ToString))
            objLogInfo.LogProperties.Add(New LogDetailInfo("PortalName", objBasePortalException.PortalName))
            objLogInfo.LogProperties.Add(New LogDetailInfo("UserID", objBasePortalException.UserID.ToString))
            objLogInfo.LogProperties.Add(New LogDetailInfo("UserName", objBasePortalException.UserName))
            objLogInfo.LogProperties.Add(New LogDetailInfo("ActiveTabID", objBasePortalException.ActiveTabID.ToString))
            objLogInfo.LogProperties.Add(New LogDetailInfo("ActiveTabName", objBasePortalException.ActiveTabName))
            objLogInfo.LogProperties.Add(New LogDetailInfo("RawURL", objBasePortalException.RawURL))
            objLogInfo.LogProperties.Add(New LogDetailInfo("AbsoluteURL", objBasePortalException.AbsoluteURL))
            objLogInfo.LogProperties.Add(New LogDetailInfo("AbsoluteURLReferrer", objBasePortalException.AbsoluteURLReferrer))
            objLogInfo.LogProperties.Add(New LogDetailInfo("UserAgent", objBasePortalException.UserAgent))
            objLogInfo.LogProperties.Add(New LogDetailInfo("DefaultDataProvider", objBasePortalException.DefaultDataProvider))
            objLogInfo.LogProperties.Add(New LogDetailInfo("ExceptionGUID", objBasePortalException.ExceptionGUID))
            objLogInfo.LogProperties.Add(New LogDetailInfo("InnerException", objBasePortalException.InnerException.Message))
            objLogInfo.LogProperties.Add(New LogDetailInfo("FileName", objBasePortalException.FileName))
            objLogInfo.LogProperties.Add(New LogDetailInfo("FileLineNumber", objBasePortalException.FileLineNumber.ToString))
            objLogInfo.LogProperties.Add(New LogDetailInfo("FileColumnNumber", objBasePortalException.FileColumnNumber.ToString))
            objLogInfo.LogProperties.Add(New LogDetailInfo("Method", objBasePortalException.Method))
            objLogInfo.LogProperties.Add(New LogDetailInfo("StackTrace", objBasePortalException.StackTrace))
            objLogInfo.LogProperties.Add(New LogDetailInfo("Message", objBasePortalException.Message))
            objLogInfo.LogProperties.Add(New LogDetailInfo("Source", objBasePortalException.Source))

            objLogInfo.LogPortalID = objBasePortalException.PortalID

            objLogController.AddLog(objLogInfo)
        End Sub

    End Class

End Namespace





