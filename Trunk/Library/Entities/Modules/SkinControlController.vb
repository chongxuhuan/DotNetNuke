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

Imports System
Imports System.Data
Imports System.Collections.Generic

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : ModuleControlController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleControlController provides the Business Layer for Module Controls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinControlController

#Region "Private Members"

        Private Shared dataProvider As DataProvider = dataProvider.Instance()

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteSkinControl deletes a Skin Control in the database
        ''' </summary>
        ''' <param name="skinControl">The Skin Control to delete</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteSkinControl(ByVal skinControl As SkinControlInfo)
            dataProvider.DeleteSkinControl(skinControl.SkinControlID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINCONTROL_DELETED)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkinControl gets a single Skin Control from the database
        ''' </summary>
        ''' <param name="skinControlID">The ID of the SkinControl</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSkinControl(ByVal skinControlID As Integer) As SkinControlInfo
            Return CBO.FillObject(Of SkinControlInfo)(dataProvider.GetSkinControl(skinControlID))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkinControlByPackageID gets a single Skin Control from the database
        ''' </summary>
        ''' <param name="packageID">The ID of the Package</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSkinControlByPackageID(ByVal packageID As Integer) As SkinControlInfo
            Return CBO.FillObject(Of SkinControlInfo)(dataProvider.GetSkinControlByPackageID(packageID))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkinControlByKey gets a single Skin Control from the database
        ''' </summary>
        ''' <param name="key">The key of the Control</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSkinControlByKey(ByVal key As String) As SkinControlInfo
            Return CBO.FillObject(Of SkinControlInfo)(dataProvider.GetSkinControlByKey(key))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkinControls gets all the Skin Controls from the database
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSkinControls() As Dictionary(Of String, SkinControlInfo)
            Return CBO.FillDictionary(Of String, SkinControlInfo)("ControlKey", dataProvider.GetSkinControls(), New Dictionary(Of String, SkinControlInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveSkinControl updates a Skin Control in the database
        ''' </summary>
        ''' <param name="skinControl">The Skin Control to save</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SaveSkinControl(ByVal skinControl As SkinControlInfo) As Integer
            Dim skinControlID As Integer = skinControl.SkinControlID
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            If skinControlID = Null.NullInteger Then
                'Add new Skin Control
                skinControlID = dataProvider.AddSkinControl(skinControl.PackageID, skinControl.ControlKey, skinControl.ControlSrc, skinControl.SupportsPartialRendering, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINCONTROL_CREATED)
            Else
                'Upgrade Skin Control
                dataProvider.UpdateSkinControl(skinControl.SkinControlID, skinControl.PackageID, skinControl.ControlKey, skinControl.ControlSrc, skinControl.SupportsPartialRendering, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINCONTROL_UPDATED)
            End If

            Return skinControlID
        End Function

#End Region

    End Class


End Namespace

