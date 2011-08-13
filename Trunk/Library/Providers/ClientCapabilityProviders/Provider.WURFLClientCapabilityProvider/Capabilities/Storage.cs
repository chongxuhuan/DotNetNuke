using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Storage
    {
        public KnownDevice Device { get; set; }
        public int MaxLengthOfUsername { get { return Device.Capability<int>("max_length_of_username"); } }
        public int MaxUrlLengthBookmark { get { return Device.Capability<int>("max_url_length_bookmark"); } }
        public int MaxNoOfBookmarks { get { return Device.Capability<int>("max_no_of_bookmarks"); } }
        public int MaxDeckSize { get { return Device.Capability<int>("max_deck_size"); } }
        public int MaxUrlLengthCachedPage { get { return Device.Capability<int>("max_url_length_cached_page"); } }
        public int MaxLengthOfPassword { get { return Device.Capability<int>("max_length_of_password"); } }
        public int MaxNoOfConnectionSettings { get { return Device.Capability<int>("max_no_of_connection_settings"); } }
        public int MaxUrlLengthInRequests { get { return Device.Capability<int>("max_url_length_in_requests"); } }
        public int MaxObjectSize { get { return Device.Capability<int>("max_object_size"); } }
        public int MaxUrlLengthHomepage { get { return Device.Capability<int>("max_url_length_homepage"); } }

    }
}