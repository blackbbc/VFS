using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace VirtualFileSystem.Core
{
    class Directory: Entry
    {
        private String name;

        private ArrayList directory = new ArrayList();

        public override String getName()
        {
            return name;
        }

        public new Entry add(Entry entry)
        {
            directory.Add(entry);
            return this;
        }

    }
}
