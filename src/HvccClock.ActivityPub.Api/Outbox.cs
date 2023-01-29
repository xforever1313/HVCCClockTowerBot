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

using System.Text.Json;
using KristofferStrube.ActivityStreams;
using KristofferStrube.ActivityStreams.JsonLD;

namespace HvccClock.ActivityPub.Api
{
    public class Outbox
    {
        // ---------------- Fields ----------------

        private static readonly Uri publicStream = new Uri(
            "https://www.w3.org/ns/activitystreams#Public"
        );

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

            var actvities = new List<Activity>( timeResult.TimeStamps.Count );
            foreach( TimeStamp timeStamp in timeResult.TimeStamps )
            {
                Uri id = new Uri( $"{clockConfig.PostUrl}?id={timeStamp.Id}" );

                actvities.Add(
                    new Create
                    {
                        Id = id.ToString(),
                        Type = new string[] { "Create" },
                        Actor = new Link[]
                        {
                            // Actor must be the profile link.
                            new Link
                            {
                                Href = clockConfig.SiteConfig.ProfileUrl
                            }
                        },
                        Published = timeStamp.DateTimeUtc,
                        To = new Link[]
                        {
                            new Link
                            {
                                Href = publicStream
                            }
                        },
                        Object = new Note[]
                        {
                            new Note
                            {
                                Id = id.ToString(),
                                Type = new string[] { "Note" },
                                Published = timeStamp.DateTimeUtc,

                                // Used to determine the profile which
                                // authored the status.
                                // So, needs the profile URL.
                                AttributedTo = new Link[]
                                {
                                    new Link
                                    {
                                        Href = clockConfig.SiteConfig.ProfileUrl
                                    }
                                },
                                // Per mastodon's docs, this should be "as:Public"
                                // to show public status.
                                // Per this URL, it appears as though it needs to be this.
                                // https://blog.joinmastodon.org/2018/06/how-to-implement-a-basic-activitypub-server/
                                To = new Link[]
                                {
                                    new Link
                                    {
                                        Href = publicStream
                                    }
                                },

                                Content = new string[]
                                { 
                                    timeStamp.GetMessageString( clockConfig )
                                },

                                // No summary, it is the CW text.
                                Summary = null,

                                ExtensionData = new Dictionary<string, JsonElement>
                                {
                                    ["sensitive"] = JsonSerializer.SerializeToElement( false )
                                }
                            }
                        }
                    }
                );
            }

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
                OrderedItems = actvities
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