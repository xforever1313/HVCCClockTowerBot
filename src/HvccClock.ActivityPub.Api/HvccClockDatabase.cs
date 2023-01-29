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

using System.Data;
using System.Data.Common;
using HvccClock.ActivityPub.Api.DatabaseSchema;

namespace HvccClock.ActivityPub.Api
{
    public class HvccClockDatabase : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly FileInfo dbFile;

        private readonly Serilog.ILogger log;

        private readonly bool pool;

        // ---------------- Constructor ----------------

        public HvccClockDatabase( FileInfo dbFile, Serilog.ILogger log ) :
            this( dbFile, log, true )
        {
        }

        public HvccClockDatabase( FileInfo dbFile, Serilog.ILogger log, bool pool )
        {
            this.log = log;
            this.dbFile = dbFile;
            this.pool = pool;
        }

        // ---------------- Functions ----------------

        public void EnsureCreated()
        {
            using( DatabaseConnection databaseConnection = Connect() )
            {
                databaseConnection.EnsureCreated();
            }
        }

        public int AddTime( string timeZone, DateTime dateTimeUtc )
        {
            int id;
            using( DatabaseConnection dbConnection = Connect() )
            {
                if( dbConnection.Dates is null )
                {
                    throw new InvalidOperationException(
                        "Dates table somehow null"
                    );
                }

                var newRow = new DateTable
                {
                    TimeStamp = dateTimeUtc,
                    TimeZone = timeZone
                };

                dbConnection.Dates.Add( newRow );

                dbConnection.SaveChanges();

                id = newRow.Id;
            }
            
            this.log.Debug( $"Added '{dateTimeUtc}' for timezone '{timeZone}'.  ID: {id}" );

            return id;
        }

        public Task<int> AddTimeAsync( string timeZone, DateTime dateTime )
        {
            return Task.Run(
                () => AddTime( timeZone, dateTime )
            );
        }

        public int GetTotalRowsForTimeZone( string timeZone )
        {
            using( DatabaseConnection dbConnection = Connect() )
            {
                return GetTotalRowsForTimeZoneInternal( dbConnection, timeZone );
            }
        }

        public Task<int> GetTotalRowsForTimeZoneAsync( string timeZone )
        {
            return Task.Run(
                () => GetTotalRowsForTimeZone( timeZone )
            );
        }

        /// <summary>
        /// Gets the current times for the given time zone.
        /// </summary>
        /// <param name="dayIndex">
        /// Which day from now to get the timestamps from.
        /// 
        /// If this is 0 or less, or null, this just returns the "index" information,
        /// which is the total timestamps and the next indexes.
        /// 
        /// If this is 1 or more, it will grab timestamps from that day.
        /// '1' being the most current.
        /// 
        /// If no times exist on a day, an empty array will be returned, but the indexes will
        /// point to the most recent.
        /// </param>
        public TimeResult GetTimesForTimeZone( string timeZone, int? dayIndex )
        {
            using( DatabaseConnection dbConnection = Connect() )
            {
                int totalRecords = GetTotalRowsForTimeZoneInternal( dbConnection, timeZone );

                if( ( dayIndex is null ) || ( dayIndex.Value <= 0 ) )
                {
                    // We just want the index record if no index is specified.
                    return new TimeResult
                    {
                        // If we contain values, then our start index
                        // should not be null and should point to
                        // the first page.
                        StartIndex = ( totalRecords != 0 ) ? 1 : null,

                        // The index we selected is 0, the index record.
                        Index = 0,

                        // Index page, therefore there is no next or previous.
                        // Go to the start page first.
                        NextIndex = null,

                        // The end index is 1, unless we have no records, then its null
                        // since there are no records at all.
                        EndIndex = ( totalRecords != 0 ) ? 1 : null,

                        // This is the index record, there is no previous record.
                        PreviousIndex = null,

                        // Index record contains no dates.
                        TimeStamps = Array.Empty<TimeStamp>(),

                        TimeZone = timeZone,
                        TotalRecords = totalRecords
                    };
                }

                IReadOnlyList<TimeStamp> timeStamps = GetTimeStampsInternal(
                    dbConnection,
                    timeZone,
                    dayIndex.Value
                );

                bool isEmpty = timeStamps.Count == 0;
                bool isFilled = timeStamps.Count >= TimeResult.TimeStampsPerIndex;

                int? nextIndex;
                if( isEmpty )
                {
                    // If we are empty, there is no next index
                    // since there is nothing current, so how can there
                    // be something above us?
                    nextIndex = null;
                }
                else if( isFilled )
                {
                    if( ( dayIndex * TimeResult.TimeStampsPerIndex ) == totalRecords )
                    {
                        // If our total records just so happen to match
                        // the day index times the total results per index,
                        // then we reached the end of the results,
                        // so there is no next index
                        nextIndex = null;
                    }
                    else
                    {
                        // Otherwise, if we are full, but there are still more
                        // results above us, the next index is just the current
                        // index plus 1.
                        nextIndex = dayIndex + 1;
                    }
                }
                else
                {
                    // If we are not filled, then there can't 
                    // be anything above the current index,
                    // so next index is null.
                    nextIndex = null;
                }

                int? endIndex;
                if( totalRecords == 0 )
                {
                    endIndex = null;
                }
                else
                {
                    endIndex = ( totalRecords ) / TimeResult.TimeStampsPerIndex;
                    if( ( totalRecords % TimeResult.TimeStampsPerIndex ) != 0 )
                    {
                        ++endIndex;
                    }
                }

                int? previousIndex;
                if( ( dayIndex <= 1 ) || ( totalRecords == 0 ) )
                {
                    // If we are the first index,
                    // there is no previous index,
                    // therefore, set to null.
                    //
                    // If there are no records at all, there is no previous,
                    // so this is also null in that case.
                    previousIndex = null;
                }
                else if( isEmpty )
                {
                    // If we are empty, then our index may be too big.
                    // Try to calculate the index that actually contains things,
                    // which is the last index
                    previousIndex = endIndex;
                }
                else
                {
                    // If we are not empty, there must be a previous
                    // index.  So, just set it to 1 minus the current index.
                    previousIndex = dayIndex - 1;
                }

                return new TimeResult
                {
                    // Start index is always one if we contain stuff.
                    StartIndex = totalRecords != 0 ? 1 : null,
                    Index = dayIndex.Value,
                    EndIndex = endIndex,
                    NextIndex = nextIndex,
                    PreviousIndex = previousIndex,
                    TimeStamps = timeStamps,
                    TimeZone = timeZone,
                    TotalRecords = totalRecords
                };
            }
        }

        /// <summary>
        /// Gets the current times for the given time zone.
        /// </summary>
        /// <param name="dayIndex">
        /// Which day from now to get the timestamps from.
        /// 
        /// If this is 0 or less, or null, this just returns the "index" information,
        /// which is the total timestamps and the next indexes.
        /// 
        /// If this is 1 or more, it will grab timestamps from that day.
        /// '1' being the most current.
        /// 
        /// If no times exist on a day, an empty array will be returned, but the indexes will
        /// point to the most recent.
        /// </param>
        public Task<TimeResult> GetTimesForTimeZoneAsync( string timeZone, int? dayIndex )
        {
            return Task.Run( () => GetTimesForTimeZone( timeZone, dayIndex ) );
        }

        public TimeStamp? TryGetTimeStampById( string timeZone, int id )
        {
            using( DatabaseConnection dbConnection = Connect() )
            {
                if( dbConnection.Dates is null )
                {
                    throw new InvalidOperationException(
                        "Dates table somehow null"
                    );
                }

                DateTable? row = dbConnection.Dates.FirstOrDefault(
                    d => ( d.Id == id ) && ( d.TimeZone == timeZone )
                );

                if( row is null )
                {
                    return null;
                }

                return new TimeStamp( row.Id, row.TimeStamp, timeZone );
            }
        }

        public Task<TimeStamp?> TryGetTimeStampByIdAsync( string timeZone, int id )
        {
            return Task.Run( () => TryGetTimeStampById( timeZone, id ) );
        }

        public void Dispose()
        {
            GC.SuppressFinalize( this );
        }

        private DatabaseConnection Connect()
        {
            return new DatabaseConnection( this.dbFile, this.pool );
        }

        private int GetTotalRowsForTimeZoneInternal( DatabaseConnection dbConnection, string timeZone )
        {
            if( dbConnection.Dates is null )
            {
                throw new InvalidOperationException(
                    "Dates table somehow null"
                );
            }

            return dbConnection.Dates.Where(
                d => d.TimeZone == timeZone
            ).Count();
        }

        private IReadOnlyList<TimeStamp> GetTimeStampsInternal(
            DatabaseConnection dbConnection,
            string timeZone,
            int dayIndex
        )
        {
            if( dbConnection.Dates is null )
            {
                throw new InvalidOperationException(
                    "Dates table somehow null"
                );
            }

            return dbConnection.Dates
                .Skip( ( dayIndex - 1 ) * TimeResult.TimeStampsPerIndex )
                .Take( TimeResult.TimeStampsPerIndex )
                .Where( d => d.TimeZone == timeZone )
                .OrderBy( d => d )
                .Select( d => new TimeStamp( d.Id, d.TimeStamp, timeZone ) )
                .ToList()
                .AsReadOnly();
        }
    }
}
