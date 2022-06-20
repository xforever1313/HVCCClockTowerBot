using System.Diagnostics;
using System.Text;
using Quartz;

namespace HvccClock
{
    public class TweetJob : IJob
    {
        // ---------------- Fields ----------------

        private readonly Stopwatch stopWatch = new Stopwatch();

        // ---------------- Functions ----------------

        public async Task Execute( IJobExecutionContext context )
        {
            DateTime timeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                context.FireTimeUtc.DateTime,
                TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" )
            );

            if( stopWatch.IsRunning )
            {
                if( stopWatch.Elapsed <= new TimeSpan( 0, 55, 0 ) )
                {
                    Console.WriteLine( $"Fired {timeStamp} too quickly, ignoring." );
                    return;
                }
            }

            stopWatch.Restart();

            Console.Write( GetTweetString( timeStamp ) );

            await Task.Delay( 1 );
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
