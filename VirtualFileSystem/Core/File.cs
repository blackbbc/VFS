using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace VirtualFileSystem.Core
{
    class File: Entry
    {
        private INode inode;

        private String name;

        //创建文件
        public File(String name)
        {
            this.name = name;

            //第一步,检查重名

            //第二步,寻找空闲的inode
            for (int i = 0; i < Config.GROUPS; i++ )
            {
                if (VFS.BLOCK_GROUPS[i].hasFreeINode())
                {
                    this.inode = VFS.BLOCK_GROUPS[i].getFreeInode();
                    return;
                }
            }

        }
        public override string getName()
        {
            return this.name;
        }
        public override string getModifiedTime()
        {
            throw new NotImplementedException();
        }

        public override string getType()
        {
            return "文本文件";
        }

        public override string getSize()
        {
            throw new NotImplementedException();
        }

        public override Entry add(Entry entry)
        {
            throw new NotImplementedException();
        }

        //写文件
        public void save(String content)
        {
            Block first_index;

            //先擦除blocks
            this.inode.clearBlock();

            //然后写
            char[] contents = content.ToCharArray();
            //长度
            int length = contents.GetLength(0);
            int num = length / 1024 + 1;

            //寻找空闲block
            ArrayList free_blocks = VFS.getFreeBlocks(num+1);

            //存储一级索引
            ArrayList first_index_blocks = new ArrayList();

            first_index = (Block)free_blocks[0];
            free_blocks.Remove(first_index);

            for (int i = 0; i < num; i++)
            {
                //首先获得一段字符串
                char[] content_sequence = new char[1024];
                //Array.ConstrainedCopy(contents, i * 1024, content_sequence, 0, 1024);
                Array.Copy(contents, 1024 * i, content_sequence, 0, Math.Min(1024, length - i * 1024));

                //然后写

                if (i < 12)
                {
                    Block block = (Block)free_blocks[0];
                    block.saveData(content_sequence);
                    inode.blocks[i] = block;
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

            if (num > 12)
            {
                first_index.saveBlocks(first_index_blocks);
                inode.blocks[13] = first_index;
            }

        }

        //读文件
        public override String getContent()
        {
            String content = "";

            for (int i = 0; i < 15; i++ )
            {
                if (this.inode.blocks[i] != null)
                    content += this.inode.blocks[i].getContent();
                else
                    break;
            }
                return content;
        }
    }
}
