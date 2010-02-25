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
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Html
    ''' Project:    DotNetNuke
    ''' Class:      WorkflowStateController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The WorkflowStateController is the Controller class for managing workflows and states for the HtmlText module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WorkflowStateController

        Private Const WORKFLOW_CACHE_KEY As String = "Workflow{0}"
        Private Const WORKFLOW_CACHE_TIMEOUT As Integer = 20
        Private Const WORKFLOW_CACHE_PRIORITY As CacheItemPriority = CacheItemPriority.Normal

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlows retrieves a collection of workflows for the portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflows(ByVal PortalID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().GetWorkflows(PortalID), GetType(WorkflowStateInfo))

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlowStates retrieves a collection of WorkflowStateInfo objects for the Workflow from the cache
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflowStates(ByVal WorkflowID As Integer) As ArrayList

            Dim cacheKey As String = String.Format(WORKFLOW_CACHE_KEY, WorkflowID.ToString())
            Return CBO.GetCachedObject(Of ArrayList)(New CacheItemArgs(cacheKey, WORKFLOW_CACHE_TIMEOUT, WORKFLOW_CACHE_PRIORITY, WorkflowID), AddressOf GetWorkflowStatesCallBack)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlowStatesCallback retrieves a collection of WorkflowStateInfo objects for the Workflow from the database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="cacheItemArgs">Arguments passed by the GetWorkflowStates method</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflowStatesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object

            Dim WorkflowID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillCollection(DataProvider.Instance().GetWorkflowStates(WorkflowID), GetType(WorkflowStateInfo))

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFirstWorkFlowStateID retrieves the first StateID for the Workflow
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetFirstWorkflowStateID(ByVal WorkflowID As Integer) As Integer
            Dim intStateID As Integer = -1
            Dim arrWorkflowStates As ArrayList = GetWorkflowStates(WorkflowID)
            If arrWorkflowStates.Count > 0 Then
                intStateID = CType(arrWorkflowStates(0), WorkflowStateInfo).StateID
            End If
            Return intStateID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPreviousWorkFlowStateID retrieves the previous StateID for the Workflow and State specified
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <param name="StateID">The ID of the State</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPreviousWorkflowStateID(ByVal WorkflowID As Integer, ByVal StateID As Integer) As Integer
            Dim intPreviousStateID As Integer = -1
            Dim arrWorkflowStates As ArrayList = GetWorkflowStates(WorkflowID)
            Dim intItem As Integer = 0

            ' locate the current state
            For intItem = 0 To arrWorkflowStates.Count - 1
                If CType(arrWorkflowStates(intItem), WorkflowStateInfo).StateID = StateID Then
                    intPreviousStateID = StateID
                    Exit For
                End If
            Next

            ' get previous active state
            If intPreviousStateID = StateID Then
                intItem = intItem - 1
                While intItem >= 0
                    If CType(arrWorkflowStates(intItem), WorkflowStateInfo).IsActive Then
                        intPreviousStateID = CType(arrWorkflowStates(intItem), WorkflowStateInfo).StateID
                        Exit While
                    End If
                    intItem = intItem - 1
                End While
            End If

            ' if none found then reset to first state
            If intPreviousStateID = -1 Then
                intPreviousStateID = GetFirstWorkflowStateID(WorkflowID)
            End If

            Return intPreviousStateID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetNextWorkFlowStateID retrieves the next StateID for the Workflow and State specified
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <param name="StateID">The ID of the State</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetNextWorkflowStateID(ByVal WorkflowID As Integer, ByVal StateID As Integer) As Integer
            Dim intNextStateID As Integer = -1
            Dim arrWorkflowStates As ArrayList = GetWorkflowStates(WorkflowID)
            Dim intItem As Integer = 0

            ' locate the current state
            For intItem = 0 To arrWorkflowStates.Count - 1
                If CType(arrWorkflowStates(intItem), WorkflowStateInfo).StateID = StateID Then
                    intNextStateID = StateID
                    Exit For
                End If
            Next

            ' get next active state
            If intNextStateID = StateID Then
                intItem = intItem + 1
                While intItem < arrWorkflowStates.Count
                    If CType(arrWorkflowStates(intItem), WorkflowStateInfo).IsActive Then
                        intNextStateID = CType(arrWorkflowStates(intItem), WorkflowStateInfo).StateID
                        Exit While
                    End If
                    intItem = intItem + 1
                End While
            End If

            ' if none found then reset to first state
            If intNextStateID = -1 Then
                intNextStateID = GetFirstWorkflowStateID(WorkflowID)
            End If

            Return intNextStateID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetLastWorkFlowStateID retrieves the last StateID for the Workflow
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetLastWorkflowStateID(ByVal WorkflowID As Integer) As Integer
            Dim intStateID As Integer = -1
            Dim arrWorkflowStates As ArrayList = GetWorkflowStates(WorkflowID)
            If arrWorkflowStates.Count > 0 Then
                intStateID = CType(arrWorkflowStates(arrWorkflowStates.Count - 1), WorkflowStateInfo).StateID
            End If
            Return intStateID
        End Function

#End Region

    End Class

End Namespace

