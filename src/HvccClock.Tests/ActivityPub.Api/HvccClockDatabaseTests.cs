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

        const string timeZone = "America/New_York";

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

            // Act
            Assert.IsNotNull( this.uut );
            int id1 = this.uut.AddTime( timeZone, d1 );
            int id2 = this.uut.AddTime( timeZone, d2 );

            // Check
            Assert.AreEqual( 2, this.uut.GetTotalRowsForTimeZone( timeZone ) );
            Assert.AreNotEqual( id1, id2 );

            // Some other timezone, meanwhile, should have no results.
            Assert.AreEqual( 0, this.uut.GetTotalRowsForTimeZone( "America/Phoenix" ) );
        }

        // -------- Index Record Tests --------

        [TestMethod]
        public void GetNullIndexTimeStampsWithEmptyDatabaseTest()
        {
            TimeResult result = DoGetIndexTest( null, true );
            Assert.AreEqual( 0, result.TotalRecords );
        }

        [TestMethod]
        public void Get0IndexTimeStampsWithEmptyDatabaseTest()
        {
            TimeResult result = DoGetIndexTest( 0, true );
            Assert.AreEqual( 0, result.TotalRecords );
        }

        [TestMethod]
        public void GetFirstIndexOfEmptyDatabaseTest()
        {
            TimeResult result = DoGetIndexTest( 1, true );
            Assert.AreEqual( 0, result.TotalRecords );
        }

        [TestMethod]
        public void GetSecondIndexOfEmptyDatabaseTest()
        {
            TimeResult result = DoGetIndexTest( 2, true );
            Assert.AreEqual( 0, result.TotalRecords );
        }

        [TestMethod]
        public void GetIndexWithASingleTimeStampTest()
        {
            // Setup
            DateTime d1 = new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc );

            Assert.IsNotNull( this.uut );
            this.uut.AddTime( timeZone, d1 );

            // Act
            TimeResult result = DoGetIndexTest( null, false );

            // Check
            Assert.AreEqual( 1, result.TotalRecords );
        }

        // -------- Get Times Tests --------

        [TestMethod]
        public void GetFirstIndexWithSingleTimeTest()
        {
            // Setup
            const int index = 1;
            DateTime d1 = new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc );

            Assert.IsNotNull( this.uut );
            this.uut.AddTime( timeZone, d1 );

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // Just 1 entry, should be no indexes in both directions.
            Assert.IsNull( result.NextIndex );
            Assert.IsNull( result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( 1, result.TimeStamps.Count );
            Assert.AreEqual( result.TimeStamps[0], d1 );
            Assert.AreEqual( 1, result.TotalRecords );
        }

        [TestMethod]
        public void GetFirstIndexWithSomeTimesTest()
        {
            // Setup
            const int index = 1;
            var dateTimeArray = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc )
            };


            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in dateTimeArray )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // Just enough for 1 day, should be no indexes in both directions.
            Assert.IsNull( result.NextIndex );
            Assert.IsNull( result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( dateTimeArray.Length, result.TimeStamps.Count );
            for( int i = 0; i < dateTimeArray.Length; ++i )
            {
                Assert.AreEqual( dateTimeArray[i], result.TimeStamps[i] );
            }
            Assert.AreEqual( dateTimeArray.Length, result.TotalRecords );
        }

        [TestMethod]
        public void OutOfRangeIndexWithSomeFirstDayTimesTest()
        {
            // Setup
            var dateTimeArray = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc )
            };

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in dateTimeArray )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            var intsToTry = new int[] { 2, 3, 4 };
            foreach( int index in intsToTry )
            {
                // Act
                TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

                // Check
                Assert.AreEqual( 0, result.StartIndex );
                Assert.AreEqual( index, result.Index );
                Assert.AreEqual( 1, result.EndIndex );

                // Beyond the first day, there should be no next index at all.
                Assert.IsNull( result.NextIndex );

                // However, there are records on the first day,
                // so make that the previous index.
                Assert.AreEqual( 1, result.PreviousIndex );

                Assert.AreEqual( timeZone, result.TimeZone );
                Assert.AreEqual( 0, result.TimeStamps.Count );
                Assert.AreEqual( dateTimeArray.Length, result.TotalRecords );
            }
        }

        [TestMethod]
        public void GetFirstIndexWithDaysWorthOfTimesTest()
        {
            // Setup
            const int index = 1;
            var dateTimeArray = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc ),
            };

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, dateTimeArray.Length );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in dateTimeArray )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // Just enough for 1 day, should be no indexes in both directions.
            Assert.IsNull( result.NextIndex );
            Assert.IsNull( result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( dateTimeArray.Length, result.TimeStamps.Count );
            for( int i = 0; i < dateTimeArray.Length; ++i )
            {
                Assert.AreEqual( dateTimeArray[i], result.TimeStamps[i] );
            }
            Assert.AreEqual( dateTimeArray.Length, result.TotalRecords );
        }

        [TestMethod]
        public void OutOfRangeIndexWithOneDaysWorthOfTimesTest()
        {
            // Setup
            var dateTimeArray = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc ),
            };

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in dateTimeArray )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            var intsToTry = new int[] { 2, 3, 4 };
            foreach( int index in intsToTry )
            {
                // Act
                TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

                // Check
                Assert.AreEqual( 0, result.StartIndex );
                Assert.AreEqual( index, result.Index );
                Assert.AreEqual( 1, result.EndIndex );

                // Beyond the first day, there should be no next index at all.
                Assert.IsNull( result.NextIndex );

                // However, there are records on the first day,
                // so make that the previous index.
                Assert.AreEqual( 1, result.PreviousIndex );

                Assert.AreEqual( timeZone, result.TimeZone );
                Assert.AreEqual( 0, result.TimeStamps.Count );
                Assert.AreEqual( dateTimeArray.Length, result.TotalRecords );
            }
        }

        [TestMethod]
        public void GetFirstIndexWith1MoreThanDaysWorthOfTimesTest()
        {
            // Setup
            const int index = 1;
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc )
            };

            int expectedLength = firstDayList.Length;

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( 2, result.EndIndex );

            // There is more than 1 day's worth of indexes,
            // next index should not be null.
            Assert.AreEqual( 2, result.NextIndex );

            // However, this is the first index, there should be no previous index.
            Assert.IsNull( result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( expectedLength, result.TimeStamps.Count );
            for( int i = 0; i < expectedLength; ++i )
            {
                Assert.AreEqual( firstDayList[i], result.TimeStamps[i] );
            }

            // Total records should include the additional date.
            Assert.AreEqual( allDates.Count, result.TotalRecords );
        }

        [TestMethod]
        public void GetSecondIndexWith1DayWorthOfTimesTest()
        {
            // Setup
            const int index = 2;
            var dateTimeArray = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, dateTimeArray.Length );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in dateTimeArray )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( 1, result.EndIndex );

            // Just one day's worth of addresses,
            // there should be no next address, expecially
            // since our index is bigger than the first day's.
            Assert.IsNull( result.NextIndex );

            // There are results, however, in the previous index.
            // Return that for previous one.
            Assert.AreEqual( 1, result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );

            // Should be nothing in this index,
            // since we don't contain enough for day 2.
            Assert.AreEqual( 0, result.TimeStamps.Count );

            // Total records should include all dates.
            Assert.AreEqual( dateTimeArray.Length, result.TotalRecords );
        }

        [TestMethod]
        public void GetSecondIndexWith1MoreThanDaysWorthOfTimesTest()
        {
            // Setup
            const int index = 2;
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc )
            };

            int expectedLength = secondDayList.Length;

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // There are only 2 days worth of dates,
            // so there should be no next index.
            Assert.IsNull( result.NextIndex );

            // This is the second index, so there should be a previous index.
            Assert.AreEqual( 1, result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( expectedLength, result.TimeStamps.Count );
            for( int i = 0; i < expectedLength; ++i )
            {
                Assert.AreEqual( secondDayList[i], result.TimeStamps[i] );
            }

            // Total records should include the additional dates.
            Assert.AreEqual( allDates.Count, result.TotalRecords );
        }

        [TestMethod]
        public void GetSecondIndexWithSomeMoreThanDaysWorthOfTimesTest()
        {
            // Setup
            const int index = 2;
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 5, 0, 0, DateTimeKind.Utc ),
            };

            int expectedLength = secondDayList.Length;

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // There are only 2 days worth of dates,
            // so there should be no next index.
            Assert.IsNull( result.NextIndex );

            // This is the second index, so there should be a previous index.
            Assert.AreEqual( 1, result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( expectedLength, result.TimeStamps.Count );
            for( int i = 0; i < expectedLength; ++i )
            {
                Assert.AreEqual( secondDayList[i], result.TimeStamps[i] );
            }

            // Total records should include the additional dates.
            Assert.AreEqual( allDates.Count, result.TotalRecords );
        }

        [TestMethod]
        public void OutOfRangeSecondIndexWithSomeMoreThanDaysWorthOfTimesTest()
        {
            // Setup
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 5, 0, 0, DateTimeKind.Utc ),
            };

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            var indexesToTry = new int[] { 3, 4, 5 };

            // Act
            foreach( int index in indexesToTry )
            {
                TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

                // Check
                Assert.AreEqual( 0, result.StartIndex );
                Assert.AreEqual( index, result.Index );
                Assert.AreEqual( 2, result.EndIndex );

                // Beyond the total number of days,
                // next index should be null.
                Assert.IsNull( result.NextIndex );

                // Closet index with dates in it is 2, make that previous index.
                Assert.AreEqual( 2, result.PreviousIndex );

                Assert.AreEqual( timeZone, result.TimeZone );
                Assert.AreEqual( 0, result.TimeStamps.Count );

                // Total records should include the additional dates.
                Assert.AreEqual( allDates.Count, result.TotalRecords );
            }
        }

        [TestMethod]
        public void OutOfRangeSecondIndexWithTwoDaysWorthOfTimesTest()
        {
            // Setup
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 17, 0, 0, DateTimeKind.Utc ),
            };

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, secondDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            var indexesToTry = new int[] { 3, 4, 5 };

            // Act
            foreach( int index in indexesToTry )
            {
                TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

                // Check
                Assert.AreEqual( 0, result.StartIndex );
                Assert.AreEqual( index, result.Index );
                Assert.AreEqual( 2, result.EndIndex );

                // Beyond the total number of days,
                // next index should be null.
                Assert.IsNull( result.NextIndex );

                // Closet index with dates in it is 2, make that previous index.
                Assert.AreEqual( 2, result.PreviousIndex );

                Assert.AreEqual( timeZone, result.TimeZone );
                Assert.AreEqual( 0, result.TimeStamps.Count );

                // Total records should include the additional dates.
                Assert.AreEqual( allDates.Count, result.TotalRecords );
            }
        }

        [TestMethod]
        public void GetSecondIndexWithTwoDaysWorthOfTimesTest()
        {
            // Setup
            const int index = 2;
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 17, 0, 0, DateTimeKind.Utc ),
            };

            int expectedLength = secondDayList.Length;

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, secondDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( index, result.EndIndex );

            // There are only 2 days worth of dates,
            // so there should be no next index.
            Assert.IsNull( result.NextIndex );

            // This is the second index, so there should be a previous index.
            Assert.AreEqual( 1, result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( expectedLength, result.TimeStamps.Count );
            for( int i = 0; i < expectedLength; ++i )
            {
                Assert.AreEqual( secondDayList[i], result.TimeStamps[i] );
            }

            // Total records should include the additional dates.
            Assert.AreEqual( allDates.Count, result.TotalRecords );
        }

        [TestMethod]
        public void GetSecondIndexWith1MoreThan3DaysWorthOfTimesTest()
        {
            // Setup
            const int index = 2;
            var firstDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 28, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 28, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 17, 0, 0, DateTimeKind.Utc )
            };

            var secondDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 27, 16, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 15, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 14, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 13, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 12, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 11, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 10, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 9, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 8, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 7, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 6, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 5, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 4, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 3, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 2, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 1, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 27, 0, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 23, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 22, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 21, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 20, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 19, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 18, 0, 0, DateTimeKind.Utc ),
                new DateTime( 2023, 1, 26, 17, 0, 0, DateTimeKind.Utc ),
            };

            var thirdDayList = new DateTime[]
            {
                new DateTime( 2023, 1, 26, 16, 0, 0, DateTimeKind.Utc ),
            };

            int expectedLength = secondDayList.Length;

            // Sanity check
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, firstDayList.Length );
            Assert.AreEqual( TimeResult.TimeStampsPerIndex, secondDayList.Length );

            var allDates = new List<DateTime>();
            allDates.AddRange( firstDayList );
            allDates.AddRange( secondDayList );
            allDates.AddRange( thirdDayList );

            Assert.IsNotNull( this.uut );
            foreach( DateTime timeStamp in allDates )
            {
                this.uut.AddTime( timeZone, timeStamp );
            }

            // Act
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index, result.Index );
            Assert.AreEqual( 3, result.EndIndex );

            // There are only just over 2 days worth of dates,
            // so there should be a next index.
            Assert.AreEqual( 3, result.NextIndex );

            // This is the second index, so there should be a previous index.
            Assert.AreEqual( 1, result.PreviousIndex );

            Assert.AreEqual( timeZone, result.TimeZone );
            Assert.AreEqual( expectedLength, result.TimeStamps.Count );
            for( int i = 0; i < expectedLength; ++i )
            {
                Assert.AreEqual( secondDayList[i], result.TimeStamps[i] );
            }

            // Total records should include the additional dates.
            Assert.AreEqual( allDates.Count, result.TotalRecords );
        }

        // ---------------- Test Helpers ----------------

        private TimeResult DoGetIndexTest( int? index, bool shouldBeEmpty )
        {
            // Act
            Assert.IsNotNull( this.uut );
            TimeResult result = this.uut.GetTimesForTimeZone( timeZone, index );

            // Check
            Assert.AreEqual( 0, result.StartIndex );
            Assert.AreEqual( index ?? 0, result.Index );

            // Index result, should not have a next index
            // if we are an empty database.
            if( shouldBeEmpty )
            {
                Assert.IsNull( result.NextIndex );
                Assert.AreEqual( 0, result.EndIndex );
            }
            else
            {
                Assert.AreEqual( 1, result.NextIndex );
                Assert.AreEqual( 1, result.EndIndex );
            }
            
            // Index result, should have no previous result.
            Assert.IsNull( result.PreviousIndex );

            // Index result, should contain no timestamps.
            Assert.AreEqual( 0, result.TimeStamps.Count );
            Assert.AreEqual( timeZone, result.TimeZone );

            return result;
        }
    }
}
