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
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Web

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic

Namespace DotNetNuke.UI.Skins

    Public Enum SkinParser
        Localized
        Portable
    End Enum

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : SkinFileProcessor
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles processing of a list of uploaded skin files into a working skin.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[willhsc]	3/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinFileProcessor

#Region "Private Members"

        Private ReadOnly m_SkinRoot As String
        Private ReadOnly m_SkinPath As String
        Private ReadOnly m_SkinName As String
        Private ReadOnly m_SkinAttributes As New XmlDocument
        Private ReadOnly m_PathFactory As New PathParser
        Private ReadOnly m_ControlFactory As ControlParser
        Private ReadOnly m_ObjectFactory As ObjectParser
        Private ReadOnly m_ControlList As New Hashtable
        Private m_Message As String = ""

        'Localized Strings
        Private INITIALIZE_PROCESSOR As String = Util.GetLocalizedString("StartProcessor")
        Private PACKAGE_LOAD As String = Util.GetLocalizedString("PackageLoad")
        Private PACKAGE_LOAD_ERROR As String = Util.GetLocalizedString("PackageLoad.Error")
        Private DUPLICATE_ERROR As String = Util.GetLocalizedString("DuplicateSkinObject.Error")
        Private DUPLICATE_DETAIL As String = Util.GetLocalizedString("DuplicateSkinObject.Detail")
        Private LOAD_SKIN_TOKEN As String = Util.GetLocalizedString("LoadingSkinToken")
        Private FILE_BEGIN As String = Util.GetLocalizedString("BeginSkinFile")
        Private FILE_END As String = Util.GetLocalizedString("EndSkinFile")
        Private FILES_END As String = Util.GetLocalizedString("EndSkinFiles")

        Private ReadOnly Property PathFactory() As PathParser
            Get
                Return m_PathFactory
            End Get
        End Property

        Private ReadOnly Property ControlFactory() As ControlParser
            Get
                Return m_ControlFactory
            End Get
        End Property

        Private ReadOnly Property ObjectFactory() As ObjectParser
            Get
                Return m_ObjectFactory
            End Get
        End Property

        Private ReadOnly Property SkinAttributes() As XmlDocument
            Get
                Return m_SkinAttributes
            End Get
        End Property

        Private Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = Value
            End Set
        End Property

#End Region

#Region "Public Properties"

        Public ReadOnly Property SkinRoot() As String
            Get
                Return m_SkinRoot
            End Get
        End Property

        Public ReadOnly Property SkinPath() As String
            Get
                Return m_SkinPath
            End Get
        End Property

        Public ReadOnly Property SkinName() As String
            Get
                Return m_SkinName
            End Get
        End Property

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     SkinFileProcessor class constructor.
        ''' </summary>
        ''' <remarks>
        '''     This constructor parses a memory based skin
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/21/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal ControlKey As String, ByVal ControlSrc As String)
            m_ControlList.Add(ControlKey, ControlSrc)

            ' Instantiate the control parser with the list of skin objects
            m_ControlFactory = New ControlParser(m_ControlList)

            ' Instantiate the object parser with the list of skin objects
            m_ObjectFactory = New ObjectParser(m_ControlList)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     SkinFileProcessor class constructor.
        ''' </summary>
        ''' <param name="SkinPath">File path to the portals upload directory.</param>
        ''' <param name="SkinRoot">Specifies type of skin (Skins or Containers)</param>
        ''' <param name="SkinName">Name of folder in which skin will reside (Zip file name)</param>
        ''' <remarks>
        '''     The constructor primes the file processor with path information and
        '''     control data that should only be retrieved once.  It checks for the
        '''     existentce of a skin level attribute file and read it in, if found.
        '''     It also sorts through the complete list of controls and creates
        '''     a hashtable which contains only the skin objects and their source paths.
        '''     These are recognized by their ControlKey's which are formatted like
        '''     tokens ("[TOKEN]").  The hashtable is required for speed as it will be
        '''     processed for each token found in the source file by the Control Parser.
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal SkinPath As String, ByVal SkinRoot As String, ByVal SkinName As String)
            Me.Message += SkinController.FormatMessage(INITIALIZE_PROCESSOR, SkinRoot & " :: " & SkinName, 0, False)

            ' Save path information for future use
            m_SkinRoot = SkinRoot
            m_SkinPath = SkinPath
            m_SkinName = SkinName

            ' Check for and read skin package level attribute information file
            Dim FileName As String = Me.SkinPath & Me.SkinRoot & "\" & Me.SkinName & "\" & SkinRoot.Substring(0, SkinRoot.Length - 1) & ".xml"
            If File.Exists(FileName) Then
                Try
                    Me.SkinAttributes.Load(FileName)
                    Me.Message += SkinController.FormatMessage(PACKAGE_LOAD, Path.GetFileName(FileName), 2, False)
                Catch ex As Exception
                    ' could not load XML file
                    Me.Message += SkinController.FormatMessage(String.Format(PACKAGE_LOAD_ERROR, ex.Message), Path.GetFileName(FileName), 2, True)
                End Try
            End If

            ' Look at every control
            Dim Token As String
            For Each objSkinControl As SkinControlInfo In SkinControlController.GetSkinControls().Values
                Token = objSkinControl.ControlKey.ToUpper

                ' If the control is already in the hash table
                If m_ControlList.ContainsKey(Token) Then
                    ' Record an error message and skip it
                    Me.Message += SkinController.FormatMessage(String.Format(DUPLICATE_ERROR, objSkinControl.ControlKey.ToString.ToUpper), String.Format(DUPLICATE_DETAIL, DirectCast(m_ControlList.Item(Token), String), objSkinControl.ControlSrc.ToString), 2, True)
                Else
                    ' Add it
                    Me.Message += SkinController.FormatMessage(String.Format(LOAD_SKIN_TOKEN, objSkinControl.ControlKey.ToString.ToUpper), objSkinControl.ControlSrc.ToString, 2, False)
                    m_ControlList.Add(Token, objSkinControl.ControlSrc)
                End If
            Next

            ' Instantiate the control parser with the list of skin objects
            m_ControlFactory = New ControlParser(m_ControlList)

            ' Instantiate the object parser with the list of skin objects
            m_ObjectFactory = New ObjectParser(m_ControlList)
        End Sub

#End Region

        Public Function ProcessFile(ByVal FileName As String, ByVal ParseOption As UI.Skins.SkinParser) As String
            Dim strMessage As String = SkinController.FormatMessage(FILE_BEGIN, Path.GetFileName(FileName), 0, False)

            ' create a skin file object to aid in processing
            Dim objSkinFile As New SkinFile(Me.SkinRoot, FileName, Me.SkinAttributes)

            ' choose processing based on type of file
            Select Case objSkinFile.FileExtension
                Case ".htm", ".html"
                    ' replace paths, process control tokens and convert html to ascx format
                    strMessage += Me.ObjectFactory.Parse(objSkinFile.Contents)
                    strMessage += Me.PathFactory.Parse(objSkinFile.Contents, Me.PathFactory.HTMLList, objSkinFile.SkinRootPath, ParseOption)
                    strMessage += Me.ControlFactory.Parse(objSkinFile.Contents, objSkinFile.Attributes)

                    Dim Registrations As New ArrayList
                    Registrations.AddRange(Me.ControlFactory.Registrations)
                    Registrations.AddRange(Me.ObjectFactory.Registrations)
                    strMessage += objSkinFile.PrependASCXDirectives(Registrations)
            End Select

            objSkinFile.Write()
            strMessage += objSkinFile.Messages

            strMessage += SkinController.FormatMessage(FILE_END, Path.GetFileName(FileName), 1, False)

            Return strMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Perform processing on list of files to generate skin.
        ''' </summary>
        ''' <param name="FileList">ArrayList of files to be processed.</param>
        ''' <returns>HTML formatted string of informational messages.</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ProcessList(ByVal FileList As ArrayList) As String
            Return ProcessList(FileList, SkinParser.Localized)
        End Function

        Public Function ProcessList(ByVal FileList As ArrayList, ByVal ParseOption As UI.Skins.SkinParser) As String
            ' process each file in the list
            For Each FileName As String In FileList
                Me.Message += ProcessFile(FileName, ParseOption)
            Next

            Me.Message += SkinController.FormatMessage(FILES_END, Me.SkinRoot & " :: " & Me.SkinName, 0, False)

            Return Me.Message
        End Function

        Public Function ProcessSkin(ByVal SkinSource As String, ByVal SkinAttributes As XmlDocument, ByVal ParseOption As UI.Skins.SkinParser) As String
            ' create a skin file object to aid in processing
            Dim objSkinFile As New SkinFile(SkinSource, SkinAttributes)

            ' process control tokens and convert html to ascx format
            Me.Message += Me.ControlFactory.Parse(objSkinFile.Contents, objSkinFile.Attributes)
            Me.Message += objSkinFile.PrependASCXDirectives(Me.ControlFactory.Registrations)

            Return objSkinFile.Contents
        End Function

#Region "Private SkinFile class"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Utility class for processing of skin files.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class SkinFile
            Private ReadOnly m_FileName As String
            Private ReadOnly m_FileExtension As String
            Private ReadOnly m_WriteFileName As String
            Private ReadOnly m_SkinRoot As String
            Private ReadOnly m_SkinRootPath As String
            Private ReadOnly m_FileAttributes As XmlDocument
            Private m_FileContents As String
            Private m_Messages As String = ""

            'Localized Strings
            Private FILE_FORMAT_ERROR As String = Util.GetLocalizedString("FileFormat.Error")
            Private FILE_FORMAT_DETAIL As String = Util.GetLocalizedString("FileFormat.Detail")
            Private FILE_LOAD As String = Util.GetLocalizedString("SkinFileLoad")
            Private FILE_LOAD_ERROR As String = Util.GetLocalizedString("SkinFileLoad.Error")
            Private FILE_WRITE As String = Util.GetLocalizedString("FileWrite")
            Private CONTROL_DIR As String = Util.GetLocalizedString("ControlDirective")
            Private CONTROL_REG As String = Util.GetLocalizedString("ControlRegister")

            Public ReadOnly Property SkinRoot() As String
                Get
                    Return m_SkinRoot
                End Get
            End Property

            Public ReadOnly Property Attributes() As XmlDocument
                Get
                    Return m_FileAttributes
                End Get
            End Property

            Public ReadOnly Property Messages() As String
                Get
                    Return m_Messages
                End Get
            End Property

            Public ReadOnly Property FileName() As String
                Get
                    Return m_FileName
                End Get
            End Property

            Public ReadOnly Property WriteFileName() As String
                Get
                    Return m_WriteFileName
                End Get
            End Property

            Public ReadOnly Property FileExtension() As String
                Get
                    Return m_FileExtension
                End Get
            End Property

            Public ReadOnly Property SkinRootPath() As String
                Get
                    Return m_SkinRootPath
                End Get
            End Property

            Public Property Contents() As String
                Get
                    Return m_FileContents
                End Get
                Set(ByVal Value As String)
                    m_FileContents = Value
                End Set
            End Property

            Public Sub New(ByVal SkinContents As String, ByVal SkinAttributes As XmlDocument)
                ' set attributes
                m_FileAttributes = SkinAttributes

                ' set file contents
                Me.Contents = SkinContents
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     SkinFile class constructor.
            ''' </summary>
            ''' <param name="SkinRoot"></param>
            ''' <param name="FileName"></param>
            ''' <param name="SkinAttributes"></param>
            ''' <remarks>
            '''     The constructor primes the utility class with basic file information.
            '''     It also checks for the existentce of a skinfile level attribute file
            '''     and read it in, if found.  
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal SkinRoot As String, ByVal FileName As String, ByVal SkinAttributes As XmlDocument)
                ' capture file information
                m_FileName = FileName
                m_FileExtension = Path.GetExtension(FileName)
                m_SkinRoot = SkinRoot
                m_FileAttributes = SkinAttributes

                ' determine and store path to portals skin root folder
                Dim strTemp As String = Replace(FileName, Path.GetFileName(FileName), "")
                strTemp = Replace(strTemp, "\", "/")
                m_SkinRootPath = Common.Globals.ApplicationPath & Mid(strTemp, InStr(1, strTemp.ToUpper(), "/PORTALS"))

                ' read file contents
                Me.Contents = Read(FileName)

                ' setup some attributes based on file extension
                Select Case Me.FileExtension
                    Case ".htm", ".html"
                        ' set output file name to <filename>.ASCX
                        m_WriteFileName = FileName.Replace(Path.GetExtension(FileName), ".ascx")
                        ' capture warning if file does not contain a id="ContentPane" or [CONTENTPANE]
                        Dim PaneCheck1 As New Regex("\s*id\s*=\s*""" & glbDefaultPane & """", RegexOptions.IgnoreCase)
                        Dim PaneCheck2 As New Regex("\s*[" & glbDefaultPane & "]", RegexOptions.IgnoreCase)
                        If PaneCheck1.IsMatch(Me.Contents) = False And PaneCheck2.IsMatch(Me.Contents) = False Then
                            m_Messages += SkinController.FormatMessage(FILE_FORMAT_ERROR, String.Format(FILE_FORMAT_ERROR, FileName), 2, True)
                        End If

                        ' Check for existence of and load skin file level attribute information 
                        If File.Exists(FileName.Replace(Me.FileExtension, ".xml")) Then
                            Try
                                m_FileAttributes.Load(FileName.Replace(Me.FileExtension, ".xml"))
                                m_Messages += SkinController.FormatMessage(FILE_LOAD, FileName, 2, False)
                            Catch ex As Exception       ' could not load XML file
                                m_FileAttributes = SkinAttributes
                                m_Messages += SkinController.FormatMessage(FILE_LOAD_ERROR, FileName, 2, True)
                            End Try
                        End If
                    Case Else
                        ' output file name is same as input file name
                        m_WriteFileName = FileName
                End Select
            End Sub

            Private Function Read(ByVal FileName As String) As String
                Dim objStreamReader As New StreamReader(FileName)
                Dim strFileContents As String = objStreamReader.ReadToEnd()
                objStreamReader.Close()

                Return strFileContents
            End Function

            Public Sub Write()
                ' delete the file before attempting to write
                If File.Exists(Me.WriteFileName) Then
                    File.Delete(Me.WriteFileName)
                End If

                m_Messages += SkinController.FormatMessage(FILE_WRITE, Path.GetFileName(Me.WriteFileName), 2, False)
                Dim objStreamWriter As New StreamWriter(Me.WriteFileName)
                objStreamWriter.WriteLine(Me.Contents)
                objStreamWriter.Flush()
                objStreamWriter.Close()
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Prepend ascx control directives to file contents.
            ''' </summary>
            ''' <param name="Registrations">ArrayList of registration directives.</param>
            ''' <remarks>
            '''     This procedure formats the @Control directive and prepends it and all
            '''     registration directives to the file contents.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function PrependASCXDirectives(ByVal Registrations As ArrayList) As String
                Dim Messages As String = ""
                Dim Prefix As String = ""

                ' if the skin source is an HTML document, extract the content within the <body> tags
                Dim strPattern As String = "<\s*body[^>]*>(?<skin>.*)<\s*/\s*body\s*>"
                Dim objMatch As Match
                objMatch = Regex.Match(Me.Contents, strPattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
                If objMatch.Groups(1).Value <> "" Then
                    Me.Contents = objMatch.Groups(1).Value
                End If

                ' format and save @Control directive
                Select Case Me.SkinRoot
                    Case SkinController.RootSkin
                        Prefix += "<%@ Control language=""vb"" AutoEventWireup=""false"" Explicit=""True"" Inherits=""DotNetNuke.UI.Skins.Skin"" %>" & vbCrLf
                    Case SkinController.RootContainer
                        Prefix += "<%@ Control language=""vb"" AutoEventWireup=""false"" Explicit=""True"" Inherits=""DotNetNuke.UI.Containers.Container"" %>" & vbCrLf
                End Select

                Messages += SkinController.FormatMessage(CONTROL_DIR, HttpUtility.HtmlEncode(Prefix), 2, False)

                ' add preformatted Control Registrations
                Dim Item As String
                For Each Item In Registrations
                    Messages += SkinController.FormatMessage(CONTROL_REG, HttpUtility.HtmlEncode(Item), 2, False)
                    Prefix += Item
                Next

                ' update file contents to include ascx header information
                Me.Contents = Prefix + Me.Contents

                Return Messages
            End Function

        End Class

#End Region

#Region "Private PathParser Class"

        ''' -----------------------------------------------------------------------------
        ''' Project	 : DotNetNuke
        ''' Class	 : SkinFileProcessor.PathParser
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Parsing functionality for path replacement in new skin files.
        ''' </summary>
        ''' <remarks>
        '''     This class encapsulates the data and methods necessary to appropriately
        '''     handle all the path replacement parsing needs for new skin files. Parsing
        '''     supported for CSS syntax and HTML syntax (which covers ASCX files also). 
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class PathParser

            Private m_HTMLPatterns As New ArrayList
            Private m_CSSPatterns As New ArrayList
            Private m_SkinPath As String = ""
            Private m_ParseOption As UI.Skins.SkinParser
            Private m_Messages As String = ""

            Private SUBST As String = Util.GetLocalizedString("Substituting")
            Private SUBST_DETAIL As String = Util.GetLocalizedString("Substituting.Detail")

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     List of regular expressions for processing HTML syntax.
            ''' </summary>
            ''' <returns>ArrayList of Regex objects formatted for the Parser method.</returns>
            ''' <remarks>
            '''     Additional patterns can be added to this list (if necessary) if properly
            '''     formatted to return <tag/>, <content/> and <endtag/> groups.  For future
            '''     consideration, this list could be imported from a configuration file to
            '''     provide for greater flexibility.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property HTMLList() As ArrayList
                Get
                    ' if the arraylist in uninitialized
                    If m_HTMLPatterns.Count() = 0 Then

                        ' retrieve the patterns
                        Dim arrPattern() As String = { _
                         "(?<tag><head[^>]*?\sprofile\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><object[^>]*?\s(?:codebase|data|usemap)\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><img[^>]*?\s(?:src|longdesc|usemap)\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><input[^>]*?\s(?:src|usemap)\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><iframe[^>]*?\s(?:src|longdesc)\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><(?:td|th|table|body)[^>]*?\sbackground\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><(?:script|bgsound|embed|xml|frame)[^>]*?\ssrc\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><(?:base|link|a|area)[^>]*?\shref\s*=\s*"")(?!https://|http://|\\|[~/]|javascript:|mailto:)(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><(?:blockquote|ins|del|q)[^>]*?\scite\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><(?:param\s+name\s*=\s*""(?:movie|src|base)"")[^>]*?\svalue\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)", _
                         "(?<tag><embed[^>]*?\s(?:src)\s*=\s*"")(?!https://|http://|\\|[~/])(?<content>[^""]*)(?<endtag>""[^>]*>)"}

                        ' for each pattern, create a regex object
                        Dim i As Integer
                        For i = 0 To arrPattern.GetLength(0) - 1
                            Dim re As New Regex(arrPattern(i), RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                            ' add the Regex object to the pattern array list
                            m_HTMLPatterns.Add(re)
                        Next
                        ' optimize the arraylist size since it will not change
                        m_HTMLPatterns.TrimToSize()

                    End If

                    Return m_HTMLPatterns
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     List of regular expressions for processing CSS syntax.
            ''' </summary>
            ''' <returns>ArrayList of Regex objects formatted for the Parser method.</returns>
            ''' <remarks>
            '''     Additional patterns can be added to this list (if necessary) if properly
            '''     formatted to return <tag/>, <content/> and <endtag/> groups.  For future
            '''     consideration, this list could be imported from a configuration file to
            '''     provide for greater flexibility.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property CSSList() As ArrayList
                Get
                    ' if the arraylist in uninitialized
                    If m_CSSPatterns.Count() = 0 Then

                        ' retrieve the patterns
                        Dim arrPattern() As String = { _
                         "(?<tag>\surl\u0028)(?<content>[^\u0029]*)(?<endtag>\u0029.*;)"}

                        ' for each pattern, create a regex object
                        Dim i As Integer
                        For i = 0 To arrPattern.GetLength(0) - 1
                            Dim re As New Regex(arrPattern(i), RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                            ' add the Regex object to the pattern array list
                            m_CSSPatterns.Add(re)
                        Next
                        ' optimize the arraylist size since it will not change
                        m_CSSPatterns.TrimToSize()

                    End If

                    Return m_CSSPatterns
                End Get
            End Property

            Private ReadOnly Property Handler() As MatchEvaluator
                Get
                    Return AddressOf MatchHandler
                End Get
            End Property

            Private Property SkinPath() As String
                Get
                    Return m_SkinPath
                End Get
                Set(ByVal Value As String)
                    m_SkinPath = Value
                End Set
            End Property

            Private Property ParseOption() As UI.Skins.SkinParser
                Get
                    Return m_ParseOption
                End Get
                Set(ByVal Value As UI.Skins.SkinParser)
                    m_ParseOption = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Perform parsing on the specified source file.
            ''' </summary>
            ''' <param name="Source">Pointer to Source string to be parsed.</param>
            ''' <param name="RegexList">ArrayList of properly formatted regular expression objects.</param>
            ''' <param name="SkinPath">Path to use in replacement operation.</param>
            ''' <remarks>
            '''     This procedure iterates through the list of regular expression objects
            '''     and invokes a handler for each match which uses the specified path.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function Parse(ByRef Source As String, ByRef RegexList As ArrayList, ByVal SkinPath As String, ByVal ParseOption As UI.Skins.SkinParser) As String
                m_Messages = ""

                ' set path propery which is file specific
                Me.SkinPath = SkinPath
                ' set parse option
                Me.ParseOption = ParseOption

                ' process each regular expression
                Dim i As Integer
                For i = 0 To RegexList.Count - 1
                    Source = DirectCast(RegexList(i), Regex).Replace(Source, Me.Handler)
                Next

                Return m_Messages
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Process regular expression matches.
            ''' </summary>
            ''' <param name="m">Regular expression match for path information which requires processing.</param>
            ''' <returns>Properly formatted path information.</returns>
            ''' <remarks>
            '''     The handler is invoked by the Regex.Replace method once for each match that
            '''     it encounters.  The returned value of the handler is substituted for the
            '''     original match.  So the handler properly formats the path information and
            '''     returns it in favor of the improperly formatted match.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function MatchHandler(ByVal m As Match) As String
                Dim strOldTag As String = m.Groups("tag").Value & m.Groups("content").Value & m.Groups("endtag").Value
                Dim strNewTag As String = strOldTag

                ' we do not want to process object tags to DotNetNuke widgets
                If Not m.Groups(0).Value.ToLower.Contains("codetype=""dotnetnuke/client""") Then
                    Select Case Me.ParseOption
                        Case SkinParser.Localized
                            ' if the tag does not contain the localized path
                            If strNewTag.IndexOf(Me.SkinPath) = -1 Then
                                ' insert the localized path
                                strNewTag = m.Groups("tag").Value & Me.SkinPath & m.Groups("content").Value & m.Groups("endtag").Value
                            End If
                        Case SkinParser.Portable
                            ' if the tag does not contain a reference to the skinpath
                            If strNewTag.ToLower.IndexOf("<%= skinpath %>") = -1 Then
                                ' insert the skinpath 
                                strNewTag = m.Groups("tag").Value & "<%= SkinPath %>" & m.Groups("content").Value & m.Groups("endtag").Value
                            End If
                            ' if the tag contains the localized path
                            If strNewTag.IndexOf(Me.SkinPath) <> -1 Then
                                ' remove the localized path
                                strNewTag = strNewTag.Replace(Me.SkinPath, "")
                            End If
                    End Select
                End If

                m_Messages += SkinController.FormatMessage(SUBST, String.Format(SUBST_DETAIL, System.Web.HttpUtility.HtmlEncode(strOldTag), System.Web.HttpUtility.HtmlEncode(strNewTag)), 2, False)
                Return strNewTag
            End Function

        End Class

#End Region

#Region "Private ControlParser Class"

        ''' -----------------------------------------------------------------------------
        ''' Project	 : DotNetNuke
        ''' Class	 : SkinFileProcessor.ControlParser
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Parsing functionality for token replacement in new skin files.
        ''' </summary>
        ''' <remarks>
        '''     This class encapsulates the data and methods necessary to appropriately
        '''     handle all the token parsing needs for new skin files (which is appropriate
        '''     only for HTML files).  The parser accomodates some ill formatting of tokens
        '''     (ignoring whitespace and casing) and allows for naming of token instances
        '''     if more than one instance of a particular control is desired on a skin.  The
        '''     proper syntax for an instance is: "[TOKEN:INSTANCE]" where the instance can
        '''     be any alphanumeric string.  Generated control ID's all take the
        '''     form of "TOKENINSTANCE".
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class ControlParser

            Private ReadOnly m_ControlList As New Hashtable
            Private ReadOnly m_InitMessages As String = ""
            Private m_RegisterList As New ArrayList
            Private m_Attributes As New XmlDocument
            Private m_ParseMessages As String = ""

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Registration directives generated as a result of the Parse method.
            ''' </summary>
            ''' <returns>ArrayList of formatted registration directives.</returns>
            ''' <remarks>
            '''     In addition to the updated file contents, the Parse method also
            '''     creates this list of formatted registration directives which can
            '''     be processed later.  They are not performed in place during the
            '''     Parse method in order to preserve the formatting of the input file
            '''     in case additional parsing might not anticipate the formatting of
            '''     those directives.  Since they are properly formatted, it is better
            '''     to exclude them from being subject to parsing.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Friend ReadOnly Property Registrations() As ArrayList
                Get
                    Return m_RegisterList
                End Get
            End Property

            Private ReadOnly Property Handler() As MatchEvaluator
                Get
                    Return AddressOf TokenMatchHandler
                End Get
            End Property

            Private Property RegisterList() As ArrayList
                Get
                    Return m_RegisterList
                End Get
                Set(ByVal Value As ArrayList)
                    m_RegisterList = Value
                End Set
            End Property

            Private ReadOnly Property ControlList() As Hashtable
                Get
                    Return m_ControlList
                End Get
            End Property

            Private Property Attributes() As XmlDocument
                Get
                    Return m_Attributes
                End Get
                Set(ByVal Value As XmlDocument)
                    m_Attributes = Value
                End Set
            End Property

            Private Property Messages() As String
                Get
                    Return m_ParseMessages
                End Get
                Set(ByVal Value As String)
                    m_ParseMessages = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     ControlParser class constructor.
            ''' </summary>
            ''' <remarks>
            '''     The constructor processes accepts a hashtable of skin objects to process against.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal ControlList As Hashtable)
                m_ControlList = DirectCast(ControlList.Clone, Hashtable)
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Perform parsing on the specified source file using the specified attributes.
            ''' </summary>
            ''' <param name="Source">Pointer to Source string to be parsed.</param>
            ''' <param name="Attributes">XML document containing token attribute information (can be empty).</param>
            ''' <remarks>
            '''     This procedure invokes a handler for each match of a formatted token.
            '''     The attributes are first set because they will be referenced by the
            '''     match handler.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function Parse(ByRef Source As String, ByVal Attributes As XmlDocument) As String
                Me.Messages = m_InitMessages

                ' set the token attributes
                Me.Attributes = Attributes
                ' clear register list
                Me.RegisterList.Clear()

                ' define the regular expression to match tokens
                Dim FindTokenInstance As New Regex("\[\s*(?<token>\w*)\s*:?\s*(?<instance>\w*)\s*]", RegexOptions.IgnoreCase)

                ' parse the file
                Source = FindTokenInstance.Replace(Source, Me.Handler)

                Return Messages
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Process regular expression matches.
            ''' </summary>
            ''' <param name="m">Regular expression match for token which requires processing.</param>
            ''' <returns>Properly formatted token.</returns>
            ''' <remarks>
            '''     The handler is invoked by the Regex.Replace method once for each match that
            '''     it encounters.  The returned value of the handler is substituted for the
            '''     original match.  So the handler properly formats the replacement for the
            '''     token and returns it instead.  If an unknown token is encountered, the token
            '''     is unmodified.  This can happen if a token is used for a skin object which
            '''     has not yet been installed.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function TokenMatchHandler(ByVal m As Match) As String
                Dim TOKEN_PROC As String = Util.GetLocalizedString("ProcessToken")
                Dim TOKEN_SKIN As String = Util.GetLocalizedString("SkinToken")
                Dim TOKEN_PANE As String = Util.GetLocalizedString("PaneToken")
                Dim TOKEN_FOUND As String = Util.GetLocalizedString("TokenFound")
                Dim TOKEN_FORMAT As String = Util.GetLocalizedString("TokenFormat")
                Dim TOKEN_NOTFOUND_INFILE As String = Util.GetLocalizedString("TokenNotFoundInFile")
                Dim CONTROL_FORMAT As String = Util.GetLocalizedString("ControlFormat")
                Dim TOKEN_NOTFOUND As String = Util.GetLocalizedString("TokenNotFound")

                Dim Token As String = m.Groups("token").Value.ToUpper
                Dim ControlName As String = Token & m.Groups("instance").Value

                ' if the token has an instance name, use it to look for the corresponding attributes
                Dim AttributeNode As String = Token & Convert.ToString(IIf(m.Groups("instance").Value = "", "", ":" & m.Groups("instance").Value))

                Me.Messages += SkinController.FormatMessage(TOKEN_PROC, "[" & AttributeNode & "]", 2, False)

                ' if the token is a recognized skin control
                If Me.ControlList.ContainsKey(Token) = True Or Token.IndexOf("CONTENTPANE") <> -1 Then

                    Dim SkinControl As String = ""

                    If Me.ControlList.ContainsKey(Token) Then
                        Me.Messages += SkinController.FormatMessage(TOKEN_SKIN, DirectCast(Me.ControlList.Item(Token), String), 2, False)
                    Else
                        Me.Messages += SkinController.FormatMessage(TOKEN_PANE, Token, 2, False)
                    End If

                    ' if there is an attribute file
                    If Not Me.Attributes.DocumentElement Is Nothing Then
                        ' look for the the node of this instance of the token
                        Dim xmlSkinAttributeRoot As XmlNode = Me.Attributes.DocumentElement.SelectSingleNode("descendant::Object[Token='[" & AttributeNode & "]']")
                        ' if the token is found
                        If Not xmlSkinAttributeRoot Is Nothing Then
                            Me.Messages += SkinController.FormatMessage(TOKEN_FOUND, "[" & AttributeNode & "]", 2, False)
                            ' process each token attribute
                            Dim xmlSkinAttribute As XmlNode
                            For Each xmlSkinAttribute In xmlSkinAttributeRoot.SelectNodes(".//Settings/Setting")
                                If xmlSkinAttribute.SelectSingleNode("Value").InnerText <> "" Then
                                    ' append the formatted attribute to the inner contents of the control statement
                                    Me.Messages += SkinController.FormatMessage(TOKEN_FORMAT, xmlSkinAttribute.SelectSingleNode("Name").InnerText & "=""" & xmlSkinAttribute.SelectSingleNode("Value").InnerText & """", 2, False)
                                    SkinControl += " " & xmlSkinAttribute.SelectSingleNode("Name").InnerText & "=""" & xmlSkinAttribute.SelectSingleNode("Value").InnerText.Replace("""", "&quot;") & """"
                                End If
                            Next
                        Else
                            Me.Messages += SkinController.FormatMessage(TOKEN_NOTFOUND_INFILE, "[" & AttributeNode & "]", 2, False)
                        End If
                    End If

                    If Me.ControlList.ContainsKey(Token) Then
                        ' create the skin object user control tag
                        SkinControl = "dnn:" & Token & " runat=""server"" id=""dnn" & ControlName & """" & SkinControl

                        ' save control registration statement
                        Dim ControlRegistration As String = "<%@ Register TagPrefix=""dnn"" TagName=""" & Token & """ Src=""~/" & DirectCast(Me.ControlList.Item(Token), String) & """ %>" & vbCrLf
                        If RegisterList.Contains(ControlRegistration) = False Then
                            RegisterList.Add(ControlRegistration)
                        End If

                        ' return the control statement
                        Me.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" & SkinControl & " /&gt;", 2, False)

                        SkinControl = "<" & SkinControl & " />"
                    Else
                        If SkinControl.ToLower.IndexOf("id=") = -1 Then
                            SkinControl = " id=""ContentPane"""
                        End If
                        SkinControl = "div runat=""server""" & SkinControl & "></div"

                        ' return the control statement
                        Me.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" & SkinControl & "&gt;", 2, False)

                        SkinControl = "<" & SkinControl & ">"
                    End If

                    Return SkinControl
                Else
                    ' return the unmodified token
                    ' note that this is currently protecting array syntax in embedded javascript
                    ' should be fixed in the regular expressions but is not, currently.
                    Me.Messages += SkinController.FormatMessage(TOKEN_NOTFOUND, "[" & m.Groups("token").Value & "]", 2, False)
                    Return "[" & m.Groups("token").Value & "]"
                End If
            End Function

        End Class

#End Region

#Region "Private ObjectParser Class"

        ''' -----------------------------------------------------------------------------
        ''' Project	 : DotNetNuke
        ''' Class	 : SkinFileProcessor.ObjectParser
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Parsing functionality for token replacement in new skin files.
        ''' </summary>
        ''' <remarks>
        '''     This class encapsulates the data and methods necessary to appropriately
        '''     handle all the object parsing needs for new skin files (which is appropriate
        '''     only for HTML files).  The parser accomodates some ill formatting of objects
        '''     (ignoring whitespace and casing) and allows for naming of object instances
        '''     if more than one instance of a particular control is desired on a skin.  The
        '''     proper syntax for an instance is: "[OBJECT:INSTANCE]" where the instance can
        '''     be any alphanumeric string.  Generated control ID's all take the
        '''     form of "OBJECTINSTANCE".
        ''' </remarks>
        ''' <history>
        ''' 	[willhsc]	3/3/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class ObjectParser

            Private ReadOnly m_ControlList As New Hashtable
            Private ReadOnly m_InitMessages As String = ""
            Private m_RegisterList As New ArrayList
            Private m_ParseMessages As String = ""

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Registration directives generated as a result of the Parse method.
            ''' </summary>
            ''' <returns>ArrayList of formatted registration directives.</returns>
            ''' <remarks>
            '''     In addition to the updated file contents, the Parse method also
            '''     creates this list of formatted registration directives which can
            '''     be processed later.  They are not performed in place during the
            '''     Parse method in order to preserve the formatting of the input file
            '''     in case additional parsing might not anticipate the formatting of
            '''     those directives.  Since they are properly formatted, it is better
            '''     to exclude them from being subject to parsing.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Friend ReadOnly Property Registrations() As ArrayList
                Get
                    Return m_RegisterList
                End Get
            End Property

            Private ReadOnly Property Handler() As MatchEvaluator
                Get
                    Return AddressOf ObjectMatchHandler
                End Get
            End Property

            Private Property RegisterList() As ArrayList
                Get
                    Return m_RegisterList
                End Get
                Set(ByVal Value As ArrayList)
                    m_RegisterList = Value
                End Set
            End Property

            Private ReadOnly Property ControlList() As Hashtable
                Get
                    Return m_ControlList
                End Get
            End Property

            Private Property Messages() As String
                Get
                    Return m_ParseMessages
                End Get
                Set(ByVal Value As String)
                    m_ParseMessages = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     ControlParser class constructor.
            ''' </summary>
            ''' <remarks>
            '''     The constructor processes accepts a hashtable of skin objects to process against.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal ControlList As Hashtable)
                m_ControlList = DirectCast(ControlList.Clone, Hashtable)
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Perform parsing on the specified source file.
            ''' </summary>
            ''' <param name="Source">Pointer to Source string to be parsed.</param>
            ''' <remarks>
            '''     This procedure invokes a handler for each match of a formatted object.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function Parse(ByRef Source As String) As String
                Me.Messages = m_InitMessages

                ' clear register list
                Me.RegisterList.Clear()

                ' define the regular expression to match objects
                Dim FindObjectInstance As New Regex("\<object(?<token>.*?)</object>", RegexOptions.Singleline Or RegexOptions.IgnoreCase)

                ' parse the file
                Source = FindObjectInstance.Replace(Source, Me.Handler)

                Return Messages
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Process regular expression matches.
            ''' </summary>
            ''' <param name="m">Regular expression match for object which requires processing.</param>
            ''' <returns>Properly formatted token.</returns>
            ''' <remarks>
            '''     The handler is invoked by the Regex.Replace method once for each match that
            '''     it encounters.  The returned value of the handler is substituted for the
            '''     original match.  So the handler properly formats the replacement for the
            '''     object and returns it instead.  If an unknown object is encountered, the object
            '''     is unmodified.  This can happen if an object is a client-side object or 
            '''     has not yet been installed.
            ''' </remarks>
            ''' <history>
            ''' 	[willhsc]	3/3/2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function ObjectMatchHandler(ByVal m As Match) As String
                Dim OBJECT_PROC As String = Util.GetLocalizedString("ProcessObject")
                Dim OBJECT_SKIN As String = Util.GetLocalizedString("SkinObject")
                Dim OBJECT_PANE As String = Util.GetLocalizedString("PaneObject")
                Dim OBJECT_FOUND As String = Util.GetLocalizedString("ObjectFound")
                Dim CONTROL_FORMAT As String = Util.GetLocalizedString("ControlFormat")
                Dim OBJECT_NOTFOUND As String = Util.GetLocalizedString("ObjectNotFound")

                ' "token" string matches will be in the form of (" id=".." codetype=".." codebase=".." etc...><param name=".." value=".." />")
                ' we need to assume properly formatted HTML - attributes will be enclosed in double quotes and there will no spaces between assignments ( ie. attribute="value" )

                ' extract the embedded object attributes (" id=".." codetype=".." codebase=".." etc...")
                Dim EmbeddedObjectAttributes As String = m.Groups("token").Value.Substring(0, m.Groups("token").Value.IndexOf(">"))

                ' split into array
                Dim Attributes() As String = EmbeddedObjectAttributes.Split(" "c)

                ' declare skin object elements
                Dim AttributeNode As String = ""
                Dim Token As String = ""
                Dim ControlName As String = ""

                ' iterate and process valid attributes
                Dim Attribute() As String
                Dim AttributeName As String
                Dim AttributeValue As String
                For Each strAttribute As String In Attributes
                    If strAttribute <> String.Empty Then
                        Attribute = strAttribute.Split("="c)
                        AttributeName = Attribute(0).Trim
                        AttributeValue = Attribute(1).Trim.Replace("""", "")
                        Select Case AttributeName.ToLower()
                            Case "id"
                                ControlName = AttributeValue
                            Case "codetype"
                                AttributeNode = AttributeValue
                            Case "codebase"
                                Token = AttributeValue.ToUpper
                        End Select
                    End If
                Next

                ' process skin object
                If AttributeNode.ToLower = "dotnetnuke/server" Then
                    ' we have a valid skin object specification
                    Me.Messages += SkinController.FormatMessage(OBJECT_PROC, Token, 2, False)

                    ' if the embedded object is a recognized skin object
                    If Me.ControlList.ContainsKey(Token) = True Or Token = "CONTENTPANE" Then

                        Dim SkinControl As String = ""

                        If Me.ControlList.ContainsKey(Token) Then
                            Me.Messages += SkinController.FormatMessage(OBJECT_SKIN, DirectCast(Me.ControlList.Item(Token), String), 2, False)
                        Else
                            Me.Messages += SkinController.FormatMessage(OBJECT_PANE, Token, 2, False)
                        End If

                        ' process embedded object params
                        Dim Parameters As String = m.Groups("token").Value.Substring(m.Groups("token").Value.IndexOf(">") + 1)
                        Parameters = Parameters.Replace("<param name=""", "")
                        Parameters = Parameters.Replace(""" value", "")
                        Parameters = Parameters.Replace("/>", "")

                        ' convert multiple spaces and carriage returns into single spaces 
                        Parameters = Regex.Replace(Parameters, "\s+", " ")

                        If Me.ControlList.ContainsKey(Token) Then
                            ' create the skin object user control tag
                            SkinControl = "dnn:" & Token & " runat=""server"" "
                            If ControlName <> "" Then
                                SkinControl += "id=""" & ControlName & """ "
                            End If
                            SkinControl += Parameters

                            ' save control registration statement
                            Dim ControlRegistration As String = "<%@ Register TagPrefix=""dnn"" TagName=""" & Token & """ Src=""~/" & DirectCast(Me.ControlList.Item(Token), String) & """ %>" & vbCrLf
                            If RegisterList.Contains(ControlRegistration) = False Then
                                RegisterList.Add(ControlRegistration)
                            End If

                            ' return the control statement
                            Me.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" & SkinControl & " /&gt;", 2, False)

                            SkinControl = "<" & SkinControl & "/>"
                        Else
                            SkinControl = "div runat=""server"" "
                            If ControlName <> "" Then
                                SkinControl += "id=""" & ControlName & """ "
                            Else
                                SkinControl += "id=""ContentPane"" "
                            End If
                            SkinControl += Parameters & "></div"

                            ' return the control statement
                            Me.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" & SkinControl & "&gt;", 2, False)

                            SkinControl = "<" & SkinControl & ">"
                        End If

                        Return SkinControl
                    Else
                        ' return the unmodified embedded object
                        Me.Messages += SkinController.FormatMessage(OBJECT_NOTFOUND, Token, 2, False)
                        Return "<object" & m.Groups("token").Value & "</object>"
                    End If
                Else
                    ' return unmodified embedded object
                    Return "<object" & m.Groups("token").Value & "</object>"
                End If

            End Function

        End Class

#End Region

    End Class

End Namespace