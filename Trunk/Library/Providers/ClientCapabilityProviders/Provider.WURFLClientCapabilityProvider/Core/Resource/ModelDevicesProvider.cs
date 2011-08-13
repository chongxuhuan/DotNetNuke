/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    public class ModelDevicesProvider : IModelDevicesProvider
    {
        private const string ATTR_CAPABILITY_NAME = "name";
        private const string ATTR_CAPABILITY_VALUE = "value";
        private const string ATTR_DEVICE_ACTUAL_DEVICE_ROOT = "actual_device_root";
        private const string ATTR_DEVICE_FALL_BACK = "fall_back";
        private const string ATTR_DEVICE_ID = "id";
        private const string ATTR_DEVICE_USER_AGENT = "user_agent";


        private const string ATTR_GROUP_ID = "id";
        private const string NODE_CAPABILITY = "capability";
        private const string NODE_DEVICE = "device";
        private const string NODE_GROUP = "group";

        private bool _actualDeviceRoot;

        private IDictionary<string, string> _capabilities;
        private IDictionary<string, string> _capabilitiesByGroup;
        private string _deviceID;
        private IList<ModelDevice> _devices;
        private string _fallBack;
        private string _userAgent;
        private bool patch;


        public ModelDevicesProvider() : this(false)
        {
        }

        public ModelDevicesProvider(bool patch)
        {
            this.patch = patch;
        }

        #region IModelDevicesProvider Members

        public IList<ModelDevice> GetModelDevices(WURFLResource wurflResource)
        {
            _devices = new List<ModelDevice>();


            using (Stream stream = wurflResource.GetStream())
            {
                using (XmlReader xmlReader = XmlReader.Create(stream, null))
                {
                    LoadData(xmlReader);
                }
            }

            /*
           if (wurflResource.)
            {
                Stream fileStream = new FileStream(wurflResource, FileMode.Open, FileAccess.Read);
                GZipStream gzStream = new GZipStream(fileStream, CompressionMode.Decompress);

                using (XmlReader xmlReader = XmlReader.Create(gzStream, null))
                {
                    LoadData(xmlReader);
                }
            }
            else
            {

                using (XmlReader xmlReader = XmlReader.Create(wurflResource))
                {
                    LoadData(xmlReader);
                }

            }
             * 
             */
            return _devices;
        }

        #endregion

        private void LoadData(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        HanldleStartElement(xmlReader);
                        break;
                    case XmlNodeType.EndElement:
                        HandleEndElement(xmlReader);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HanldleStartElement(XmlReader xmlReader)
        {
            string groupID = null;
            string capabilityName = null;
            string capabilityValue = null;

            switch (xmlReader.Name)
            {
                case NODE_DEVICE:
                    _deviceID = xmlReader.GetAttribute(ATTR_DEVICE_ID);
                    _userAgent = xmlReader.GetAttribute(ATTR_DEVICE_USER_AGENT);
                    _fallBack = xmlReader.GetAttribute(ATTR_DEVICE_FALL_BACK);
                    string tmp = xmlReader.GetAttribute(ATTR_DEVICE_ACTUAL_DEVICE_ROOT);
                    _actualDeviceRoot = false;

                    bool.TryParse(tmp, out _actualDeviceRoot);

                    if (xmlReader.IsEmptyElement)
                    {
                        ModelDevice device = new ModelDevice(_userAgent, _deviceID, _fallBack, _actualDeviceRoot);
                        _devices.Add(device);
                    }

                    _capabilities = new Dictionary<string, string>();
                    _capabilitiesByGroup = new Dictionary<string, string>();

                    break;
                case NODE_GROUP:
                    groupID = xmlReader.GetAttribute(ATTR_GROUP_ID);
                    break;
                case NODE_CAPABILITY:
                    capabilityName = xmlReader.GetAttribute(ATTR_CAPABILITY_NAME);
                    capabilityValue = xmlReader.GetAttribute(ATTR_CAPABILITY_VALUE);

                    _capabilities.Add(capabilityName, capabilityValue);
                    _capabilitiesByGroup.Add(capabilityName, groupID);

                    break;
            }
        }

        private void HandleEndElement(XmlReader xmlReader)
        {
            switch (xmlReader.Name)
            {
                case NODE_DEVICE:
                    ModelDevice device = new ModelDevice(_userAgent, _deviceID, _fallBack, _actualDeviceRoot,
                                                         _capabilities, _capabilitiesByGroup);
                    _devices.Add(device);
                    break;
                case NODE_GROUP:
                    break;
                default:
                    break;
            }
        }
    }

    internal enum WURFLNodeNames
    {
        device,
        group,
        capability
    }
}