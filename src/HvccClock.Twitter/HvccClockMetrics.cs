//
//          Copyright Seth Hendrick 2015-2021.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using Prometheus;

namespace HvccClock.Twitter
{
    internal static class HvccClockMetrics
    {
        // ---------------- Fields ----------------

        private static readonly Gauge ExceptionsInARow =
            Metrics.CreateGauge( "consecutive_exceptions", "How many exceptions we got in a row.  Reset on a succesful tweet." );

        private static readonly Counter TotalExceptions =
            Metrics.CreateCounter( "total_exceptions", "Running total of the total exceptions while this service is running." );

        // ---------------- Functions ----------------

        public static void Init()
        {
            // Does nothing - Once this method is called,
            // the static class kicks in and metrics are created.
        }

        public static void RecordSuccess()
        {
            ExceptionsInARow.Set( 0 );
        }

        public static void RecordException()
        {
            ExceptionsInARow.Inc();
            TotalExceptions.Inc();
        }
    }
}
