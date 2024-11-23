//
// HvccClock - A bot that chimes the time every hour.
// Copyright (C) 2022-2024 Seth Hendrick
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

using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvccClock.Common
{
    public interface IHvccClockConfig
    {
        /// <summary>
        /// Where to log messages to.
        /// If null, no file logging will take place.
        /// </summary>
        FileInfo? LogFile { get; }

        /// <summary>
        /// The Telegram bot to log messages to.
        /// 
        /// If null, no Telegram logging will take place.
        /// </summary>
        string? TelegramBotToken { get; }

        /// <summary>
        /// The Telegram chat to log messages to.
        /// 
        /// If null, no Telegram logging will take place.
        /// </summary>
        string? TelegramChatId { get; }

        string ApplicationContext { get; }
    }
}
