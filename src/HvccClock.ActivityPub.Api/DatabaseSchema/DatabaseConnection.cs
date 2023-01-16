﻿//
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

using Microsoft.EntityFrameworkCore;

namespace HvccClock.ActivityPub.Api.DatabaseSchema
{
    internal sealed class DatabaseConnection : DbContext
    {
        // ---------------- Constructor ----------------

        public DatabaseConnection( FileInfo databaseLocation )
        {
            this.DatabaseLocation = databaseLocation;

            this.Database.EnsureCreated();
        }

        // ---------------- Properties ----------------

        public FileInfo DatabaseLocation { get; private set; }

        public DbSet<DateTable>? Dates { get; set; }

        // ---------------- Functions ----------------

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            optionsBuilder.UseSqlite( $"Data Source={this.DatabaseLocation.FullName}" );
            base.OnConfiguring( optionsBuilder );
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );
        }
    }
}
