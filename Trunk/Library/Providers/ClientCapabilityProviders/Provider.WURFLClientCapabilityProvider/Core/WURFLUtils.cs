/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Resource;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// Utility Class for retrieving detailed info about wurfl devices.
    /// </summary>
    public sealed class WURFLUtils
    {
        private readonly IWURFLModel _wurflModel;
        private IModelDevicesProvider _devicesProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WURFLUtils"/> class.
        /// </summary>
        /// <param name="wurflModel">The wurfl model.</param>
        public WURFLUtils(IWURFLModel wurflModel)
            : this(wurflModel, new ModelDevicesProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WURFLUtils"/> class.
        /// </summary>
        /// <param name="wurflModel">The wurfl model.</param>
        /// <param name="deviceProvider">The device provider.</param>
        public WURFLUtils(IWURFLModel wurflModel, IModelDevicesProvider deviceProvider)
        {
            _wurflModel = wurflModel;
            _devicesProvider = deviceProvider;
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
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            return _wurflModel.IsDeviceDefined(id);
        }

        /// <summary>
        /// Gets the model device by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ModelDevice GetModelDeviceByID(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            return _wurflModel.GetDeviceByID(id);
        }


        /// <summary>
        /// Gets the model devices.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public IList<ModelDevice> GetModelDevices(IList<string> ids)
        {
            if (ids == null || ids.Count < 0)
            {
                throw new ArgumentException("ids must contain at least one element");
            }

            return _wurflModel.GetDevicesFor(ids);
        }


        /// <summary>
        /// Gets all devices ID.
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetAllDevicesID()
        {
            return _wurflModel.DevicesID;
        }

        /// <summary>
        /// Gets all model devices.
        /// </summary>
        /// <returns></returns>
        public ICollection<ModelDevice> GetAllModelDevices()
        {
            return _wurflModel.Devices;
        }

        /// <summary>
        /// Gets the model device hierarchy.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public IList<ModelDevice> getModelDeviceHierarchy(ModelDevice start)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            return _wurflModel.GetDeviceHierarchy(start);
        }
    }
}