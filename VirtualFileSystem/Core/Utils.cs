using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{
    static class Utils
    {
        public static long getUnixTimeStamp()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);
            return (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
        }

        public static DateTime getDateTime(long timeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
