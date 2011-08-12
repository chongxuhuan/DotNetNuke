using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Services.RequestProfile
{
    public class ClientCapabilityProfile: IRequestProfile
    {
        public ClientCapabilityProfile(IRequestProfile reqProfile)
        {
            this._CapabilityProfile = reqProfile;
        }
        private IRequestProfile _CapabilityProfile { get; set; }

        #region Implemented Methods
        public string ID
        {
            get { throw new NotImplementedException(); }
        }

        public string UserAgent
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMobile
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTablet
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTouchScreen
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFacebook
        {
            get { throw new NotImplementedException(); }
        }

        public int Width
        {
            get { throw new NotImplementedException(); }
        }

        public int Height
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsFlash
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, string> Capabilities
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        private Dictionary<string, string> Caps = new Dictionary<string, string>();
        public T Capability<T>(string name)
        {
            string ret = String.Empty;
            if (Caps.ContainsKey(name))
                ret = Caps[name];
            else
            {
                ret = _CapabilityProfile.GetCapability(name);
                Caps.Add(name, ret);
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }

        public string GetCapability(string capabilityName)
        {
            return this.Capability<string>(capabilityName);
        }
    }
}
