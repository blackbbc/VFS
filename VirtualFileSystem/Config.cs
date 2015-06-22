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
        public const long GROUPS = 1024; //块组数量
        public const long BLOCK_SIZE = 1024; //块组大小
        public const long BLOCKS_PER_GROUP = 1024; //每个块组的block数
        public const long INODES_PER_GROUP = 50;   //每个块组的inode数

    }
}
