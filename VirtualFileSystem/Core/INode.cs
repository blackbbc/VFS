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
            Block first_index;

            //先擦除blocks
            clearBlock();

            //然后写
            char[] contents = content.ToCharArray();
            //长度
            int length = contents.GetLength(0);
            int num = length / (int)Config.BLOCK_SIZE + 1;

            if (length == 0)
                return;

            //寻找空闲block
            ArrayList free_blocks = VFS.getFreeBlocks(num + 1);

            //存储一级索引
            ArrayList first_index_blocks = new ArrayList();

            first_index = (Block)free_blocks[0];
            free_blocks.Remove(first_index);

            for (int i = 0; i < num; i++)
            {
                //首先获得一段字符串
                long realBlockSize = Math.Min(Config.BLOCK_SIZE, length - i * Config.BLOCK_SIZE);
                char[] content_sequence = new char[realBlockSize];
                Array.Copy(contents, Config.BLOCK_SIZE * i, content_sequence, 0, realBlockSize);

                //然后写

                if (i < 12)
                {
                    //直接索引
                    Block block = (Block)free_blocks[0];
                    block.saveData(content_sequence);
                    blocks[i] = block;
                    free_blocks.Remove(block);
                }
                else
                {
                    //写到一级索引里
                    Block block = (Block)free_blocks[0];
                    block.saveData(content_sequence);
                    first_index_blocks.Add(block);
                    free_blocks.Remove(block);
                }

            }

            //写回一级索引
            if (num > 12)
            {
                first_index.saveBlocks(first_index_blocks);
                blocks[13] = first_index;
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
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.inode_index, false);
        }
    }

}
