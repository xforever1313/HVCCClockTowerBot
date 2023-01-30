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
using System.Text;
using Quartz;
using Serilog;

namespace HvccClock.Common
{
    public abstract class BaseMessageJob : IJob
    {
        // ---------------- Fields ----------------

        private static readonly Stopwatch stopWatch = new Stopwatch();

        private readonly ILogger log;

        // ---------------- Constructor ----------------

        protected BaseMessageJob( ILogger log )
        {
            this.log = log;
        }

        // ---------------- Functions ----------------

        public async Task Execute( IJobExecutionContext context )
        {
            try
            {
                DateTime timeStamp = context.FireTimeUtc.DateTime;

                if( stopWatch.IsRunning )
                {
                    if( stopWatch.Elapsed <= TimeSpan.FromMinutes( 55 ) )
                    {
                        this.log.Warning( $"Fired {timeStamp} (UTC) too quickly, ignoring." );
                        return;
                    }
                }

                stopWatch.Restart();

                string tweet = GetMessageString( timeStamp, "HVCC" );
                await SendMessage( timeStamp, context.CancellationToken );
            }
            catch( Exception e )
            {
                this.log.Error( $"Error sending message: {Environment.NewLine}{e}" );
            }
        }

        protected abstract Task SendMessage( DateTime utcTime, CancellationToken cancelToken );

        public static string GetMessageString( DateTime time, string location )
        {
            var tweet = new StringBuilder();
            int hour = time.Hour;
            if( hour == 0 )
            {
                hour = 12;
            }
            else if( hour >= 13 )
            {
                hour = hour - 12;
            }

            for( int i = 0; i < hour; ++i )
            {
                tweet.Append( "BONG! " );
            }

            tweet.Remove( tweet.Length - 1, 1 );
            tweet.AppendLine();
            tweet.AppendLine();
            tweet.Append( $"The time in {location} currently is: {time.ToString( "dddd, MMMM d yyyy, h:00tt" )}." );

            return tweet.ToString();
        }
    }
}
