using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

using ProtoBuf;

using VirtualFileSystem;

namespace VirtualFileSystem.Core
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    class Block
    {
        public char[] data;        //数据
        public List<Block> blocks; //索引表
        private int flag;      //表示类型 0 -- 数据 1 -- 一级索引 2 -- 二级索引
        public bool effective;     //是否有效

        private int block_group_index; //所在块组号
        private int block_index; //所在块号

        public Block() { }

        public Block(int block_group_index, int block_index)
        {
            this.effective = false;
            this.block_group_index = block_group_index;
            this.block_index = block_index;
        }

        public bool isFull()
        {
            if (flag == 0 || blocks.Count >= Config.BLOCK_SIZE)
                return true;
            else
                return false;
        }

        public void setFlag(int flag)
        {
            //表示开始使用了
            this.effective = true;
            this.flag = flag;
            if (this.flag == 0)
            {
                this.blocks = null;
            }
            else
            {
                this.blocks = new List<Block>();
                this.data = null;
            }

            //刷新位图
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, true);

        }

        public void saveData(char[] newData)
        {
            if (this.flag == 0)
            {
                //自己是用来存数据的
                this.data = newData;
            }
            else
            {
                //自己已经是索引了
                //找索引的尾巴是不是满了
                //Block tailBlock = blocks[blocks.Count - 1];
                if (blocks.Count == 0 || blocks[blocks.Count-1].isFull())
                {
                    //尾巴已经满了，找新的空间
                    ArrayList freeBlocks = VFS.getFreeBlocks(1);
                    Block freeBlock = freeBlocks[0] as Block;
                    freeBlock.setFlag(this.flag - 1);
                    freeBlock.saveData(newData);

                    blocks.Add(freeBlock);
                }
                else
                    //尾巴没有满，存在尾巴里
                    blocks[blocks.Count - 1].saveData(newData);

            }
        }


        public long getSize()
        {
            if (!this.effective)
                return 0;

            if (flag == 0)
                return 1024;
            else
            {
                long size = 0;
                foreach (Block block in this.blocks)
                    size += block.getSize();
                return size;
            }
        }

        public String getContent()
        {
            if (!this.effective)
                return "";

            if (flag == 0)
                return new String(this.data);
            else
            {
                String temp = "";

                foreach (Block block in this.blocks)
                    temp += block.getContent();

                return temp;
            }
        }

        public void delete()
        {
            if (!this.effective)
                return;

            this.effective = false;

            //刷新位图
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, false);

            //刷新儿子
            if (flag != 0)
            {
                foreach (Block block in this.blocks)
                    block.delete();
            }
        }

    }
}
