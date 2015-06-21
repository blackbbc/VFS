using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{
    abstract class Entry
    {
        public abstract String getName();
        public String getContent() { return ""; }
        public Entry add(Entry entry)
        { 
            return this; 
        }

    }
}
