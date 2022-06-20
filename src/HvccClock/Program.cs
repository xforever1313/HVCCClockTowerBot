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
using Quartz;

IHost host = Host.CreateDefaultBuilder( args )
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
    ).Build();

await host.RunAsync();
