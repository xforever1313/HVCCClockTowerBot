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
using HvccClock.ActivityPub.Api;
using Microsoft.AspNetCore.Mvc;

namespace HvccClock.ActivityPub.Web.Controllers
{
    public class ProfileController : Controller
    {
        // ---------------- Fields ----------------

        private readonly IHvccClockApi clockBotApi;

        private readonly ActivityPubInboxApi inboxApi;

        // ---------------- Constructor ----------------

        public ProfileController(
            IHvccClockApi clockBotApi,
            ActivityPubInboxApi inboxApi
        )
        {
            this.clockBotApi = clockBotApi;
            this.inboxApi = inboxApi;
        }

        /// <summary>
        /// If no ID is given,
        /// redirect to the list of clock bots.
        /// </summary>
        [Route( "/Profile" )]
        public IActionResult Index()
        {
            return RedirectPermanent(
                "https://activitypub.shendrick.net/bots/clockbots/#list-of-clock-bots"
            );
        }

        /// <summary>
        /// For the index, redirect to the profile page
        /// with information about the bot.
        /// </summary>
        [Route( "/Profile/{profileId}" )]
        public IActionResult Index( [FromRoute] string profileId )
        {
            if( string.IsNullOrEmpty( profileId ) )
            {
                // If no profile is specified, just return a list
                // of the profiles to follow.
                return RedirectPermanent(
                    "https://activitypub.shendrick.net/bots/clockbots/#list-of-clock-bots"
                );
            }
            else if( this.clockBotApi.ActivityPubConfig.ClockTowerConfigs.ContainsKey( profileId ) == false )
            {
                return NotFound( "Clock Bot not found" );
            }

            ClockTowerConfig clockTowerConfig = this.clockBotApi.ActivityPubConfig.ClockTowerConfigs[profileId];
            return Redirect( clockTowerConfig.SiteConfig.ProfileUrl.ToString() );
        }

        [Route( "/Profile/{profileId}/outbox.json" )]
        public IActionResult Outbox( [FromRoute] string profileId )
        {
            return Ok( $"{profileId}/outbox.json" );
        }

        [Route( "/Profile/{profileId}/inbox.json" )]
        public IActionResult Inbox( [FromRoute] string profileId )
        {
            return Ok( $"{profileId}/inbox.json" );
        }

        [Route( "/Profile/{profileId}/followers.json" )]
        public IActionResult Followers( [FromRoute] string profileId )
        {
            return Ok( $"{profileId}/followers.json" );
        }
    }
}
