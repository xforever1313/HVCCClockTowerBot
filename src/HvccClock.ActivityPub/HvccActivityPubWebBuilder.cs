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

using ActivityPub.Inbox.Common;
using ActivityPub.WebBuilder;

namespace HvccClock.ActivityPub
{
    public class HvccActivityPubWebBuilder : ActivityPubWebBuilder, IDisposable
    {
        // ---------------- Fields ----------------

        private HvccClockApi? api;

        private ActivityPubInboxApi? inboxApi;

        // ---------------- Constructor ----------------

        public HvccActivityPubWebBuilder( string[] args ) :
            base( args )
        {
            // Don't construct the API here, just in case they just
            // want to print the version, we don't want something
            // to not validate.
        }

        // ---------------- Properties ----------------

        public override TextWriter HelpWriter => Console.Out;

        public override string ApplicationName =>
            $"{GetType().Assembly.GetName().Name} v{GetVersion()}";

        // ---------------- Functions ----------------

        public static string GetVersion()
        {
            return typeof( Program ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version";
        }

        protected override void ConfigureBuilder( WebApplicationBuilder builder )
        {
            if( this.Log is null )
            {
                throw new InvalidOperationException(
                    "Log was null, someting went out-of-order when setting up"
                );
            }

            var inboxConfig = new HvccInboxConfig();
            this.inboxApi = new ActivityPubInboxApi( inboxConfig, this.Log );
            this.api = new HvccClockApi( this.Log );
            builder.Services.AddSingleton( this.api );
            base.ConfigureBuilder( builder );
        }

        protected override void PrintCredits()
        {
            Console.WriteLine( "NOT IMPLEMENTED YET!" );
        }

        protected override void PrintLicense()
        {
            Console.WriteLine( "NOT IMPLEMENTED YET!" );
        }

        protected override void PrintVersion()
        {
            Console.WriteLine( GetVersion() );
        }

        public void Dispose()
        {
            this.api?.Dispose();
            this.inboxApi?.Dispose();
        }
    }
}
