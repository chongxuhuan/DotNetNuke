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

Imports System.Collections.Generic
Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ScriptInstaller installs Script Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	08/07/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ScriptInstaller
        Inherits FileInstaller

#Region "Private Members"

        Private _InstallScript As InstallFile
        Private _InstallScripts As New SortedList(Of System.Version, InstallFile)
        Private _Transaction As DbTransaction
        Private _UnInstallScripts As New SortedList(Of System.Version, InstallFile)
        Private _UpgradeScript As InstallFile

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the base Install Script (if present)
        ''' </summary>
        ''' <value>An InstallFile</value>
        ''' <history>
        ''' 	[cnurse]	05/20/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property InstallScript() As InstallFile
            Get
                Return _InstallScript
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of Install Scripts
        ''' </summary>
        ''' <value>A List(Of InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property InstallScripts() As SortedList(Of System.Version, InstallFile)
            Get
                Return _InstallScripts
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of UnInstall Scripts
        ''' </summary>
        ''' <value>A List(Of InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UnInstallScripts() As SortedList(Of System.Version, InstallFile)
            Get
                Return _UnInstallScripts
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("scripts")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "scripts"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("script")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "script"
            End Get
        End Property

        Protected ReadOnly Property ProviderConfiguration() As Framework.Providers.ProviderConfiguration
            Get
                Return Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data")
            End Get
        End Property

        '''' -----------------------------------------------------------------------------
        '''' <summary>
        '''' Gets the Database Transaction
        '''' </summary>
        '''' <value>A DbTransaction object</value>
        '''' <history>
        '''' 	[cnurse]	08/08/2007  created
        '''' </history>
        '''' -----------------------------------------------------------------------------
        'Protected ReadOnly Property Transaction() As DbTransaction
        '    Get
        '        If _Transaction Is Nothing Then
        '            _Transaction = DataProvider.Instance.GetTransaction()
        '        End If
        '        Return _Transaction
        '    End Get
        'End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Upgrade Script (if present)
        ''' </summary>
        ''' <value>An InstallFile</value>
        ''' <history>
        ''' 	[cnurse]	07/14/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UpgradeScript() As InstallFile
            Get
                Return _UpgradeScript
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "*dataprovider"
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function ExecuteSql(ByVal scriptFile As InstallFile, ByVal useTransaction As Boolean) As Boolean

            Dim bSuccess As Boolean = True

            Log.AddInfo(String.Format(Util.SQL_BeginFile, scriptFile.Name))

            ' read script file for installation
            Dim strScript As String = FileSystemUtils.ReadFile(PhysicalBasePath + scriptFile.FullName)

            'This check needs to be included because the unicode Byte Order mark results in an extra character at the start of the file
            'The extra character - '?' - causes an error with the database.
            If strScript.StartsWith("?") Then
                strScript = strScript.Substring(1)
            End If

            Dim strSQLExceptions As String = Null.NullString

            'If useTransaction Then
            '    strSQLExceptions = DataProvider.Instance.ExecuteScript(strScript, Transaction)
            'Else
            strSQLExceptions = DataProvider.Instance.ExecuteScript(strScript)
            'End If

            If strSQLExceptions <> "" Then
                If Package.InstallerInfo.IsLegacyMode Then
                    Log.AddWarning(String.Format(Util.SQL_Exceptions, vbCrLf, strSQLExceptions))
                Else
                    Log.AddFailure(String.Format(Util.SQL_Exceptions, vbCrLf, strSQLExceptions))
                    bSuccess = False
                End If
            End If

            Log.AddInfo(String.Format(Util.SQL_EndFile, scriptFile.Name))

            Return bSuccess
        End Function

#End Region

#Region "Protected Methods"

        Private Function InstallScriptFile(ByVal scriptFile As InstallFile) As Boolean
            'Call base InstallFile method to copy file
            Dim bSuccess As Boolean = InstallFile(scriptFile)

            'Process the file if it is an Install Script
            If bSuccess AndAlso ProviderConfiguration.DefaultProvider.ToLower = Path.GetExtension(scriptFile.Name.ToLower).Substring(1) Then
                Log.AddInfo(Util.SQL_Executing + scriptFile.Name)
                'bSuccess = ExecuteSql(scriptFile, True)
                bSuccess = ExecuteSql(scriptFile, False)
            End If

            Return bSuccess
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that determines what type of file this installer supports
        ''' </summary>
        ''' <param name="type">The type of file being processed</param>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function IsCorrectType(ByVal type As InstallFileType) As Boolean
            Return (type = InstallFileType.Script)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ProcessFile method determines what to do with parsed "file" node
        ''' </summary>
        ''' <param name="file">The file represented by the node</param>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ProcessFile(ByVal file As InstallFile, ByVal nav As XPathNavigator)
            Dim type As String = nav.GetAttribute("type", "")

            If file IsNot Nothing AndAlso IsCorrectType(file.Type) Then
                If file.Name.ToLower.StartsWith("install.") Then
                    'This is the initial script when installing
                    _InstallScript = file
                ElseIf file.Name.ToLower.StartsWith("upgrade.") Then
                    _UpgradeScript = file
                ElseIf type.ToLower = "install" Then
                    'These are the Install/Upgrade scripts
                    InstallScripts(file.Version) = file
                Else
                    'These are the Uninstall scripts
                    UnInstallScripts(file.Version) = file
                End If
            End If

            'Call base method to set up for file processing
            MyBase.ProcessFile(file, nav)
        End Sub

        Protected Overrides Sub UnInstallFile(ByVal scriptFile As InstallFile)
            'Process the file if it is an UnInstall Script
            If UnInstallScripts.ContainsValue(scriptFile) AndAlso ProviderConfiguration.DefaultProvider.ToLower = Path.GetExtension(scriptFile.Name.ToLower).Substring(1) Then
                If scriptFile.Name.ToLower.StartsWith("uninstall.") Then
                    'Install Script
                    Log.AddInfo(Util.SQL_Executing + scriptFile.Name)
                    ExecuteSql(scriptFile, False)
                End If
            End If

            'Call base method to delete file
            MyBase.UnInstallFile(scriptFile)
        End Sub


#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <remarks>In the case of Files this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
            'Try
            '    DataProvider.Instance.CommitTransaction(Transaction)

            '    If Transaction.Connection IsNot Nothing Then
            '        Transaction.Connection.Close()
            '    End If
            '    Log.AddInfo(Util.SQL_Committed)
            'Catch ex As Exception
            '    Log.AddFailure(Util.EXCEPTION + " - " + ex.Message)
            'End Try

            'Call base method
            MyBase.Commit()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the script component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Log.AddInfo(Util.SQL_Begin)

            Try
                Dim bSuccess As Boolean = True
                Dim installedVersion As System.Version = Package.InstalledVersion

                'First process InstallScript
                If installedVersion = New Version(0, 0, 0) Then
                    'New Install so process InstallScript if present
                    If InstallScript IsNot Nothing Then
                        bSuccess = InstallScriptFile(InstallScript)
                        installedVersion = InstallScript.Version
                    End If
                End If

                'Then process remain Install/Upgrade Scripts
                If bSuccess Then
                    For Each file As InstallFile In InstallScripts.Values
                        If file.Version > installedVersion Then
                            bSuccess = InstallScriptFile(file)
                            If Not bSuccess Then
                                Exit For
                            End If
                        End If
                    Next
                End If

                'Next process UpgradeScript - this script always runs if present
                If UpgradeScript IsNot Nothing Then
                    bSuccess = InstallScriptFile(UpgradeScript)
                    installedVersion = UpgradeScript.Version
                End If

                'Then process uninstallScripts - these need to be copied but not executed
                If bSuccess Then
                    For Each file As InstallFile In UnInstallScripts.Values
                        bSuccess = InstallFile(file)
                        If Not bSuccess Then
                            Exit For
                        End If
                    Next
                End If

                Completed = bSuccess
            Catch ex As Exception
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message)
            End Try

            Log.AddInfo(Util.SQL_End)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the script component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'Try
            '    DataProvider.Instance.RollbackTransaction(Transaction)

            '    If Transaction.Connection IsNot Nothing Then
            '        Transaction.Connection.Close()
            '    End If
            '    Log.AddInfo(Util.SQL_RolledBack)
            'Catch ex As Exception
            '    Log.AddFailure(Util.EXCEPTION + " - " + ex.Message)
            'End Try

            'Call base method
            MyBase.Rollback()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the script component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            Log.AddInfo(Util.SQL_BeginUnInstall)

            'Call the base method
            MyBase.UnInstall()

            Log.AddInfo(Util.SQL_EndUnInstall)
        End Sub

#End Region

    End Class

End Namespace
