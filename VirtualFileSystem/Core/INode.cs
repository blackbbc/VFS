using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

using VirtualFileSystem.Core;

namespace VirtualFileSystem.Core
{
    class INode
    {
        public Block[] blocks; //数据块指针数组
        public long c_time; //文件创建时间
        public long m_time; //文件修改时间
        public bool is_open;
        private int block_group_index; //所在块组号
        private int inode_index; //所在inode号


        public INode(int block_group_index, int inode_index)
        {
            blocks = new Block[15];

            this.is_open = false;
            this.c_time = Utils.getUnixTimeStamp();
            this.m_time = this.c_time;

            this.block_group_index = block_group_index;
            this.inode_index = inode_index;

        }

        public long getSize()
        {
            long size = 0;
            for (int i = 0; i < 15; i++)
            {
                if (blocks[i] != null)
                    size += blocks[i].getSize();
                else
                    break;
            }
            return size;
        }

        public void save(String content)
        {
            //先刷新修改时间
            this.c_time = Utils.getUnixTimeStamp();

            //先擦除blocks
            clearBlock();

            //然后写
            char[] contents = content.ToCharArray();
            //长度
            int length = contents.GetLength(0);
            int num = length / (int)Config.BLOCK_SIZE + 1;

            if (length == 0)
                return;

            //初始化blocks
            ArrayList free_blocks = VFS.getFreeBlocks(15);


            for (int i = 0; i < Math.Min(12, num); i++)
            {
                blocks[i] = free_blocks[i] as Block;
                blocks[i].setFlag(0);
            }
            if (num > 12)
            {
                blocks[12] = free_blocks[12] as Block;
                blocks[12].setFlag(1);
            }
            if (num > 12 + Config.BLOCK_SIZE)
            {
                blocks[13] = free_blocks[13] as Block;
                blocks[13].setFlag(2);
            }
            if (num > 12 + Config.BLOCK_SIZE + Config.BLOCK_SIZE * Config.BLOCK_SIZE)
            {
                blocks[14] = free_blocks[14] as Block;
                blocks[14].setFlag(3);
            }

            int currentBlockIndex = 0;

            for (int i = 0; i < num; i++)
            {
                //首先获取一段字符串
                long realBlockSize = Math.Min(Config.BLOCK_SIZE, length - i * Config.BLOCK_SIZE);
                char[] content_sequence = new char[realBlockSize];
                Array.Copy(contents, Config.BLOCK_SIZE * i, content_sequence, 0, realBlockSize);

                //然后写

                if (currentBlockIndex < 12)
                {
                    //直接索引
                    blocks[currentBlockIndex].saveData(content_sequence);
                    currentBlockIndex++;
                }
                else
                {
                    //分级索引
                    blocks[currentBlockIndex].saveData(content_sequence);
                    if (blocks[currentBlockIndex].isFull())
                        currentBlockIndex++;
                }
            }
        }

        public String getContent()
        {
            String content = "";
            for (int i = 0; i < 15; i++)
            {
                if (blocks[i] != null)
                    content += blocks[i].getContent();
                else
                    break;
            }
            return content;
        }

        public void clearBlock()
        {
            foreach (Block block in this.blocks)
                if (block != null)
                    block.delete();
        }

        public void delete()
        {
            //刷新位图
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.inode_index, false);
        }
    }

}
