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
Imports System.Collections
Imports System.IO
Imports System.Xml

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization


Namespace DotNetNuke.Services.Installer


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The XmlMerge class is a utility class for XmlSplicing config files
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	08/03/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class XmlMerge

#Region "Private Methods"

        Private _Sender As String
        Private _SourceConfig As XmlDocument
        Private _TargetConfig As XmlDocument
        Private _TargetFileName As String
        Private _Version As String

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the XmlMerge class.
        ''' </summary>
        ''' <param name="version"></param>
        ''' <param name="sender"></param>
        ''' <param name="sourceFileName"></param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal sourceFileName As String, ByVal version As String, ByVal sender As String)
            _Version = version
            _Sender = sender
            _SourceConfig = New XmlDocument
            _SourceConfig.Load(sourceFileName)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the XmlMerge class.
        ''' </summary>
        ''' <param name="version"></param>
        ''' <param name="sender"></param>
        ''' <param name="sourceStream"></param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal sourceStream As Stream, ByVal version As String, ByVal sender As String)
            _Version = version
            _Sender = sender
            _SourceConfig = New XmlDocument
            _SourceConfig.Load(sourceStream)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the XmlMerge class.
        ''' </summary>
        ''' <param name="version"></param>
        ''' <param name="sender"></param>
        ''' <param name="sourceReader"></param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal sourceReader As TextReader, ByVal version As String, ByVal sender As String)
            _Version = version
            _Sender = sender
            _SourceConfig = New XmlDocument
            _SourceConfig.Load(sourceReader)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the XmlMerge class.
        ''' </summary>
        ''' <param name="version"></param>
        ''' <param name="sender"></param>
        ''' <param name="sourceDoc"></param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal sourceDoc As XmlDocument, ByVal version As String, ByVal sender As String)
            _Version = version
            _Sender = sender
            _SourceConfig = sourceDoc
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Source for the Config file
        ''' </summary>
        ''' <value>An XmlDocument</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SourceConfig() As XmlDocument
            Get
                Return _SourceConfig
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Sender (source) of the changes to be merged
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Sender() As String
            Get
                Return _Sender
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Target Config file
        ''' </summary>
        ''' <value>An XmlDocument</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property TargetConfig() As XmlDocument
            Get
                Return _TargetConfig
            End Get
            Set(ByVal value As XmlDocument)
                _TargetConfig = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the File Name of the Target Config file
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TargetFileName() As String
            Get
                Return _TargetFileName
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Version of the changes to be merged
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Version() As String
            Get
                Return _Version
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub AddNode(ByVal rootNode As XmlNode, ByVal actionNode As XmlNode)
            For Each child As XmlNode In actionNode.ChildNodes
                If child.NodeType = XmlNodeType.Element OrElse child.NodeType = XmlNodeType.Comment Then
                    rootNode.AppendChild(TargetConfig.ImportNode(child, True))
                End If
            Next
        End Sub

        Private Sub InsertNode(ByVal childRootNode As XmlNode, ByVal actionNode As XmlNode, ByVal mode As NodeInsertType)
            Dim rootNode As XmlNode = childRootNode.ParentNode
            For Each child As XmlNode In actionNode.ChildNodes
                If child.NodeType = XmlNodeType.Element OrElse child.NodeType = XmlNodeType.Comment Then
                    Select Case mode
                        Case NodeInsertType.Before
                            rootNode.InsertBefore(TargetConfig.ImportNode(child, True), childRootNode)
                        Case NodeInsertType.After
                            rootNode.InsertAfter(TargetConfig.ImportNode(child, True), childRootNode)
                    End Select
                End If
            Next
        End Sub

        Private Sub ProcessNode(ByVal node As XmlNode)
            Dim rootNodePath As String = node.Attributes("path").Value
            Dim rootNode As XmlNode
            If node.Attributes("nameSpace") Is Nothing Then
                rootNode = TargetConfig.SelectSingleNode(rootNodePath)
            Else
                'Use Namespace Manager
                Dim xmlNameSpace As String = node.Attributes("nameSpace").Value
                Dim xmlNameSpacePrefix As String = node.Attributes("nameSpacePrefix").Value

                Dim nsmgr As XmlNamespaceManager = New XmlNamespaceManager(TargetConfig.NameTable)
                nsmgr.AddNamespace(xmlNameSpacePrefix, xmlNameSpace)
                rootNode = TargetConfig.SelectSingleNode(rootNodePath, nsmgr)
            End If

            If rootNode Is Nothing Then
                'TODO: what happens if Node can't be found
            End If

            Dim nodeAction As String = node.Attributes("action").Value

            Select Case nodeAction.ToLowerInvariant()
                Case "add"
                    AddNode(rootNode, node)
                Case "insertbefore"
                    InsertNode(rootNode, node, NodeInsertType.Before)
                Case "insertafter"
                    InsertNode(rootNode, node, NodeInsertType.After)
                Case "remove"
                    RemoveNode(rootNode)
                Case "removeattribute"
                    RemoveAttribute(rootNode, node)
                Case "update"
                    UpdateNode(rootNode, node)
                Case "updateattribute"
                    UpdateAttribute(rootNode, node)
            End Select

        End Sub

        Private Sub ProcessNodes(ByVal nodes As XmlNodeList, ByVal saveConfig As Boolean)
            'The nodes definition is not correct so skip changes
            If TargetConfig IsNot Nothing Then
                For Each node As XmlNode In nodes
                    ProcessNode(node)
                Next

                If saveConfig Then
                    Config.Save(TargetConfig, TargetFileName)
                End If
            End If
        End Sub

        Private Sub RemoveAttribute(ByVal rootNode As XmlNode, ByVal actionNode As XmlNode)
            Dim AttributeName As String = Null.NullString

            If actionNode.Attributes("name") IsNot Nothing Then
                AttributeName = actionNode.Attributes("name").Value
                If Not String.IsNullOrEmpty(AttributeName) Then
                    If rootNode.Attributes(AttributeName) IsNot Nothing Then
                        rootNode.Attributes.Remove(rootNode.Attributes(AttributeName))
                    End If
                End If
            End If
        End Sub

        Private Sub RemoveNode(ByVal node As XmlNode)
            'Get Parent
            Dim parentNode As XmlNode = node.ParentNode

            'Remove current Node
            parentNode.RemoveChild(node)

        End Sub

        Private Sub UpdateAttribute(ByVal rootNode As XmlNode, ByVal actionNode As XmlNode)
            Dim AttributeName As String = Null.NullString
            Dim AttributeValue As String = Null.NullString

            If actionNode.Attributes("name") IsNot Nothing AndAlso actionNode.Attributes("value") IsNot Nothing Then
                AttributeName = actionNode.Attributes("name").Value
                AttributeValue = actionNode.Attributes("value").Value
                If Not String.IsNullOrEmpty(AttributeName) Then
                    If rootNode.Attributes(AttributeName) Is Nothing Then
                        rootNode.Attributes.Append(TargetConfig.CreateAttribute(AttributeName))
                    End If
                    rootNode.Attributes(AttributeName).Value = AttributeValue
                End If
            End If
        End Sub

        Private Sub UpdateNode(ByVal rootNode As XmlNode, ByVal actionNode As XmlNode)
            Dim keyAttribute As String = Null.NullString
            Dim targetPath As String = Null.NullString
            If actionNode.Attributes("key") IsNot Nothing Then
                keyAttribute = actionNode.Attributes("key").Value
            End If

            For Each child As XmlNode In actionNode.ChildNodes
                If child.NodeType = XmlNodeType.Element Then
                    Dim targetNode As XmlNode = Nothing
                    If Not String.IsNullOrEmpty(keyAttribute) Then
                        If child.Attributes(keyAttribute) IsNot Nothing Then
                            Dim path As String = String.Format("{0}[@{1}='{2}']", child.LocalName, keyAttribute, child.Attributes(keyAttribute).Value)
                            targetNode = rootNode.SelectSingleNode(path)
                        End If
                    Else
                        If actionNode.Attributes("targetpath") IsNot Nothing Then
                            Dim path As String = actionNode.Attributes("targetpath").Value
                            'targetNode = rootNode.SelectSingleNode(path)

                            If actionNode.Attributes("nameSpace") Is Nothing Then
                                targetNode = rootNode.SelectSingleNode(path)
                            Else
                                'Use Namespace Manager
                                Dim xmlNameSpace As String = actionNode.Attributes("nameSpace").Value
                                Dim xmlNameSpacePrefix As String = actionNode.Attributes("nameSpacePrefix").Value

                                Dim nsmgr As XmlNamespaceManager = New XmlNamespaceManager(TargetConfig.NameTable)
                                nsmgr.AddNamespace(xmlNameSpacePrefix, xmlNameSpace)
                                targetNode = rootNode.SelectSingleNode(path, nsmgr)
                            End If

                        End If
                    End If

                    If targetNode Is Nothing Then
                        ' Since there is no collision we can just add the node
                        rootNode.AppendChild(TargetConfig.ImportNode(child, True))
                        Continue For
                    Else
                        ' There is a collision so we need to determine what to do.
                        Dim collisionAction As String = actionNode.Attributes("collision").Value
                        Select Case collisionAction.ToLowerInvariant()
                            Case "overwrite"
                                rootNode.RemoveChild(targetNode)
                                rootNode.AppendChild(TargetConfig.ImportNode(child, True))
                            Case "save"
                                Dim commentHeaderText As String = String.Format(Localization.Localization.GetString("XMLMERGE_Upgrade", Localization.Localization.SharedResourceFile), Environment.NewLine, Sender, Version, DateTime.Now)
                                Dim commentHeader As XmlComment = TargetConfig.CreateComment(commentHeaderText)
                                Dim commentNode As XmlComment = TargetConfig.CreateComment(targetNode.OuterXml)
                                rootNode.RemoveChild(targetNode)
                                rootNode.AppendChild(commentHeader)
                                rootNode.AppendChild(commentNode)
                                rootNode.AppendChild(TargetConfig.ImportNode(child, True))
                            Case "ignore"
                                'Do nothing
                        End Select
                    End If
                End If
            Next
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UpdateConfig method processes the source file and updates the Target
        ''' Config Xml Document.
        ''' </summary>
        ''' <param name="target">An Xml Document represent the Target Xml File</param>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateConfig(ByVal target As XmlDocument)
            TargetConfig = target

            If TargetConfig IsNot Nothing Then
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), False)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UpdateConfig method processes the source file and updates the Target
        ''' Config file.
        ''' </summary>
        ''' <param name="target">An Xml Document represent the Target Xml File</param>
        ''' <param name="fileName">The fileName for the Target Xml File - relative to the webroot</param>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateConfig(ByVal target As XmlDocument, ByVal fileName As String)
            _TargetFileName = fileName
            TargetConfig = target

            If TargetConfig IsNot Nothing Then
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UpdateConfigs method processes the source file and updates the various config 
        ''' files
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateConfigs()
            For Each configNode As XmlNode In SourceConfig.SelectNodes("/configuration/nodes")
                'Attempt to load TargetFile property from configFile Atribute
                _TargetFileName = configNode.Attributes("configfile").Value
                TargetConfig = Config.Load(TargetFileName)

                'The nodes definition is not correct so skip changes
                If TargetConfig IsNot Nothing Then
                    ProcessNodes(configNode.SelectNodes("node"), True)
                End If
            Next
        End Sub

#End Region

    End Class

End Namespace

