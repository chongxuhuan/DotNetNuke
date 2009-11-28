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
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AssemblyInstaller installs Assembly Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AssemblyInstaller
        Inherits FileInstaller

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("assemblies")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "assemblies"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the default Path for the file - if not present in the manifest
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/10/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property DefaultPath() As String
            Get
                Return "bin\"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("assembly")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "assembly"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PhysicalBasePath for the assemblies
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property PhysicalBasePath() As String
            Get
                Return PhysicalSitePath + "\"
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
                Return "dll"
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteFile method deletes a single assembly.
        ''' </summary>
        ''' <param name="file">The InstallFile to delete</param>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub DeleteFile(ByVal file As InstallFile)
            'Attempt to unregister assembly this will return False if the assembly is used by another package and
            'cannot be delete andtrue if it is not being used and can be deleted
            If DataProvider.Instance.UnRegisterAssembly(Me.Package.PackageID, file.Name) Then
                Log.AddInfo(Util.ASSEMBLY_UnRegistered + " - " + file.FullName)
                'Call base class version to deleteFile file from \bin
                MyBase.DeleteFile(file)
            Else
                Log.AddInfo(Util.ASSEMBLY_InUse + " - " + file.FullName)
            End If
        End Sub

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
            Return (type = InstallFileType.Assembly)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The InstallFile method installs a single assembly.
        ''' </summary>
        ''' <param name="file">The InstallFile to install</param>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function InstallFile(ByVal file As InstallFile) As Boolean
            Dim bSuccess As Boolean = True

            If file.Action = "UnRegister" Then
                DeleteFile(file)
            Else
                'Attempt to register assembly this will return False if the assembly exists and true if it does not or is older
                Dim returnCode As Integer = DataProvider.Instance.RegisterAssembly(Me.Package.PackageID, file.Name, file.Version.ToString(3))
                Select Case returnCode
                    Case 0
                        'Assembly Does Not Exist
                        Log.AddInfo(Util.ASSEMBLY_Added + " - " + file.FullName)
                    Case 1
                        'Older version of Assembly Exists
                        Log.AddInfo(Util.ASSEMBLY_Updated + " - " + file.FullName)
                    Case 2, 3
                        'Assembly already Registered
                        Log.AddInfo(Util.ASSEMBLY_Registered + " - " + file.FullName)
                End Select

                'If assembly not registered, is newer (or is the same version and we are in repair mode)
                If returnCode < 2 OrElse (returnCode = 2 AndAlso file.InstallerInfo.RepairInstall) Then
                    'Call base class version to copy file to \bin
                    bSuccess = MyBase.InstallFile(file)
                End If
            End If
            Return bSuccess
        End Function

#End Region

    End Class

End Namespace
