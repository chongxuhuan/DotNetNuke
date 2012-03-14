using System;
using System.Collections;
using System.Web;

namespace DotNetNuke.HttpModules.Services.Internal
{
    public class ServicesContextWrapper : IServicesContext
    {
        private const string DnnReal401 = "DnnReal401";
        private const string DnnDigestNonceIsStale = "DnnDigestNonceIsStale";
        private readonly IDictionary _items;
        private readonly HttpContextBase _context;

        public ServicesContextWrapper(HttpContext context) : this (new HttpContextWrapper(context)) {}

        public ServicesContextWrapper(HttpContextBase context)
        {
            _context = context;
            _items = context.Items;
        }

        public bool DoA401
        {
            get { return _items.Contains(DnnReal401); }
            set
            {
                if (value)
                {
                    _items[DnnReal401] = true;
                }
                else
                {
                    _items.Remove(DnnReal401);
                }
            }
        }

        public bool SupportBasicAuth
        {
            get { return !IsXHRequest(); }
        }

        public bool SupportDigestAuth
        {
            get { return !IsXHRequest(); }
        }

        private bool IsXHRequest()
        {
            string header = _context.Request.Headers.Get("X-REQUESTED-WITH");
            return !String.IsNullOrEmpty(header) &&
                   header.Equals("XmlHttpRequest", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsStale
        {
            get { return _items.Contains(DnnDigestNonceIsStale); }
            set
            {
                if(value)
                {
                    _items[DnnDigestNonceIsStale] = true;
                }
                else
                {
                    _items.Remove(DnnDigestNonceIsStale);
                }
            }
        }

        public HttpContextBase BaseContext
        {
            get { return _context; }
        }
    }
}