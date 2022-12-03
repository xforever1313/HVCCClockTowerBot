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

using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using Serilog.Sinks.Telegram;

namespace HvccClock.Common
{
    public static class HostingExtensions
    {
        public static ILogger CreateLog( IHvccClockConfig config )
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console( Serilog.Events.LogEventLevel.Information );

            FileInfo? logFile = config.LogFile;
            if( logFile is not null )
            {
                logger.WriteTo.File(
                    logFile.FullName,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 512 * 1000 * 1000, // 512 MB
                    shared: false
                );
            }

            string? telegramBotToken = config.TelegramBotToken;
            string? telegramChatId = config.TelegramChatId;
            if(
                ( string.IsNullOrWhiteSpace( telegramBotToken ) == false ) &&
                ( string.IsNullOrWhiteSpace( telegramChatId ) == false )
            )
            {
                var telegramOptions = new TelegramSinkOptions(
                    botToken: telegramBotToken,
                    chatId: telegramChatId,
                    dateFormat: "dd.MM.yyyy HH:mm:sszzz",
                    applicationName: config.ApplicationContext
                );

                logger.WriteTo.Telegram(
                    telegramOptions,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
                );
            }

            return logger.CreateLogger();
        }

        public static IServiceCollection ConfigureHvccServices<TJob, TConfig>(
            this IServiceCollection services,
            TConfig config
        ) where TJob : BaseMessageJob
          where TConfig : class, IHvccClockConfig
        {
            services.AddSingleton<TConfig>( config );
            services.AddQuartz(
                q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    JobKey jobKey = JobKey.Create( typeof( TJob ).Name );
                    q.AddJob<TJob>( jobKey );

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

            return services;
        }
    }
}
