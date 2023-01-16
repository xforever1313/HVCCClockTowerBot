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

using System.Diagnostics;
using HvccClock.ActivityPub.Api;
using HvccClock.Common;
using Quartz;

namespace HvccClock.ActivityPub.Web
{
    public class UpdateJob : IJob
    {
        // ---------------- Fields ----------------

        private readonly IHvccClockApi api;

        // ---------------- Constructor ----------------

        public UpdateJob( IHvccClockApi api )
        {
            this.api = api;
        }

        // ---------------- Functions ----------------

        public async Task Execute( IJobExecutionContext context )
        {
            try
            {
                DateTime timeStamp = context.FireTimeUtc.DateTime;
                string? timeZone = context.Trigger.Description;
                if( timeZone is null )
                {
                    throw new InvalidOperationException(
                        "Got null timezone during time update job"
                    );
                }

                await this.api.Database.AddTimeAsync( timeZone, timeStamp );
            }
            catch( Exception e )
            {
                this.api.Log.Error( $"Error generating timestamp: {Environment.NewLine}{e}" );
            }
        }
    }
}
