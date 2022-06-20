namespace HvccClock.Tests
{
    [TestClass]
    public class TweetCheckerTests
    {
        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures 12 bongs and the longest month (September)
        /// and longest day (Wednesday)
        /// can fit in 160 characters.
        /// </summary>
        [TestMethod]
        public void CheckAt12AM()
        {
            // Setup
            var time = new DateTime( 2022, 9, 21, 0, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Wednesday, September 21 2022, 12:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        /// <summary>
        /// Ensures if we are 1 second off at 11AM, we still report
        /// 11:00
        /// </summary>
        [TestMethod]
        public void CheckAt11AM()
        {
            // Setup
            var time = new DateTime( 2022, 6, 20, 11, 0, 1, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, June 20 2022, 11:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        /// <summary>
        /// Ensures if we are 1 minute off at 10AM, we still report
        /// 10:00
        /// </summary>
        [TestMethod]
        public void CheckAt10AM()
        {
            // Setup
            var time = new DateTime( 2022, 2, 1, 10, 1, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Tuesday, February 1 2022, 10:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt9AM()
        {
            // Setup
            var time = new DateTime( 2022, 1, 16, 9, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Sunday, January 16 2022, 9:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt8AM()
        {
            // Setup
            var time = new DateTime( 2022, 12, 25, 8, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Sunday, December 25 2022, 8:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt7AM()
        {
            // Setup
            var time = new DateTime( 2022, 5, 5, 7, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Thursday, May 5 2022, 7:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt6AM()
        {
            // Setup
            var time = new DateTime( 2022, 7, 4, 6, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, July 4 2022, 6:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt5AM()
        {
            // Setup
            var time = new DateTime( 2022, 8, 27, 5, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Saturday, August 27 2022, 5:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt4AM()
        {
            // Setup
            var time = new DateTime( 2022, 10, 31, 4, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, October 31 2022, 4:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt3AM()
        {
            // Setup
            var time = new DateTime( 2022, 3, 13, 3, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG!";
            const string timeStamp = "Sunday, March 13 2022, 3:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt2AM()
        {
            // Setup
            var time = new DateTime( 2022, 11, 20, 2, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG!";
            const string timeStamp = "Sunday, November 20 2022, 2:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt1AM()
        {
            // Setup
            var time = new DateTime( 2022, 6, 19, 1, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG!";
            const string timeStamp = "Sunday, June 19 2022, 1:00AM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        /// <summary>
        /// Ensures 12 bongs and the longest month (September)
        /// and longest day (Wednesday)
        /// can fit in 160 characters.
        /// </summary>
        [TestMethod]
        public void CheckAt12PM()
        {
            // Setup
            var time = new DateTime( 2022, 9, 21, 12, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Wednesday, September 21 2022, 12:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        /// <summary>
        /// Ensures if we are 1 second off at 11AM, we still report
        /// 11:00
        /// </summary>
        [TestMethod]
        public void CheckAt11PM()
        {
            // Setup
            var time = new DateTime( 2022, 6, 20, 23, 0, 1, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, June 20 2022, 11:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        /// <summary>
        /// Ensures if we are 1 minute off at 10AM, we still report
        /// 10:00
        /// </summary>
        [TestMethod]
        public void CheckAt10PM()
        {
            // Setup
            var time = new DateTime( 2022, 2, 1, 22, 1, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Tuesday, February 1 2022, 10:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt9PM()
        {
            // Setup
            var time = new DateTime( 2022, 1, 16, 21, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Sunday, January 16 2022, 9:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt8PM()
        {
            // Setup
            var time = new DateTime( 2022, 12, 25, 20, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Sunday, December 25 2022, 8:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt7PM()
        {
            // Setup
            var time = new DateTime( 2022, 5, 5, 19, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Thursday, May 5 2022, 7:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt6PM()
        {
            // Setup
            var time = new DateTime( 2022, 7, 4, 18, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, July 4 2022, 6:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt5PM()
        {
            // Setup
            var time = new DateTime( 2022, 8, 27, 17, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG! BONG!";
            const string timeStamp = "Saturday, August 27 2022, 5:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt4PM()
        {
            // Setup
            var time = new DateTime( 2022, 10, 31, 16, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG! BONG!";
            const string timeStamp = "Monday, October 31 2022, 4:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt3PM()
        {
            // Setup
            var time = new DateTime( 2022, 3, 13, 15, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG! BONG!";
            const string timeStamp = "Sunday, March 13 2022, 3:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt2PM()
        {
            // Setup
            var time = new DateTime( 2022, 11, 20, 14, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG! BONG!";
            const string timeStamp = "Sunday, November 20 2022, 2:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        [TestMethod]
        public void CheckAt1PM()
        {
            // Setup
            var time = new DateTime( 2022, 6, 19, 13, 0, 0, DateTimeKind.Local );
            const string bongs = "BONG!";
            const string timeStamp = "Sunday, June 19 2022, 1:00PM";

            // Act / Check
            DoGetTweetStringTest( time, bongs, timeStamp );
        }

        // ---------------- Test Helpers ----------------

        private void DoGetTweetStringTest(
            DateTime expectedTime,
            string bongPortion,
            string timestampPortion
        )
        {
            string expectedTweet =
@$"{bongPortion}

The time at HVCC currently is: {timestampPortion}.";

            string actualTweet = TweetJob.GetTweetString( expectedTime );
            Assert.AreEqual( expectedTweet, actualTweet );

            // Tweets must be less than 160 characters.
            Assert.IsTrue( actualTweet.Length <= 160 );
        }
    }
}