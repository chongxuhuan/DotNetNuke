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

Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports System.Net
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The XmlUtils class provides Shared/Static methods for manipulating xml files
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	11/08/2004	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class XmlUtils

        Public Shared Sub AppendElement(ByRef objDoc As XmlDocument, ByVal objNode As XmlNode, ByVal attName As String, ByVal attValue As String, ByVal includeIfEmpty As Boolean)
            AppendElement(objDoc, objNode, attName, attValue, includeIfEmpty, False)
        End Sub

        Public Shared Sub AppendElement(ByRef objDoc As XmlDocument, ByVal objNode As XmlNode, ByVal attName As String, ByVal attValue As String, ByVal includeIfEmpty As Boolean, ByVal CDATA As Boolean)
            If attValue = "" And Not includeIfEmpty Then
                Exit Sub
            End If
            If CDATA Then
                objNode.AppendChild(CreateCDataElement(objDoc, attName, attValue))
            Else
                objNode.AppendChild(CreateElement(objDoc, attName, attValue))
            End If
        End Sub

        Public Shared Function CreateAttribute(ByVal objDoc As XmlDocument, ByVal attName As String, ByVal attValue As String) As XmlAttribute
            Dim attribute As XmlAttribute = objDoc.CreateAttribute(attName)
            attribute.Value = attValue
            Return attribute
        End Function

        Public Shared Sub CreateAttribute(ByVal objDoc As XmlDocument, ByVal objNode As XmlNode, ByVal attName As String, ByVal attValue As String)
            Dim attribute As XmlAttribute = objDoc.CreateAttribute(attName)
            attribute.Value = attValue
            objNode.Attributes.Append(attribute)
        End Sub

        <Obsolete("Removed in DotNetNuke 5.5")> _
        Public Shared Function CreateElement(ByVal objDoc As XmlDocument, ByVal NodeName As String) As XmlElement
            Return objDoc.CreateElement(NodeName)
        End Function

        Public Shared Function CreateElement(ByVal objDoc As XmlDocument, ByVal NodeName As String, ByVal NodeValue As String) As XmlElement
            Dim element As XmlElement = objDoc.CreateElement(NodeName)
            element.InnerText = NodeValue
            Return element
        End Function

        Public Shared Function CreateCDataElement(ByVal objDoc As XmlDocument, ByVal NodeName As String, ByVal NodeValue As String) As XmlElement
            Dim element As XmlElement = objDoc.CreateElement(NodeName)
            element.AppendChild(objDoc.CreateCDataSection(NodeValue))
            Return element
        End Function

        Public Shared Function Deserialize(ByVal xmlObject As String, ByVal type As System.Type) As Object
            Dim ser As XmlSerializer = New XmlSerializer(type)
            Dim sr As New StringReader(xmlObject)
            Dim obj As Object = ser.Deserialize(sr)
            sr.Close()
            Return obj
        End Function

        <Obsolete("Replaced in DotNetNuke 5.5 with CBO.DeserializeObject")> _
        Public Shared Function Deserialize(ByVal objStream As Stream, ByVal type As System.Type) As Object

            Dim obj As Object = Activator.CreateInstance(type)

            Dim tabDic As Dictionary(Of Integer, TabInfo) = TryCast(obj, Dictionary(Of Integer, TabInfo))
            If Not tabDic Is Nothing Then
                obj = DeSerializeDictionary(Of TabInfo)(objStream, "dictionary")
                Return obj
            End If
            Dim moduleDic As Dictionary(Of Integer, ModuleInfo) = TryCast(obj, Dictionary(Of Integer, ModuleInfo))
            If Not moduleDic Is Nothing Then
                obj = DeSerializeDictionary(Of ModuleInfo)(objStream, "dictionary")
                Return obj
            End If
            Dim tabPermDic As Dictionary(Of Integer, TabPermissionCollection) = TryCast(obj, Dictionary(Of Integer, TabPermissionCollection))
            If Not tabPermDic Is Nothing Then
                obj = DeSerializeDictionary(Of TabPermissionCollection)(objStream, "dictionary")
                Return obj
            End If
            Dim modPermDic As Dictionary(Of Integer, ModulePermissionCollection) = TryCast(obj, Dictionary(Of Integer, ModulePermissionCollection))
            If Not modPermDic Is Nothing Then
                obj = DeSerializeDictionary(Of ModulePermissionCollection)(objStream, "dictionary")
                Return obj
            End If

            Dim serializer As XmlSerializer = New XmlSerializer(type)
            Dim tr As TextReader = New StreamReader(objStream)
            obj = serializer.Deserialize(tr)
            tr.Close()

            Return obj
        End Function

        Public Shared Function DeSerializeDictionary(Of TValue)(ByVal objStream As Stream, ByVal rootname As String) As Dictionary(Of Integer, TValue)
            Dim objDictionary As Dictionary(Of Integer, TValue) = Nothing

            Dim xmlDoc As New XmlDocument
            xmlDoc.Load(objStream)

            objDictionary = New Dictionary(Of Integer, TValue)

            For Each xmlItem As XmlElement In xmlDoc.SelectNodes(rootname + "/item")
                Dim key As Integer = Convert.ToInt32(xmlItem.GetAttribute("key"))
                Dim typeName As String = xmlItem.GetAttribute("type")

                Dim objValue As TValue = Activator.CreateInstance(Of TValue)()

                'Create the XmlSerializer
                Dim xser As New XmlSerializer(objValue.GetType)

                'A reader is needed to read the XML document.
                Dim reader As New XmlTextReader(New StringReader(xmlItem.InnerXml))

                ' Use the Deserialize method to restore the object's state, and store it
                ' in the Hashtable
                objDictionary.Add(key, CType(xser.Deserialize(reader), TValue))
            Next

            Return objDictionary
        End Function

        Public Shared Function DeSerializeHashtable(ByVal xmlSource As String, ByVal rootname As String) As Hashtable
            Dim objHashTable As Hashtable
            If xmlSource <> "" Then
                objHashTable = New Hashtable

                Dim xmlDoc As New XmlDocument
                xmlDoc.LoadXml(xmlSource)

                For Each xmlItem As XmlElement In xmlDoc.SelectNodes(rootname + "/item")
                    Dim key As String = xmlItem.GetAttribute("key")
                    Dim typeName As String = xmlItem.GetAttribute("type")

                    'Create the XmlSerializer
                    Dim xser As New XmlSerializer(Type.GetType(typeName))

                    'A reader is needed to read the XML document.
                    Dim reader As New XmlTextReader(New StringReader(xmlItem.InnerXml))

                    ' Use the Deserialize method to restore the object's state, and store it
                    ' in the Hashtable
                    objHashTable.Add(key, xser.Deserialize(reader))
                Next
            Else
                objHashTable = New Hashtable
            End If
            Return objHashTable
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of an attribute
        ''' </summary>
        ''' <param name="nav">Parent XPathNavigator</param>
        ''' <param name="AttributeName">Thename of the Attribute</param>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	05/14/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAttributeValue(ByVal nav As XPathNavigator, ByVal AttributeName As String) As String
            Return nav.GetAttribute(AttributeName, "")
        End Function

        Public Shared Function GetAttributeValueAsInteger(ByVal nav As XPathNavigator, ByVal AttributeName As String, ByVal DefaultValue As Integer) As Integer
            Dim intValue As Integer = DefaultValue

            Dim strValue As String = GetAttributeValue(nav, AttributeName)
            If Not String.IsNullOrEmpty(strValue) Then
                intValue = Convert.ToInt32(strValue)
            End If

            Return intValue
        End Function

        Public Shared Function GetAttributeValueAsLong(ByVal nav As XPathNavigator, ByVal AttributeName As String, ByVal DefaultValue As Long) As Long
            Dim intValue As Long = DefaultValue

            Dim strValue As String = GetAttributeValue(nav, AttributeName)
            If Not String.IsNullOrEmpty(strValue) Then
                intValue = Convert.ToInt64(strValue)
            End If

            Return intValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of a node
        ''' </summary>
        ''' <param name="nav">Parent XPathNavigator</param>
        ''' <param name="path">The Xpath expression to the value</param>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValue(ByVal nav As XPathNavigator, ByVal path As String) As String
            Dim strValue As String = Null.NullString

            Dim elementNav As XPathNavigator = nav.SelectSingleNode(path)
            If elementNav IsNot Nothing Then
                strValue = elementNav.Value
            End If

            Return strValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <param name="DefaultValue">Default value to return</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Created
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValue(ByVal objNode As XmlNode, ByVal NodeName As String, Optional ByVal DefaultValue As String = "") As String
            Dim strValue As String = DefaultValue

            If (objNode.Item(NodeName) IsNot Nothing) Then
                strValue = objNode.Item(NodeName).InnerText

                If strValue = "" And DefaultValue <> "" Then
                    strValue = DefaultValue
                End If
            End If

            Return strValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value (False) will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueBoolean(ByVal objNode As XmlNode, ByVal NodeName As String) As Boolean
            Return GetNodeValueBoolean(objNode, NodeName, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <param name="DefaultValue">Default value to return</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueBoolean(ByVal objNode As XmlNode, ByVal NodeName As String, ByVal DefaultValue As Boolean) As Boolean
            Dim bValue As Boolean = DefaultValue

            If (objNode.Item(NodeName) IsNot Nothing) Then
                Dim strValue As String = objNode.Item(NodeName).InnerText

                If Not String.IsNullOrEmpty(strValue) Then
                    bValue = Convert.ToBoolean(strValue)
                End If
            End If

            Return bValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <param name="DefaultValue">Default value to return</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueDate(ByVal objNode As XmlNode, ByVal NodeName As String, ByVal DefaultValue As DateTime) As DateTime
            Dim dateValue As DateTime = DefaultValue

            If (objNode.Item(NodeName) IsNot Nothing) Then
                Dim strValue As String = objNode.Item(NodeName).InnerText

                If Not String.IsNullOrEmpty(strValue) Then

                    dateValue = Convert.ToDateTime(strValue)
                    If dateValue.Date.Equals(Null.NullDate.Date) Then
                        dateValue = Null.NullDate
                    End If
                End If
            End If

            Return dateValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value (0) will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueInt(ByVal objNode As XmlNode, ByVal NodeName As String) As Integer
            Return GetNodeValueInt(objNode, NodeName, 0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <param name="DefaultValue">Default value to return</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueInt(ByVal objNode As XmlNode, ByVal NodeName As String, ByVal DefaultValue As Integer) As Integer
            Dim intValue As Integer = DefaultValue

            If (objNode.Item(NodeName) IsNot Nothing) Then
                Dim strValue As String = objNode.Item(NodeName).InnerText

                If Not String.IsNullOrEmpty(strValue) Then
                    intValue = Convert.ToInt32(strValue)
                End If
            End If

            Return intValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value (0) will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueSingle(ByVal objNode As XmlNode, ByVal NodeName As String) As Single
            Return GetNodeValueSingle(objNode, NodeName, 0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of node
        ''' </summary>
        ''' <param name="objNode">Parent node</param>
        ''' <param name="NodeName">Child node to look for</param>
        ''' <param name="DefaultValue">Default value to return</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If the node does not exist or it causes any error the default value will be returned.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	09/09/2004	Added new method to return converted values
        ''' 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNodeValueSingle(ByVal objNode As XmlNode, ByVal NodeName As String, ByVal DefaultValue As Single) As Single
            Dim sValue As Single = DefaultValue

            If (objNode.Item(NodeName) IsNot Nothing) Then
                Dim strValue As String = objNode.Item(NodeName).InnerText

                If Not String.IsNullOrEmpty(strValue) Then
                    sValue = Convert.ToSingle(strValue, CultureInfo.InvariantCulture)
                End If
            End If

            Return sValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlWriterSettings object
        ''' </summary>
        ''' <param name="conformance">Conformance Level</param>
        ''' <returns>An XmlWriterSettings</returns>
        ''' <history>
        ''' 	[cnurse]	08/22/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetXmlWriterSettings(ByVal conformance As ConformanceLevel) As XmlWriterSettings
            Dim settings As New XmlWriterSettings()
            settings.ConformanceLevel = conformance
            settings.OmitXmlDeclaration = True
            settings.Indent = True

            Return settings
        End Function

        Public Shared Function SerializeDictionary(ByVal Source As IDictionary, ByVal rootName As String) As String
            Dim strString As String
            If Source.Count <> 0 Then
                Dim xser As XmlSerializer
                Dim sw As StringWriter

                Dim xmlDoc As New XmlDocument
                Dim xmlRoot As XmlElement = xmlDoc.CreateElement(rootName)
                xmlDoc.AppendChild(xmlRoot)

                For Each key As Object In Source.Keys
                    'Create the item Node
                    Dim xmlItem As XmlElement = xmlDoc.CreateElement("item")

                    'Save the key name and the object type
                    xmlItem.SetAttribute("key", Convert.ToString(key))
                    xmlItem.SetAttribute("type", Source(key).GetType.AssemblyQualifiedName.ToString)

                    'Serialize the object
                    Dim xmlObject As New XmlDocument
                    xser = New XmlSerializer(Source(key).GetType)
                    sw = New StringWriter
                    xser.Serialize(sw, Source(key))
                    xmlObject.LoadXml(sw.ToString)

                    'import and append the node to the root
                    xmlItem.AppendChild(xmlDoc.ImportNode(xmlObject.DocumentElement, True))
                    xmlRoot.AppendChild(xmlItem)
                Next

                'Return the OuterXml of the profile
                strString = xmlDoc.OuterXml
            Else
                strString = ""
            End If
            Return strString
        End Function

        Public Shared Sub SerializeHashtable(ByVal Hashtable As Hashtable, ByVal XmlDocument As XmlDocument, ByVal RootNode As XmlNode, ByVal ElementName As String, ByVal KeyField As String, ByVal ValueField As String)
            Dim sKey As String
            Dim sOuterElementName As String
            Dim sInnerElementName As String
            Dim nodeSetting, nodeSettings, nodeSettingName, nodeSettingValue As XmlNode

            sOuterElementName = ElementName & "s"
            sInnerElementName = ElementName

            nodeSettings = RootNode.AppendChild(XmlDocument.CreateElement(sOuterElementName))
            For Each sKey In Hashtable.Keys
                nodeSetting = nodeSettings.AppendChild(XmlDocument.CreateElement(sInnerElementName))
                nodeSettingName = nodeSetting.AppendChild(XmlDocument.CreateElement(KeyField))
                nodeSettingName.InnerText = sKey
                nodeSettingValue = nodeSetting.AppendChild(XmlDocument.CreateElement(ValueField))
                nodeSettingValue.InnerText = Hashtable(sKey).ToString
            Next
        End Sub

        Public Shared Sub UpdateAttribute(ByVal node As XmlNode, ByVal attName As String, ByVal attValue As String)
            If Not node Is Nothing Then
                Dim attrib As XmlAttribute = node.Attributes(attName)
                attrib.InnerText = attValue
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Xml Encodes HTML
        ''' </summary>
        ''' <param name="HTML">The HTML to encode</param>
        ''' <returns></returns>
        ''' <history>
        '''		[cnurse]	09/29/2005	moved from Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function XMLEncode(ByVal HTML As String) As String
            Return "<![CDATA[" & HTML & "]]>"
        End Function

        Public Shared Sub XSLTransform(ByVal doc As XmlDocument, ByRef writer As StreamWriter, ByVal xsltUrl As String)

            Dim xslt As Xsl.XslCompiledTransform = New Xsl.XslCompiledTransform
            xslt.Load(xsltUrl)
            'Transform the file.
            xslt.Transform(doc, Nothing, writer)
        End Sub

        Public Shared Function Serialize(ByVal obj As Object) As String

            Dim xmlObject As String
            Dim dic As IDictionary = TryCast(obj, IDictionary)
            If Not dic Is Nothing Then
                xmlObject = SerializeDictionary(dic, "dictionary")
            Else
                Dim xmlDoc As New XmlDocument
                Dim xser As XmlSerializer = New XmlSerializer(obj.GetType)
                Dim sw As StringWriter = New StringWriter

                xser.Serialize(sw, obj)

                xmlDoc.LoadXml(sw.GetStringBuilder().ToString())
                Dim xmlDocEl As XmlNode = xmlDoc.DocumentElement
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes.ItemOf("xmlns:xsd"))
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes.ItemOf("xmlns:xsi"))

                xmlObject = xmlDocEl.OuterXml
            End If
            Return xmlObject
        End Function

        <Obsolete("This method is obsolete.")> _
        Public Shared Function GetXMLContent(ByVal ContentUrl As String) As XmlDocument
            'This function reads an Xml document via a Url and returns it as an XmlDocument object

            GetXMLContent = New XmlDocument
            Dim req As WebRequest = WebRequest.Create(ContentUrl)
            Dim result As WebResponse = req.GetResponse()
            Dim ReceiveStream As Stream = result.GetResponseStream()
            Dim objXmlReader As XmlReader = New XmlTextReader(result.GetResponseStream())
            GetXMLContent.Load(objXmlReader)

        End Function

    End Class

End Namespace
