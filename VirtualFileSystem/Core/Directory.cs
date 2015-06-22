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
        private long modifiedTime;

        private ArrayList directory = new ArrayList();

        public override String getName()
        {
            return name;
        }

        public override String getModifiedTime()
        {
            throw new NotImplementedException();
        }

        public override String getType()
        {
            return "文件夹";
        }

        public override String getSize()
        {
            return "";
        }

        public override String getContent()
        {
            throw new NotImplementedException();
        }

        public override Entry add(Entry entry)
        {
            directory.Add(entry);
            return this;
        }

    }
}
