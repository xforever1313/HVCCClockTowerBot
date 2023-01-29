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

using System.Xml.Linq;
using ActivityPub.Inbox.Common;
using SethCS.Exceptions;
using SethCS.Extensions;

namespace HvccClock.ActivityPub.Api
{
    public record class ClockTowerConfig(
        string Id,
        string TimeZone,
        ActivityPubSiteConfig SiteConfig,
        Uri OutboxUrl
    )
    {
        public IEnumerable<string> TryValidate()
        {
            var errors = new List<string>();

            if( string.IsNullOrWhiteSpace( this.Id ) )
            {
                errors.Add( $"{nameof( this.Id )} can not be null, empty, or whitespace" );
            }

            if( string.IsNullOrWhiteSpace( this.TimeZone ) )
            {
                errors.Add( $"{nameof( this.TimeZone )} can not be null, empty, or whitespace" );
            }
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById( this.TimeZone );
            }
            catch( TimeZoneNotFoundException )
            {
                errors.Add( $"Invalid Timezone passed in: {this.TimeZone}" );
            }

            errors.AddRange( this.SiteConfig.TryValidate() );

            return errors;
        }

        public void Validate()
        {
            IEnumerable<string> errors = TryValidate();

            if( errors.Any() )
            {
                throw new ListedValidationException(
                    $"Errors found when validating {nameof( ClockTowerConfig )}",
                    errors
                );
            }
        }
    }

    public static class ClockTowerConfigExtensions
    {
        // ---------------- Fields ----------------

        public static readonly string ClockTowerConfigRootName = "ClockTowerConfig";

        public static readonly string ClockTowerConfigElementName = "ClockTower";

        // ---------------- Functions ----------------

        public static ClockTowerConfig DeserializeConfig( XElement towerElement )
        {
            if( ClockTowerConfigElementName.EqualsIgnoreCase( towerElement.Name.LocalName ) == false )
            {
                throw new ArgumentException(
                    $"Passed in XML element doesn't have correct name.  Expected: {ClockTowerConfigElementName}, Got: {towerElement.Name.LocalName}.",
                    nameof( towerElement )
                );
            }

            string? timeZone = null;
            string? id = null;
            ActivityPubSiteConfig? siteConfig = null;
            Uri? outboxUri = null;

            string? baseKeyDirStr = Environment.GetEnvironmentVariable( "APP_BASE_KEY_DIRECTORY" );
            DirectoryInfo? baseKeyDir = null;
            if( baseKeyDirStr is not null )
            {
                baseKeyDir = new DirectoryInfo( baseKeyDirStr );
            }

            foreach( XElement element in towerElement.Elements() )
            {
                string name = element.Name.LocalName;
                if( string.IsNullOrWhiteSpace( name ) )
                {
                    continue;
                }
                else if( "TimeZone".EqualsIgnoreCase( name ) )
                {
                    timeZone = element.Value;
                }
                else if( ActivityPubSiteConfigExtensions.SiteConfigElementName.EqualsIgnoreCase( name ) )
                {
                    siteConfig = ActivityPubSiteConfigExtensions.DeserializeSiteConfig(
                        element,
                        baseKeyDir
                    );
                }
                else if( "OutboxUrl".EndsWithIgnoreCase( name ) )
                {
                    outboxUri = new Uri( element.Value );
                }
            }

            foreach( XAttribute attr in towerElement.Attributes() )
            {
                string name = attr.Name.LocalName;
                if( string.IsNullOrEmpty( name ) )
                {
                    continue;
                }
                else if( "id".EqualsIgnoreCase( name ) )
                {
                    id = attr.Value;
                }
            }

            if(
                ( timeZone is null ) ||
                ( siteConfig is null ) ||
                ( id is null ) ||
                ( outboxUri is null )
            )
            {
                var missing = new List<string>();

                if( timeZone is null ) { missing.Add( nameof( timeZone ) ); }
                if( siteConfig is null ) { missing.Add( nameof( siteConfig) ); }
                if( id is null ) { missing.Add( nameof( id ) ); }
                if( outboxUri is null ) { missing.Add( nameof( outboxUri ) ); }

                throw new ListedValidationException(
                    "Missing the following from the XML file for a clock tower.",
                    missing
                );
            }

            return new ClockTowerConfig(
                id,
                timeZone,
                siteConfig,
                outboxUri
            );
        }

        public static IEnumerable<ClockTowerConfig> DeserializeConfigs( XElement configElement )
        {
            if( ClockTowerConfigRootName.EqualsIgnoreCase( configElement.Name.LocalName ) == false )
            {
                throw new ArgumentException(
                    $"Passed in XML element doesn't have correct name.  Expected: {ClockTowerConfigRootName}, Got: {configElement.Name.LocalName}.",
                    nameof( configElement )
                );
            }

            var configs = new List<ClockTowerConfig>();
            foreach( XElement child in configElement.Elements() )
            {
                if( ClockTowerConfigElementName.EqualsIgnoreCase( child.Name.LocalName ) )
                {
                    ClockTowerConfig clockTowerConfig = DeserializeConfig( child );
                    configs.Add( clockTowerConfig );
                }
            }

            return configs;
        }
    }
}
