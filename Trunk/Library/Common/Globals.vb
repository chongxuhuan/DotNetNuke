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
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Threading
Imports System.Xml
Imports System.Web
Imports System.Web.UI
Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Application
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.Skins
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Security.Permissions
Imports System.Text.RegularExpressions
Imports DotNetNuke.Entities


Namespace DotNetNuke.Common

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Common
    ''' Project:    DotNetNuke
    ''' Module:     Globals
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' This module contains global utility functions, constants, and enumerations.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/16/2004	documented
    '''     [vnguyen]   30/04/2010  modified: added Guid and VersionGuid to files and folders
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Module Globals

#Region "Public Enums"

        ''' <summary>
        ''' The values of the enum are used to calculate cache settings throughout the portal.
        ''' Calculating based on these numbers keeps the scaling linear for all caching.
        ''' </summary>
        Public Enum PerformanceSettings
            ''' No caching
            ''' </summary>
            NoCaching = 0
            ''' <summary>
            ''' VEry light chaching
            ''' </summary>
            LightCaching = 1
            ''' <summary>
            ''' Intermedia/moderate caching
            ''' </summary>
            ModerateCaching = 3
            ''' <summary>
            ''' Maximum caching
            ''' </summary>
            HeavyCaching = 6
        End Enum

        ''' <summary>
        ''' Status of upgrade or install
        ''' </summary>
        Public Enum UpgradeStatus
            ''' <summary>
            ''' Upgrade at hand
            ''' </summary>
            Upgrade
            ''' <summary>
            ''' Installation busy
            ''' </summary>
            Install
            ''' <summary>
            ''' Nothign special, just running
            ''' </summary>
            None
            ''' <summary>
            ''' There is something wrong here.
            ''' </summary>
            [Error]
        End Enum

        ''' <summary>
        ''' Signals the type or registration that is used for the portal
        ''' </summary>
        Public Enum PortalRegistrationType
            ''' <summary>
            ''' No registration at hand
            ''' </summary>
            NoRegistration = 0
            ''' <summary>
            ''' Only private registration
            ''' </summary>
            PrivateRegistration = 1
            ''' <summary>
            ''' People can register publically
            ''' </summary>
            PublicRegistration = 2
            ''' <summary>
            ''' Public registration with verification
            ''' </summary>
            VerifiedRegistration = 3
        End Enum


#End Region

#Region "Public Constants"
        ''' <summary>
        ''' Constant for the Role of All users
        ''' </summary>
        Public Const glbRoleAllUsers As String = "-1"
        ''' <summary>
        ''' Constant for the Role of SuperUser
        ''' </summary>
        Public Const glbRoleSuperUser As String = "-2"
        ''' <summary>
        ''' Constant for the Role of Unauthenticated users
        ''' </summary>
        Public Const glbRoleUnauthUser As String = "-3"
        ''' <summary>
        ''' Constant for the No Role
        ''' </summary>
        Public Const glbRoleNothing As String = "-4"

        ''' <summary>
        ''' Name of the role of All Users
        ''' </summary>
        Public Const glbRoleAllUsersName As String = "All Users"
        ''' <summary>
        ''' Name of the role of Superusers
        ''' </summary>
        Public Const glbRoleSuperUserName As String = "Superuser"
        ''' <summary>
        ''' Name of the role of Unauthenticated Users
        ''' </summary>
        Public Const glbRoleUnauthUserName As String = "Unauthenticated Users"

        ''' <summary>
        ''' Default page for every page
        ''' </summary>
        Public Const glbDefaultPage As String = "Default.aspx"
        ''' <summary>
        ''' Default subfolder in the Portals folder for host based skins
        ''' </summary>
        Public Const glbHostSkinFolder As String = "_default"
        ''' <summary>
        ''' Default page for the control panel
        ''' </summary>
        Public Const glbDefaultControlPanel As String = "Admin/ControlPanel/IconBar.ascx"
        ''' <summary>
        ''' Default name of the main content pane
        ''' </summary>
        Public Const glbDefaultPane As String = "ContentPane"
        ''' <summary>
        ''' Image file types that are supported
        ''' </summary>
        Public Const glbImageFileTypes As String = "jpg,jpeg,jpe,gif,bmp,png,swf"
        ''' <summary>
        ''' Default folder for congurations
        ''' </summary>
        Public Const glbConfigFolder As String = "\Config\"
        ''' <summary>
        ''' Default page used for the About
        ''' </summary>
        Public Const glbAboutPage As String = "about.htm"
        ''' <summary>
        ''' Default config file for DotnetNuke
        ''' </summary>
        Public Const glbDotNetNukeConfig As String = "DotNetNuke.config"

        ''' <summary>
        ''' Name of the application for the Superuser
        ''' </summary>
        Public Const glbSuperUserAppName As Integer = -1
        ''' <summary>
        ''' Default extension to be used for protected files
        ''' </summary>
        Public Const glbProtectedExtension As String = ".resources"
        ''' <summary>
        ''' RegEx for validation of an email address
        ''' </summary>
        Public Const glbEmailRegEx As String = "\b[a-zA-Z0-9._%\-+']+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,4}\b"
        ''' <summary>
        ''' Code to be used for inserting javascript in a page
        ''' </summary>
        Public Const glbScriptFormat As String = "<script type=""text/javascript"" src=""{0}"" ></script>"
#End Region

#Region "Private Members"

        ' global constants for the life of the application ( set in Application_Start )
        Private _ApplicationPath As String
        Private _ApplicationMapPath As String
        Private _DataBaseVersion As System.Version
        Private _HostMapPath As String
        Private _HostPath As String
        Private _InstallMapPath As String
        Private _InstallPath As String
        Private _IISAppName As String
        Private _IsInstalled As Boolean
        Private _ServerName As String
        Private _OperatingSystemVersion As System.Version
        Private _NETFrameworkVersion As System.Version
        Private _DatabaseEngineVersion As System.Version
        Private _Status As UpgradeStatus = UpgradeStatus.None

        Private _HostSettings As Hashtable
        Private _WebFarmEnabled As Boolean = Null.NullBoolean
        Private _WebFarmEnabledSet As Boolean = Null.NullBoolean
        Private _PerformanceSetting As PerformanceSettings

#End Region

#Region "Public Properties"
        ''' <summary>
        ''' Gets or sets the application path.
        ''' </summary>
        ''' <value>The application path.</value>
        Public Property ApplicationPath() As String
            Get
                Return _ApplicationPath
            End Get
            Set(ByVal Value As String)
                _ApplicationPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the application map path.
        ''' </summary>
        ''' <value>The application map path.</value>
        Public Property ApplicationMapPath() As String
            Get
                Return _ApplicationMapPath
            End Get
            Set(ByVal Value As String)
                _ApplicationMapPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the database version.
        ''' </summary>
        ''' <value>The database version.</value>
        Public ReadOnly Property DataBaseVersion() As System.Version
            Get
                Return _DataBaseVersion
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the host map path.
        ''' </summary>
        ''' <value>The host map path.</value>
        Public Property HostMapPath() As String
            Get
                Return _HostMapPath
            End Get
            Set(ByVal Value As String)
                _HostMapPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the host path.
        ''' </summary>
        ''' <value>The host path.</value>
        Public Property HostPath() As String
            Get
                Return _HostPath
            End Get
            Set(ByVal Value As String)
                _HostPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the install map path.
        ''' </summary>
        ''' <value>The install map path.</value>
        Public Property InstallMapPath() As String
            Get
                Return _InstallMapPath
            End Get
            Set(ByVal Value As String)
                _InstallMapPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the install path.
        ''' </summary>
        ''' <value>The install path.</value>
        Public Property InstallPath() As String
            Get
                Return _InstallPath
            End Get
            Set(ByVal Value As String)
                _InstallPath = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the IIS app.
        ''' </summary>
        ''' <value>The name of the IIS app.</value>
        Public Property IISAppName() As String
            Get
                Return _IISAppName
            End Get
            Set(ByVal value As String)
                _IISAppName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the server.
        ''' </summary>
        ''' <value>The name of the server.</value>
        Public Property ServerName() As String
            Get
                Return _ServerName
            End Get
            Set(ByVal Value As String)
                _ServerName = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the operating system version.
        ''' </summary>
        ''' <value>The operating system version.</value>
        Public Property OperatingSystemVersion() As System.Version
            Get
                Return _OperatingSystemVersion
            End Get
            Set(ByVal Value As System.Version)
                _OperatingSystemVersion = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the NET framework version.
        ''' </summary>
        ''' <value>The NET framework version.</value>
        Public Property NETFrameworkVersion() As System.Version
            Get
                Return _NETFrameworkVersion
            End Get
            Set(ByVal Value As System.Version)
                _NETFrameworkVersion = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the database engine version.
        ''' </summary>
        ''' <value>The database engine version.</value>
        Public Property DatabaseEngineVersion() As System.Version
            Get
                Return _DatabaseEngineVersion
            End Get
            Set(ByVal Value As System.Version)
                _DatabaseEngineVersion = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the Ugrade status.
        ''' <seealso cref="UpgradeStatus"></seealso></summary>
        ''' <value>The status.</value>
        Public ReadOnly Property Status() As UpgradeStatus
            Get
                Return _Status
            End Get
        End Property

#End Region

#Region "Installation Tests"

        ''' <summary>
        ''' IsInstalled looks at various file artifacts to determine if DotNetNuke has already been installed.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' If DotNetNuke has been installed, then we should treat database connection errors as real errors.  
        ''' If DotNetNuke has not been installed, then we should expect to have database connection problems
        ''' since the connection string may not have been configured yet, which can occur during the installation
        ''' wizard.
        ''' </remarks>
        Private Function IsInstalled() As Boolean
            Const c_PassingScore As Integer = 4

            Dim installationdatefactor As Integer = CType(IIf(HasInstallationDate(), 1, 0), Integer)
            Dim dataproviderfactor As Integer = CType(IIf(HasDataProviderLogFiles(), 3, 0), Integer)
            Dim htmlmodulefactor As Integer = CType(IIf(ModuleDirectoryExists("html"), 2, 0), Integer)
            Dim portaldirectoryfactor As Integer = CType(IIf(HasNonDefaultPortalDirectory(), 2, 0), Integer)
            'Dim directInstallfactor As Integer = CType(IIf(IsInstallationURL(), c_PassingScore, 0), Integer)
            Dim localexecutionfactor As Integer = CType(IIf(HttpContext.Current.Request.IsLocal, c_PassingScore - 1, 0), Integer)

            'This calculation ensures that you have a more than one item that indicates you have already installed DNN.
            'While it is possible that you might not have an installation date or that you have deleted log files
            'it is unlikely that you have removed every trace of an installation and yet still have a working install
            Return (Not IsInstallationURL()) AndAlso ((installationdatefactor + dataproviderfactor + htmlmodulefactor + portaldirectoryfactor + localexecutionfactor) >= c_PassingScore)
        End Function

        Private Function HasDataProviderLogFiles() As Boolean
            Dim currentdataprovider As Providers.Provider = Config.GetDefaultProvider("data")
            Dim providerpath As String = currentdataprovider.Attributes("providerPath")

            'If the provider path does not exist, then there can't be any log files
            If Not String.IsNullOrEmpty(providerpath) Then
                providerpath = HttpContext.Current.Server.MapPath(providerpath)
                If Directory.Exists(providerpath) Then
                    Return Directory.GetFiles(providerpath, "*.log.resources").Length > 0
                End If
            End If

            Return False
        End Function

        Private Function HasInstallationDate() As Boolean
            Return CType(IIf(Config.GetSetting("InstallationDate") Is Nothing, False, True), Boolean)
        End Function

        Private Function ModuleDirectoryExists(ByVal ModuleName As String) As Boolean
            Dim dir As String = Globals.ApplicationMapPath & "\desktopmodules\" & ModuleName
            Return Directory.Exists(dir)
        End Function

        Private Function HasNonDefaultPortalDirectory() As Boolean
            Dim dir As String = Globals.ApplicationMapPath & "\portals"
            If Directory.Exists(dir) Then
                Return Directory.GetDirectories(dir).Length > 1
            End If
            Return False
        End Function

        Private Function IsInstallationURL() As Boolean
            Dim requestURL As String = HttpContext.Current.Request.RawUrl.ToLowerInvariant

            Return requestURL.Contains("\install.aspx") Or requestURL.Contains("\installwizard.aspx")
        End Function

#End Region

        Private Function GetCultureCode(ByVal TabID As Integer, ByVal IsSuperTab As Boolean, ByVal settings As PortalSettings) As String
            Dim cultureCode As String = Null.NullString
            If settings IsNot Nothing Then
                Dim linkTab As TabInfo
                Dim controller As New TabController()
                If IsSuperTab Then
                    linkTab = controller.GetTab(TabID, Null.NullInteger, False)
                Else
                    linkTab = controller.GetTab(TabID, settings.PortalId, False)
                End If
                If linkTab IsNot Nothing Then
                    cultureCode = linkTab.CultureCode
                End If
                If String.IsNullOrEmpty(cultureCode) Then
                    cultureCode = Thread.CurrentThread.CurrentUICulture.Name
                End If
            End If

            Return cultureCode
        End Function

#Region "Public Methods"

        Public Function BuildCrossTabDataSet(ByVal DataSetName As String, ByVal result As IDataReader, ByVal FixedColumns As String, ByVal VariableColumns As String, ByVal KeyColumn As String, ByVal FieldColumn As String, ByVal FieldTypeColumn As String, ByVal StringValueColumn As String, ByVal NumericValueColumn As String) As DataSet
            Return BuildCrossTabDataSet(DataSetName, result, FixedColumns, VariableColumns, KeyColumn, FieldColumn, FieldTypeColumn, StringValueColumn, NumericValueColumn, System.Globalization.CultureInfo.CurrentCulture)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' converts a data reader with serialized fields into a typed data set
        ''' </summary>
        ''' <param name="DataSetName">Name of the dataset to be created</param>
        ''' <param name="result">Data reader that contains all field values serialized</param>
        ''' <param name="FixedColumns">List of fixed columns, delimited by commas. Columns must be contained in DataReader</param>
        ''' <param name="VariableColumns">List of variable columns, delimited by commas. Columns must be contained in DataReader</param>
        ''' <param name="KeyColumn">Name of the column, that contains the row ID. Column must be contained in DataReader</param>
        ''' <param name="FieldColumn">Name of the column, that contains the field name. Column must be contained in DataReader</param>
        ''' <param name="FieldTypeColumn">Name of the column, that contains the field type name. Column must be contained in DataReader</param>
        ''' <param name="StringValueColumn">Name of the column, that contains the field value, if stored as string. Column must be contained in DataReader</param>
        ''' <param name="NumericValueColumn">Name of the column, that contains the field value, if stored as number. Column must be contained in DataReader</param>
        ''' <param name="culture">culture of the field values in data reader's string value column</param>
        ''' <returns>The generated DataSet</returns>
        ''' <history>
        ''' 	[sleupold]     08/24/2006	Created temporary clone of core function and added support for culture based parsing of numeric values
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function BuildCrossTabDataSet(ByVal DataSetName As String, ByVal result As IDataReader, ByVal FixedColumns As String, ByVal VariableColumns As String, ByVal KeyColumn As String, ByVal FieldColumn As String, ByVal FieldTypeColumn As String, ByVal StringValueColumn As String, ByVal NumericValueColumn As String, ByVal Culture As System.Globalization.CultureInfo) As DataSet

            Dim arrFixedColumns As String() = Nothing
            Dim arrVariableColumns As String() = Nothing
            Dim arrField As String()
            Dim FieldType As String
            Dim intColumn As Integer
            Dim intKeyColumn As Integer

            ' create dataset
            Dim crosstab As New DataSet(DataSetName)
            crosstab.Namespace = "NetFrameWork"

            ' create table
            Dim tab As New DataTable(DataSetName)

            ' split fixed columns
            arrFixedColumns = FixedColumns.Split(",".ToCharArray())

            ' add fixed columns to table
            For intColumn = LBound(arrFixedColumns) To UBound(arrFixedColumns)
                arrField = arrFixedColumns(intColumn).Split("|".ToCharArray())
                Dim col As New DataColumn(arrField(0), System.Type.GetType("System." & arrField(1)))
                tab.Columns.Add(col)
            Next intColumn

            ' split variable columns
            If VariableColumns <> "" Then
                arrVariableColumns = VariableColumns.Split(",".ToCharArray())

                ' add varible columns to table
                For intColumn = LBound(arrVariableColumns) To UBound(arrVariableColumns)
                    arrField = arrVariableColumns(intColumn).Split("|".ToCharArray())
                    Dim col As New DataColumn(arrField(0), System.Type.GetType("System." & arrField(1)))
                    col.AllowDBNull = True
                    tab.Columns.Add(col)
                Next intColumn
            End If

            ' add table to dataset
            crosstab.Tables.Add(tab)

            ' add rows to table
            intKeyColumn = -1
            Dim row As DataRow = Nothing
            While result.Read()
                ' loop using KeyColumn as control break
                If Convert.ToInt32(result(KeyColumn)) <> intKeyColumn Then
                    ' add row
                    If intKeyColumn <> -1 Then
                        tab.Rows.Add(row)
                    End If

                    ' create new row
                    row = tab.NewRow()

                    ' assign fixed column values
                    For intColumn = LBound(arrFixedColumns) To UBound(arrFixedColumns)
                        arrField = arrFixedColumns(intColumn).Split("|".ToCharArray())
                        row(arrField(0)) = result(arrField(0))
                    Next intColumn

                    ' initialize variable column values
                    If VariableColumns <> "" Then
                        For intColumn = LBound(arrVariableColumns) To UBound(arrVariableColumns)
                            arrField = arrVariableColumns(intColumn).Split("|".ToCharArray())
                            Select Case arrField(1)
                                Case "Decimal"
                                    row(arrField(0)) = 0
                                Case "String"
                                    row(arrField(0)) = ""
                            End Select
                        Next intColumn
                    End If

                    intKeyColumn = Convert.ToInt32(result(KeyColumn))
                End If

                ' assign pivot column value
                If FieldTypeColumn <> "" Then
                    FieldType = result(FieldTypeColumn).ToString
                Else
                    FieldType = "String"
                End If
                Select Case FieldType
                    Case "Decimal"       ' decimal
                        row(Convert.ToInt32(result(FieldColumn))) = result(NumericValueColumn)
                    Case "String"       ' string
                        If Culture Is System.Globalization.CultureInfo.CurrentCulture Then
                            row(result(FieldColumn).ToString) = result(StringValueColumn)
                        Else
                            Select Case tab.Columns(result(FieldColumn).ToString).DataType.ToString
                                Case "System.Decimal", "System.Currency"
                                    row(result(FieldColumn).ToString) = Decimal.Parse(result(StringValueColumn).ToString, Culture)
                                Case "System.Int32"
                                    row(result(FieldColumn).ToString) = Int32.Parse(result(StringValueColumn).ToString, Culture)
                                Case Else
                                    row(result(FieldColumn).ToString) = result(StringValueColumn)
                            End Select
                        End If
                End Select
            End While

            result.Close()

            ' add row
            If intKeyColumn <> -1 Then
                tab.Rows.Add(row)
            End If

            ' finalize dataset
            crosstab.AcceptChanges()

            ' return the dataset
            Return crosstab

        End Function

        ' convert datareader to dataset
        Public Function ConvertDataReaderToDataSet(ByVal reader As IDataReader) As DataSet

            ' add datatable to dataset
            Dim objDataSet As New DataSet
            objDataSet.Tables.Add(ConvertDataReaderToDataTable(reader))

            Return objDataSet

        End Function

        ' convert datareader to dataset
        Public Function ConvertDataReaderToDataTable(ByVal reader As IDataReader) As DataTable

            ' create datatable from datareader
            Dim objDataTable As New DataTable
            Dim intFieldCount As Integer = reader.FieldCount
            Dim intCounter As Integer
            For intCounter = 0 To intFieldCount - 1
                objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter))
            Next intCounter

            ' populate datatable
            objDataTable.BeginLoadData()
            Dim objValues(intFieldCount - 1) As Object
            While reader.Read()
                reader.GetValues(objValues)
                objDataTable.LoadDataRow(objValues, True)
            End While
            reader.Close()
            objDataTable.EndLoadData()

            Return objDataTable

        End Function

        ' returns the absolute server path to the root ( ie. D:\Inetpub\wwwroot\directory\ )
        Public Function GetAbsoluteServerPath(ByVal Request As HttpRequest) As String
            Dim strServerPath As String

            strServerPath = Request.MapPath(Request.ApplicationPath)
            If Not strServerPath.EndsWith("\") Then
                strServerPath += "\"
            End If

            GetAbsoluteServerPath = strServerPath
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ApplicationName for the MemberRole API.
        ''' </summary>
        ''' <remarks>
        ''' This overload is used to get the current ApplcationName.  The Application
        ''' Name is in the form Prefix_Id, where Prefix is the object qualifier
        ''' for this instance of DotNetNuke, and Id is the current PortalId for normal
        ''' users or glbSuperUserAppName for SuperUsers.
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	01/18/2005	documented and modifeid to handle a Prefix
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetApplicationName() As String
            Dim appName As String

            If Convert.ToString(HttpContext.Current.Items("ApplicationName")) = "" Then
                'No Application Name saved
                Dim _PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                If _PortalSettings Is Nothing Then
                    'No Name is defined and no portal is current so return "/"
                    appName = "/"
                Else
                    'Get the "default' Application Name based on the PortalId
                    appName = GetApplicationName(_PortalSettings.PortalId)
                End If
            Else
                appName = Convert.ToString(HttpContext.Current.Items("ApplicationName"))
            End If

            Return appName

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ApplicationName for the MemberRole API.
        ''' </summary>
        ''' <remarks>
        ''' This overload is used to build the Application Name from the Portal Id
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	01/18/2005	documented and modifeid to handle a Prefix
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetApplicationName(ByVal PortalID As Integer) As String

            Dim appName As String

            'Get the Data Provider Configuration
            Dim _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data")

            ' Read the configuration specific information for the current Provider
            Dim objProvider As Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

            'Get the Object Qualifier frm the Provider Configuration
            Dim _objectQualifier As String = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            appName = _objectQualifier + Convert.ToString(PortalID)

            Return appName
        End Function

#Region "Database Version Methods"

        Public Function FindDatabaseVersion(ByVal Major As Integer, ByVal Minor As Integer, ByVal Build As Integer) As Boolean
            Dim version As Boolean = False
            Dim dr As IDataReader = Nothing
            Try
                dr = DataProvider.Instance().FindDatabaseVersion(Major, Minor, Build)
                If dr.Read Then
                    version = True
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
            Return version
        End Function

        Public Sub UpdateDataBaseVersion(ByVal version As System.Version)
            ' update the version
            DataProvider.Instance().UpdateDatabaseVersion(version.Major, version.Minor, version.Build, DotNetNukeContext.Current.Application.Name)

            _DataBaseVersion = version
        End Sub

#End Region

#Region "GetDomainName"

        Public Function AddPort(ByVal httpAlias As String, ByVal originalUrl As String) As String
            Dim uri As New Uri(originalUrl)
            Dim aliasUri As New Uri(httpAlias)

            If uri.Port <> 80 Then
                httpAlias = AddHTTP(aliasUri.Host & ":" & uri.Port & aliasUri.LocalPath)
            End If

            Return httpAlias
        End Function

        Public Function GetDomainName(ByVal Request As HttpRequest) As String
            Return GetDomainName(Request, False)
        End Function

        ' returns the domain name of the current request ( ie. www.domain.com or 207.132.12.123 or www.domain.com/directory if subhost )
        ' Actually, more to the point, returns the URL for the portal of this request.url.
        '   USE:
        '       Returns URI appropriate for checking against Portal Aliases.
        '   ASSUMPTIONS:
        '       portal access is always centric thru *.aspx or *.axd file;
        '       DotNetNuke application directory names are special (and not part of portal alias);
        '           so only specific DNN top level directory names are examined
        Public Function GetDomainName(ByVal Request As HttpRequest, ByVal ParsePortNumber As Boolean) As String
            Dim DomainName As StringBuilder = New StringBuilder
            Dim URL() As String
            Dim URI As String   ' holds Request.Url, less the "?" parameters
            Dim intURL As Integer

            ' split both URL separater, and parameter separator
            ' We trim right of '?' so test for filename extensions can occur at END of URL-componenet.
            ' Test:   'www.aspxforum.net'  should be returned as a valid domain name.
            ' just consider left of '?' in URI
            ' Binary, else '?' isn't taken literally; only interested in one (left) string
            URI = Request.Url.ToString()
            Dim hostHeader As String = Config.GetSetting("HostHeader")
            If Not hostHeader Is Nothing Then
                If hostHeader.Length > 0 Then
                    URI = URI.ToLower.Replace(hostHeader.ToLower, "")
                End If
            End If
            intURL = InStr(URI, "?", CompareMethod.Binary)
            If intURL > 0 Then
                URI = Left(URI, intURL - 1)
            End If

            URL = Split(URI, "/")

            For intURL = 2 To URL.GetUpperBound(0)
                Select Case URL(intURL).ToLower
                    Case "admin", "controls", "desktopmodules", "mobilemodules", "premiummodules", "providers"
                        Exit For
                    Case Else
                        ' exclude filenames ENDing in ".aspx" or ".axd" --- 
                        '   we'll use reverse match,
                        '   - but that means we are checking position of left end of the match;
                        '   - and to do that, we need to ensure the string we test against is long enough;
                        If (URL(intURL).Length >= ".aspx".Length) Then      'long enough for both tests
                            If InStrRev(URL(intURL).ToLower(), ".aspx") = (URL(intURL).Length - (".aspx".Length - 1)) _
                            Or InStrRev(URL(intURL).ToLower(), ".axd") = (URL(intURL).Length - (".axd".Length - 1)) _
                            Or InStrRev(URL(intURL).ToLower(), ".ashx") = (URL(intURL).Length - (".ashx".Length - 1)) Then
                                Exit For
                            End If
                        End If
                        ' non of the exclusionary names found
                        DomainName.Append(IIf(DomainName.ToString <> "", "/", "").ToString & URL(intURL))
                End Select
            Next intURL

            ' handle port specification
            If ParsePortNumber Then
                If DomainName.ToString.IndexOf(":") <> -1 Then
                    If UsePortNumber() = False Then
                        DomainName = DomainName.Replace(":" & Request.Url.Port.ToString(), "")
                    End If
                End If
            End If

            Return DomainName.ToString

        End Function

        Public Function UsePortNumber() As Boolean
            Dim usePort As Boolean = True
            If Not Utilities.Config.GetSetting("UsePortNumber") Is Nothing Then
                usePort = Boolean.Parse(Utilities.Config.GetSetting("UsePortNumber"))
            End If
            Return usePort
        End Function

#End Region

#Region "GetFileList"

        Public Function GetFileList() As ArrayList
            Return GetFileList(-1, "", True, "", False)
        End Function

        Public Function GetFileList(ByVal PortalId As Integer) As ArrayList
            Return GetFileList(PortalId, "", True, "", False)
        End Function

        Public Function GetFileList(ByVal PortalId As Integer, ByVal strExtensions As String) As ArrayList
            Return GetFileList(PortalId, strExtensions, True, "", False)
        End Function

        Public Function GetFileList(ByVal PortalId As Integer, ByVal strExtensions As String, ByVal NoneSpecified As Boolean) As ArrayList
            Return GetFileList(PortalId, strExtensions, NoneSpecified, "", False)
        End Function

        Public Function GetFileList(ByVal PortalId As Integer, ByVal strExtensions As String, ByVal NoneSpecified As Boolean, ByVal Folder As String) As ArrayList
            Return GetFileList(PortalId, strExtensions, NoneSpecified, Folder, False)
        End Function

        Public Function GetFileList(ByVal PortalId As Integer, ByVal strExtensions As String, ByVal NoneSpecified As Boolean, ByVal Folder As String, ByVal includeHidden As Boolean) As ArrayList
            Dim arrFileList As New ArrayList

            If NoneSpecified Then
                arrFileList.Add(New FileItem("", "<" + Localization.GetString("None_Specified") + ">"))
            End If

            Dim portalRoot As String
            If PortalId = Null.NullInteger Then
                portalRoot = HostMapPath
            Else
                Dim objPortals As New PortalController
                Dim objPortal As PortalInfo = objPortals.GetPortal(PortalId)
                portalRoot = objPortal.HomeDirectoryMapPath
            End If

            Dim objFolder As FolderInfo = FileSystemUtils.GetFolder(PortalId, Folder)
            If Not objFolder Is Nothing Then
                Dim objFiles As New FileController
                Dim dr As IDataReader = Nothing
                Try
                    dr = objFiles.GetFiles(PortalId, objFolder.FolderID)
                    While dr.Read()
                        If FilenameMatchesExtensions(dr("FileName").ToString, strExtensions) Then
                            Dim filePath As String = (portalRoot & dr("Folder").ToString & dr("fileName").ToString).Replace("/", "\")
                            Dim StorageLocation As Integer = 0

                            If Not dr("StorageLocation") Is Nothing Then
                                StorageLocation = CType(dr("StorageLocation"), Integer)
                                Select Case StorageLocation
                                    Case 1 : filePath = filePath & glbProtectedExtension ' Secure File System
                                    Case 2 ' Secure Database
                                    Case Else ' Insecure File System
                                End Select
                            End If

                            ' check if file exists - as the database may not be synchronized with the file system
                            ' Make sure its not a file stored in the db, if it is we don't worry about seeing if it exists
                            If Not StorageLocation = 2 Then
                                If File.Exists(filePath) Then
                                    ' check if file is hidden
                                    If includeHidden Then
                                        arrFileList.Add(New FileItem(dr("FileID").ToString, dr("FileName").ToString))
                                    Else
                                        Dim attributes As System.IO.FileAttributes = File.GetAttributes(filePath)
                                        If Not ((attributes And FileAttributes.Hidden) = FileAttributes.Hidden) Then
                                            arrFileList.Add(New FileItem(dr("FileID").ToString, dr("FileName").ToString))
                                        End If
                                    End If
                                End If
                            Else
                                ' File is stored in DB - Just add to arraylist
                                arrFileList.Add(New FileItem(dr("FileID").ToString, dr("FileName").ToString))
                            End If
                            'END Change
                        End If
                    End While
                Catch ex As Exception
                    LogException(ex)
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try
            End If

            Return arrFileList
        End Function

#End Region

        Public Function GetHostPortalSettings() As PortalSettings
            Dim TabId As Integer = -1
            Dim PortalId As Integer = -1

            Dim objPortalAliasInfo As PortalAliasInfo = Nothing

            'if the portal alias exists
            If Host.HostPortalID > Null.NullInteger Then
                PortalId = Host.HostPortalID

                ' use the host portal
                objPortalAliasInfo = New PortalAliasInfo
                objPortalAliasInfo.PortalID = PortalId
            End If

            ' load the PortalSettings into current context
            Return New PortalSettings(TabId, objPortalAliasInfo)
        End Function

        ' retrieves the domain name of the portal ( ie. http://www.domain.com " )
        Public Function GetPortalDomainName(ByVal strPortalAlias As String, Optional ByVal Request As HttpRequest = Nothing, Optional ByVal blnAddHTTP As Boolean = True) As String

            Dim strDomainName As String = ""

            Dim strURL As String = ""
            Dim arrPortalAlias() As String
            Dim intAlias As Integer

            If Not Request Is Nothing Then
                strURL = GetDomainName(Request)
            End If

            arrPortalAlias = Split(strPortalAlias, ",")
            For intAlias = 0 To arrPortalAlias.Length - 1
                If arrPortalAlias(intAlias) = strURL Then
                    strDomainName = arrPortalAlias(intAlias)
                End If
            Next
            If strDomainName = "" Then
                strDomainName = arrPortalAlias(0)
            End If

            If blnAddHTTP Then
                strDomainName = AddHTTP(strDomainName)
            End If

            GetPortalDomainName = strDomainName

        End Function

        Public Function GetPortalSettings() As PortalSettings

            Dim portalSettings As PortalSettings = Nothing

            'Try getting the settings from the Context
            If Not HttpContext.Current Is Nothing Then
                portalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            End If

            ' If nothing then try getting the Host Settings
            If portalSettings Is Nothing Then
                portalSettings = GetHostPortalSettings()
            End If

            Return portalSettings

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the folder path under the root for the portal 
        ''' </summary>
        ''' <param name="strFileNamePath">The folder the absolute path</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
        '''   [Philip Beadle] 6 October 2004 Moved to Globals from WebUpload.ascx.vb so can be accessed by URLControl.ascx
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSubFolderPath(ByVal strFileNamePath As String, ByVal portalId As Integer) As String
            Dim ParentFolderName As String
            If portalId = Null.NullInteger Then
                ParentFolderName = Common.Globals.HostMapPath.Replace("/", "\")
            Else
                ' Get Portal
                Dim objPortals As New PortalController
                Dim objPortal As PortalInfo = objPortals.GetPortal(portalId)

                ParentFolderName = objPortal.HomeDirectoryMapPath.Replace("/", "\")
            End If
            Dim strFolderpath As String = strFileNamePath.Substring(0, strFileNamePath.LastIndexOf("\") + 1)

            GetSubFolderPath = strFolderpath.Substring(ParentFolderName.Length).Replace("\", "/")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetTotalRecords method gets the number of Records returned.
        ''' </summary>
        ''' <param name="dr">An <see cref="IDataReader"/> containing the Total no of records</param>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	02/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetTotalRecords(ByRef dr As IDataReader) As Integer

            Dim total As Integer = 0

            If dr.Read Then
                Try
                    total = Convert.ToInt32(dr("TotalRecords"))
                Catch ex As Exception
                    total = -1
                End Try
            End If

            Return total

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetStatus - determines whether an upgrade/install is required and sest the
        ''' Database Version and Status accordingly
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/12/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub GetStatus()
            Dim strMessage As String = ""

            'First check if there is a Database version
            If DataBaseVersion IsNot Nothing Then
                Exit Sub
            End If

            _Status = UpgradeStatus.None

            ' first call GetProviderPath - this insures that the Database is Initialised correctly
            'and also generates the appropriate error message if it cannot be initialised correctly
            strMessage = DataProvider.Instance().GetProviderPath()

            ' get current database version from DB
            If Not strMessage.StartsWith("ERROR:") Then
                Try
                    _DataBaseVersion = DataProvider.Instance().GetVersion
                Catch ex As Exception
                    strMessage = "ERROR:" & ex.Message
                End Try
            End If

            If strMessage.StartsWith("ERROR") Then
                If IsInstalled() AndAlso Not HttpContext.Current.Request.IsLocal Then
                    'Errors connecting to the database after an initial installation should be treated as errors.
                    _Status = UpgradeStatus.Error
                Else
                    'An error that occurs before the database has been installed should be treated as a new install
                    _Status = UpgradeStatus.Install
                End If
            ElseIf DataBaseVersion Is Nothing Then
                'No Db Version so Install
                _Status = UpgradeStatus.Install
            Else
                If DotNetNukeContext.Current.Application.Version.Major > DataBaseVersion.Major Then
                    'Upgrade Required (Major Version Upgrade)
                    _Status = UpgradeStatus.Upgrade
                ElseIf DotNetNukeContext.Current.Application.Version.Major = DataBaseVersion.Major AndAlso _
                            DotNetNukeContext.Current.Application.Version.Minor > DataBaseVersion.Minor Then
                    'Upgrade Required (Minor Version Upgrade)
                    _Status = UpgradeStatus.Upgrade
                ElseIf DotNetNukeContext.Current.Application.Version.Major = DataBaseVersion.Major AndAlso _
                            DotNetNukeContext.Current.Application.Version.Minor = DataBaseVersion.Minor AndAlso _
                            DotNetNukeContext.Current.Application.Version.Build > DataBaseVersion.Build Then
                    'Upgrade Required (Build Version Upgrade)
                    _Status = UpgradeStatus.Upgrade
                End If
            End If

        End Sub

        Public Sub SetStatus(ByVal status As UpgradeStatus)
            _Status = status
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ImportFile - converts a file url (/Portals/0/somefile.gif) to the appropriate 
        ''' FileID=xx identification for use in importing portals, tabs and modules
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>An UpgradeStatus enum Upgrade/Install/None</returns>
        ''' <history>
        ''' 	[cnurse]	10/11/2007	moved from PortalController so the same 
        '''                             logic can be used by Module and Tab templates
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ImportFile(ByVal PortalId As Integer, ByVal url As String) As String

            Dim strUrl As String = url

            If GetURLType(url) = TabType.File Then
                Dim objFileController As New FileController
                Dim fileId As Integer = objFileController.ConvertFilePathToFileId(url, PortalId)
                If fileId >= 0 Then
                    strUrl = "FileID=" + fileId.ToString
                End If
            End If

            Return strUrl
        End Function

        ' encodes a URL for posting to an external site
        Public Function HTTPPOSTEncode(ByVal strPost As String) As String
            strPost = Replace(strPost, "\", "")
            strPost = System.Web.HttpUtility.UrlEncode(strPost)
            strPost = Replace(strPost, "%2f", "/")
            HTTPPOSTEncode = strPost
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the ApplicationName for the MemberRole API
        ''' </summary>
        ''' <remarks>
        ''' This overload takes a the PortalId
        ''' </remarks>
        '''	<param name="PortalID">The Portal Id</param>
        ''' <history>
        '''		[cnurse]	01/18/2005	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetApplicationName(ByVal PortalID As Integer)
            HttpContext.Current.Items("ApplicationName") = GetApplicationName(PortalID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the ApplicationName for the MemberRole API
        ''' </summary>
        ''' <remarks>
        ''' This overload takes a the PortalId
        ''' </remarks>
        '''	<param name="ApplicationName">The Application Name to set</param>
        ''' <history>
        '''		[cnurse]	01/18/2005	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetApplicationName(ByVal ApplicationName As String)
            HttpContext.Current.Items("ApplicationName") = ApplicationName
        End Sub

#End Region

        ''' <summary>
        ''' Formats the address on a single line ( ie. Unit, Street, City, Region, Country, PostalCode )
        ''' </summary>
        ''' <param name="Unit">The unit.</param>
        ''' <param name="Street">The street.</param>
        ''' <param name="City">The city.</param>
        ''' <param name="Region">The region.</param>
        ''' <param name="Country">The country.</param>
        ''' <param name="PostalCode">The postal code.</param>
        ''' <returns></returns>
        Public Function FormatAddress(ByVal Unit As Object, ByVal Street As Object, ByVal City As Object, ByVal Region As Object, ByVal Country As Object, ByVal PostalCode As Object) As String

            Dim strAddress As String = ""

            If Not Unit Is Nothing Then
                If Trim(Unit.ToString()) <> "" Then
                    strAddress += ", " & Unit.ToString
                End If
            End If
            If Not Street Is Nothing Then
                If Trim(Street.ToString()) <> "" Then
                    strAddress += ", " & Street.ToString
                End If
            End If
            If Not City Is Nothing Then
                If Trim(City.ToString()) <> "" Then
                    strAddress += ", " & City.ToString
                End If
            End If
            If Not Region Is Nothing Then
                If Trim(Region.ToString()) <> "" Then
                    strAddress += ", " & Region.ToString
                End If
            End If
            If Not Country Is Nothing Then
                If Trim(Country.ToString()) <> "" Then
                    strAddress += ", " & Country.ToString
                End If
            End If
            If Not PostalCode Is Nothing Then
                If Trim(PostalCode.ToString()) <> "" Then
                    strAddress += ", " & PostalCode.ToString
                End If
            End If
            If Trim(strAddress) <> "" Then
                strAddress = Mid(strAddress, 3)
            End If

            FormatAddress = strAddress

        End Function

        ''' <summary>
        ''' Formats the system.version into the standard format nn.nn.nn
        ''' </summary>
        ''' <param name="version">The version.</param>
        ''' <returns>Formatted  version as string</returns>
        Public Function FormatVersion(ByVal version As System.Version) As String
            Return FormatVersion(version, False)
        End Function

        ''' <summary>
        ''' Formats a version into the standard format nn.nn.nn
        ''' </summary>
        ''' <param name="version">The version to be formatted.</param>
        ''' <param name="includeBuild">if set to <c>true</c> [include build].</param>
        ''' <returns>Formatted version as string</returns>
        Public Function FormatVersion(ByVal version As System.Version, ByVal includeBuild As Boolean) As String
            Dim strVersion As String = version.Major.ToString("00") & "." & version.Minor.ToString("00") & "." & version.Build.ToString("00")
            If includeBuild Then
                strVersion += " (" + version.Revision.ToString + ")"
            End If
            Return strVersion
        End Function

        ''' <summary>
        ''' Formats  a version into the standard format nn.nn.nn
        ''' </summary>
        ''' <param name="version">The version to be formatted.</param>
        ''' <param name="fieldFormat">The field format.</param>
        ''' <param name="fieldCount">The field count.</param>
        ''' <param name="delimiterCharacter">The delimiter character.</param>
        ''' <returns>Formatted version as a string</returns>
        Public Function FormatVersion(ByVal version As System.Version, ByVal fieldFormat As String, ByVal fieldCount As Integer, ByVal delimiterCharacter As String) As String
            Dim strVersion As String = ""
            Dim intZero As Integer = 0
            If version IsNot Nothing Then
                If fieldCount > 0 Then
                    If version.Major >= 0 Then
                        strVersion += version.Major.ToString(fieldFormat)
                    Else
                        strVersion += intZero.ToString(fieldFormat)
                    End If
                End If
                If fieldCount > 1 Then
                    strVersion += delimiterCharacter
                    If version.Minor >= 0 Then
                        strVersion += version.Minor.ToString(fieldFormat)
                    Else
                        strVersion += intZero.ToString(fieldFormat)
                    End If
                End If
                If fieldCount > 2 Then
                    strVersion += delimiterCharacter
                    If version.Build >= 0 Then
                        strVersion += version.Build.ToString(fieldFormat)
                    Else
                        strVersion += intZero.ToString(fieldFormat)
                    End If
                End If
                If fieldCount > 3 Then
                    strVersion += delimiterCharacter
                    If version.Revision >= 0 Then
                        strVersion += version.Revision.ToString(fieldFormat)
                    Else
                        strVersion += intZero.ToString(fieldFormat)
                    End If
                End If
            End If
            Return strVersion
        End Function

        ''' <summary>
        ''' Cloaks the text, obfuscate sensitive data to prevent collection by robots and spiders and crawlers
        ''' </summary>
        ''' <param name="PersonalInfo">The personal info.</param>
        ''' <returns>obfuscated sensitive data by hustling ASCII characters</returns>
        Public Function CloakText(ByVal PersonalInfo As String) As String

            If Not PersonalInfo Is Nothing Then
                Dim sb As New StringBuilder

                ' convert to ASCII character codes
                sb.Remove(0, sb.Length)
                Dim StringLength As Integer = PersonalInfo.Length - 1
                For i As Integer = 0 To StringLength
                    sb.Append(Asc(PersonalInfo.Substring(i, 1)).ToString)
                    If i < StringLength Then
                        sb.Append(",")
                    End If
                Next

                ' build script block
                Dim sbScript As New StringBuilder

                sbScript.Append(vbCrLf & "<script type=""text/javascript"">" & vbCrLf)
                sbScript.Append("//<![CDATA[" & vbCrLf)
                sbScript.Append("   document.write(String.fromCharCode(" & sb.ToString & "))" & vbCrLf)
                sbScript.Append("//]]>" & vbCrLf)
                sbScript.Append("</script>" & vbCrLf)

                Return sbScript.ToString
            Else : Return Null.NullString
            End If

        End Function

        ''' <summary>
        ''' Gets the medium date.
        ''' </summary>
        ''' <param name="strDate">The date.</param>
        ''' <returns>a SQL Server compatible date</returns>
        Public Function GetMediumDate(ByVal strDate As String) As String

            If strDate <> "" Then
                Dim datDate As Date = Convert.ToDateTime(strDate)

                Dim strYear As String = Year(datDate).ToString
                Dim strMonth As String = MonthName(Month(datDate), True)
                Dim strDay As String = Day(datDate).ToString

                strDate = strDay & "-" & strMonth & "-" & strYear
            End If

            Return strDate

        End Function

        ''' <summary>
        ''' Gets the short date.
        ''' </summary>
        ''' <param name="strDate">The date.</param>
        ''' <returns>returns a SQL Server compatible date</returns>
        Public Function GetShortDate(ByVal strDate As String) As String

            If strDate <> "" Then
                Dim datDate As Date = Convert.ToDateTime(strDate)

                Dim strYear As String = Year(datDate).ToString
                Dim strMonth As String = Month(datDate).ToString
                Dim strDay As String = Day(datDate).ToString

                strDate = strMonth & "/" & strDay & "/" & strYear
            End If

            Return strDate

        End Function

        ''' <summary>
        ''' Returns a boolean value whether the control is an admin control
        ''' </summary>
        ''' <returns>
        ''' <c>true</c> if is admin control; otherwise, <c>false</c>.
        ''' </returns>
        Public Function IsAdminControl() As Boolean

            ' This is needed to avoid an exception if there is no Context.  This will occur if code is called from the Scheduler
            If HttpContext.Current Is Nothing Then
                Return False
            End If
            Return (Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("mid"))) OrElse (Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ctl")))
        End Function

        ''' <summary>
        ''' Returns a boolean value whether the page should display an admin skin
        ''' </summary>
        ''' <returns>
        ''' <c>true</c> if is admin skin; otherwise, <c>false</c>.
        ''' </returns>
        Public Function IsAdminSkin() As Boolean
            Dim _IsAdminSkin As Boolean = Null.NullBoolean

            If HttpContext.Current IsNot Nothing Then
                Dim AdminKeys As String = "tab,module,importmodule,exportmodule,help"

                Dim ControlKey As String = ""
                If Not HttpContext.Current.Request.QueryString("ctl") Is Nothing Then
                    ControlKey = HttpContext.Current.Request.QueryString("ctl").ToLower
                End If

                Dim ModuleID As Integer = -1
                If Not HttpContext.Current.Request.QueryString("mid") Is Nothing Then
                    Integer.TryParse(HttpContext.Current.Request.QueryString("mid"), ModuleID)
                End If

                _IsAdminSkin = (ControlKey <> "" And ControlKey <> "view" And ModuleID <> -1) OrElse (ControlKey <> "" And AdminKeys.IndexOf(ControlKey) <> -1 And ModuleID = -1)
            End If

            Return _IsAdminSkin
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the current tab is in EditMode
        ''' </summary>
        ''' <returns><c>true</c> if the tab is in Edit mode; otherwise <c>false</c></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	06/04/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsEditMode() As Boolean
            Return (TabPermissionController.CanAddContentToPage AndAlso _
                    PortalController.GetCurrentPortalSettings.UserMode = PortalSettings.Mode.Edit)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the current tab is in LayoutMode
        ''' </summary>
        ''' <returns><c>true</c> if the current tab is in layout mode; otherwise <c>false</c></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	9/16/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsLayoutMode() As Boolean
            Return (TabPermissionController.CanAddContentToPage AndAlso _
                    PortalController.GetCurrentPortalSettings.UserMode = PortalSettings.Mode.Layout)
        End Function

        ''' <summary>
        ''' Creates the RSS files
        ''' </summary>
        ''' <param name="dr">The datareader</param>
        ''' <param name="TitleField">The title field.</param>
        ''' <param name="URLField">The URL field.</param>
        ''' <param name="CreatedDateField">The created date field.</param>
        ''' <param name="SyndicateField">The syndicate field.</param>
        ''' <param name="DomainName">Name of the domain.</param>
        ''' <param name="FileName">Name of the file.</param>
        Public Sub CreateRSS(ByVal dr As IDataReader, ByVal TitleField As String, ByVal URLField As String, ByVal CreatedDateField As String, ByVal SyndicateField As String, ByVal DomainName As String, ByVal FileName As String)

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            ' create RSS file
            Dim strRSS As String = ""

            Dim strRelativePath As String = DomainName & Replace(Mid(FileName, InStr(1, FileName, "\Portals")), "\", "/")
            strRelativePath = Left(strRelativePath, InStrRev(strRelativePath, "/"))

            Try
                While dr.Read()
                    If Convert.ToBoolean(dr(SyndicateField)) Then
                        strRSS += "      <item>" & ControlChars.CrLf
                        strRSS += "         <title>" & dr(TitleField).ToString & "</title>" & ControlChars.CrLf
                        If InStr(1, dr("URL").ToString, "://") = 0 Then
                            If IsNumeric(dr("URL").ToString) Then
                                strRSS += "         <link>" & DomainName & "/" & glbDefaultPage & "?tabid=" & dr(URLField).ToString & "</link>" & ControlChars.CrLf
                            Else
                                strRSS += "         <link>" & strRelativePath & dr(URLField).ToString & "</link>" & ControlChars.CrLf
                            End If
                        Else
                            strRSS += "         <link>" & dr(URLField).ToString & "</link>" & ControlChars.CrLf
                        End If
                        strRSS += "         <description>" & _portalSettings.PortalName & " " & GetMediumDate(dr(CreatedDateField).ToString) & "</description>" & ControlChars.CrLf
                        strRSS += "     </item>" & ControlChars.CrLf
                    End If
                End While
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            If strRSS <> "" Then
                strRSS = "<?xml version=""1.0"" encoding=""iso-8859-1""?>" & ControlChars.CrLf & _
                 "<rss version=""0.91"">" & ControlChars.CrLf & _
                 "  <channel>" & ControlChars.CrLf & _
                 "     <title>" & _portalSettings.PortalName & "</title>" & ControlChars.CrLf & _
                 "     <link>" & DomainName & "</link>" & ControlChars.CrLf & _
                 "     <description>" & _portalSettings.PortalName & "</description>" & ControlChars.CrLf & _
                 "     <language>en-us</language>" & ControlChars.CrLf & _
                 "     <copyright>" & _portalSettings.FooterText & "</copyright>" & ControlChars.CrLf & _
                 "     <webMaster>" & _portalSettings.Email & "</webMaster>" & ControlChars.CrLf & _
                 strRSS & _
                 "   </channel>" & ControlChars.CrLf & _
                 "</rss>"

                Dim objStream As StreamWriter
                objStream = File.CreateText(FileName)
                objStream.WriteLine(strRSS)
                objStream.Close()
            Else
                If File.Exists(FileName) Then
                    File.Delete(FileName)
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' injects the upload directory into raw HTML for src and background tags
        ''' </summary>
        ''' <param name="strHTML">raw HTML text</param>
        ''' <param name="strUploadDirectory">path of portal image directory</param>
        ''' <returns>HTML with paths for images and background corrected</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[sleupold]	8/18/2007	corrected and refactored
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ManageUploadDirectory(ByVal strHTML As String, ByVal strUploadDirectory As String) As String
            strHTML = ManageTokenUploadDirectory(strHTML, strUploadDirectory, "src")
            Return ManageTokenUploadDirectory(strHTML, strUploadDirectory, "background")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' injects the upload directory into raw HTML for a single token
        ''' </summary>
        ''' <param name="strHTML">raw HTML text</param>
        ''' <param name="strUploadDirectory">path of portal image directory</param>
        ''' <param name="strToken">token to be replaced</param>
        ''' <returns>HTML with paths for images and background corrected</returns>
        ''' <remarks>
        ''' called by ManageUploadDirectory for each token.
        ''' </remarks>
        ''' <history>
        ''' 	[sleupold]	8/18/2007	created as refactoring of ManageUploadDirectory
        '''                             added proper handling of subdirectory installations.
        '''     [sleupold] 11/03/2007   case insensitivity added
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ManageTokenUploadDirectory(ByVal strHTML As String, ByVal strUploadDirectory As String, ByVal strToken As String) As String
            Dim P As Integer
            Dim R As Integer
            Dim S As Integer = 0
            Dim tLen As Integer
            Dim strURL As String
            Dim sbBuff As New StringBuilder("")

            If strHTML <> "" Then
                tLen = strToken.Length + 2
                Dim _UploadDirectory As String = strUploadDirectory.ToLower

                'find position of first occurrance:
                P = strHTML.IndexOf(strToken & "=""", StringComparison.InvariantCultureIgnoreCase)
                While P <> -1
                    sbBuff.Append(strHTML.Substring(S, P - S + tLen)) 'keep charactes left of URL
                    S = P + tLen 'save startpos of URL
                    R = strHTML.IndexOf("""", S) 'end of URL
                    If R >= 0 Then
                        strURL = strHTML.Substring(S, R - S).ToLower
                    Else
                        strURL = strHTML.Substring(S).ToLower
                    End If
                    ' add uploaddirectory if we are linking internally and the uploaddirectory is not already included
                    If Not strURL.Contains("://") AndAlso Not strURL.StartsWith("/") AndAlso Not strURL.StartsWith(_UploadDirectory) Then
                        sbBuff.Append(strUploadDirectory)
                    End If
                    'find position of next occurrance:
                    P = strHTML.IndexOf(strToken & "=""", S + Len(strURL) + 2, StringComparison.InvariantCultureIgnoreCase)
                End While

                If S > -1 Then sbBuff.Append(strHTML.Substring(S)) 'append characters of last URL and behind
            End If

            Return sbBuff.ToString
        End Function

        ''' <summary>
        ''' Uses recursion to search the control hierarchy for a specific control based on controlname.
        ''' </summary>
        ''' <param name="objControl">The control object.</param>
        ''' <param name="strControlName">Name of the control.</param>
        ''' <returns></returns>
        Public Function FindControlRecursive(ByVal objControl As Control, ByVal strControlName As String) As Control
            If objControl.Parent Is Nothing Then
                Return Nothing
            Else
                If Not objControl.Parent.FindControl(strControlName) Is Nothing Then
                    Return objControl.Parent.FindControl(strControlName)
                Else
                    Return FindControlRecursive(objControl.Parent, strControlName)
                End If
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Searches control hierarchy from top down to find a control matching the passed in name
        ''' </summary>
        ''' <param name="objParent">Root control to begin searching</param>
        ''' <param name="strControlName">Name of control to look for</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' This differs from FindControlRecursive in that it looks down the control hierarchy, whereas, the 
        ''' FindControlRecursive starts at the passed in control and walks the tree up.  Therefore, this function is 
        ''' more a expensive task.
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	9/17/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FindControlRecursiveDown(ByVal objParent As Control, ByVal strControlName As String) As Control
            Dim objCtl As Control
            Dim objChild As Control
            objCtl = objParent.FindControl(strControlName)
            If objCtl Is Nothing Then
                For Each objChild In objParent.Controls
                    objCtl = FindControlRecursiveDown(objChild, strControlName)
                    If Not objCtl Is Nothing Then Exit For
                Next
            End If
            Return objCtl
        End Function

        ''' <summary>
        ''' Sets focus to any control
        ''' </summary>
        ''' <param name="control">The control to set the focus to.</param>
        Public Sub SetFormFocus(ByVal control As Control)
            If Not control.Page Is Nothing And control.Visible Then
                If control.Page.Request.Browser.EcmaScriptVersion.Major >= 1 Then

                    'JH dnn.js mod
                    If DotNetNuke.UI.Utilities.ClientAPI.ClientAPIDisabled = False Then
                        DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(control.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn)
                        UI.Utilities.DNNClientAPI.AddBodyOnloadEventHandler(control.Page, "__dnn_SetInitialFocus('" & control.ClientID & "');")
                    Else
                        ' Create JavaScript 
                        Dim sb As New System.Text.StringBuilder
                        sb.Append("<script type=""text/javascript"">")
                        sb.Append("<!--")
                        sb.Append(ControlChars.Lf)
                        sb.Append("function SetInitialFocus() {")
                        sb.Append(ControlChars.Lf)
                        sb.Append(" document.")

                        ' Find the Form 
                        Dim objParent As Control = control.Parent
                        While Not TypeOf objParent Is System.Web.UI.HtmlControls.HtmlForm
                            objParent = objParent.Parent
                        End While
                        sb.Append(objParent.ClientID)
                        sb.Append("['")
                        sb.Append(control.UniqueID)
                        sb.Append("'].focus(); }")
                        sb.Append("window.onload = SetInitialFocus;")
                        sb.Append(ControlChars.Lf)
                        sb.Append("// -->")
                        sb.Append(ControlChars.Lf)
                        sb.Append("</script>")

                        ' Register Client Script 
                        ClientAPI.RegisterClientScriptBlock(control.Page, "InitialFocus", sb.ToString())
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Gets the external request.
        ''' </summary>
        ''' <param name="Address">The address.</param>
        ''' <returns>HttpWebRequest</returns>
        Public Function GetExternalRequest(ByVal Address As String) As HttpWebRequest
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            ' Create the request object
            Dim objRequest As HttpWebRequest = CType(WebRequest.Create(Address), HttpWebRequest)

            ' Set a time out to the request ... 10 seconds
            objRequest.Timeout = Host.WebRequestTimeout

            ' Attach a User Agent to the request
            objRequest.UserAgent = "DotNetNuke"

            ' If there is Proxy info, apply it to the Request
            If Not String.IsNullOrEmpty(Host.ProxyServer) Then

                ' Create a new Proxy
                Dim Proxy As WebProxy

                ' Create a new Network Credentials item
                Dim ProxyCredentials As NetworkCredential

                ' Fill Proxy info from host settings
                Proxy = New WebProxy(Host.ProxyServer, Host.ProxyPort)

                If Not String.IsNullOrEmpty(Host.ProxyUsername) Then
                    ' Fill the credential info from host settings
                    ProxyCredentials = New NetworkCredential(Host.ProxyUsername, Host.ProxyPassword)

                    'Apply credentials to proxy
                    Proxy.Credentials = ProxyCredentials
                End If

                ' Apply Proxy to Request
                objRequest.Proxy = Proxy

            End If
            Return objRequest
        End Function

        ''' <summary>
        ''' Gets the external request.
        ''' </summary>
        ''' <param name="Address">The address.</param>
        ''' <param name="Credentials">The network credentials.</param>
        ''' <returns>HttpWebRequest</returns>
        Public Function GetExternalRequest(ByVal Address As String, ByVal Credentials As NetworkCredential) As HttpWebRequest
            ' Create the request object
            Dim objRequest As HttpWebRequest = CType(WebRequest.Create(Address), HttpWebRequest)

            ' Set a time out to the request ... 10 seconds
            objRequest.Timeout = Host.WebRequestTimeout

            ' Attach a User Agent to the request
            objRequest.UserAgent = "DotNetNuke"

            ' Attach supplied credentials
            If Not Credentials.UserName Is Nothing Then
                objRequest.Credentials = Credentials
            End If

            ' If there is Proxy info, apply it to the Request
            If Not String.IsNullOrEmpty(Host.ProxyServer) Then

                ' Create a new Proxy
                Dim Proxy As WebProxy

                ' Create a new Network Credentials item
                Dim ProxyCredentials As NetworkCredential

                ' Fill Proxy info from host settings
                Proxy = New WebProxy(Host.ProxyServer, Host.ProxyPort)

                If Not String.IsNullOrEmpty(Host.ProxyUsername) Then
                    ' Fill the credential info from host settings
                    ProxyCredentials = New NetworkCredential(Host.ProxyUsername, Host.ProxyPassword)

                    'Apply credentials to proxy
                    Proxy.Credentials = ProxyCredentials
                End If

                ' Apply Proxy to Request
                objRequest.Proxy = Proxy

            End If
            Return objRequest
        End Function

        ''' <summary>
        ''' Deletes the folder recursive, including all subfolders.
        ''' </summary>
        ''' <param name="strRoot">The rootfolder to start with.</param>
        Public Sub DeleteFolderRecursive(ByVal strRoot As String)
            If strRoot <> "" Then
                Dim strFolder As String
                If Directory.Exists(strRoot) Then
                    For Each strFolder In Directory.GetDirectories(strRoot)
                        DeleteFolderRecursive(strFolder)
                    Next
                    Dim strFile As String
                    For Each strFile In Directory.GetFiles(strRoot)
                        Try
                            FileSystemUtils.DeleteFile(strFile)
                        Catch
                            ' error deleting file
                        End Try
                    Next
                    Try
                        Directory.Delete(strRoot)
                    Catch
                        ' error deleting folder
                    End Try
                End If
            End If
        End Sub

        ''' <summary>
        ''' Deletes the files recursively, based upon a file filter
        ''' </summary>
        ''' <param name="strRoot">The root folder to start with.</param>
        ''' <param name="filter">The file filter.</param>
        Public Sub DeleteFilesRecursive(ByVal strRoot As String, ByVal filter As String)
            If strRoot <> "" Then
                Dim strFolder As String
                If Directory.Exists(strRoot) Then
                    For Each strFolder In directory.GetDirectories(strRoot)
                        Dim directory As New DirectoryInfo(strFolder)
                        If (directory.Attributes And FileAttributes.Hidden) = 0 AndAlso _
                                (directory.Attributes And FileAttributes.System) = 0 Then
                            DeleteFilesRecursive(strFolder, filter)
                        End If
                    Next

                    Dim strFile As String
                    For Each strFile In Directory.GetFiles(strRoot, "*" + filter)
                        Try
                            FileSystemUtils.DeleteFile(strFile)
                        Catch
                            ' error deleting file
                        End Try
                    Next
                End If
            End If
        End Sub

        Private Function ValidIDLookupCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return New Dictionary(Of String, String)
        End Function

        ''' <summary>
        ''' Creates the valid ID.
        ''' </summary>
        ''' <param name="inputValue">The input value.</param>
        ''' <returns>String with a valid ID</returns>
        Public Function CreateValidID(ByVal inputValue As String) As String
            Dim returnValue As String
            'Regex is expensive so we will cache the results in a lookup table
            Dim validIDLookup As Dictionary(Of String, String) = CBO.GetCachedObject(Of Dictionary(Of String, String))(New CacheItemArgs("ValidIDLookup", 200, Caching.CacheItemPriority.NotRemovable), AddressOf ValidIDLookupCallback)

            If validIDLookup.ContainsKey(inputValue) Then
                'Return value
                returnValue = validIDLookup(inputValue)
            Else
                'Create Valid ID
                ' '... letters, digits ([0-9]), hyphens ("-"), underscores ("_"), colons (":"), and periods (".")' are valid identifiers
                ' We aren't allowing hyphens or periods, even though they're valid, since the previous version of this function didn't
                ' Replace all characters that aren't in the list with an underscore
                Dim invalidCharacters As New Regex("[^A-Z0-9_:]", RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase)
                returnValue = invalidCharacters.Replace(inputValue, "_")

                ' identifiers '... must begin with a letter ([A-Za-z])' 
                ' replace a starting non-letter character with an A
                Dim invalidInitialCharacters As New Regex("^[^A-Z]", RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase)
                returnValue = invalidInitialCharacters.Replace(returnValue, "A")

                'put in Dictionary
                validIDLookup(inputValue) = returnValue
            End If

            'Return Value
            Return returnValue
        End Function

        ''' <summary>
        ''' Cleans the name of the file.
        ''' </summary>
        ''' <param name="FileName">Name of the file.</param>
        ''' <returns>Cleaned file name</returns>
        Public Function CleanFileName(ByVal FileName As String) As String
            Return CleanFileName(FileName, "", "")
        End Function

        ''' <summary>
        ''' Cleans the name of the file of invalid filename characters
        ''' </summary>
        ''' <param name="FileName">Name of the file.</param>
        ''' <param name="BadChars">The string of bad characters.</param>
        ''' <returns>Cleaned file name</returns>
        Public Function CleanFileName(ByVal FileName As String, ByVal BadChars As String) As String
            Return CleanFileName(FileName, BadChars, "")
        End Function

        ''' <summary>
        ''' Cleans the name of the file from invalid characters.
        ''' </summary>
        ''' <param name="FileName">Name of the file.</param>
        ''' <param name="BadChars">The string of bad chars.</param>
        ''' <param name="ReplaceChar">The char to use for replacement.</param>
        ''' <returns></returns>
        Public Function CleanFileName(ByVal FileName As String, ByVal BadChars As String, ByVal ReplaceChar As String) As String
            Dim strFileName As String = FileName

            If BadChars = "" Then
                BadChars = ":/\?*|" & Chr(34) & Chr(39) & Chr(9)
            End If

            If ReplaceChar = "" Then
                ReplaceChar = "_"
            End If

            Dim intCounter As Integer

            For intCounter = 0 To Len(BadChars) - 1
                strFileName = strFileName.Replace(BadChars.Substring(intCounter, 1), ReplaceChar)
            Next intCounter

            Return strFileName
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CleanName - removes characters from Module/Tab names that are being used for file names
        ''' in Module/Tab Import/Export.  
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>A cleaned string</returns>
        ''' <history>
        ''' 	[cnurse]	10/11/2007	moved from Import/Export Module user controls to avoid 
        '''                             duplication and for use in new Import and Export Tab
        '''                             user controls
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CleanName(ByVal Name As String) As String
            Dim strName As String = Name
            Dim strBadChars As String = ". ~`!@#$%^&*()-_+={[}]|\:;<,>?/" & Chr(34) & Chr(39)
            Dim intCounter As Integer
            For intCounter = 0 To Len(strBadChars) - 1
                strName = strName.Replace(strBadChars.Substring(intCounter, 1), "")
            Next intCounter
            Return strName
        End Function

        Private Function ValidClassLookupCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return New Dictionary(Of String, String)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateValidClass - removes characters from Module/Tab names which are invalid  
        ''' for use as an XHTML class attribute / CSS class selector value and optionally
        ''' prepends the letter 'A' if the first character is not alphabetic.  This differs 
        ''' from <see>CreateValidID</see> which replaces invalid characters with an underscore
        ''' and replaces the first letter with an 'A' if it is not alphabetic
        ''' </summary>
        ''' <param name="inputValue">String to use to create the class value</param>
        ''' <param name="validateFirstChar">If set true, validate whether the first character
        ''' is alphabetic and, if not, prepend the letter 'A' to the returned value</param>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>A string suitable for use as a class value</returns>
        ''' <history>
        ''' 	[jenni]	27/10/2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateValidClass(ByVal inputValue As String, ByVal validateFirstChar As Boolean) As String
            Dim returnValue As String
            'Regex is expensive so we will cache the results in a lookup table
            Dim validClassLookup As Dictionary(Of String, String) = CBO.GetCachedObject(Of Dictionary(Of String, String))(New CacheItemArgs("ValidClassLookup", 200, Caching.CacheItemPriority.NotRemovable), AddressOf ValidClassLookupCallback)

            If validClassLookup.ContainsKey(inputValue) Then
                'Return value
                returnValue = validClassLookup(inputValue)
            Else
                'Create Valid Class
                ' letters ([a-zA-Z]), digits ([0-9]), hyphens ("-") and underscores ("_") are valid in class values
                ' Remove all characters that aren't in the list
                Dim invalidCharacters As New Regex("[^A-Z0-9_-]", RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase)
                returnValue = invalidCharacters.Replace(inputValue, String.Empty)

                ' If we're asked to validate the first character...
                If (validateFirstChar) Then
                    ' classes should begin with a letter ([A-Za-z])' 
                    ' prepend a starting non-letter character with an A
                    Dim invalidInitialCharacters As New Regex("^[^A-Z]", RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase)
                    If (invalidCharacters.IsMatch(returnValue)) Then
                        returnValue = "A" & returnValue
                    End If
                End If

                'put in Dictionary
                validClassLookup(inputValue) = returnValue
            End If

            'Return Value
            Return returnValue
        End Function

#Region "Url Methods"

        Public Function AccessDeniedURL() As String
            Return AccessDeniedURL("")
        End Function

        Public Function AccessDeniedURL(ByVal Message As String) As String
            Dim strURL As String = ""
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            If HttpContext.Current.Request.IsAuthenticated Then
                If Message = "" Then
                    ' redirect to access denied page
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied")
                Else
                    ' redirect to access denied page with custom message
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied", "message=" & HttpUtility.UrlEncode(Message))
                End If
            Else
                strURL = LoginURL(HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl), False)
            End If

            Return strURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds HTTP to URL if no other protocol specified
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strURL">The url</param>
        ''' <returns>The formatted url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        '''     [cnurse]    05/06/2005  added chack for mailto: protocol
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AddHTTP(ByVal strURL As String) As String
            If strURL <> "" Then
                If InStr(1, strURL, "mailto:") = 0 And InStr(1, strURL, "://") = 0 And InStr(1, strURL, "~") = 0 And InStr(1, strURL, "\\") = 0 Then
                    If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request.IsSecureConnection Then
                        strURL = "https://" & strURL
                    Else
                        strURL = "http://" & strURL
                    End If
                End If
            End If
            Return strURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the Application root url (including the tab/page)
        ''' </summary>
        ''' <remarks>
        ''' This overload assumes the current page
        ''' </remarks>
        ''' <returns>The formatted root url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ApplicationURL() As String

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            If Not _portalSettings Is Nothing Then
                Return (ApplicationURL(_portalSettings.ActiveTab.TabID))
            Else
                Return (ApplicationURL(-1))
            End If

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the Application root url (including the tab/page)
        ''' </summary>
        ''' <remarks>
        ''' This overload takes the tabid (page id) as a parameter
        ''' </remarks>
        ''' <param name="TabID">The id of the tab/page</param>
        ''' <returns>The formatted root url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ApplicationURL(ByVal TabID As Integer) As String

            Dim strURL As String = "~/" & glbDefaultPage

            If TabID <> -1 Then
                strURL += "?tabid=" & TabID.ToString
            End If

            Return strURL

        End Function

        Public Function FormatHelpUrl(ByVal HelpUrl As String, ByVal objPortalSettings As PortalSettings, ByVal Name As String) As String
            Return FormatHelpUrl(HelpUrl, objPortalSettings, Name, "")
        End Function

        Public Function FormatHelpUrl(ByVal HelpUrl As String, ByVal objPortalSettings As PortalSettings, ByVal Name As String, ByVal Version As String) As String

            Dim strURL As String = HelpUrl

            If strURL.IndexOf("?") <> -1 Then
                strURL += "&helpculture="
            Else
                strURL += "?helpculture="
            End If

            If Thread.CurrentThread.CurrentUICulture.ToString.ToLower <> "" Then
                strURL += Thread.CurrentThread.CurrentUICulture.ToString.ToLower
            Else
                strURL += objPortalSettings.DefaultLanguage.ToLower
            End If

            If Name <> "" Then
                strURL += "&helpmodule=" & System.Web.HttpUtility.UrlEncode(Name)
            End If

            If Version <> "" Then
                strURL += "&helpversion=" & System.Web.HttpUtility.UrlEncode(Version)
            End If

            Return AddHTTP(strURL)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted friendly url.
        ''' </summary>
        ''' <remarks>
        ''' Assumes Default.aspx, and that portalsettings are saved to Context
        ''' </remarks>
        ''' <param name="tab">The current tab</param>
        ''' <param name="path">The path to format.</param>
        ''' <returns>The formatted (friendly) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String) As String

            Return FriendlyUrl(tab, path, glbDefaultPage)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted friendly url
        ''' </summary>
        ''' <remarks>
        ''' This overload includes an optional page to include in the url.
        ''' </remarks>
        ''' <param name="tab">The current tab</param>
        ''' <param name="path">The path to format.</param>
        ''' <param name="pageName">The page to include in the url.</param>
        ''' <returns>The formatted (friendly) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String) As String

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Return FriendlyUrl(tab, path, pageName, _portalSettings)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted friendly url
        ''' </summary>
        ''' <remarks>
        ''' This overload includes the portal settings for the site
        ''' </remarks>
        ''' <param name="tab">The current tab</param>
        ''' <param name="path">The path to format.</param>
        ''' <param name="settings">The portal Settings</param>
        ''' <returns>The formatted (friendly) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal settings As PortalSettings) As String

            Return FriendlyUrl(tab, path, glbDefaultPage, settings)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted friendly url
        ''' </summary>
        ''' <remarks>
        ''' This overload includes an optional page to include in the url, and the portal 
        ''' settings for the site
        ''' </remarks>
        ''' <param name="tab">The current tab</param>
        ''' <param name="path">The path to format.</param>
        ''' <param name="pageName">The page to include in the url.</param>
        ''' <param name="settings">The portal Settings</param>
        ''' <returns>The formatted (friendly) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal settings As PortalSettings) As String

            Return DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, settings)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted friendly url
        ''' </summary>
        ''' <remarks>
        ''' This overload includes an optional page to include in the url, and the portal 
        ''' settings for the site
        ''' </remarks>
        ''' <param name="tab">The current tab</param>
        ''' <param name="path">The path to format.</param>
        ''' <param name="pageName">The page to include in the url.</param>
        ''' <param name="portalAlias">The portal Alias for the site</param>
        ''' <returns>The formatted (friendly) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal portalAlias As String) As String

            Return DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, portalAlias)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the type of URl (T=other tab, F=file, U=URL, N=normal)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="URL">The url</param>
        ''' <returns>The url type</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetURLType(ByVal URL As String) As TabType
            If URL = "" Then
                Return TabType.Normal      ' normal tab
            Else
                If URL.ToLower.StartsWith("mailto:") = False AndAlso URL.IndexOf("://") = -1 AndAlso _
                                URL.StartsWith("~") = False AndAlso URL.StartsWith("\\") = False AndAlso URL.StartsWith("/") = False Then
                    If IsNumeric(URL) Then
                        Return TabType.Tab       ' internal tab ( ie. 23 = tabid )
                    Else
                        If URL.ToLower.StartsWith("userid=") Then
                            Return TabType.Member     ' userid=
                        Else
                            Return TabType.File       ' internal file ( ie. folder/file.ext or fileid= )
                        End If
                    End If
                Else
                    Return TabType.Url      ' external url ( eg. http://www.domain.com )
                End If
            End If
        End Function

        ''' <summary>
        ''' Url's as internal links to Files, Tabs and Users should only be imported if
        ''' those files, tabs and users exist. This function parses the url, and checks
        ''' whether the internal links exist. 
        ''' If the link does not exist, the function will return an empty string
        ''' </summary>
        ''' <param name="ModuleId">Integer</param>
        ''' <param name="url">String</param>
        ''' <returns>If an internal link does not exist, an empty string is returned, otherwise the passed in url is returned as is</returns>
        ''' <history>
        '''     [erikvb]    06/11/2008     corrected file check and added tab and member check
        ''' </history>
        Public Function ImportUrl(ByVal ModuleId As Integer, ByVal url As String) As String
            Dim strUrl As String = url
            Dim urlType As TabType = GetURLType(url)
            Dim intId As Integer = -1
            Dim portalSettings As PortalSettings = GetPortalSettings()
            Select Case urlType
                Case TabType.File
                    If Int32.TryParse(url.Replace("FileID=", ""), intId) Then
                        Dim objFileController As FileController = New FileController
                        Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = objFileController.GetFileById(intId, portalSettings.PortalId)
                        If objFile Is Nothing Then
                            'fileId does not exist in the portal
                            strUrl = ""
                        End If
                    Else
                        ' failed to get fileId
                        strUrl = ""
                    End If
                Case TabType.Member
                    If Int32.TryParse(url.Replace("UserID=", ""), intId) Then
                        If UserController.GetUserById(portalSettings.PortalId, intId) Is Nothing Then
                            'UserId does not exist for this portal
                            strUrl = ""
                        End If
                    Else
                        'failed to get UserId
                        strUrl = ""
                    End If
                Case TabType.Tab
                    If Int32.TryParse(url, intId) Then
                        Dim objTabController As New DotNetNuke.Entities.Tabs.TabController
                        If objTabController.GetTab(intId, portalSettings.PortalId, False) Is Nothing Then
                            ' the tab does not exist
                            strUrl = ""
                        End If
                    Else
                        ' failed to get TabId
                        strUrl = ""
                    End If
            End Select
            Return strUrl
        End Function

        Public Function LoginURL(ByVal returnURL As String, ByVal override As Boolean) As String
            Dim strURL As String = ""
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            returnURL = String.Format("returnurl={0}", returnURL)

            If _portalSettings.LoginTabId <> -1 AndAlso Not override Then
                If ValidateLoginTabID(_portalSettings.LoginTabId) Then
                    If String.IsNullOrEmpty(returnURL) Then
                        strURL = NavigateURL(_portalSettings.LoginTabId, "")
                    Else
                        strURL = NavigateURL(_portalSettings.LoginTabId, "", returnURL)
                    End If
                Else
                    Dim strMessage As String = String.Format("error={0}", Localization.GetString("NoLoginControl", Localization.GlobalResourceFile))
                    'No account module so use portal tab
                    If String.IsNullOrEmpty(returnURL) Then
                        strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", strMessage)
                    Else
                        strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", returnURL, strMessage)
                    End If
                End If
            Else
                ' portal tab
                If String.IsNullOrEmpty(returnURL) Then
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login")
                Else
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", returnURL)
                End If
            End If

            Return strURL
        End Function

        Public Function UserProfileURL(ByVal userId As Integer) As String
            Dim strURL As String = ""
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            strURL = NavigateURL(_portalSettings.UserTabId, "", String.Format("userId={0}", userId))

            Return strURL
        End Function

#Region "NavigateURL"

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL() As String

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Return NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString)

        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer) As String

            Return NavigateURL(TabID, Null.NullString)

        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer, ByVal IsSuperTab As Boolean) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim cultureCode As String = GetCultureCode(TabID, IsSuperTab, _portalSettings)
            Return NavigateURL(TabID, IsSuperTab, _portalSettings, Null.NullString, cultureCode, Nothing)
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal ControlKey As String) As String
            If ControlKey = "Access Denied" Then
                Return AccessDeniedURL()
            Else
                Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Return NavigateURL(_portalSettings.ActiveTab.TabID, ControlKey)
            End If
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return NavigateURL(_portalSettings.ActiveTab.TabID, ControlKey, AdditionalParameters)
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer, ByVal ControlKey As String) As String

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Return NavigateURL(TabID, _portalSettings, ControlKey, Nothing)

        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer, ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Return NavigateURL(TabID, _portalSettings, ControlKey, AdditionalParameters)

        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer, ByVal settings As PortalSettings, ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String

            Dim isSuperTab As Boolean = False
            If Not (settings Is Nothing) Then
                If settings.ActiveTab.IsSuperTab Then
                    isSuperTab = True
                End If
            End If

            Return NavigateURL(TabID, isSuperTab, settings, ControlKey, AdditionalParameters)

        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function NavigateURL(ByVal TabID As Integer, ByVal IsSuperTab As Boolean, ByVal settings As PortalSettings, ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String
            Dim cultureCode As String = GetCultureCode(TabID, IsSuperTab, settings)
            Return NavigateURL(TabID, IsSuperTab, settings, ControlKey, cultureCode, AdditionalParameters)
        End Function

        ''' <summary>
        ''' Returns a full internal url
        ''' </summary>
        ''' <param name="TabID">The tab id linking to</param>
        ''' <param name="IsSuperTab">Is the destination tab a host tab?</param>
        ''' <param name="settings">the Portalsettings</param>
        ''' <param name="ControlKey">an optional controlkey. If no controlkey is needed, pass "" </param>
        ''' <param name="Language">an optional language. If language is an empty string, the current active culture will be added in the url.</param>
        ''' <param name="AdditionalParameters">Any aditional querystring parameters. Use this format: "param1=value1", "param2=value2", ... , "paramN=valueN"</param>
        ''' <returns>Returns a full internal url</returns>
        ''' <remarks></remarks>
        Public Function NavigateURL(ByVal TabID As Integer, ByVal IsSuperTab As Boolean, ByVal settings As PortalSettings, ByVal ControlKey As String, ByVal Language As String, ByVal ParamArray AdditionalParameters As String()) As String
            Dim strURL As String

            If TabID = Null.NullInteger Then
                strURL = ApplicationURL()
            Else
                strURL = ApplicationURL(TabID)
            End If

            If ControlKey <> "" Then
                strURL += "&ctl=" & ControlKey
            End If

            If Not (AdditionalParameters Is Nothing) Then
                For Each parameter As String In AdditionalParameters
                    If Not String.IsNullOrEmpty(parameter) Then
                        strURL += "&" & parameter
                    End If
                Next
            End If

            If IsSuperTab Then
                strURL += "&portalid=" & settings.PortalId.ToString
            End If

            Dim controller As New TabController()
            Dim objTab As TabInfo = Nothing

            If (settings IsNot Nothing) Then
                If IsSuperTab Then
                    objTab = controller.GetTab(TabID, Null.NullInteger, False)
                Else
                    objTab = controller.GetTab(TabID, settings.PortalId, False)
                End If
            End If

            'only add language to url if more than one locale is enabled
            If (settings IsNot Nothing) AndAlso (LocaleController.Instance().GetLocales(settings.PortalId).Count > 1) Then
                If settings.ContentLocalizationEnabled Then
                    If Language = "" Then
                        If objTab IsNot Nothing AndAlso Not String.IsNullOrEmpty(objTab.CultureCode) Then
                            strURL += "&language=" & objTab.CultureCode
                        End If
                    Else
                        strURL += "&language=" & Language
                    End If
                ElseIf settings.EnableUrlLanguage Then
                    'legacy pre 5.5 behavior
                    If Language = "" Then
                        strURL += "&language=" & Thread.CurrentThread.CurrentCulture.Name
                    Else
                        strURL += "&language=" & Language
                    End If
                End If
            End If

            If Host.UseFriendlyUrls Then
                If objTab IsNot Nothing Then
                    Return FriendlyUrl(objTab, strURL, settings)
                End If
                Return FriendlyUrl(Nothing, strURL, settings)
            Else
                Return ResolveUrl(strURL)
            End If

        End Function

#End Region

        Public Function QueryStringEncode(ByVal QueryString As String) As String
            QueryString = HttpUtility.UrlEncode(QueryString)
            Return QueryString
        End Function

        Public Function QueryStringDecode(ByVal QueryString As String) As String
            QueryString = HttpUtility.UrlDecode(QueryString)
            Dim fullPath As String
            Try
                fullPath = HttpContext.Current.Request.MapPath(QueryString, HttpContext.Current.Request.ApplicationPath, False)
            Catch ex As HttpException
                'attempted cross application mapping
                Throw New HttpException(404, "Not Found")
            End Try
            Dim strDoubleDecodeURL As String = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(QueryString))
            If QueryString.IndexOf("..") <> -1 Or strDoubleDecodeURL.IndexOf("..") <> -1 Then
                'attempted parent path traversal
                Throw New HttpException(404, "Not Found")
            End If
            Return QueryString
        End Function

        Public Function RegisterURL(ByVal returnURL As String, ByVal originalURL As String) As String
            Dim strURL As String = ""
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim extraParams As String = String.Concat("returnurl=", returnURL)
            If Not String.IsNullOrEmpty(originalURL) Then
                extraParams += String.Concat("&orignalurl=", originalURL)
            End If

            If _portalSettings.RegisterTabId <> -1 Then
                ' user defined tab
                strURL = NavigateURL(_portalSettings.RegisterTabId, "", extraParams)
            Else
                ' portal tab
                strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Register", extraParams)
            End If

            Return strURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates the correctly formatted url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="url">The url to format.</param>
        ''' <returns>The formatted (resolved) url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ResolveUrl(ByVal url As String) As String

            ' String is Empty, just return Url
            If (url.Length = 0) Then
                Return url
            End If

            ' String does not contain a ~, so just return Url
            If (url.StartsWith("~") = False) Then
                Return url
            End If

            ' There is just the ~ in the Url, return the appPath
            If (url.Length = 1) Then
                Return Common.Globals.ApplicationPath
            End If

            If (url.ToCharArray()(1) = "/" Or url.ToCharArray()(1) = "\") Then

                ' Url looks like ~/ or ~\
                If (Not String.IsNullOrEmpty(Common.Globals.ApplicationPath)) AndAlso (Common.Globals.ApplicationPath.Length > 1) Then
                    Return Common.Globals.ApplicationPath + "/" & url.Substring(2)
                Else
                    Return "/" & url.Substring(2)
                End If

            Else

                ' Url look like ~something
                If (Not String.IsNullOrEmpty(Common.Globals.ApplicationPath)) AndAlso (Common.Globals.ApplicationPath.Length > 1) Then
                    Return Common.Globals.ApplicationPath & "/" & url.Substring(1)
                Else
                    Return Common.Globals.ApplicationPath & url.Substring(1)
                End If

            End If

        End Function

#End Region

        ''' <summary>
        ''' Encodes reserved HTML characters into HTMLencoded strings
        ''' </summary>
        ''' <param name="QueryString">The query string.</param>
        ''' <returns>Cleaned and where needed encoded query string</returns>
        Public Function EncodeReservedCharacters(ByVal QueryString As String) As String
            QueryString = QueryString.Replace("$", "%24")
            QueryString = QueryString.Replace("&", "%26")
            QueryString = QueryString.Replace("+", "%2B")
            QueryString = QueryString.Replace(",", "%2C")
            QueryString = QueryString.Replace("/", "%2F")
            QueryString = QueryString.Replace(":", "%3A")
            QueryString = QueryString.Replace(";", "%3B")
            QueryString = QueryString.Replace("=", "%3D")
            QueryString = QueryString.Replace("?", "%3F")
            QueryString = QueryString.Replace("@", "%40")

            Return QueryString
        End Function

        ''' <summary>
        ''' Converts a date to a string.
        ''' </summary>
        ''' <param name="DateValue">The date value.</param>
        ''' <returns>String in a valid date format (s)</returns>
        Public Function DateToString(ByVal DateValue As DateTime) As String
            Try
                If Not Null.IsNull(DateValue) Then
                    Return DateValue.ToString("s")
                Else
                    Return Null.NullString
                End If
            Catch ex As Exception
                Return Null.NullString
            End Try
        End Function

        ''' <summary>
        ''' Gets the hash value.
        ''' </summary>
        ''' <param name="HashObject">The hash object.</param>
        ''' <param name="DefaultValue">The default value.</param>
        ''' <returns>String with a hash value</returns>
        Public Function GetHashValue(ByVal HashObject As Object, ByVal DefaultValue As String) As String
            If Not HashObject Is Nothing Then
                If Convert.ToString(HashObject) <> "" Then
                    Return Convert.ToString(HashObject)
                Else
                    Return DefaultValue
                End If
            Else
                Return DefaultValue
            End If
        End Function

#Region "LinkClick"

        Public Function LinkClick(ByVal Link As String, ByVal TabID As Integer, ByVal ModuleID As Integer) As String
            Return LinkClick(Link, TabID, ModuleID, True, "")
        End Function

        Public Function LinkClick(ByVal Link As String, ByVal TabID As Integer, ByVal ModuleID As Integer, ByVal TrackClicks As Boolean) As String
            Return LinkClick(Link, TabID, ModuleID, TrackClicks, "")
        End Function

        Public Function LinkClick(ByVal Link As String, ByVal TabID As Integer, ByVal ModuleID As Integer, ByVal TrackClicks As Boolean, ByVal ContentType As String) As String
            Return LinkClick(Link, TabID, ModuleID, TrackClicks, ContentType <> "")
        End Function

        Public Function LinkClick(ByVal Link As String, ByVal TabID As Integer, ByVal ModuleID As Integer, ByVal TrackClicks As Boolean, ByVal ForceDownload As Boolean) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return LinkClick(Link, TabID, ModuleID, TrackClicks, ForceDownload, _portalSettings.PortalId, _portalSettings.EnableUrlLanguage, _portalSettings.GUID.ToString)
        End Function

        Public Function LinkClick(ByVal Link As String, ByVal TabID As Integer, ByVal ModuleID As Integer, ByVal TrackClicks As Boolean, ByVal ForceDownload As Boolean, ByVal PortalId As Integer, ByVal EnableUrlLanguage As Boolean, ByVal portalGuid As String) As String
            Dim strLink As String = ""
            Dim UrlType As TabType = GetURLType(Link)

            If UrlType = TabType.Member Then
                strLink = UserProfileURL(Convert.ToInt32(UrlUtils.GetParameterValue(Link)))
            ElseIf TrackClicks = True OrElse ForceDownload = True OrElse UrlType = TabType.File Then
                ' format LinkClick wrapper
                If Link.ToLowerInvariant.StartsWith("fileid=") Then
                    strLink = Common.Globals.ApplicationPath & "/LinkClick.aspx?fileticket=" & UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Link))
                End If
                If strLink = "" Then
                    strLink = Common.Globals.ApplicationPath & "/LinkClick.aspx?link=" & HttpUtility.UrlEncode(Link)
                End If

                ' tabid is required to identify the portal where the click originated
                If TabID <> Null.NullInteger Then
                    strLink += "&tabid=" & TabID.ToString
                End If

                ' moduleid is used to identify the module where the url is stored
                If ModuleID <> -1 Then
                    strLink += "&mid=" & ModuleID.ToString
                End If

                'only add language to url if more than one locale is enabled, and if admin did not turn it off
                If LocaleController.Instance().GetLocales(PortalId).Count > 1 AndAlso EnableUrlLanguage Then
                    strLink += "&language=" & Thread.CurrentThread.CurrentCulture.Name
                End If

                ' force a download dialog
                If ForceDownload Then
                    strLink += "&forcedownload=true"
                End If
            Else
                Select Case UrlType
                    Case TabType.Tab
                        strLink = NavigateURL(Integer.Parse(Link))
                    Case Else
                        strLink = Link
                End Select
            End If

            Return strLink

        End Function

#End Region

        ''' <summary>
        ''' Gets the name of the role.
        ''' </summary>
        ''' <param name="RoleID">The role ID.</param>
        ''' <returns>String with the name of the role</returns>
        Public Function GetRoleName(ByVal RoleID As Integer) As String

            If Convert.ToString(RoleID) = glbRoleAllUsers Then
                Return "All Users"
            ElseIf Convert.ToString(RoleID) = glbRoleUnauthUser Then
                Return "Unauthenticated Users"
            End If

            Dim htRoles As Hashtable = Nothing
            If Not Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                htRoles = CType(DataCache.GetCache("GetRoles"), Hashtable)
            End If

            If htRoles Is Nothing Then
                Dim objRoleController As New RoleController
                Dim arrRoles As ArrayList
                arrRoles = objRoleController.GetRoles()
                htRoles = New Hashtable
                Dim i As Integer
                For i = 0 To arrRoles.Count - 1
                    Dim objRole As RoleInfo
                    objRole = CType(arrRoles(i), RoleInfo)
                    htRoles.Add(objRole.RoleID, objRole.RoleName)
                Next
                If Not Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                    DataCache.SetCache("GetRoles", htRoles)
                End If
            End If
            Return CType(htRoles(RoleID), String)
        End Function

        Public Function GetContent(ByVal Content As String, ByVal ContentType As String) As XmlNode
            Dim xmlDoc As New XmlDocument
            xmlDoc.LoadXml(Content)
            If ContentType = "" Then
                Return xmlDoc.DocumentElement
            Else
                Return xmlDoc.SelectSingleNode(ContentType)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GenerateTabPath generates the TabPath used in Friendly URLS
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ParentId">The Id of the Parent Tab</param>
        ''' <param name="TabName">The Name of the current Tab</param>
        ''' <returns>The TabPath</returns>
        ''' <history>
        '''		[cnurse]	1/28/2005	documented
        '''                             modified to remove characters not allowed in urls
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GenerateTabPath(ByVal ParentId As Integer, ByVal TabName As String) As String
            Dim strTabPath As String = ""
            Dim objTabs As New TabController
            Dim objTab As TabInfo
            Dim strTabName As String
            objTab = objTabs.GetTab(ParentId, Null.NullInteger, False)
            Do While Not objTab Is Nothing
                strTabName = HtmlUtils.StripNonWord(objTab.TabName, False)
                strTabPath = "//" & strTabName & strTabPath
                If Null.IsNull(objTab.ParentId) Then
                    objTab = Nothing
                Else
                    objTab = objTabs.GetTab(objTab.ParentId, objTab.PortalID, False)
                End If
            Loop

            strTabPath = strTabPath & "//" & HtmlUtils.StripNonWord(TabName, False)
            Return strTabPath
        End Function

        ''' <summary>
        ''' Gets the help text.
        ''' </summary>
        ''' <param name="moduleControlId">The module control id.</param>
        ''' <returns>The help text as a string</returns>
        Public Function GetHelpText(ByVal moduleControlId As Integer) As String
            Dim helpText As String = Null.NullString
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControl(moduleControlId)
            If Not objModuleControl Is Nothing Then
                Dim FileName As String = Path.GetFileName(objModuleControl.ControlSrc)
                Dim LocalResourceFile As String = objModuleControl.ControlSrc.Replace(FileName, Services.Localization.Localization.LocalResourceDirectory & "/" & FileName)
                If Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile) <> "" Then
                    helpText = Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile)
                End If
            End If
            Return helpText
        End Function

        ''' <summary>
        ''' Gets the on line help.
        ''' </summary>
        ''' <param name="HelpUrl">The help URL.</param>
        ''' <param name="moduleConfig">The module config.</param>
        ''' <returns>String with the URL of the on line help</returns>
        Public Function GetOnLineHelp(ByVal HelpUrl As String, ByVal moduleConfig As ModuleInfo) As String
            Dim isAdminModule As Boolean = moduleConfig.DesktopModule.IsAdmin
            Dim ctlString As String = CType(HttpContext.Current.Request.QueryString("ctl"), String)

            If ((Host.EnableModuleOnLineHelp And Not isAdminModule) Or (isAdminModule)) Then
                If (isAdminModule) Or (IsAdminControl() And ctlString = "Module") Or (IsAdminControl() And ctlString = "Tab") Then
                    HelpUrl = Host.HelpURL
                End If
            Else
                HelpUrl = ""
            End If

            Return HelpUrl
        End Function

        ''' <summary>
        ''' Validates the login tab ID.
        ''' </summary>
        ''' <param name="tabId">The tab id.</param>
        ''' <returns><c>true</c> if the tab is has a login module; otherwise <c>false</c></returns>
        Public Function ValidateLoginTabID(ByVal tabId As Integer) As Boolean
            Dim hasAccountModule As Boolean = Null.NullBoolean
            For Each objModule As ModuleInfo In New ModuleController().GetTabModules(tabId).Values
                If objModule.ModuleDefinition.FriendlyName = "Account Login" Then
                    'We need to ensure that Anonymous Users or All Users have View permissions to the login page
                    Dim tab As TabInfo = New TabController().GetTab(tabId, objModule.PortalID, False)
                    If TabPermissionController.CanViewPage(tab) Then
                        hasAccountModule = True
                        Exit For
                    End If
                End If
            Next

            Return hasAccountModule
        End Function

#Region "Private Methods"

        Private Function FilenameMatchesExtensions(ByVal filename As String, ByVal strExtensions As String) As Boolean
            Dim result As Boolean = (strExtensions = String.Empty)
            If Not result Then
                filename = filename.ToUpper
                strExtensions = strExtensions.ToUpper
                For Each extension As String In strExtensions.Split(","c)
                    extension = extension.Trim
                    If Not (extension.StartsWith(".")) Then extension = "." & extension
                    result = filename.EndsWith(extension)
                    If result Then Exit For
                Next
            End If
            Return result
        End Function

#End Region

#Region "Serialization Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeserializeHashTableBase64 deserializes a Hashtable using Binary Formatting
        ''' </summary>
        ''' <remarks>
        ''' While this method of serializing is no longer supported (due to Medium Trust
        ''' issue, it is still required for upgrade purposes.
        ''' </remarks>
        ''' <param name="Source">The String Source to deserialize</param>
        ''' <returns>The deserialized Hashtable</returns>
        ''' <history>
        '''		[cnurse]	2/16/2005	moved to Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DeserializeHashTableBase64(ByVal Source As String) As Hashtable
            Dim objHashTable As Hashtable
            If Source <> "" Then
                Dim bits As Byte() = Convert.FromBase64String(Source)
                Dim mem As MemoryStream = New MemoryStream(bits)
                Dim bin As BinaryFormatter = New BinaryFormatter
                Try
                    objHashTable = CType(bin.Deserialize(mem), Hashtable)
                Catch ex As Exception
                    objHashTable = New Hashtable
                End Try
                mem.Close()
            Else
                objHashTable = New Hashtable
            End If
            Return objHashTable
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeserializeHashTableXml deserializes a Hashtable using Xml Serialization
        ''' </summary>
        ''' <remarks>
        ''' This is the preferred method of serialization under Medium Trust
        ''' </remarks>
        ''' <param name="Source">The String Source to deserialize</param>
        ''' <returns>The deserialized Hashtable</returns>
        ''' <history>
        '''		[cnurse]	2/16/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DeserializeHashTableXml(ByVal Source As String) As Hashtable
            Return XmlUtils.DeSerializeHashtable(Source, "profile")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeHashTableBase64 serializes a Hashtable using Binary Formatting
        ''' </summary>
        ''' <remarks>
        ''' While this method of serializing is no longer supported (due to Medium Trust
        ''' issue, it is still required for upgrade purposes.
        ''' </remarks>
        ''' <param name="Source">The Hashtable to serialize</param>
        ''' <returns>The serialized String</returns>
        ''' <history>
        '''		[cnurse]	2/16/2005	moved to Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SerializeHashTableBase64(ByVal Source As Hashtable) As String
            Dim strString As String
            If Source.Count <> 0 Then
                Dim bin As BinaryFormatter = New BinaryFormatter
                Dim mem As MemoryStream = New MemoryStream
                Try
                    bin.Serialize(mem, Source)
                    strString = Convert.ToBase64String(mem.GetBuffer(), 0, Convert.ToInt32(mem.Length))
                Catch ex As Exception
                    strString = ""
                Finally
                    mem.Close()
                End Try
            Else
                strString = ""
            End If
            Return strString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SerializeHashTableXml serializes a Hashtable using Xml Serialization
        ''' </summary>
        ''' <remarks>
        ''' This is the preferred method of serialization under Medium Trust
        ''' </remarks>
        ''' <param name="Source">The Hashtable to serialize</param>
        ''' <returns>The serialized String</returns>
        ''' <history>
        '''		[cnurse]	2/16/2005	moved to Globals
        '''     [cnurse]    01/19/2007  extracted to XmlUtils
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SerializeHashTableXml(ByVal Source As Hashtable) As String
            Return XmlUtils.SerializeDictionary(Source, "profile")
        End Function


#End Region

#Region "Obsolete - retained for Binary Compatability"

        ' TODO:  These constants are deprecated but cannot be removed until the next batch of breaking change
        ' ****************************************************************************************
        ' Constants are inlined in code and would require a rebuild of any module or skinobject
        ' that may be using these constants.
        ' ****************************************************************************************
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultSkinFolder As String = "/DNN-Blue/"
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultSkin As String = "Horizontal Menu - Fixed Width.ascx"
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultAdminSkin As String = "Horizontal Menu - Fixed Width.ascx"
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultContainerFolder As String = "/DNN-Blue/"
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultContainer As String = "Image Header - Color Background.ascx"
        <Obsolete("Replaced by SkinDefaults class in DotNetNuke 4.7")> _
        Public Const glbDefaultAdminContainer As String = "Image Header - Color Background.ascx"
        ' ****************************************************************************************

        <Obsolete("This method has been deprecated.")> _
        Public Sub AddFile(ByVal strFileName As String, ByVal strExtension As String, ByVal FolderPath As String, ByVal strContentType As String, ByVal Length As Integer, ByVal imageWidth As Integer, ByVal imageHeight As Integer)

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim PortalId As Integer = FileSystemUtils.GetFolderPortalId(_portalSettings)
            Dim objFiles As New FileController
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)

            If Not objFolder Is Nothing Then
                Dim objFile As New DotNetNuke.Services.FileSystem.FileInfo(PortalId, strFileName, strExtension, Length, imageWidth, imageHeight, strContentType, FolderPath, objFolder.FolderID, objFolder.StorageLocation, True)
                objFiles.AddFile(objFile)
            End If
        End Sub

        <Obsolete("Replaced in DotNetNuke 5.0 by SkinController.GetDefaultAdminSkin and SkinController.GetDefaultPortalSkin")> _
        Public ReadOnly Property DefaultSkin() As SkinDefaults
            Get
                Return SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo)
            End Get
        End Property

        Public ReadOnly Property DefaultContainer() As SkinDefaults
            Get
                Return SkinDefaults.GetSkinDefaults(SkinDefaultType.ContainerInfo)
            End Get
        End Property

        <Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.Config.GetConnectionString")> _
        Public Function GetDBConnectionString() As String
            Return Config.GetConnectionString()
        End Function

        <Obsolete("This method has been deprecated. ")> _
        Public Function GetFileList(ByVal CurrentDirectory As DirectoryInfo, Optional ByVal strExtensions As String = "", Optional ByVal NoneSpecified As Boolean = True) As ArrayList
            Dim arrFileList As New ArrayList
            Dim strExtension As String = ""

            If NoneSpecified Then
                arrFileList.Add(New FileItem("", "<" + Services.Localization.Localization.GetString("None_Specified") + ">"))
            End If

            Dim File As String
            Dim Files As String() = Directory.GetFiles(CurrentDirectory.FullName)
            For Each File In Files
                If Convert.ToBoolean(InStr(1, File, ".")) Then
                    strExtension = Mid(File, InStrRev(File, ".") + 1)
                End If
                Dim FileName As String = File.Substring(CurrentDirectory.FullName.Length)
                If InStr(1, strExtensions.ToUpper, strExtension.ToUpper) <> 0 Or strExtensions = "" Then
                    arrFileList.Add(New FileItem(FileName, FileName))
                End If
            Next

            GetFileList = arrFileList
        End Function

        <Obsolete("This method has been replaced by DesktopModuleController.GetDesktopModuleByModuleName() in DotNetNuke 5.0")> _
        Public Function GetDesktopModuleByName(ByVal name As String) As DesktopModuleInfo
            Return DesktopModuleController.GetDesktopModuleByModuleName(name, Null.NullInteger)
        End Function

        <Obsolete("This method has been deprecated. Replaced by GetSubFolderPath(ByVal strFileNamePath As String, ByVal portaId as Integer).")> _
        Public Function GetSubFolderPath(ByVal strFileNamePath As String) As String
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim ParentFolderName As String
            If _portalSettings.ActiveTab.ParentId = _portalSettings.SuperTabId Then
                ParentFolderName = Common.Globals.HostMapPath.Replace("/", "\")
            Else
                ParentFolderName = _portalSettings.HomeDirectoryMapPath.Replace("/", "\")
            End If
            Dim strFolderpath As String = strFileNamePath.Substring(0, strFileNamePath.LastIndexOf("\") + 1)

            GetSubFolderPath = strFolderpath.Substring(ParentFolderName.Length).Replace("\", "/")

        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by the DatabaseVersion property.")> _
        Public Function GetDatabaseVersion() As String
            Dim strDatabaseVersion As String = ""
            Try
                Dim databaseVersion As System.Version = DataProvider.Instance().GetVersion
                strDatabaseVersion = Format(databaseVersion.Major, "00") & Format(databaseVersion.Minor, "00") & Format(databaseVersion.Build, "00")
            Catch ex As Exception
                strDatabaseVersion = "ERROR:" & ex.Message
            End Try

            Return strDatabaseVersion
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by the DatabaseVersion property.")> _
        Public Function GetDatabaseVersion(ByVal separator As String) As String
            Dim strDatabaseVersion As String = ""
            Try
                Dim databaseVersion As System.Version = DataProvider.Instance().GetVersion
                strDatabaseVersion = Format(databaseVersion.Major, "00") & separator & Format(databaseVersion.Minor, "00") & separator & Format(databaseVersion.Build, "00")
            Catch ex As Exception
                strDatabaseVersion = "ERROR:" & ex.Message
            End Try

            Return strDatabaseVersion
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by TabController.GetPortalTabs().")> _
        Public Function GetPortalTabs(ByVal intPortalId As Integer, ByVal blnNoneSpecified As Boolean, ByVal blnHidden As Boolean, ByVal blnDeleted As Boolean, ByVal blnURL As Boolean, ByVal bCheckAuthorised As Boolean) As ArrayList
            Dim listTabs As List(Of TabInfo) = TabController.GetPortalTabs(intPortalId, Null.NullInteger, blnNoneSpecified, Null.NullString, blnHidden, blnDeleted, blnURL, False, bCheckAuthorised)
            Dim arrTabs As New ArrayList()
            For Each objTab As TabInfo In listTabs
                Dim tabTemp As TabInfo = objTab.Clone
                tabTemp.TabName = tabTemp.IndentedTabName
                arrTabs.Add(tabTemp)
            Next
            Return arrTabs
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by TabController.GetPortalTabs().")> _
        Public Function GetPortalTabs(ByVal intPortalId As Integer, ByVal blnIncludeActiveTab As Boolean, ByVal blnNoneSpecified As Boolean, ByVal blnHidden As Boolean, ByVal blnDeleted As Boolean, ByVal blnURL As Boolean, ByVal bCheckAuthorised As Boolean) As ArrayList
            ' Obtain current PortalSettings from Current Context
            Dim excludeTabId As Integer = Null.NullInteger
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            If Not blnIncludeActiveTab Then
                excludeTabId = _portalSettings.ActiveTab.TabID
            End If

            Dim listTabs As List(Of TabInfo) = TabController.GetPortalTabs(intPortalId, excludeTabId, blnNoneSpecified, Null.NullString, blnHidden, blnDeleted, blnURL, False, bCheckAuthorised)
            Dim arrTabs As New ArrayList()
            For Each objTab As TabInfo In listTabs
                Dim tabTemp As TabInfo = objTab.Clone
                tabTemp.TabName = tabTemp.IndentedTabName
                arrTabs.Add(tabTemp)
            Next
            Return arrTabs
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by TabController.GetPortalTabs().")> _
        Public Function GetPortalTabs(ByVal objDesktopTabs As ArrayList, ByVal blnNoneSpecified As Boolean, ByVal blnHidden As Boolean) As ArrayList
            Dim arrPortalTabs As ArrayList = New ArrayList
            Dim objTab As TabInfo

            If blnNoneSpecified Then
                objTab = New TabInfo
                objTab.TabID = -1
                objTab.TabName = "<" + Services.Localization.Localization.GetString("None_Specified") + ">"
                objTab.TabOrder = 0
                objTab.ParentId = -2
                arrPortalTabs.Add(objTab)
            End If

            For Each objTab In objDesktopTabs
                If Not objTab.IsSuperTab Then
                    If (objTab.IsVisible = True OrElse blnHidden = True) AndAlso _
                            (objTab.IsDeleted = False) AndAlso (objTab.TabType = TabType.Normal) Then
                        Dim tabTemp As TabInfo = objTab.Clone
                        tabTemp.TabName = tabTemp.IndentedTabName
                        arrPortalTabs.Add(tabTemp)
                    End If

                End If
            Next

            Return arrPortalTabs
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by TabController.GetPortalTabs().")> _
        Public Function GetPortalTabs(ByVal objDesktopTabs As ArrayList, ByVal blnNoneSpecified As Boolean, ByVal blnHidden As Boolean, ByVal blnDeleted As Boolean, ByVal blnURL As Boolean) As ArrayList
            Dim arrPortalTabs As ArrayList = New ArrayList
            Dim objTab As TabInfo

            If blnNoneSpecified Then
                objTab = New TabInfo
                objTab.TabID = -1
                objTab.TabName = "<" + Services.Localization.Localization.GetString("None_Specified") + ">"
                objTab.TabOrder = 0
                objTab.ParentId = -2
                arrPortalTabs.Add(objTab)
            End If

            For Each objTab In objDesktopTabs
                If Not objTab.IsSuperTab Then
                    If (objTab.IsVisible = True OrElse blnHidden = True) AndAlso _
                            (objTab.IsDeleted = False OrElse blnDeleted = True) AndAlso _
                            (objTab.TabType = TabType.Normal Or blnURL = True) Then
                        Dim tabTemp As TabInfo = objTab.Clone
                        tabTemp.TabName = tabTemp.IndentedTabName
                        arrPortalTabs.Add(tabTemp)
                    End If

                End If
            Next

            Return arrPortalTabs
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by TabController.GetPortalTabs().")> _
        Public Function GetPortalTabs(ByVal objDesktopTabs As ArrayList, ByVal currentTab As Integer, ByVal blnNoneSpecified As Boolean, ByVal blnHidden As Boolean, ByVal blnDeleted As Boolean, ByVal blnURL As Boolean, ByVal bCheckAuthorised As Boolean) As ArrayList
            Dim arrPortalTabs As ArrayList = New ArrayList
            Dim objTab As TabInfo

            If blnNoneSpecified Then
                objTab = New TabInfo
                objTab.TabID = -1
                objTab.TabName = "<" + Services.Localization.Localization.GetString("None_Specified") + ">"
                objTab.TabOrder = 0
                objTab.ParentId = -2
                arrPortalTabs.Add(objTab)
            End If

            For Each objTab In objDesktopTabs
                If ((currentTab < 0) OrElse (objTab.TabID <> currentTab)) AndAlso Not objTab.IsSuperTab Then
                    If (objTab.IsVisible = True OrElse blnHidden = True) AndAlso _
                            (objTab.IsDeleted = False OrElse blnDeleted = True) AndAlso _
                            (objTab.TabType = TabType.Normal Or blnURL = True) Then
                        Dim tabTemp As TabInfo = objTab.Clone
                        tabTemp.TabName = tabTemp.IndentedTabName
                        If bCheckAuthorised Then
                            'Check if User has Administrator rights to this tab
                            If TabPermissionController.CanAdminPage(tabTemp) Then
                                arrPortalTabs.Add(tabTemp)
                            End If
                        Else
                            arrPortalTabs.Add(tabTemp)
                        End If
                    End If

                End If
            Next

            Return arrPortalTabs
        End Function

        <Obsolete("This method has been replaced in DotNetNuke 5.0 by the Status property. and the GetStatus method.")> _
        Public Function GetUpgradeStatus() As UpgradeStatus
            GetStatus()
            Return Status
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by Host.GetHostSettingDictionary")> _
        Public ReadOnly Property HostSettings() As Hashtable
            Get
                Dim h As New Hashtable
                For Each kvp As ConfigurationSetting In HostController.Instance.GetSettings().Values
                    h.Add(kvp.Key, kvp.Value)
                Next
                Return h
            End Get
        End Property

        <Obsolete("This method has been replaced by IsAdminSkin() in DotNetNuke 5.0, as there is no longer the concept of an Admin Tab/Page")> _
        Public Function IsAdminSkin(ByVal IsAdminTab As Boolean) As Boolean

            Dim AdminKeys As String = "tab,module,importmodule,exportmodule,help"

            Dim ControlKey As String = ""
            If Not HttpContext.Current.Request.QueryString("ctl") Is Nothing Then
                ControlKey = HttpContext.Current.Request.QueryString("ctl").ToLower
            End If

            Dim ModuleID As Integer = -1
            If Not HttpContext.Current.Request.QueryString("mid") Is Nothing Then
                Integer.TryParse(HttpContext.Current.Request.QueryString("mid"), ModuleID)
            End If

            Return IsAdminTab OrElse (ControlKey <> "" And ControlKey <> "view" And ModuleID <> -1) OrElse (ControlKey <> "" And AdminKeys.IndexOf(ControlKey) <> -1 And ModuleID = -1)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the tab being displayed is in preview mode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	9/16/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Deprecated in DotNetNuke 5.0")> _
        Public Function IsTabPreview() As Boolean
            Return (PortalController.GetCurrentPortalSettings.UserMode = PortalSettings.Mode.View)
        End Function

        <Obsolete("This function has been obsoleted: Use Common.Globals.LinkClick() for proper handling of URLs")> _
        Public Function LinkClickURL(ByVal Link As String) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return LinkClick(Link, _portalSettings.ActiveTab.TabID, -1, False)
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by Host.PerformanceSetting")> _
        Public ReadOnly Property PerformanceSetting() As PerformanceSettings
            Get
                Return Host.PerformanceSetting
            End Get
        End Property

        <Obsolete("Deprecated PreventSQLInjection Function to consolidate Security Filter functions in the PortalSecurity class")> _
        Public Function PreventSQLInjection(ByVal strSQL As String) As String
            Return (New PortalSecurity).InputFilter(strSQL, PortalSecurity.FilterFlag.NoSQL)
        End Function

        <Obsolete("Deprecated in DNN 5.3.  Replaced by UserProfileURL")> _
        Public Function ProfileURL(ByVal userID As Integer) As String
            Dim strURL As String = ""
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            If _portalSettings.UserTabId <> -1 Then
                strURL = NavigateURL(_portalSettings.UserTabId)
            Else
                strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Profile", "UserID=" + userID.ToString())
            End If

            Return strURL
        End Function

        <Obsolete("This method has been deprecated. Replaced by same method in FileSystemUtils class.")> _
        Public Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As HttpPostedFile, Optional ByVal Unzip As Boolean = False) As String
            Return FileSystemUtils.UploadFile(RootPath, objHtmlInputFile, Unzip)
        End Function

        <Obsolete("Deprecated in 5.1. Replaced by CachingProvider.Instance.IsWebFarm.")> _
        Public ReadOnly Property WebFarmEnabled() As Boolean
            Get
                Return CachingProvider.Instance().IsWebFarm
            End Get
        End Property


#Region "Html functions moved to HtmlUtils.vb"

        <Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.HtmlUtils.FormatEmail")> _
        Public Function FormatEmail(ByVal Email As String) As String
            Return HtmlUtils.FormatEmail(Email)
        End Function

        <Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.HtmlUtils.FormatWebsite")> _
        Public Function FormatWebsite(ByVal Website As Object) As String
            Return HtmlUtils.FormatWebsite(Website)
        End Function

#End Region

#Region "Xml functions moved to XmlUtils.vb"

        <Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.XmlUtils.XMLEncode")> _
        Public Function XMLEncode(ByVal HTML As String) As String
            Return XmlUtils.XMLEncode(HTML)
        End Function

#End Region

#End Region

    End Module

End Namespace
