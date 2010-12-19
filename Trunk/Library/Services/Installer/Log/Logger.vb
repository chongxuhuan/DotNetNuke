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
Imports System.Collections.Generic
Imports DotNetNuke.Services.Localization


Namespace DotNetNuke.Services.Installer.Log

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Logger class provides an Installer Log
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Logger

#Region "Private Members"

        Private _ErrorClass As String
        Private _HasWarnings As Boolean
        Private _HighlightClass As String
        Private _Logs As List(Of LogEntry)
        Private _NormalClass As String
        Private _Valid As Boolean

#End Region

#Region "Constructors"

        Public Sub New()
            _Logs = New List(Of LogEntry)
            _Valid = True
            _HasWarnings = Null.NullBoolean
        End Sub

#End Region

#Region "Public Propertys"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Css Class used for Error Log Entries
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ErrorClass() As String
            Get
                If _ErrorClass = "" Then
                    _ErrorClass = "NormalRed"
                End If
                Return _ErrorClass
            End Get
            Set(ByVal Value As String)
                _ErrorClass = Value
            End Set
        End Property

        Public ReadOnly Property HasWarnings() As Boolean
            Get
                Return _HasWarnings
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Css Class used for Log Entries that should be highlighted
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HighlightClass() As String
            Get
                If _HighlightClass = "" Then
                    _HighlightClass = "NormalBold"
                End If
                Return _HighlightClass
            End Get
            Set(ByVal Value As String)
                _HighlightClass = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a List of Log Entries
        ''' </summary>
        ''' <value>A List of LogEntrys</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Logs() As List(Of LogEntry)
            Get
                Return _Logs
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Css Class used for normal Log Entries
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property NormalClass() As String
            Get
                If _NormalClass = "" Then
                    _NormalClass = "Normal"
                End If
                Return _NormalClass
            End Get
            Set(ByVal Value As String)
                _NormalClass = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Flag that indicates whether the Installation was Valid
        ''' </summary>
        ''' <value>A List of LogEntrys</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Valid() As Boolean
            Get
                Return _Valid
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The AddFailure method adds a new LogEntry of type Failure to the Logs collection
        ''' </summary>
        ''' <remarks>This method also sets the Valid flag to false</remarks>
        ''' <param name="failure">The description of the LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddFailure(ByVal failure As String)
            _Logs.Add(New LogEntry(LogType.Failure, failure))
            _Valid = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The AddFailure method adds a new LogEntry of type Failure to the Logs collection
        ''' </summary>
        ''' <remarks>This method also sets the Valid flag to false</remarks>
        ''' <param name="ex">The Exception</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddFailure(ByVal ex As Exception)
            AddFailure((Util.EXCEPTION + ex.ToString()))
            LogException(ex)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The AddInfo method adds a new LogEntry of type Info to the Logs collection
        ''' </summary>
        ''' <param name="info">The description of the LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddInfo(ByVal info As String)
            _Logs.Add(New LogEntry(LogType.Info, info))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The AddWarning method adds a new LogEntry of type Warning to the Logs collection
        ''' </summary>
        ''' <param name="warning">The description of the LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddWarning(ByVal warning As String)
            _Logs.Add(New LogEntry(LogType.Warning, warning))
            _HasWarnings = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The EndJob method adds a new LogEntry of type EndJob to the Logs collection
        ''' </summary>
        ''' <param name="job">The description of the LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub EndJob(ByVal job As String)
            _Logs.Add(New LogEntry(LogType.EndJob, job))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetLogsTable formats log entries in an HtmlTable
        ''' </summary>
        ''' <history>
        '''   [jbrinkman] 24/11/2004  Created new method.  Moved from WebUpload.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetLogsTable() As HtmlTable
            Dim arrayTable As New HtmlTable

            For Each entry As LogEntry In Logs
                Dim tr As New HtmlTableRow
                Dim tdType As New HtmlTableCell
                tdType.InnerText = Util.GetLocalizedString("LOG.PALogger." & entry.Type.ToString)
                Dim tdDescription As New HtmlTableCell
                tdDescription.InnerText = entry.Description
                tr.Cells.Add(tdType)
                tr.Cells.Add(tdDescription)
                Select Case entry.Type
                    Case LogType.Failure, LogType.Warning
                        tdType.Attributes.Add("class", ErrorClass)
                        tdDescription.Attributes.Add("class", ErrorClass)
                    Case LogType.StartJob, LogType.EndJob
                        tdType.Attributes.Add("class", HighlightClass)
                        tdDescription.Attributes.Add("class", HighlightClass)
                    Case LogType.Info
                        tdType.Attributes.Add("class", NormalClass)
                        tdDescription.Attributes.Add("class", NormalClass)
                End Select
                arrayTable.Rows.Add(tr)
                If entry.Type = LogType.EndJob Then
                    Dim SpaceTR As New HtmlTableRow
                    Dim SpaceTD As New HtmlTableCell
                    SpaceTD.ColSpan = 2
                    SpaceTD.InnerHtml = "&nbsp;"
                    SpaceTR.Cells.Add(SpaceTD)
                    arrayTable.Rows.Add(SpaceTR)
                End If
            Next

            Return arrayTable
        End Function

        Public Sub ResetFlags()
            _Valid = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The StartJob method adds a new LogEntry of type StartJob to the Logs collection
        ''' </summary>
        ''' <param name="job">The description of the LogEntry</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub StartJob(ByVal job As String)
            _Logs.Add(New LogEntry(LogType.StartJob, job))
        End Sub

#End Region

    End Class

End Namespace
