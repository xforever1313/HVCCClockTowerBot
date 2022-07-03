//
// HvccClock - A Twitter bot that chimes the time every hour.
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
using System.Net.Http.Headers;
using System.Text;
using Quartz;
using Tweetinvi;
using Tweetinvi.Core.Web;

namespace HvccClock
{
    public class TweetJob : IJob
    {
        // ---------------- Fields ----------------

        public static event Action? OnSuccess;

        public static event Action<Exception>? OnException;

        private static readonly HttpClient httpClient;
        private static readonly Stopwatch stopWatch = new Stopwatch();

        private const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
        private const string baseAuthUrl = "https://api.twitter.com/oauth/authorize";
        private const string accessTokenUrl = "https://api.twitter.com/oauth/access_token";
        private const string tweetUrl = "https://api.twitter.com/2/tweets";

        private static readonly HvccClockConfig hvccConfig = new HvccClockConfig();

        // ---------------- Constructor ----------------

        public TweetJob()
        {
        }

        static TweetJob()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                // We only send one request per hour,
                // we can set this to a bigger value, such as
                // 30 minutes.
                PooledConnectionLifetime = TimeSpan.FromMinutes( 30 )
            };

            httpClient = new HttpClient( socketsHandler );

            string version = typeof( TweetJob ).Assembly.GetName()?.Version?.ToString( 3 ) ?? "Unkown Version";

            var productValue = new ProductInfoHeaderValue( "HvccClock", version );
            var productComment = new ProductInfoHeaderValue( "(+@HVCC_Clock - https://github.com/xforever1313/HVCCClockTowerBot)" );

            httpClient.DefaultRequestHeaders.UserAgent.Add( productValue );
            httpClient.DefaultRequestHeaders.UserAgent.Add( productComment );
        }

        // ---------------- Functions ----------------

        public static void DisposeHttpClient()
        {
            httpClient?.Dispose();
        }

        public async Task Execute( IJobExecutionContext context )
        {
            try
            {
                DateTime timeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                    context.FireTimeUtc.DateTime,
                    TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" )
                );

                if( stopWatch.IsRunning )
                {
                    if( stopWatch.Elapsed <= TimeSpan.FromMinutes( 55 ) )
                    {
                        Console.WriteLine( $"Fired {timeStamp} too quickly, ignoring." );
                        return;
                    }
                }

                stopWatch.Restart();

                string tweet = GetTweetString( timeStamp );
                await SendTweet( tweet, context.CancellationToken );

                OnSuccess?.Invoke();
            }
            catch( Exception e )
            {
                OnException?.Invoke( e );
            }
        }

        private async Task SendTweet( string tweetText, CancellationToken cancelToken )
        {
            var hvccConfig = new HvccClockConfig();
            var client = new TwitterClient(
                hvccConfig.ConsumerKey,
                hvccConfig.ConsumerSecret,
                hvccConfig.AccessToken,
                hvccConfig.AccessTokenSecret
            );

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

        public static string GetTweetString( DateTime time )
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
            tweet.Append( $"The time at HVCC currently is: {time.ToString( "dddd, MMMM d yyyy, h:00tt")}." );

            return tweet.ToString();
        }
    }
}
