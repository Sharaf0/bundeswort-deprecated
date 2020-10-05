using System;

namespace Bundeswort.Controllers
{
    static class Constants
    {
        //Sets how long the cache entry can be inactive (e.g. not accessed) before it will be removed.
        //This will not extend the entry lifetime beyond the absolute expiration (if set).
        public static TimeSpan SlidingExpiration = TimeSpan.FromDays(1);
        
        //Sets an absolute expiration time, relative to now.
        public static TimeSpan AbsoluteExpiration = TimeSpan.FromDays(6);
    }
}