'
' DotNetNuke® - http:'www.dotnetnuke.com
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
Imports System.IO
Imports System.Web.Caching
Imports System.Xml.Serialization
Imports System.Xml.XPath
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Host
Imports System.Xml
Imports System.Collections

Namespace DotNetNuke.HttpModules.RequestFilter

    <Serializable(), XmlRoot("RewriterConfig")> _
    Public Class RequestFilterSettings

#Region "Constants"

        Private Const c_RequestFilterConfig As String = "RequestFilter.Config"

#End Region

        Private _Enabled As Boolean
        Public ReadOnly Property Enabled() As Boolean
            Get
                Return Host.EnableRequestFilters
            End Get
        End Property

        Private _Rules As List(Of RequestFilterRule) = New List(Of RequestFilterRule)()
        Public Property Rules() As List(Of RequestFilterRule)
            Get
                Return _Rules
            End Get
            Set(ByVal value As List(Of RequestFilterRule))
                _Rules = value
            End Set
        End Property

        ''' <summary>
        ''' Get the current settings from the xml config file
        ''' </summary>
        Public Shared Function GetSettings() As RequestFilterSettings
            Dim _Settings As RequestFilterSettings = CType(DataCache.GetCache(c_RequestFilterConfig), RequestFilterSettings)

            If _Settings Is Nothing Then
                _Settings = New RequestFilterSettings()

                Dim filePath As String = DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.DotNetNuke)

                'Create a FileStream for the Config file
                Dim filereader As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                Dim doc As New XPathDocument(fileReader)

                Dim ruleList As XPathNodeIterator = doc.CreateNavigator().Select("/configuration/blockrequests/rule")

                While ruleList.MoveNext()
                    Try
                        Dim serverVar As String = ruleList.Current.GetAttribute("servervar", String.Empty)
                        Dim values As String = ruleList.Current.GetAttribute("values", String.Empty)
                        Dim ac As RequestFilterRuleType = CType([Enum].Parse(GetType(RequestFilterRuleType), ruleList.Current.GetAttribute("action", String.Empty)), RequestFilterRuleType)
                        Dim op As RequestFilterOperatorType = CType([Enum].Parse(GetType(RequestFilterOperatorType), ruleList.Current.GetAttribute("operator", String.Empty)), RequestFilterOperatorType)
                        Dim location As String = ruleList.Current.GetAttribute("location", String.Empty)
                        Dim rule As New RequestFilterRule(serverVar, values, op, ac, location)
                        _Settings.Rules.Add(rule)
                    Catch ex As Exception
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(New Exception(String.Format("Unable to read RequestFilter Rule: {0}:", ruleList.Current.OuterXml), ex))
                    End Try
                End While

                If (File.Exists(filePath)) Then
                    'Set back into Cache
                    DataCache.SetCache(c_RequestFilterConfig, _Settings, New DNNCacheDependency(filePath))
                End If
            End If

            Return _Settings
        End Function

        Public Shared Sub Save(ByVal rules As List(Of RequestFilterRule))

            Dim filePath As String = DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.DotNetNuke)

            Dim doc As New XmlDocument()
            doc.Load(filePath)

            Dim ruleRoot As XmlNode = doc.SelectSingleNode("/configuration/blockrequests")
            ruleRoot.RemoveAll()

            For Each rule As RequestFilterRule In rules
                Dim xmlRule As XmlElement = doc.CreateElement("rule")

                Dim var As XmlAttribute = doc.CreateAttribute("servervar")
                var.Value = rule.ServerVariable
                xmlRule.Attributes.Append(var)

                Dim val As XmlAttribute = doc.CreateAttribute("values")
                Val.Value = rule.RawValue
                xmlRule.Attributes.Append(Val)

                Dim op As XmlAttribute = doc.CreateAttribute("operator")
                op.Value = rule.Operator.ToString()
                xmlRule.Attributes.Append(op)

                Dim action As XmlAttribute = doc.CreateAttribute("action")
                action.Value = rule.Action.ToString()
                xmlRule.Attributes.Append(action)

                Dim location As XmlAttribute = doc.CreateAttribute("location")
                location.Value = rule.Location
                xmlRule.Attributes.Append(location)

                ruleRoot.AppendChild(xmlRule)
            Next

            Dim settings As New XmlWriterSettings()
            settings.Indent = True

            Using writer As XmlWriter = XmlWriter.Create(filePath, settings)
                doc.WriteContentTo(writer)
            End Using
        End Sub

    End Class

End Namespace
