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

using HvccClock.ActivityPub.Api.DatabaseSchema;

namespace HvccClock.ActivityPub.Api
{
    public class HvccClockDatabase : IDisposable
    {
        // ---------------- Fields ----------------

        private readonly FileInfo dbFile;

        private readonly Serilog.ILogger log;

        // ---------------- Constructor ----------------

        public HvccClockDatabase( FileInfo dbFile, Serilog.ILogger log )
        {
            this.log = log;
            this.dbFile = dbFile;
        }

        // ---------------- Functions ----------------

        public int AddTime( string timeZone, DateTime dateTimeUtc )
        {
            int id;
            using( DatabaseConnection dbConnection = new DatabaseConnection( this.dbFile ) )
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
            using( DatabaseConnection dbConnection = new DatabaseConnection( this.dbFile ) )
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
        }

        public Task<int> GetTotalRowsForTimeZoneAsync( string timeZone )
        {
            return Task.Run(
                () => GetTotalRowsForTimeZone( timeZone )
            );
        }

        public void Dispose()
        {
        }
    }
}
