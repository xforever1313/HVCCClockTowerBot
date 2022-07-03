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
            this.ConsumerKey = Environment.GetEnvironmentVariable( "TWITTER_CONSUMER_KEY" ) ?? string.Empty;
            this.ConsumerSecret = Environment.GetEnvironmentVariable( "TWITTER_CONSUMER_SECRET" ) ?? string.Empty;
            this.AccessToken = Environment.GetEnvironmentVariable( "TWITTER_ACCESS_TOKEN" ) ?? string.Empty;
            this.AccessTokenSecret = Environment.GetEnvironmentVariable( "TWITTER_ACCESS_TOKEN_SECRET" ) ?? string.Empty;
            this.ClientId = Environment.GetEnvironmentVariable( "TWITTER_CLIENT_ID" ) ?? string.Empty;
            this.ClientSecret = Environment.GetEnvironmentVariable( "TWITTER_CLIENT_SECRET" ) ?? string.Empty;
        }

        // ---------------- Properties ----------------

        public int Port => 9100;

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }

        public string AccessToken { get; private set; }

        public string AccessTokenSecret { get; private set; }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        // ---------------- Functions ----------------

        public bool TryValidate( out string error )
        {
            var errors = new List<string>();

            if( string.IsNullOrWhiteSpace( this.ConsumerKey ) )
            {
                errors.Add( "TWITTER_CONSUMER_KEY env var not specfied" );
            }

            if( string.IsNullOrEmpty( this.ConsumerSecret ) )
            {
                errors.Add( "TWITTER_CONSUMER_SECRET env var not specified" );
            }

            if( string.IsNullOrEmpty( this.AccessToken ) )
            {
                errors.Add( "TWITTER_ACCESS_TOKEN env var not specified" );
            }

            if( string.IsNullOrEmpty( this.AccessTokenSecret ) )
            {
                errors.Add( "TWITTER_ACCESS_TOKEN_SECRET env var not specified" );
            }

#if false
            if( string.IsNullOrEmpty( this.ClientId ) )
            {
                errors.Add( "TWITTER_CLIENT_ID env var not specified" );
            }

            if( string.IsNullOrEmpty( this.ClientSecret ) )
            {
                errors.Add( "TWITTER_CLIENT_SECRET env var not specified" );
            }
#endif
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
