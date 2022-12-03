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

namespace HvccClock.ActivityPub
{
    public class HvccClockDatabase : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly Serilog.ILogger log;

        private readonly HvccClockConfig config;

        private readonly Queue<string> messages;

        // ---------------- Constructor ----------------

        public HvccClockDatabase( Serilog.ILogger log, HvccClockConfig config )
        {
            this.log = log;
            this.config = config;
            this.messages = new Queue<string>( config.MessagesToKeep + 1 );
        }

        // ---------------- Functions ----------------

        public void AddMessage( string message )
        {
            lock( this.messages )
            {
                this.messages.Enqueue( message );
                if( messages.Count > this.config.MessagesToKeep )
                {
                    this.messages.Dequeue();
                    this.log.Debug( "Maximum messages exceeded, removing oldest" );
                }
            }
        }

        /// <summary>
        /// Gets a list of times messages were sent out.
        /// The index 0 is the newest one.
        /// </summary>
        public IList<string> GetAllMessageTimes()
        {
            lock( this.messages )
            {
                return this.messages.ToList();
            }
        }

        public void Dispose()
        {
        }
    }
}
