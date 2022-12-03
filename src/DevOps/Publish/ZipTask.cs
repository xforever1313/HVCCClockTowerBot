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
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Publish
{
    [TaskName( "publish_zip" )]
    public sealed class ZipTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override bool ShouldRun( BuildContext context )
        {
            FilePath licenseFile = context.LooseFilesDistFolder.CombineWithFilePath( "LICENSE.txt" );
            if( context.FileExists( licenseFile ) == false )
            {
                context.Information( $"Could not find license file in '{licenseFile}', please publish first" );
                return false;
            }

            return true;
        }

        public override void Run( BuildContext context )
        {
            context.EnsureDirectoryExists( context.ZipFilesDistFolder );
            context.CleanDirectory( context.ZipFilesDistFolder );

            FilePath zipFile = context.ZipFilesDistFolder.CombineWithFilePath( "HvccClock.zip" );
            context.Zip( context.LooseFilesDistFolder, zipFile );
        }
    }
}
