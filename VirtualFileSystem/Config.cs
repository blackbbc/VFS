using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{
    static class Config
    {
        public const long GROUPS = 1024;
        public const long BLOCK_SIZE = 1024;
        public const long BLOCKS_PER_GROUP = 100;
        public const long INODES_PER_GROUP = 10;

    }
}
