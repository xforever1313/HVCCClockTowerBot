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

        public OrderedCollection GenerateIndexPage( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            if( timeResult.Index > 0 )
            {
                throw new ArgumentException(
                    $"Got an index of not zero, {nameof( GenerateCollectionPage )} should have been called instead.",
                    nameof( timeResult )
                );
            }

            var collection = new OrderedCollection
            {
                JsonLDContext = new ITermDefinition[]
                {
                    new ReferenceTermDefinition( new Uri( "https://www.w3.org/ns/activitystreams" ) )
                },
                Id = clockConfig.OutboxUrl.ToString(),
                Type = new string[] { "OrderedCollection" },
                Current = GetStartUrl( clockConfig, timeResult ),
                First = GetStartUrl( clockConfig, timeResult ),
                Last = GetEndUrl( clockConfig, timeResult ),
                TotalItems = (uint)timeResult.TotalRecords
            };

            return collection;
        }

        public Task<OrderedCollection> GenerateIndexPageAsync( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            return Task.Run( () => GenerateIndexPage( clockConfig, timeResult ) );
        }

        public OrderedCollectionPage GenerateCollectionPage( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            if( timeResult.Index <= 0 )
            {
                throw new ArgumentException(
                    $"Got an index of zero, {nameof( GenerateIndexPage )} should have been called instead.",
                    nameof( timeResult )
                );
            }

            Uri outboxUrl = clockConfig.OutboxUrl;

            var collection = new OrderedCollectionPage
            {
                JsonLDContext = new ITermDefinition[]
                {
                    new ReferenceTermDefinition( new Uri( "https://www.w3.org/ns/activitystreams") )
                },
                Id = $"{outboxUrl}?index={timeResult.Index}",
                Type = new string[]{ "OrderedCollectionPage" },
                // Per the spec, the curent points to the page containing the items
                // that have been created or updated the most recently.
                // May as well make that the first page, which is the
                // most recent messages.
                Current = ( timeResult.StartIndex is null ) ? null : new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.StartIndex}" )
                },
                PartOf = new Link
                {
                    Href = outboxUrl
                },
                Prev = ( timeResult.PreviousIndex is null ) ? null : new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.PreviousIndex}" )
                },
                Next = ( timeResult.NextIndex is null ) ? null : new Link
                {
                    Href = new Uri( $"{outboxUrl}?index={timeResult.NextIndex}" )
                },
                First = GetStartUrl( clockConfig, timeResult ),
                Last = GetEndUrl( clockConfig, timeResult ),
                TotalItems = (uint)timeResult.TotalRecords,
            };

            return collection;
        }

        public Task<OrderedCollectionPage> GenerateCollectionPageAsync( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            return Task.Run( () => GenerateCollectionPage( clockConfig, timeResult ) );
        }

        private static Link? GetStartUrl( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            if( timeResult.StartIndex is null )
            {
                return null;
            }

            return new Link
            {
                Href = new Uri( $"{clockConfig.OutboxUrl}?index={timeResult.StartIndex}" )
            };
        }

        private static Link? GetEndUrl( ClockTowerConfig clockConfig, TimeResult timeResult )
        {
            if( timeResult.EndIndex is null )
            {
                return null;
            }

            return new Link
            {
                Href = new Uri( $"{clockConfig.OutboxUrl}?index={timeResult.EndIndex}" )
            };
        }
    }
}