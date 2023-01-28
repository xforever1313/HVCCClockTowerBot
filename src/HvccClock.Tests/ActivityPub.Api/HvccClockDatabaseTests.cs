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

using HvccClock.ActivityPub.Api;
using Moq;

namespace HvccClock.Tests.ActivityPub.Api
{
    [TestClass]
    [DoNotParallelize]
    public sealed class HvccClockDatabaseTests
    {
        // ---------------- Fields ----------------

        private FileInfo? dbFile;

        private HvccClockDatabase? uut;

        private Mock<Serilog.ILogger>? mockLog;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            string assemblyPath = typeof( HvccClockDatabaseTests ).Assembly.Location;
            string? directory = Path.GetDirectoryName( assemblyPath );
            if( directory is null )
            {
                throw new InvalidOperationException(
                    "Assembly directory name somehow null"
                );
            }

            this.dbFile = new FileInfo(
                Path.Combine( directory, "test.db" )
            );

            if( this.dbFile.Exists )
            {
                File.Delete( dbFile.FullName );
            }

            this.mockLog = new Mock<Serilog.ILogger>( MockBehavior.Loose );
            this.uut = new HvccClockDatabase(
                this.dbFile,
                this.mockLog.Object,
                false
            );
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.uut?.Dispose();
            this.uut = null;

            if( ( this.dbFile is not null ) && this.dbFile.Exists )
            {
                File.Delete( dbFile.FullName );
                this.dbFile = null;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void AddTest()
        {
            // Setup
            DateTime d1 = new DateTime( 2023, 1, 16, 13, 0, 0, DateTimeKind.Utc );
            DateTime d2 = new DateTime( 2023, 1, 16, 14, 0, 0, DateTimeKind.Utc );

            const string timeZone = "America/New_York";

            // Act
            Assert.IsNotNull( this.uut );
            int id1 = this.uut.AddTime( timeZone, d1 );
            int id2 = this.uut.AddTime( timeZone, d2 );

            // Check
            Assert.AreEqual( 2, this.uut.GetTotalRowsForTimeZone( timeZone ) );
            Assert.AreNotEqual( id1, id2 );
        }
    }
}
