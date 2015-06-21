using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VirtualFileSystem.Core;

namespace VirtualFileSystem.Core
{
    class INode
    {
        public Block[] blocks; //数据块指针数组
        public long c_time; //文件创建时间
        public long m_time; //文件修改时间
        private int block_group_index; //所在块组号
        private int inode_index; //所在inode号


        public INode(int block_group_index, int inode_index)
        {
            blocks = new Block[15];

            this.c_time = Utils.getUnixTimeStamp();
            this.m_time = this.c_time;

            this.block_group_index = block_group_index;
            this.inode_index = inode_index;

        }

        public void clearBlock()
        {
            foreach (Block block in this.blocks)
                if (block != null)
                    block.delete();
        }
    }

}
