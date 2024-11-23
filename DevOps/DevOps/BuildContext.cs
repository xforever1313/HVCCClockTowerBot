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

using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps
{
    public class BuildContext : FrostingContext
    {
        // ---------------- Constructor ----------------

        public BuildContext( ICakeContext context ) :
            base( context )
        {
            this.RepoRoot = context.Environment.WorkingDirectory;
            this.SrcDir = this.RepoRoot.Combine( "src" );
            this.Solution = this.SrcDir.CombineWithFilePath( "HvccClock.sln" );
            this.DistFolder = this.RepoRoot.Combine( "dist" );
            this.TwitterDistFolder = this.DistFolder.Combine( "twitter" );
            this.ActivityPubDistFolder = this.DistFolder.Combine( "actpub" );
            this.TwitterLooseFilesDistFolder = this.TwitterDistFolder.Combine( "files" );
            this.TwitterZipFilesDistFolder = this.TwitterDistFolder.Combine( "zip" );
            this.TestResultsFolder = this.RepoRoot.Combine( "TestResults" );
            this.TestCsProj = this.SrcDir.CombineWithFilePath( "HvccClock.Tests/HvccClock.Tests.csproj" );
        }

        // ---------------- Properties ----------------

        public DirectoryPath RepoRoot { get; private set; }

        public DirectoryPath SrcDir { get; private set; }

        public FilePath Solution { get; private set; }

        public DirectoryPath DistFolder { get; private set; }

        public DirectoryPath TwitterDistFolder { get; private set; }

        public DirectoryPath ActivityPubDistFolder { get; private set; }

        public DirectoryPath TwitterLooseFilesDistFolder { get; private set; }

        public DirectoryPath TestResultsFolder { get; private set; }

        public DirectoryPath TwitterZipFilesDistFolder { get; private set; }

        public FilePath TestCsProj { get; private set; }

        // ---------------- Functions ----------------

        public DotNetMSBuildSettings GetBuildSettings()
        {
            var settings = new DotNetMSBuildSettings();

            settings.SetMaxCpuCount( System.Environment.ProcessorCount );

            return settings;
        }
    }
}
