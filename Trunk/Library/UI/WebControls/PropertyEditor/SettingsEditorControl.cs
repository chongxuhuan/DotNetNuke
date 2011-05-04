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
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:SettingsEditorControl runat=server></{0}:SettingsEditorControl>")]
    public class SettingsEditorControl : PropertyEditorControl
    {
        private IEnumerable _UnderlyingDataSource;

        protected override IEnumerable UnderlyingDataSource
        {
            get
            {
                if (_UnderlyingDataSource == null)
                {
                    _UnderlyingDataSource = GetSettings();
                }
                return _UnderlyingDataSource;
            }
        }

        [Browsable(false)]
        public Hashtable CustomEditors { get; set; }

        public Hashtable Visibility { get; set; }

        private ArrayList GetSettings()
        {
            var settings = (Hashtable) DataSource;
            var arrSettings = new ArrayList();
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                var info = new SettingInfo(settingsEnumerator.Key, settingsEnumerator.Value);
                if ((CustomEditors != null) && (CustomEditors[settingsEnumerator.Key] != null))
                {
                    info.Editor = Convert.ToString(CustomEditors[settingsEnumerator.Key]);
                }
                arrSettings.Add(info);
            }
            arrSettings.Sort(new SettingNameComparer());
            return arrSettings;
        }

        protected override void AddEditorRow(Table table, object obj)
        {
            var info = (SettingInfo) obj;
            AddEditorRow(table, info.Name, new SettingsEditorInfoAdapter(DataSource, obj, ID));
        }
        protected override void AddEditorRow(object obj)
        {
            var info = (SettingInfo)obj; 
            AddEditorRow(this, info.Name, new SettingsEditorInfoAdapter(DataSource, obj, ID));
        }
        protected override bool GetRowVisibility(object obj)
        {
            var info = (SettingInfo) obj;
            bool _IsVisible = true;
            if ((Visibility != null) && (Visibility[info.Name] != null))
            {
                _IsVisible = Convert.ToBoolean(Visibility[info.Name]);
            }
            return _IsVisible;
        }
    }
}
