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
        public Block[] block; //数据块指针数组
        public long c_time; //文件创建时间
        public long m_time; //文件修改时间
        public int block_group_index; //所在块组号

        public INode(int block_group_index)
        {
            block = new Block[15];

            this.c_time = Utils.getUnixTimeStamp();
            this.m_time = this.c_time;

            this.block_group_index = block_group_index;
        }
    }

}
