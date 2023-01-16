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

using HvccClock.Common;
using Tweetinvi;
using Tweetinvi.Core.Web;

namespace HvccClock.Twitter
{
    public class TweetJob : BaseMessageJob
    {
        // ---------------- Fields ----------------

        private readonly TwitterClient client;

        // ---------------- Constructor ----------------

        public TweetJob( Serilog.ILogger log, HvccClockConfig hvccConfig ) :
            base( log )
        {
            this.client = new TwitterClient(
                hvccConfig.ConsumerKey,
                hvccConfig.ConsumerSecret,
                hvccConfig.AccessToken,
                hvccConfig.AccessTokenSecret
            );
        }

        // ---------------- Functions ----------------

        protected override async Task SendMessage( DateTime utcTime, CancellationToken cancelToken )
        {
            DateTime timeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                utcTime,
                TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" )
            );

            string tweetText = GetMessageString( timeStamp, "HVCC" );

            var poster = new TweetsV2Poster( client );

            ITwitterResult result = await poster.PostTweet(
                new TweetV2PostRequest
                {
                    Text = tweetText
                }
            );

            if( result.Response.IsSuccessStatusCode == false )
            {
                throw new Exception(
                    "Error when posting tweet: " + Environment.NewLine + result.Content
                );
            }
        }
    }
}
