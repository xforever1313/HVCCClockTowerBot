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


using SethCS.Exceptions;
using SethCS.Extensions;

namespace HvccClock
{
    public class HvccClockConfig
    {
        // ---------------- Constructor ----------------

        public HvccClockConfig()
        {
            this.ApiKey = Environment.GetEnvironmentVariable( "TWITTER_API_KEY" ) ?? string.Empty;
            this.ApiSecret = Environment.GetEnvironmentVariable( "TWITTER_API_SECRET" ) ?? string.Empty;
        }

        // ---------------- Properties ----------------

        public string ApiKey { get; private set; }

        public string ApiSecret { get; private set; }

        // ---------------- Functions ----------------

        public bool TryValidate( out string error )
        {
            var errors = new List<string>();

            if( string.IsNullOrWhiteSpace( this.ApiKey ) )
            {
                errors.Add( "TWITTER_API_KEY env var not specfied" );
            }

            if( string.IsNullOrEmpty( this.ApiSecret ) )
            {
                errors.Add( "TWITTER_API_SECRET env var not specified" );
            }

            if( errors.Any() )
            {
                error = errors.ToListString( "-" );
                return false;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }

        public void Validate()
        {
            bool success = TryValidate( out string error );
            if( success == false )
            {
                throw new ValidationException( error );
            }
        }
    }
}
