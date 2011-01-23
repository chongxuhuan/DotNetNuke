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

Namespace DotNetNuke.Services.Log.EventLog

    Partial Public Class EventLogController
        Inherits LogController

        Public Enum EventLogType
            USER_CREATED
            USER_DELETED
            LOGIN_SUPERUSER
            LOGIN_SUCCESS
            LOGIN_FAILURE
            LOGIN_USERLOCKEDOUT
            LOGIN_USERNOTAPPROVED
            CACHE_REFRESHED
            PASSWORD_SENT_SUCCESS
            PASSWORD_SENT_FAILURE
            LOG_NOTIFICATION_FAILURE
            PORTAL_CREATED
            PORTAL_DELETED
            TAB_CREATED
            TAB_UPDATED
            TAB_DELETED
            TAB_SENT_TO_RECYCLE_BIN
            TAB_RESTORED
            USER_ROLE_CREATED
            USER_ROLE_DELETED
            USER_ROLE_UPDATED
            ROLE_CREATED
            ROLE_UPDATED
            ROLE_DELETED
            MODULE_CREATED
            MODULE_UPDATED
            MODULE_DELETED
            MODULE_SENT_TO_RECYCLE_BIN
            MODULE_RESTORED
            SCHEDULER_EVENT_STARTED
            SCHEDULER_EVENT_PROGRESSING
            SCHEDULER_EVENT_COMPLETED
            APPLICATION_START
            APPLICATION_END
            APPLICATION_SHUTTING_DOWN
            SCHEDULER_STARTED
            SCHEDULER_SHUTTING_DOWN
            SCHEDULER_STOPPED
            ADMIN_ALERT
            HOST_ALERT
            CACHE_REMOVED
            CACHE_EXPIRED
            CACHE_UNDERUSED
            CACHE_DEPENDENCYCHANGED
            CACHE_OVERFLOW
            CACHE_REFRESH
            LISTENTRY_CREATED
            LISTENTRY_UPDATED
            LISTENTRY_DELETED
            DESKTOPMODULE_CREATED
            DESKTOPMODULE_UPDATED
            DESKTOPMODULE_DELETED
            SKINCONTROL_CREATED
            SKINCONTROL_UPDATED
            SKINCONTROL_DELETED
            PORTALALIAS_CREATED
            PORTALALIAS_UPDATED
            PORTALALIAS_DELETED
            PROFILEPROPERTY_CREATED
            PROFILEPROPERTY_UPDATED
            PROFILEPROPERTY_DELETED
            USER_UPDATED
            DESKTOPMODULEPERMISSION_CREATED
            DESKTOPMODULEPERMISSION_UPDATED
            DESKTOPMODULEPERMISSION_DELETED
            PERMISSION_CREATED
            PERMISSION_UPDATED
            PERMISSION_DELETED
            TABPERMISSION_CREATED
            TABPERMISSION_UPDATED
            TABPERMISSION_DELETED
            AUTHENTICATION_CREATED
            AUTHENTICATION_UPDATED
            AUTHENTICATION_DELETED
            FOLDER_CREATED
            FOLDER_UPDATED
            FOLDER_DELETED
            PACKAGE_CREATED
            PACKAGE_UPDATED
            PACKAGE_DELETED
            LANGUAGEPACK_CREATED
            LANGUAGEPACK_UPDATED
            LANGUAGEPACK_DELETED
            LANGUAGE_CREATED
            LANGUAGE_UPDATED
            LANGUAGE_DELETED
            SKINPACKAGE_CREATED
            SKINPACKAGE_UPDATED
            SKINPACKAGE_DELETED
            SCHEDULE_CREATED
            SCHEDULE_UPDATED
            SCHEDULE_DELETED
            HOST_SETTING_CREATED
            HOST_SETTING_UPDATED
            HOST_SETTING_DELETED
            PORTALDESKTOPMODULE_CREATED
            PORTALDESKTOPMODULE_UPDATED
            PORTALDESKTOPMODULE_DELETED
            TABMODULE_CREATED
            TABMODULE_UPDATED
            TABMODULE_DELETED
            TABMODULE_SETTING_CREATED
            TABMODULE_SETTING_UPDATED
            TABMODULE_SETTING_DELETED
            MODULE_SETTING_CREATED
            MODULE_SETTING_UPDATED
            MODULE_SETTING_DELETED
            PORTAL_SETTING_CREATED
            PORTAL_SETTING_UPDATED
            PORTAL_SETTING_DELETED
            PORTALINFO_CREATED
            PORTALINFO_UPDATED
            PORTALINFO_DELETED
            AUTHENTICATION_USER_CREATED
            AUTHENTICATION_USER_UPDATED
            AUTHENTICATION_USER_DELETED
            LANGUAGETOPORTAL_CREATED
            LANGUAGETOPORTAL_UPDATED
            LANGUAGETOPORTAL_DELETED
			TAB_ORDER_UPDATED
			TAB_SETTING_CREATED
			TAB_SETTING_UPDATED
            TAB_SETTING_DELETED
            HOST_SQL_EXECUTED
        End Enum
    End Class
End Namespace
