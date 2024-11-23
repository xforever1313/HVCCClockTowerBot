//
// HvccClock - A bot that chimes the time every hour.
// Copyright (C) 2022-2024 Seth Hendrick
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

using HvccClock.Common;
using Serilog.Extensions.Logging;
using X.Bluesky;

namespace HvccClock.Bsky
{
    public sealed class SkeetJob : BaseMessageJob
    {
        // ---------------- Fields ----------------

        private readonly BlueskyClient client;

        // ---------------- Constructor ----------------

        public SkeetJob( Serilog.ILogger log, HvccClockConfig hvccConfig ) :
            base( log )
        {
            var microsoftLogger = new SerilogLoggerFactory( log );
            this.client = new BlueskyClient(
                hvccConfig.BlueSkyUser,
                hvccConfig.BlueSkyPassword,
                true,
                microsoftLogger.CreateLogger<BlueskyClient>()
            );
        }

        // ---------------- Methods ----------------

        protected override async Task SendMessage( DateTime utcTime, CancellationToken cancelToken )
        {
            DateTime timeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                utcTime,
                TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" )
            );

            // For some reason, need at least one hash tag for the message to get
            // sent to BlueSky?
            string postText = GetMessageString( timeStamp, "HVCC" ) + Environment.NewLine + Environment.NewLine + "#HVCC";

            await this.client.Post( postText );
        }
    }
}
