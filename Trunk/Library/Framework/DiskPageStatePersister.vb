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

Imports System.IO
Imports System.Text

Namespace DotNetNuke.Framework

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Framework
    ''' Project:    DotNetNuke
    ''' Class:      DiskPageStatePersister
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' DiskPageStatePersister provides a disk (stream) based page state peristence mechanism
    ''' </summary>
    ''' <history>
    '''		[cnurse]	11/30/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DiskPageStatePersister
        Inherits PageStatePersister

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the DiskPageStatePersister
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal page As Page)
            MyBase.New(page)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CacheDirectory property is used to return the location of the "Cache"
        ''' Directory for the Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 11/30/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property CacheDirectory() As String
            Get
                Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The StateFileName property is used to store the FileName for the State
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 11/30/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property StateFileName() As String
            Get
                Dim key As New StringBuilder()
                With key
                    .Append("VIEWSTATE_")
                    .Append(Page.Session.SessionID)
                    .Append("_")
                    .Append(Page.Request.RawUrl)
                End With
                Return CacheDirectory & "\" & CleanFileName(key.ToString) & ".txt"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads the Page State from the Cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Load()

            Dim reader As StreamReader = Nothing

            ' Read the state string, using the StateFormatter.
            Try
                reader = New StreamReader(StateFileName)

                Dim serializedStatePair As String = reader.ReadToEnd

                Dim formatter As IStateFormatter = Me.StateFormatter

                ' Deserialize returns the Pair object that is serialized in
                ' the Save method.      
                Dim statePair As Pair = CType(formatter.Deserialize(serializedStatePair), Pair)

                ViewState = statePair.First
                ControlState = statePair.Second
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the Page State to the Cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Save()

            'No processing needed if no states available
            If ViewState Is Nothing And ControlState Is Nothing Then
                Exit Sub
            End If

            If Not (Page.Session Is Nothing) Then
                If Not Directory.Exists(CacheDirectory) Then
                    Directory.CreateDirectory(CacheDirectory)
                End If

                ' Write a state string, using the StateFormatter.
                Dim writer As New StreamWriter(StateFileName, False)

                Dim formatter As IStateFormatter = Me.StateFormatter

                Dim statePair As New Pair(ViewState, ControlState)

                Dim serializedState As String = formatter.Serialize(statePair)

                writer.Write(serializedState)
                writer.Close()
            End If
        End Sub
    End Class

End Namespace

