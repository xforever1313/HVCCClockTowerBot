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
