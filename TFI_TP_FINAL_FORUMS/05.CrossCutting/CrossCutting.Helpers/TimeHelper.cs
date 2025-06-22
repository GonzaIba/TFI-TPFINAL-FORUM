using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Helpers
{
    public class TimeHelper
    {
        /// <summary>
        /// Determines whether the specified <paramref name="referenceDate"/> has expired,
        /// given an <paramref name="timeSpan"/> threshold.
        /// </summary>
        /// <param name="timeSpan">The elapsed-time threshold (e.g., 5 minutes).</param>
        /// <param name="referenceDate">The date to compare against the current moment.</param>
        /// <returns>
        /// <c>true</c> if the difference between now and <paramref name="referenceDate"/>  
        /// is **greater than or equal to** <paramref name="timeSpan"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExpired(TimeSpan timeSpan, DateTime referenceDate)
        {
            // Always compare in UTC to avoid timezone issues.
            var elapsed = DateTime.Now - referenceDate;
            return elapsed >= timeSpan;
        }

    }
}
