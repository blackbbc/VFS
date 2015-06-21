using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{
    class BlockGroup
    {
        //块组

        SuperBlock super_block;

        long g_free_blocks_count;
        long g_free_inodes_count;

        bool[] block_index;
        bool[] inode_index;

        INode[] inodes;

        Block[] block;

        public BlockGroup()
        {
            this.super_block = new SuperBlock();

            this.g_free_blocks_count = Config.BLOCKS_PER_GROUP;
            this.g_free_inodes_count = Config.INODES_PER_GROUP;

            this.block_index = new bool[Config.BLOCKS_PER_GROUP];
            this.inode_index = new bool[Config.INODES_PER_GROUP];

            this.block = new Block[Config.BLOCKS_PER_GROUP];
            this.inodes = new INode[Config.INODES_PER_GROUP];
        }

    }
}
