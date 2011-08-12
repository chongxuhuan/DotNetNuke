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
    /// <summary>
    /// 
    /// </summary>
    public interface IWURFLModel
    {
        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <value>The devices.</value>
        ICollection<ModelDevice> Devices { get; }

        /// <summary>
        /// Gets the devices ID.
        /// </summary>
        /// <value>The devices ID.</value>
        ICollection<String> DevicesID { get; }

        /// <summary>
        /// Gets the name of the capabilities.
        /// </summary>
        /// <value>The name of the capabilities.</value>
        ICollection<string> CapabilitiesName { get; }

        /// <summary>
        /// Return all defined group's identifiers.
        /// </summary>
        /// <value>The groups.</value>
        /// Return all defined group's identifiers.         
        ICollection<string> Groups { get; }

        /// <summary>
        /// Gets the device by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        ModelDevice GetDeviceByID(String id);


        /// <summary>
        /// Determines whether [is device defined] [the specified id].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if [is device defined] [the specified id]; otherwise, <c>false</c>.
        /// </returns>
        bool IsDeviceDefined(String id);

        /// <summary>
        /// Gets the devices for.
        /// </summary>
        /// <param name="devicesId">The devices id.</param>
        /// <returns></returns>
        IList<ModelDevice> GetDevicesFor(IList<string> devicesID);

        /// <summary>
        /// Gets the device hierarchy.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        IList<ModelDevice> GetDeviceHierarchy(ModelDevice start);

        /// <summary>
        /// Gets the fall back device.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        ModelDevice GetFallBackDevice(ModelDevice start);

        /// <summary>
        /// Gets the device ancestor.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        ModelDevice GetDeviceAncestor(ModelDevice start);


        int Count();

        // Capabilities *******************************************************


        /// <summary>
        /// Determines whether [is capability defined] [the specified capability].
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns>
        /// 	<c>true</c> if [is capability defined] [the specified capability]; otherwise, <c>false</c>.
        /// </returns>
        bool IsCapabilityDefined(String capability);


        /// <summary>
        /// Gets the group by capability.
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns></returns>
        string GetGroupByCapability(String capability);


        /// <summary>
        /// Gets the device where capability is defined.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="capability">The capability.</param>
        /// <returns></returns>
        ModelDevice GetDeviceWhereCapabilityIsDefined(ModelDevice root,
                                                      String capability);

        // Groups *************************************************************


        /// <summary>
        /// Determines whether [is group defined] [the specified group id].
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        /// 	<c>true</c> if [is group defined] [the specified group id]; otherwise, <c>false</c>.
        /// </returns>
        bool IsGroupDefined(String groupID);


        /// <summary>
        /// Gets the capabilities for group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <returns></returns>
        ICollection<string> GetCapabilitiesForGroup(String groupID);
    }
}