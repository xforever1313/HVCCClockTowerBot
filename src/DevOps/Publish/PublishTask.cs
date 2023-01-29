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

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Publish
{
    [TaskName( "publish_twitter" )]
    public sealed class PublishTwitterTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override void Run( BuildContext context )
        {
            context.EnsureDirectoryExists( context.TwitterDistFolder );

            DirectoryPath looseFilesDir = context.TwitterLooseFilesDistFolder;
            context.EnsureDirectoryExists( looseFilesDir );
            context.CleanDirectory( looseFilesDir );

            context.Information( "Publishing App" );

            var publishOptions = new DotNetPublishSettings
            {
                Configuration = "Release",
                OutputDirectory = looseFilesDir.ToString(),
                MSBuildSettings = context.GetBuildSettings()
            };

            FilePath servicePath = context.SrcDir.CombineWithFilePath(
                "HvccClock.Twitter/HvccClock.Twitter.csproj"
            );

            context.DotNetPublish( servicePath.ToString(), publishOptions );
            context.Information( string.Empty );

            CopyRootFile( context, "Readme.md" );
            CopyRootFile( context, "Credits.md" );
            CopyRootFile( context, "LICENSE.txt" );
        }

        [TaskName( "publish_actpub" )]
        public sealed class PublishActPubTask : DevopsTask
        {
            public override void Run( BuildContext context )
            {
                context.EnsureDirectoryExists( context.ActivityPubDistFolder );

                DirectoryPath actPubFolder = context.ActivityPubDistFolder;
                context.EnsureDirectoryExists( actPubFolder );
                context.CleanDirectory( actPubFolder );

                context.Information( "Publishing App" );

                var publishOptions = new DotNetPublishSettings
                {
                    Configuration = "Release",
                    OutputDirectory = actPubFolder.ToString(),
                    MSBuildSettings = context.GetBuildSettings(),
                    SelfContained = true,
                    PublishReadyToRun = true,
                    PublishSingleFile = true,
                    PublishTrimmed = false, // <- JSON library causes warnings if true.
                    Runtime = "linux-x64"
                };

                FilePath servicePath = context.SrcDir.CombineWithFilePath(
                    "HvccClock.ActivityPub.Web/HvccClock.ActivityPub.Web.csproj"
                );

                context.DotNetPublish( servicePath.ToString(), publishOptions );
                context.Information( string.Empty );
            }
        }

        private void CopyRootFile( BuildContext context, FilePath fileName )
        {
            fileName = context.RepoRoot.CombineWithFilePath( fileName );
            context.Information( $"Copying '{fileName}' to dist" );
            context.CopyFileToDirectory( fileName, context.TwitterLooseFilesDistFolder );
        }
    }
}
