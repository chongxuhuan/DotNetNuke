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

using System.Collections;

using DotNetNuke.Security;

#endregion

namespace DotNetNuke.Entities.Modules.Actions
{
    ///-----------------------------------------------------------------------------
    /// Project		: DotNetNuke
    /// Class		: ModuleActionCollection
    ///-----------------------------------------------------------------------------
    /// <summary>
    /// Represents a collection of <see cref="T:DotNetNuke.ModuleAction" /> objects.
    /// </summary>
    /// <remarks>The ModuleActionCollection is a custom collection of ModuleActions.
    /// Each ModuleAction in the collection has it's own <see cref="P:DotNetNuke.ModuleAction.Actions" />
    ///  collection which provides the ability to create a heirarchy of ModuleActions.</remarks>
    /// <history>
    /// 	[Joe] 	10/9/2003	Created
    /// </history>
    ///-----------------------------------------------------------------------------
    public class ModuleActionCollection : CollectionBase
    {
        public ModuleActionCollection()
        {
        }

        public ModuleActionCollection(ModuleActionCollection value)
        {
            AddRange(value);
        }

        public ModuleActionCollection(ModuleAction[] value)
        {
            AddRange(value);
        }

        public ModuleAction this[int index]
        {
            get
            {
                return (ModuleAction) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(ModuleAction value)
        {
            return List.Add(value);
        }

        public ModuleAction Add(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible, bool NewWindow)
        {
            return Add(ID, Title, CmdName, CmdArg, Icon, Url, "", UseActionEvent, Secure, Visible, NewWindow);
        }

        public ModuleAction Add(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible,
                                bool NewWindow)
        {
            var ModAction = new ModuleAction(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible, NewWindow);
            Add(ModAction);
            return ModAction;
        }

        public void AddRange(ModuleAction[] value)
        {
            int i;
            for (i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        public void AddRange(ModuleActionCollection value)
        {
            foreach (ModuleAction mA in value)
            {
                Add(mA);
            }
        }

        public bool Contains(ModuleAction value)
        {
            return List.Contains(value);
        }

        public ModuleAction GetActionByCommandName(string name)
        {
            ModuleAction retAction = null;
            foreach (ModuleAction modAction in List)
            {
                if (modAction.CommandName == name)
                {
                    retAction = modAction;
                    break;
                }
                if (modAction.HasChildren())
                {
                    ModuleAction childAction = modAction.Actions.GetActionByCommandName(name);
                    if (childAction != null)
                    {
                        retAction = childAction;
                        break;
                    }
                }
            }
            return retAction;
        }

        public ModuleActionCollection GetActionsByCommandName(string name)
        {
            var retActions = new ModuleActionCollection();
            foreach (ModuleAction modAction in List)
            {
                if (modAction.CommandName == name)
                {
                    retActions.Add(modAction);
                }
                if (modAction.HasChildren())
                {
                    retActions.AddRange(modAction.Actions.GetActionsByCommandName(name));
                }
            }
            return retActions;
        }

        public ModuleAction GetActionByID(int id)
        {
            ModuleAction retAction = null;
            foreach (ModuleAction modAction in List)
            {
                if (modAction.ID == id)
                {
                    retAction = modAction;
                    break;
                }
                if (modAction.HasChildren())
                {
                    ModuleAction childAction = modAction.Actions.GetActionByID(id);
                    if (childAction != null)
                    {
                        retAction = childAction;
                        break;
                    }
                }
            }
            return retAction;
        }

        public int IndexOf(ModuleAction value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ModuleAction value)
        {
            List.Insert(index, value);
        }

        public void Remove(ModuleAction value)
        {
            List.Remove(value);
        }
    }
}
