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

using HvccClock.ActivityPub.Api;
using HvccClock.Common;

namespace HvccClock.ActivityPub.Web
{
    public class UpdateJob : BaseMessageJob
    {
        // ---------------- Fields ----------------

        private readonly IHvccClockApi api;

        // ---------------- Constructor ----------------

        public UpdateJob( IHvccClockApi api ) :
            base( api.Log )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        protected override async Task SendMessage( string tweetText, CancellationToken cancelToken )
        {
            this.api.Database.AddMessage( tweetText );
            await Task.Delay( 0, cancelToken );
        }
    }
}
