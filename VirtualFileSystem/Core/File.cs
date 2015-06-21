using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{
    class File: Entry
    {
        private long inode_index;

        private String name;
        private String content;

        public File(String name, String content)
        {
            this.name = name;
            this.content = content;
        }

        public override String getName()
        {
            return this.name;
        }

        public override String getContent()
        {
            return this.content;
        }
    }
}
