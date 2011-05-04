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

using DotNetNuke.Security;

#endregion

namespace DotNetNuke.Entities.Modules.Actions
{
    ///-----------------------------------------------------------------------------
    /// Project		: DotNetNuke
    /// Class		: ModuleAction
    ///-----------------------------------------------------------------------------
    /// <summary>
    /// Each Module Action represents a separate functional action as defined by the
    /// associated module.
    /// </summary>
    /// <remarks>A module action is used to define a specific function for a given module.
    /// Each module can define one or more actions which the portal will present to the
    /// user.  These actions may be presented as a menu, a dropdown list or even a group
    /// of linkbuttons.
    /// <seealso cref="T:DotNetNuke.ModuleActionCollection" /></remarks>
    /// <history>
    /// 	[Joe] 	10/9/2003	Created
    /// </history>
    ///-----------------------------------------------------------------------------
    public class ModuleAction
    {
        public ModuleAction(int id) : this(id, "", "", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName) : this(id, title, cmdName, "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg) : this(id, title, cmdName, cmdArg, "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon) : this(id, title, cmdName, cmdArg, icon, "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url)
            : this(id, title, cmdName, cmdArg, icon, url, "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url, string clientScript)
            : this(id, title, cmdName, cmdArg, icon, url, clientScript, false, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url, string clientScript, bool useActionEvent)
            : this(id, title, cmdName, cmdArg, icon, url, clientScript, useActionEvent, SecurityAccessLevel.Anonymous, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url, string clientScript, bool useActionEvent, SecurityAccessLevel secure)
            : this(id, title, cmdName, cmdArg, icon, url, clientScript, useActionEvent, secure, true, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url, string clientScript, bool useActionEvent, SecurityAccessLevel secure, bool visible)
            : this(id, title, cmdName, cmdArg, icon, url, clientScript, useActionEvent, secure, visible, false)
        {
        }

        public ModuleAction(int id, string title, string cmdName, string cmdArg, string icon, string url, string clientScript, bool useActionEvent, SecurityAccessLevel secure, bool visible,
                            bool newWindow)
        {
            ID = id;
            Title = title;
            CommandName = cmdName;
            CommandArgument = cmdArg;
            Icon = icon;
            Url = url;
            ClientScript = clientScript;
            UseActionEvent = useActionEvent;
            Secure = secure;
            Visible = visible;
            NewWindow = newWindow;
            Actions = new ModuleActionCollection();
        }

        public ModuleActionCollection Actions { get; set; }

        public int ID { get; set; }

        public bool Visible { get; set; }

        public SecurityAccessLevel Secure { get; set; }

        public string CommandName { get; set; }

        public string CommandArgument { get; set; }

        internal string ControlKey
        {
            get
            {
                string controlKey = String.Empty;
                if (!String.IsNullOrEmpty(Url))
                {
                    int startIndex = Url.IndexOf("/ctl/");
                    int endIndex = -1;
                    if (startIndex > -1)
                    {
                        startIndex += 4;
                        endIndex = Url.IndexOf("/", startIndex + 1);
                    }
                    else
                    {
                        startIndex = Url.IndexOf("ctl=");
                        if (startIndex > -1)
                        {
                            startIndex += 4;
                            endIndex = Url.IndexOf("&", startIndex + 1);
                        }
                    }
                    if (startIndex > -1)
                    {
                        controlKey = endIndex > -1 ? Url.Substring(startIndex + 1, endIndex - startIndex - 1) : Url.Substring(startIndex + 1);
                    }
                }
                return controlKey;
            }
        }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public string ClientScript { get; set; }

        public bool UseActionEvent { get; set; }

        public bool NewWindow { get; set; }

        public bool HasChildren()
        {
            return (Actions.Count > 0);
        }
    }
}
