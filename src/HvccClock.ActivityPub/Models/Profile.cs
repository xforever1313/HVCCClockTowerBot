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

using System.Globalization;

namespace HvccClock.ActivityPub.Models
{
    public record Profile
    {
        // ---------------- Properties ----------------

        // Mastodon supports the following properties:
        // https://docs.joinmastodon.org/spec/activitypub/#supported-activities-for-profiles

        /// <summary>
        /// Used for Webfinger lookup. Must be unique on the domain, and most
        /// correspond to a Webfinger acct: URI.
        /// </summary>
        public string PreferredUserName { get; init; } = "";

        /// <summary>
        /// Used as profile display name.
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// Used as profile bio.
        /// </summary>
        public string Summary { get; init; } = "";

        /// <summary>
        /// Assumed to be Person.
        /// If type is Application or Service, it will be interpreted as a bot flag.
        /// </summary>
        public string Type { get; init; } = "Service";

        /// <summary>
        /// Used as profile link.
        /// </summary>
        public string Url { get; init; } = "";

        /// <summary>
        /// Used as profile avatar.
        /// </summary>
        public string IconUrl { get; init; } = "";

        /// <summary>
        /// Used as profile header.
        /// </summary>
        public string Image { get; init; } = "";

        /// <summary>
        /// Will be shown as a locked account.
        /// </summary>
        public bool ManuallyApproveFollowers { get; init; } = false;

        /// <summary>
        /// Will be shown in the profile directory.
        /// </summary>
        public bool Discoverable { get; init; } = true;

        /// <summary>
        /// When the profile was created.
        /// </summary>
        public DateTime Published { get; init; } = new DateTime( 2022, 12, 3, 0, 0, 0, DateTimeKind.Utc );

        /// <summary>
        /// Website of the account.
        /// </summary>
        public string Website { get; init; } = "";

        /// <summary>
        /// Website of the source code.
        /// </summary>
        public string GitHub { get; init; } = "";
    }

    internal static class ProfileExtensions
    {
        // ---------------- Functions ----------------

        public static Profile FromEnvVar()
        {
            bool NotNull( string envName, out string envValue )
            {
                envValue = Environment.GetEnvironmentVariable( envName ) ?? "";
                return string.IsNullOrWhiteSpace( envValue ) == false;
            }

            var profile = new Profile();

            if( NotNull( "PROFILE_PREFERRED_USER_NAME", out string userName ) )
            {
                profile = profile with { PreferredUserName = userName };
            }
            
            if( NotNull( "PROFILE_NAME", out string profileName ) )
            {
                profile = profile with { Name = profileName };
            }
            
            if( NotNull( "PROFILE_SUMMARY", out string summary ) )
            {
                profile = profile with { Summary = summary };
            }

            if( NotNull( "PROFILE_TYPE", out string type ) )
            {
                profile = profile with { Type = type };
            }
            
            if( NotNull( "PROFILE_URL", out string url ) )
            {
                profile = profile with { Url = url };
            }

            if( NotNull( "PROFILE_ICON_URL", out string iconUrl ) )
            {
                profile = profile with { IconUrl = iconUrl };
            }

            if( NotNull( "PROFILE_IMAGE_URL", out string imageUrl ) )
            {
                profile = profile with { Image = imageUrl };
            }

            if( NotNull( "PROFILE_MANUALLY_APPROVE_FOLLOWERS", out string approve ) )
            {
                profile = profile with { ManuallyApproveFollowers = bool.Parse( approve ) };
            }

            if( NotNull( "PROFILE_DISCOVERABLE", out string discover ) )
            {
                profile = profile with { Discoverable = bool.Parse( discover ) };
            }

            if( NotNull( "PROFILE_PUBLISHED", out string published ) )
            {
                profile = profile with
                {
                    Published = DateTime.ParseExact(
                        published,
                        "O",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces
                    )
                };
            }

            if( NotNull( "PROFILE_WEBSITE", out string website ) )
            {
                profile = profile with { Website = website };
            }

            if( NotNull( "PROFILE_GITHUB", out string github ) )
            {
                profile = profile with { GitHub = github };
            }

            return profile;
        }

        public static string ToJson( this Profile profile )
        {
            return "";
        }
    }
}
