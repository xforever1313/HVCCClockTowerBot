﻿//
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

namespace HvccClock.ActivityPub.Api
{
    public interface IHvccClockApi
    {
        HvccClockDatabase Database { get; }

        Serilog.ILogger Log { get; }
    }

    public sealed class HvccClockApi : IHvccClockApi, IDisposable
    {
        // ---------------- Constructor ----------------

        public HvccClockApi( Serilog.ILogger log )
        {
            this.Log = log;

            this.Database = new HvccClockDatabase( this.Log );
        }

        // ---------------- Properties ----------------

        public HvccClockDatabase Database { get; private set; }

        public Serilog.ILogger Log { get; private set; }

        // ---------------- Functions ----------------

        public void Dispose()
        {
            this.Database?.Dispose();
        }
    }
}
