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

Namespace DotNetNuke.UI.WebControls

    Public Class NavDataPageHierarchyData
        Implements IHierarchyData, INavigateUIData

        Private m_objNode As DNNNode = Nothing

        Public Sub New(ByVal obj As DNNNode)
            m_objNode = obj
        End Sub 'New

        Public Overrides Function ToString() As String
            Return m_objNode.Text
        End Function 'ToString

        ' IHierarchyData implementation.
        ''' <summary>
        ''' Indicates whether the hierarchical data node that the IHierarchyData object represents has any child nodes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property HasChildren() As Boolean _
         Implements IHierarchyData.HasChildren
            Get
                Return m_objNode.HasNodes
            End Get
        End Property

        ''' <summary>
        ''' Gets the hierarchical path of the node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Path() As String _
         Implements IHierarchyData.Path
            Get
                Return GetValuePath(m_objNode) '.Key
            End Get
        End Property

        ''' <summary>
        ''' Gets the hierarchical data node that the IHierarchyData object represents.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Item() As Object _
         Implements IHierarchyData.Item
            Get
                Return m_objNode
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the type of Object contained in the Item property.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Type() As String _
         Implements IHierarchyData.Type
            Get
                Return "NavDataPageHierarchyData"
            End Get
        End Property

        ''' <summary>
        ''' Gets an enumeration object that represents all the child nodes of the current hierarchical node. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetChildren() As IHierarchicalEnumerable _
         Implements IHierarchyData.GetChildren
            Dim objNodes As New NavDataPageHierarchicalEnumerable()

            If Not m_objNode Is Nothing Then
                Dim objNode As DNNNode
                For Each objNode In m_objNode.DNNNodes
                    objNodes.Add(New NavDataPageHierarchyData(objNode))
                Next
            End If
            Return objNodes
        End Function 'GetChildren

        ''' <summary>
        ''' Gets an enumeration object that represents the parent node of the current hierarchical node. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetParent() As IHierarchyData _
         Implements IHierarchyData.GetParent
            If Not m_objNode Is Nothing Then
                Return New NavDataPageHierarchyData(m_objNode.ParentNode)
            Else
                Return Nothing
            End If
        End Function 'GetParent

        ''' <summary>
        ''' Returns node name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Name() As String Implements INavigateUIData.Name
            Get
                Return GetSafeValue(m_objNode.Text, "")
            End Get
        End Property

        ''' <summary>
        ''' Returns value path of node
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Value() As String Implements INavigateUIData.Value
            Get
                Return GetValuePath(m_objNode)
            End Get
        End Property

        ''' <summary>
        ''' Returns nodes image
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ImageUrl() As String
            Get
                If Len(m_objNode.Image) = 0 OrElse m_objNode.Image.StartsWith("/") Then
                    Return m_objNode.Image
                Else
                    Return DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.HomeDirectory & m_objNode.Image
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns node navigation url
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property NavigateUrl() As String Implements INavigateUIData.NavigateUrl
            Get
                Return GetSafeValue(m_objNode.NavigateURL, "")
            End Get
        End Property

        ''' <summary>
        ''' Returns Node description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Description() As String Implements INavigateUIData.Description
            Get
                Return GetSafeValue(m_objNode.ToolTip, "")
            End Get
        End Property

        ''' <summary>
        ''' Helper function to handle cases where property is null (Nothing)
        ''' </summary>
        ''' <param name="Value">Value to evaluate for null</param>
        ''' <param name="Def">If null, return this default</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetSafeValue(ByVal Value As String, ByVal Def As String) As String
            If Not Value Is Nothing Then
                Return Value
            Else
                Return Def
            End If
        End Function

        ''' <summary>
        ''' Computes valuepath necessary for ASP.NET controls to guarantee uniqueness
        ''' </summary>
        ''' <param name="objNode"></param>
        ''' <returns>ValuePath</returns>
        ''' <remarks>Not sure if it is ok to hardcode the "\" separator, but also not sure where I would get it from</remarks>
        Private Function GetValuePath(ByVal objNode As DNNNode) As String
            Dim objParent As DNNNode = objNode.ParentNode
            Dim strPath As String = GetSafeValue(objNode.Key, "")
            Do
                If objParent Is Nothing OrElse objParent.Level = -1 Then Exit Do
                strPath = GetSafeValue(objParent.Key, "") & "\" & strPath
                objParent = objParent.ParentNode
            Loop
            Return strPath
        End Function

    End Class 'NavDataPageHierarchyData

End Namespace
