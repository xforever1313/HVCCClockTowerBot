//
// ActivityPub.Inbox - Inbox service for https://activitypub.shendrick.net
// Copyright (C) 2022 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using ActivityPub.Inbox.Common;

namespace HvccClock.ActivityPub
{
    public class HvccInboxConfig : IActivityPubInboxConfig
    {
        // ---------------- Constructor ----------------

        public HvccInboxConfig() :
            this( ActivityPubSiteConfigExtensions.FromEnvVar() )
        {
        }

        public HvccInboxConfig( ActivityPubSiteConfig siteConfig )
        {
            this.Sites = new ActivityPubSiteConfig[] { siteConfig };
        }

        // ---------------- Properties ----------------

        public IEnumerable<ActivityPubSiteConfig> Sites { get; private set; }

        // ---------------- Functions ----------------

        public void Validate()
        {
            this.Sites.First().Validate();
        }
    }

    internal static class ActivityPubSiteConfigExtensions
    {
        // ---------------- Functions ----------------

        public static ActivityPubSiteConfig FromEnvVar()
        {
            return new ActivityPubSiteConfig(
                new FileInfo( Environment.GetEnvironmentVariable( "APP_PRIVATE_KEY_FILE" ) ?? "" ),
                new FileInfo( Environment.GetEnvironmentVariable( "APP_PUBLIC_KEY_FILE" ) ?? "" ),
                new Uri( Environment.GetEnvironmentVariable( "APP_PROFILE_URL" ) ?? "" ),
                "HVCC_CLOCK" // <- Only 1 possible ID.
            );
        }
    }
}
