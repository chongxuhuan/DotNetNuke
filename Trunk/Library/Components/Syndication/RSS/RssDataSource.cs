/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Services.Syndication 
{
    /// <summary>
    /// RSS data source control implementation, including the designer
    /// </summary>
    [DefaultProperty("Url")]
    public class RssDataSource : DataSourceControl 
    {
        int _maxItems;
        string _url;
        RssDataSourceView _itemsView;
        GenericRssChannel _channel;

        public RssDataSource() 
        {
        }

        protected override DataSourceView GetView(string viewName) 
        {
            if (_itemsView == null) 
            {
                _itemsView = new RssDataSourceView(this, viewName);
            }

            return _itemsView;
        }

        public GenericRssChannel Channel  
        {
            get 
            {
                if (_channel == null) 
                {
                    if (string.IsNullOrEmpty(_url)) 
                    {
                        _channel = new GenericRssChannel();
                    }
                    else 
                    {
                        _channel = GenericRssChannel.LoadChannel(_url);
                    }
                }

                return _channel;
            }
        }

        public int MaxItems 
        {
            get 
            { 
                return _maxItems; 
            }
            set 
            { 
                _maxItems = value; 
            }
        }

        public string Url 
        {
            get 
            { 
                return _url; 
            }

            set 
            {
                _channel = null;
                _url = value;
            }
        }
    }

    public class RssDataSourceView : DataSourceView 
    {
        RssDataSource _owner;

        internal RssDataSourceView(RssDataSource owner, string viewName) : base(owner, viewName) 
        {
            _owner = owner;
        }

        public override void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback) 
        {
            callback(ExecuteSelect(arguments));
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) 
        {
            return _owner.Channel.SelectItems(_owner.MaxItems);
        }
    }
}
