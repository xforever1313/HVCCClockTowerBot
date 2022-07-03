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

using HvccClock;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Quartz;

Console.WriteLine( $"Version: {typeof( HvccClockConfig ).Assembly.GetName()?.Version?.ToString( 3 ) ?? string.Empty}." );

var config = new HvccClockConfig();
if( config.TryValidate( out string error ) == false )
{
    Console.WriteLine( "Bot is misconfigured" );
    Console.WriteLine( error );
    return 1;
}

try
{
    TweetJob.OnSuccess += TweetJob_OnSuccess;
    TweetJob.OnException += TweetJob_OnException;

    var host = WebHost.CreateDefaultBuilder( args )
        .ConfigureServices(
            services =>
            {
                services.AddQuartz(
                    q =>
                    {
                        JobKey jobKey = JobKey.Create( nameof( TweetJob ) );
                        q.AddJob<TweetJob>( jobKey );

                        q.AddTrigger(
                            ( ITriggerConfigurator config ) =>
                            {
                                config.WithCronSchedule(
                                    $"0 0 * * * ?", // Fire every hour on the hour.
                                    ( CronScheduleBuilder cronBuilder ) =>
                                    {
                                        // HVCC is in NY.
                                        cronBuilder.InTimeZone( TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" ) );
                                        // If we misfire, just do nothing.  This isn't exactly
                                        // the most important application
                                        cronBuilder.WithMisfireHandlingInstructionDoNothing();
                                        cronBuilder.Build();
                                    }
                                );
                                config.WithDescription( $"Chime!" );
                                config.ForJob( jobKey );
                                config.StartNow();
                            }
                        );
                    }
                );

                services.AddQuartzHostedService(
                    options =>
                    {
                        options.AwaitApplicationStarted = true;
                        options.WaitForJobsToComplete = true;
                    }
                );
            }
        ).UseUrls( $"http://0.0.0.0:{config.Port}" )
        .Configure(
            ( app ) =>
            {
                app.UseRouting();
                app.UseEndpoints(
                    endpoints =>
                    {
                        endpoints.MapMetrics( "/Metrics" );
                    }
                );
            }
        )
        .Build();

    await host.RunAsync();
}
catch( Exception e )
{
    Console.Error.WriteLine( "FATAL ERROR:" );
    Console.Error.WriteLine( e.ToString() );
    return 2;
}
finally
{
    TweetJob.OnSuccess -= TweetJob_OnSuccess;
    TweetJob.OnException -= TweetJob_OnException;
}

void TweetJob_OnSuccess()
{
    HvccClockMetrics.RecordSuccess();
}

void TweetJob_OnException( Exception obj )
{
    HvccClockMetrics.RecordException();

    Console.Error.WriteLine( "***************" );
    Console.Error.WriteLine( $"Exception thrown at {DateTime.Now}:" );
    Console.Error.WriteLine( obj.ToString() );
    Console.Error.WriteLine( "***************" );
}

return 0;
