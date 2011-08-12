using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Storage
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public int MaxLengthOfUsername { get { return this.CapabilityProfile.Capability<int>("max_length_of_username"); } }
        public int MaxUrlLengthBookmark { get { return this.CapabilityProfile.Capability<int>("max_url_length_bookmark"); } }
        public int MaxNoOfBookmarks { get { return this.CapabilityProfile.Capability<int>("max_no_of_bookmarks"); } }
        public int MaxDeckSize { get { return this.CapabilityProfile.Capability<int>("max_deck_size"); } }
        public int MaxUrlLengthCachedPage { get { return this.CapabilityProfile.Capability<int>("max_url_length_cached_page"); } }
        public int MaxLengthOfPassword { get { return this.CapabilityProfile.Capability<int>("max_length_of_password"); } }
        public int MaxNoOfConnectionSettings { get { return this.CapabilityProfile.Capability<int>("max_no_of_connection_settings"); } }
        public int MaxUrlLengthInRequests { get { return this.CapabilityProfile.Capability<int>("max_url_length_in_requests"); } }
        public int MaxObjectSize { get { return this.CapabilityProfile.Capability<int>("max_object_size"); } }
        public int MaxUrlLengthHomepage { get { return this.CapabilityProfile.Capability<int>("max_url_length_homepage"); } }

    }
}