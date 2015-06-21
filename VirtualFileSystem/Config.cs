using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem
{
    static class Config
    {
        public static const long GROUPS = 1024;
        public static const long BLOCK_SIZE = 1024;
        public static const long BLOCKS_PER_GROUP = 100;
        public static const long INODES_PER_GROUP = 10;
    }
}
