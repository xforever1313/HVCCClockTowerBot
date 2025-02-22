﻿//
// HvccClock - A bot that chimes the time every hour.
// Copyright (C) 2022-2024 Seth Hendrick
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
    [TaskName( "publish" )]
    [IsDependentOn( typeof( PublishTwitterTask ) )]
    [IsDependentOn( typeof( PublishBlueSkyTask ) )]
    public sealed class PublishAllTask : DevopsTask
    {
    }

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
            CopyRootFile( context, "License.md" );

            context.EnsureDirectoryExists( context.TwitterZipFilesDistFolder );
            context.CleanDirectory( context.TwitterZipFilesDistFolder );

            FilePath zipFile = context.TwitterZipFilesDistFolder.CombineWithFilePath( "HvccClock_Twitter.zip" );
            context.Zip( context.TwitterLooseFilesDistFolder, zipFile );
        }

        private void CopyRootFile( BuildContext context, FilePath fileName )
        {
            fileName = context.RepoRoot.CombineWithFilePath( fileName );
            context.Information( $"Copying '{fileName}' to dist" );
            context.CopyFileToDirectory( fileName, context.TwitterLooseFilesDistFolder );
        }
    }

    [TaskName( "publish_bluesky" )]
    public sealed class PublishBlueSkyTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override void Run( BuildContext context )
        {
            context.EnsureDirectoryExists( context.BlueSkyDistFolder );

            DirectoryPath looseFilesDir = context.BlueSkyLooseFilesDistFolder;
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
                "HvccClock.Bsky/HvccClock.Bsky.csproj"
            );

            context.DotNetPublish( servicePath.ToString(), publishOptions );
            context.Information( string.Empty );

            CopyRootFile( context, "Readme.md" );
            CopyRootFile( context, "Credits.md" );
            CopyRootFile( context, "License.md" );

            context.EnsureDirectoryExists( context.BlueSkyZipFilesDistFolder );
            context.CleanDirectory( context.BlueSkyZipFilesDistFolder );

            FilePath zipFile = context.BlueSkyZipFilesDistFolder.CombineWithFilePath( "HvccClock_BlueSky.zip" );
            context.Zip( context.BlueSkyLooseFilesDistFolder, zipFile );
        }

        private void CopyRootFile( BuildContext context, FilePath fileName )
        {
            fileName = context.RepoRoot.CombineWithFilePath( fileName );
            context.Information( $"Copying '{fileName}' to dist" );
            context.CopyFileToDirectory( fileName, context.BlueSkyLooseFilesDistFolder );
        }
    }
}
