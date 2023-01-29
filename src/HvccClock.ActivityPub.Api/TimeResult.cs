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

namespace HvccClock.ActivityPub.Api
{
    public class TimeResult
    {
        // ---------------- Fields ----------------

        /// <summary>
        /// Use a days worth of results per index (seems like a good number).
        /// </summary>
        public static int TimeStampsPerIndex = 24;

        // ---------------- Construcor ----------------

        public TimeResult()
        {
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// If there is a previous index of date records,
        /// this is the index to use.
        /// 
        /// Null for no previous index.
        /// </summary>
        public int? PreviousIndex { get; init; }

        /// <summary>
        /// If there is a next index of date records,
        /// this is the index to use.
        /// 
        /// Null for no next index.
        /// </summary>
        public int? NextIndex { get; init; }

        /// <summary>
        /// Index to the first index (usually 1).
        /// 
        /// Null if there are no dates.
        /// </summary>
        public int? StartIndex { get; init; }

        /// <summary>
        /// The index used to get the <see cref="TimeStamps"/> list.
        /// </summary>
        public int Index { get; init; }
        
        /// <summary>
        /// Index to the last entry that contains days.
        /// 
        /// Null if there are no dates.
        /// </summary>
        public int? EndIndex { get; init; }

        /// <summary>
        /// Total records within the timezone,
        /// not just within <see cref="TimeStamps"/>.
        /// </summary>
        public int TotalRecords { get; init; }

        /// <summary>
        /// The timezone used for this query.
        /// </summary>
        public string TimeZone { get; init; } = "";

        /// <summary>
        /// The list of timestamps at the given index.
        /// An empty list means no times were found.
        /// </summary>
        public IReadOnlyList<DateTime> TimeStamps { get; init; } = Array.Empty<DateTime>();
    }
}
