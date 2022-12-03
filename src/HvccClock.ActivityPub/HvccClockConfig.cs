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

using HvccClock.ActivityPub.Models;
using HvccClock.Common;

namespace HvccClock.ActivityPub
{
    public record HvccClockConfig : IHvccClockConfig
    {
        // ---------------- Properties ----------------

        // -------- Application Settings --------

        /// <summary>
        /// How many messages to cache.
        /// Defaulted to 10 days worth.
        /// </summary>
        public int MessagesToKeep { get; init; } = 240;

        public string Urls { get; init; } = "http://127.0.0.1:9913";

        public FileInfo? LogFile { get; init; } = null;

        public string? TelegramBotToken { get; init; } = null;

        public string? TelegramChatId { get; init; } = null;

        public string ApplicationContext => "HVCC Activity Pub";

        // -------- ActivityPub Settings --------

        public Profile Profile { get; init; } = new Profile();
    }

    internal static class HvccClockConfigExtensions
    {
        // ---------------- Functions ----------------

        public static HvccClockConfig FromEnvVar()
        {
            bool NotNull( string envName, out string envValue )
            {
                envValue = Environment.GetEnvironmentVariable( envName ) ?? "";
                return string.IsNullOrWhiteSpace( envValue );
            }

            var settings = new HvccClockConfig
            {
                Profile = ProfileExtensions.FromEnvVar()
            };

            if( NotNull( "ASPNETCORE_URLS", out string urls ) )
            {
                settings = settings with { Urls = urls };
            }

            if( NotNull( "APP_LOG_FILE", out string logFile ) )
            {
                settings = settings with { LogFile = new FileInfo( logFile ) };
            }

            if( NotNull( "APP_TELEGRAM_BOT_TOKEN", out string tgBotToken ) )
            {
                settings = settings with { TelegramBotToken = tgBotToken };
            }

            if( NotNull( "APP_TELEGRAM_CHAT_ID", out string tgChatId ) )
            {
                settings = settings with { TelegramChatId = tgChatId };
            }

            return settings;
        }
    }
}
