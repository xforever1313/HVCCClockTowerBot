//
// ActivityPub.Inbox - Inbox service for https://activitypub.shendrick.net
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

using System.Collections.ObjectModel;
using System.Xml.Linq;
using ActivityPub.Inbox.Common;
using SethCS.Exceptions;

namespace HvccClock.ActivityPub.Api
{
    public class HvccActivityPubConfig : IActivityPubInboxConfig
    {
        // ---------------- Constructor ----------------

        public HvccActivityPubConfig() :
            this( new FileInfo( "clockbots.db" ), new List<ClockTowerConfig>() )
        {
        }

        public HvccActivityPubConfig( FileInfo dbFile, IEnumerable<ClockTowerConfig> clockTowerConfigs )
        {
            this.DbFile = dbFile;

            var dict = new Dictionary<string, ClockTowerConfig>();
            foreach( ClockTowerConfig config in clockTowerConfigs )
            {
                dict.Add( config.Id, config );
            }
            this.ClockTowerConfigs = new ReadOnlyDictionary<string, ClockTowerConfig>( dict );
        }

        // ---------------- Properties ----------------

        public FileInfo DbFile { get; private set; }

        public IReadOnlyDictionary<string, ClockTowerConfig> ClockTowerConfigs { get; private set; }

        public IEnumerable<ActivityPubSiteConfig> Sites =>
            this.ClockTowerConfigs.Values.Select( c => c.SiteConfig );

        // ---------------- Functions ----------------

        public void Validate()
        {
            var validationErrors = new List<string>();
            foreach( ClockTowerConfig clockConfig in this.ClockTowerConfigs.Values )
            {
                validationErrors.AddRange( clockConfig.TryValidate() );
            }

            if( validationErrors.Any() )
            {
                throw new ListedValidationException(
                    $"Errors when validating {nameof( HvccActivityPubConfig )}",
                    validationErrors
                );
            }
        }
    }

    public static class HvccActivityPubConfigConfigExtensions
    {
        // ---------------- Fields ----------------

        private const string clockbotFileEnvVarName = "APP_CLOCKBOT_CONFIG_FILE";

        private const string clockBotDbFileEnvVarName = "APP_CLOCKBOT_DB_FILE";

        // ---------------- Functions -----------------

        public static HvccActivityPubConfig FromEnvVar()
        {
            string? clockFileEnvVar = Environment.GetEnvironmentVariable( clockbotFileEnvVarName );
            if( clockFileEnvVar is null )
            {
                throw new ValidationException(
                    $"{clockbotFileEnvVarName} environment variable not specified, please fill in."
                );
            }

            string? dbFileEnvVar = Environment.GetEnvironmentVariable( clockBotDbFileEnvVarName );
            if( dbFileEnvVar is null )
            {
                throw new ValidationException(
                    $"{clockBotDbFileEnvVarName} environment varible not specified, please fill in."
                );
            }

            return FromXmlFile( 
                new FileInfo( dbFileEnvVar ),
                new FileInfo( clockFileEnvVar )
            );
        }

        public static HvccActivityPubConfig FromXmlFile( FileInfo dbFile, FileInfo xmlFile )
        {
            if( xmlFile.Exists == false )
            {
                throw new FileNotFoundException(
                    $"Clock bot XML File '{xmlFile.FullName}' not found."
                );
            }

            XDocument doc = XDocument.Load( xmlFile.FullName );

            XElement? root = doc.Root;
            if( root is null )
            {
                throw new ArgumentException(
                    $"XML Root node is null in file '{xmlFile.FullName}'",
                    nameof( xmlFile )
                );
            }

            IEnumerable<ClockTowerConfig> clockTowerConfigs = ClockTowerConfigExtensions.DeserializeConfigs( root );

            return new HvccActivityPubConfig( dbFile, clockTowerConfigs );
        }
    }
}
