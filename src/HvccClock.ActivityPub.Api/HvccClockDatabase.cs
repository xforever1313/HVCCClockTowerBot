//
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
    public class HvccClockDatabase : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly Serilog.ILogger log;

        // ---------------- Constructor ----------------

        public HvccClockDatabase( Serilog.ILogger log )
        {
            this.log = log;
        }

        // ---------------- Functions ----------------

        public void AddMessage( string message )
        {
        }

        /// <summary>
        /// Gets a list of times messages were sent out.
        /// The index 0 is the newest one.
        /// </summary>
        public IList<string> GetAllMessageTimes()
        {
            return Array.Empty<string>();
        }

        public void Dispose()
        {
        }
    }
}
