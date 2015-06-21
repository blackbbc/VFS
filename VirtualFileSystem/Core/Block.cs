using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;


using VirtualFileSystem;

namespace VirtualFileSystem.Core
{
    class Block
    {
        public char[] data;        //数据
        public Block[] blocks;     //索引表
        private Boolean flag;      //表示类型
        public bool effective;     //是否有效

        private int block_group_index; //所在块组号
        private int block_index; //所在块号


        public Block(int block_group_index, int block_index)
        {
            this.effective = false;
            this.block_group_index = block_group_index;
            this.block_index = block_index;
        }

        public void saveData(char[] newData)
        {
            this.flag = true;
            this.effective = true;
            this.data = newData;
            this.blocks = null;
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, true);
        }

        public void saveBlocks(ArrayList blocks)
        {
            this.flag = false;
            this.effective = true;
            this.blocks = new Block[1024];

            for (int i = 0; i < blocks.Count; i++ )
                this.blocks[i] = (Block)blocks[i];

            this.data = null;
            VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, true);
        }

        public String getContent()
        {
            if (!this.effective)
                return "";

            if (flag)
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

            if (flag)
                VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, false);
            else
            {
                foreach (Block block in this.blocks)
                    block.delete();
                VFS.BLOCK_GROUPS[this.block_group_index].updateBlockIndex(this.block_index, false);
            }
        }

    }
}
