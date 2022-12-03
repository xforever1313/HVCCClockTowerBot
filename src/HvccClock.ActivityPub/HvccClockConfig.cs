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

using HvccClock.Common;

namespace HvccClock.ActivityPub
{
    public record HvccClockConfig : IHvccClockConfig
    {
        /// <summary>
        /// How many messages to cache.
        /// Defaulted to 10 days worth.
        /// </summary>
        public int MessagesToKeep { get; init; } = 240;

        public string BaseUrl { get; init; } = "https://activitypub.shendrick.net/@HVCC_Clock";

        public string Urls { get; init; } = "http://127.0.0.1:9913";

        public FileInfo? LogFile { get; init; } = null;

        public string? TelegramBotToken { get; init; } = null;

        public string? TelegramChatId { get; init; } = null;

        public string ApplicationContext => "HVCC Activity Pub";
    }
}
