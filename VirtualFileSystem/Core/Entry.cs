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

        public abstract String getModifiedTime();

        public abstract String getType();

        public abstract String getSize();

        public abstract String getContent();

        public abstract Entry add(Entry entry);

    }
}
