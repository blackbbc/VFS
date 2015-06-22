using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections;

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

        public static String getLegalNewName(String name, Directory dir)
        {
            int index = 0;
            String legalNewName = "";
            
            while (true)
            {
                index++;
                if (index == 1)
                {
                    legalNewName = name;
                }
                else
                {
                    legalNewName = name + "(" + index.ToString() + ")";
                }

                if (!dir.isExist(legalNewName))
                    return legalNewName;

            }
        }
    }

    public class ListViewItemComparer : IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }

        public ListViewItemComparer(int column)
        {
            col = column;
        }

        public int Compare(Object x, Object y)
        {
            return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }
}
