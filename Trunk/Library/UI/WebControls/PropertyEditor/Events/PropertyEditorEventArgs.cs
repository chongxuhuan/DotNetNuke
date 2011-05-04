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

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      PropertyEditorEventArgs
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PropertyEditorEventArgs class is a cusom EventArgs class for
    /// handling Event Args from a change in value of a property.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	02/17/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PropertyEditorEventArgs : EventArgs
    {
        public PropertyEditorEventArgs(string name) : this(name, null, null)
        {
        }

        public PropertyEditorEventArgs(string name, object newValue, object oldValue)
        {
            Name = name;
            Value = newValue;
            OldValue = oldValue;
        }

        public bool Changed { get; set; }

        public int Index { get; set; }

        public object Key { get; set; }

        public string Name { get; set; }

        public object OldValue { get; set; }

        public string StringValue { get; set; }

        public object Value { get; set; }
    }
}
