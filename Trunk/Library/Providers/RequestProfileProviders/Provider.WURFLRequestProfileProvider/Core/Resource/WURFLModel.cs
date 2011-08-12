/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    internal class WURFLModel : IWURFLModel
    {
        private readonly IDictionary<string, ModelDevice> _devices;

        private readonly ModelDevice _genericDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="WURFLModel"/> class.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public WURFLModel(IList<ModelDevice> devices)
        {
            _devices = new SortedDictionary<string, ModelDevice>();

            foreach (ModelDevice modelDevice in devices)
            {
                _devices[modelDevice.ID] = modelDevice;
            }

            _genericDevice = _devices[Constants.GENERIC];
        }

        /// <summary>
        /// Gets the generic device.
        /// </summary>
        /// <value>The generic device.</value>
        public ModelDevice GenericDevice
        {
            get { return _genericDevice; }
        }

        #region IWURFLModel Members

        /// <summary>
        /// Gets the device by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ModelDevice GetDeviceByID(string id)
        {
            ModelDevice device = null;
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }


            _devices.TryGetValue(id, out device);

            if (device == null)
            {
                throw new DeviceNotDefinedException(id);
            }

            return device;
        }

        /// <summary>
        /// Determines whether [is device defined] [the specified id].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if [is device defined] [the specified id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDeviceDefined(string id)
        {
            return _devices.ContainsKey(id);
        }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <value>The devices.</value>
        public ICollection<ModelDevice> Devices
        {
            get { return new List<ModelDevice>(_devices.Values); }
        }

        /// <summary>
        /// Returns a read only Collection of devices for the given devices id.
        /// </summary>
        /// <param name="devicesId">The devices id.</param>
        /// <returns></returns>
        public IList<ModelDevice> GetDevicesFor(IList<string> devicesID)
        {
            List<ModelDevice> devices = new List<ModelDevice>(devicesID.Count);

            foreach (string deviceID in devicesID)
            {
                devices.Add(GetDeviceByID(deviceID));
            }

            return devices.AsReadOnly();
        }


        /// <summary>
        /// Gets the devices ID.
        /// </summary>
        /// <value>The devices ID.</value>
        public ICollection<string> DevicesID
        {
            get { return _devices.Keys; }
        }


        /// <summary>
        /// Gets the device hierarchy.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public IList<ModelDevice> GetDeviceHierarchy(ModelDevice start)
        {
            Stack<ModelDevice> stack = new Stack<ModelDevice>();

            ModelDevice looper = start;

            // WARNING generic -> ... -> start
            while (!String.Equals(Constants.GENERIC, looper.ID))
            {
                stack.Push(looper);
                looper = GetFallBackDevice(looper);
            }

            stack.Push(looper);

            return new List<ModelDevice>(stack);
        }

        /// <summary>
        /// Gets the fall back device.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public ModelDevice GetFallBackDevice(ModelDevice start)
        {
            return GetDeviceByID(start.FallBack);
        }

        /// <summary>
        /// Gets the device ancestor.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public ModelDevice GetDeviceAncestor(ModelDevice start)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            IList<ModelDevice> hierarchy = GetDeviceHierarchy(start);
            ModelDevice found = null;

            for (int i = hierarchy.Count - 1; i >= 0; --i)
            {
                if (hierarchy[i].IsActualDeviceRoot)
                {
                    found = hierarchy[i];
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Returns the number of devices
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _devices.Count;
        }

        /// <summary>
        /// Gets the name of the capabilities.
        /// </summary>
        /// <value>The name of the capabilities.</value>
        public ICollection<string> CapabilitiesName
        {
            get { return GenericDevice.Capabilities.Keys; }
        }

        /// <summary>
        /// Determines whether [is capability defined] [the specified capability].
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns>
        /// 	<c>true</c> if [is capability defined] [the specified capability]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCapabilityDefined(string capability)
        {
            return CapabilitiesName.Contains(capability);
        }

        /// <summary>
        /// Returns the group ID for the given capability
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns></returns>
        public string GetGroupByCapability(string capability)
        {
            if (!IsCapabilityDefined(capability))
            {
                throw new CapabilityNotFoundException(capability);
            }

            return _genericDevice.GetGroupForCapability(capability);
        }

        /// <summary>
        /// Returns the device where capability is defined.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="capability">The capability.</param>
        /// <returns></returns>
        public ModelDevice GetDeviceWhereCapabilityIsDefined(ModelDevice root, string capability)
        {
            if (root == null)
            {
                throw new ArgumentNullException("start");
            }
            if (!IsCapabilityDefined(capability))
            {
                throw new CapabilityNotFoundException(capability);
            }

            IList<ModelDevice> hierarchy = GetDeviceHierarchy(root);
            ModelDevice found = null;

            for (int i = hierarchy.Count - 1; i >= 0; --i)
            {
                if (hierarchy[i].IsCapabilityDefined(capability))
                {
                    found = hierarchy[i];
                    break;
                }
            }

            return found;
        }

        public ICollection<string> Groups
        {
            get { return GenericDevice.CapabilitiesByGroup.Values; }
        }


        /// <summary>
        /// Determines whether the specified group id is defined in wurfl
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        /// 	<c>true</c> if [is group defined] [the specified group id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGroupDefined(string groupId)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(groupId);
            }

            return GenericDevice.IsGroupDefined(groupId);
        }

        /// <summary>
        /// Gets the capabilities for group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <returns></returns>
        public ICollection<string> GetCapabilitiesForGroup(string groupID)
        {
            if (!IsGroupDefined(groupID))
            {
                throw new GroupNotDefinedException(groupID);
            }

            IList<string> capabilities = new List<string>();
            foreach (KeyValuePair<string, string> pair in GenericDevice.CapabilitiesByGroup)
            {
                if (String.Equals(pair.Value, groupID))
                {
                    capabilities.Add(pair.Key);
                }
            }
            return capabilities;
        }

        #endregion
    }
}