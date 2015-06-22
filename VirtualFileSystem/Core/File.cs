using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Collections;

namespace VirtualFileSystem.Core
{
    public class File: Entry
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

        public override void setName(string name)
        {
            this.name = name;
        }
        public override string getModifiedTime()
        {
            DateTime dateTime = Utils.getDateTime(inode.m_time);
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public override string getType()
        {
            return "文本文件";
        }

        public override string getSize()
        {
            long size = inode.getSize();
            size /= 1024;
            return size.ToString() + "kb";
        }

        public override Entry add(Entry entry)
        {
            throw new NotImplementedException();
        }

        public override TreeNode getTreeNode()
        {
            throw new NotImplementedException();
        }

        public override ArrayList getEntries()
        {
            throw new NotImplementedException();
        }

        public override ListViewItem getListViewItem()
        {
            ListViewItem item = new ListViewItem(name, 1);
            item.Tag = this;

            ListViewItem.ListViewSubItem[] subItems;

            subItems = new ListViewItem.ListViewSubItem[]
            {
                new ListViewItem.ListViewSubItem(item, getType()),
                new ListViewItem.ListViewSubItem(item, getModifiedTime()),
                new ListViewItem.ListViewSubItem(item, getSize())
            };

            item.SubItems.AddRange(subItems);

            return item;
        }

        //写文件
        public void save(String content)
        {
            this.inode.save(content);
        }

        //读文件
        public override String getContent()
        {
            return this.inode.getContent();
        }
    }
}
