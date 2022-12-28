//
// HvccClock - A bot that chimes the time every hour.
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

namespace HvccClock.ActivityPub.Models
{
    public class HomeModel
    {
        // ---------------- Fields ----------------

        public HomeModel(
            ActivityPubInboxApi inboxApi,
            Resources resources
        )
        {
            this.BotVersion = typeof( Program ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version";
            this.InboxServiceVersion = inboxApi.Version;
            this.Resources = resources;
        }

        // ---------------- Properties ----------------

        public string BotVersion { get; private set; }

        public string InboxServiceVersion { get; private set; }

        public Resources Resources { get; private set; }
    }
}
