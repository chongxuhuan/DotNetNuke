#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.Web.UI;

using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.UI.WebControls
{
    public class NavDataPageHierarchyData : IHierarchyData, INavigateUIData
    {
        private readonly DNNNode m_objNode;

        public NavDataPageHierarchyData(DNNNode obj)
        {
            m_objNode = obj;
        }

        public virtual string ImageUrl
        {
            get
            {
                if (String.IsNullOrEmpty(m_objNode.Image) || m_objNode.Image.StartsWith("/"))
                {
                    return m_objNode.Image;
                }
                else
                {
                    return PortalController.GetCurrentPortalSettings().HomeDirectory + m_objNode.Image;
                }
            }
        }

        #region IHierarchyData Members

        /// <summary>
        /// Indicates whether the hierarchical data node that the IHierarchyData object represents has any child nodes.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool HasChildren
        {
            get
            {
                return m_objNode.HasNodes;
            }
        }

        /// <summary>
        /// Gets the hierarchical path of the node.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Path
        {
            get
            {
                return GetValuePath(m_objNode);
            }
        }

        /// <summary>
        /// Gets the hierarchical data node that the IHierarchyData object represents.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual object Item
        {
            get
            {
                return m_objNode;
            }
        }

        /// <summary>
        /// Gets the name of the type of Object contained in the Item property.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Type
        {
            get
            {
                return "NavDataPageHierarchyData";
            }
        }

        /// <summary>
        /// Gets an enumeration object that represents all the child nodes of the current hierarchical node.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual IHierarchicalEnumerable GetChildren()
        {
            var objNodes = new NavDataPageHierarchicalEnumerable();
            if (m_objNode != null)
            {
                foreach (DNNNode objNode in m_objNode.DNNNodes)
                {
                    objNodes.Add(new NavDataPageHierarchyData(objNode));
                }
            }
            return objNodes;
        }

        /// <summary>
        /// Gets an enumeration object that represents the parent node of the current hierarchical node.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual IHierarchyData GetParent()
        {
            if (m_objNode != null)
            {
                return new NavDataPageHierarchyData(m_objNode.ParentNode);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region INavigateUIData Members

        /// <summary>
        /// Returns node name
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Name
        {
            get
            {
                return GetSafeValue(m_objNode.Text, "");
            }
        }

        /// <summary>
        /// Returns value path of node
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Value
        {
            get
            {
                return GetValuePath(m_objNode);
            }
        }

        public virtual string NavigateUrl
        {
            get
            {
                return GetSafeValue(m_objNode.NavigateURL, "");
            }
        }

        public virtual string Description
        {
            get
            {
                return GetSafeValue(m_objNode.ToolTip, "");
            }
        }

        #endregion

        public override string ToString()
        {
            return m_objNode.Text;
        }

        private string GetSafeValue(string Value, string Def)
        {
            if (Value != null)
            {
                return Value;
            }
            else
            {
                return Def;
            }
        }

        private string GetValuePath(DNNNode objNode)
        {
            DNNNode objParent = objNode.ParentNode;
            string strPath = GetSafeValue(objNode.Key, "");
            do
            {
                if (objParent == null || objParent.Level == -1)
                {
                    break;
                }
                strPath = GetSafeValue(objParent.Key, "") + "\\" + strPath;
                objParent = objParent.ParentNode;
            } while (true);
            return strPath;
        }
    }
}
