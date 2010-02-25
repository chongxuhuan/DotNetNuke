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

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DataProvider is an abstract class that provides the Data Access Layer for the HtmlText module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Html", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Abstract methods"

        Public MustOverride Function GetHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer) As IDataReader
        Public MustOverride Function GetTopHtmlText(ByVal ModuleID As Integer, ByVal IsPublished As Boolean) As IDataReader
        Public MustOverride Function GetAllHtmlText(ByVal ModuleID As Integer) As IDataReader
        Public MustOverride Function AddHtmlText(ByVal ModuleId As Integer, ByVal Content As String, ByVal StateID As Integer, ByVal IsPublished As Boolean, ByVal CreatedByUserID As Integer, ByVal History As Integer) As Integer
        Public MustOverride Sub UpdateHtmlText(ByVal ItemID As Integer, ByVal Content As String, ByVal StateID As Integer, ByVal IsPublished As Boolean, ByVal LastModifiedByUserID As Integer)
        Public MustOverride Sub DeleteHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer)

        Public MustOverride Function GetHtmlTextLog(ByVal ItemID As Integer) As IDataReader
        Public MustOverride Sub AddHtmlTextLog(ByVal ItemID As Integer, ByVal StateID As Integer, ByVal Comment As String, ByVal Approved As Boolean, ByVal CreatedByUserID As Integer)

        Public MustOverride Function GetHtmlTextUser(ByVal UserID As Integer) As IDataReader
        Public MustOverride Sub AddHtmlTextUser(ByVal ItemID As Integer, ByVal StateID As Integer, ByVal ModuleID As Integer, ByVal TabID As Integer, ByVal UserID As Integer)
        Public MustOverride Sub DeleteHtmlTextUsers()

        Public MustOverride Function GetWorkflows(ByVal PortalID As Integer) As IDataReader
        Public MustOverride Function GetWorkflowStates(ByVal WorkflowID As Integer) As IDataReader

        Public MustOverride Function GetWorkflowStatePermissions() As IDataReader
        Public MustOverride Function GetWorkflowStatePermissionsByStateID(ByVal StateID As Integer) As IDataReader
#End Region

    End Class

End Namespace
