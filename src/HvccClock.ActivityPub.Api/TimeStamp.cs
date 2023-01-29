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

namespace HvccClock.ActivityPub.Api
{
    public class TimeStamp
    {
        // ---------------- Constructor ----------------

        public TimeStamp( int id, DateTime timeStamp )
        {
            this.Id = id;
            this.DateTime = timeStamp;
        }

        // ---------------- Properties ----------------

        public int Id { get; private set; }

        public DateTime DateTime { get; private set; }
    }
}
