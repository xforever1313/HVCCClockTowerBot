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
using KristofferStrube.ActivityStreams;
using KristofferStrube.ActivityStreams.JsonLD;

namespace HvccClock.ActivityPub.Api
{
    public class Outbox
    {
        // ---------------- Constructor ----------------

        public Outbox()
        {
        }

        // ---------------- Functions ----------------

        public OrderedCollectionPage GenerateOutbox( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            Uri outboxUrl = clockConfig.OutboxUrl;

            var collection = new OrderedCollectionPage
            {
                JsonLDContext = new ITermDefinition[]
                {
                    new ReferenceTermDefinition( new Uri( "https://www.w3.org/ns/activitystreams") )
                },
                Id = outboxUrl.ToString(),
                Type = new string[]{ "OrderedCollectionPage" },
                Prev = ( timeResult.PreviousIndex is null ) ? null : new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.PreviousIndex}" )
                },
                Current = new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.Index}" )
                },
                Next = ( timeResult.NextIndex is null ) ? null : new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.NextIndex}" )
                },
                First = new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.StartIndex}" )
                },
                Last = new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.EndIndex}" )
                },
                TotalItems = (uint)timeResult.TotalRecords
            };

            return collection;
        }

        public Task<OrderedCollectionPage> GenerateOutboxAsync( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            return Task.Run( () => GenerateOutbox( clockConfig, timeResult ) );
        }
    }
}