' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2009 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Namespace DotNetNuke.Modules.Dashboard.Components.Skins
    Public Class SkinInfo

#Region "Public Properties"

        Private _InUse As Boolean
        Public Property InUse() As Boolean
            Get
                Return _InUse
            End Get
            Set(ByVal value As Boolean)
                _InUse = value
            End Set
        End Property

        Private _SkinFile As String
        Public Property SkinFile() As String
            Get
                Return _SkinFile
            End Get
            Set(ByVal value As String)
                _SkinFile = value
            End Set
        End Property

        Private _SkinName As String
        Public Property SkinName() As String
            Get
                Return _SkinName
            End Get
            Set(ByVal value As String)
                _SkinName = value
            End Set
        End Property

#End Region

        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter)
            'Write start of main elemenst 
            writer.WriteStartElement("skin")

            writer.WriteElementString("skinName", SkinName)
            writer.WriteElementString("inUse", InUse.ToString())

            'Write end of Host Info 
            writer.WriteEndElement()
        End Sub
    End Class

End Namespace