/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;

namespace DotNetNuke.Services.Syndication 
{
        public delegate void InitEventHandler(object source, EventArgs e);
        public delegate void PreRenderEventHandler(object source, EventArgs e);

    /// <summary>
    /// Base class for RssHttpHandler - Generic handler and strongly typed ones are derived from it
    /// </summary>
    /// <typeparam name="RssChannelType"></typeparam>
    /// <typeparam name="RssItemType"></typeparam>
    /// <typeparam name="RssImageType"></typeparam>
    public abstract class RssHttpHandlerBase<RssChannelType, RssItemType, RssImageType> : IHttpHandler
        where RssChannelType : RssChannelBase<RssItemType, RssImageType>, new() 
        where RssItemType : RssElementBase, new() 
        where RssImageType : RssElementBase, new() 
    {

        RssChannelType _channel;
        HttpContext _context;

        public event InitEventHandler Init;
        public event PreRenderEventHandler PreRender;

        /// <summary>
        /// Triggers the Init event.
        /// </summary>
        protected virtual void OnInit(EventArgs ea)
        {
            if (Init != null)
                Init(this, ea);
        }

        /// <summary>
        /// Triggers the PreRender event.
        /// </summary>
        protected virtual void OnPreRender(EventArgs ea)
        {
            if (PreRender != null)
                PreRender(this, ea);
        }

        protected RssChannelType Channel 
        {
            get { return _channel; }
        }

        protected HttpContext Context 
        {
            get { return _context; }
        }

        private void InternalInit(HttpContext context)
        {
            _context = context;

            // create the channel
            _channel = new RssChannelType();
            _channel.SetDefaults();

            Context.Response.ContentType = "text/xml";
        }

        protected virtual void PopulateChannel(string channelName, string userName)
        {
        }

        protected virtual void Render(XmlTextWriter writer)
        {

            XmlDocument doc = _channel.SaveAsXml();
            doc.Save(writer);
        }

        void IHttpHandler.ProcessRequest(HttpContext context) 
        {
            InternalInit(context);

            // let inherited handlers setup any special handling
            OnInit(EventArgs.Empty);

            // parse the channel name and the user name from the query string
            string userName;
            string channelName;
            RssHttpHandlerHelper.ParseChannelQueryString(context.Request, out channelName, out userName);

            // populate items (call the derived class)
            PopulateChannel(channelName, userName);

            this.OnPreRender(EventArgs.Empty);

            Render(new XmlTextWriter(Context.Response.OutputStream, null));

        }

        bool IHttpHandler.IsReusable 
        {
            get 
            { 
                return false; 
            }
        }
    }
}
