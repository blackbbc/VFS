using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Collections;

namespace VirtualFileSystem.Core
{
    [Serializable]
    public class File: Entry
    {
        private INode inode;
        private String name;

        //记录父目录
        public Directory parent;

        //复制构造函数
        public File(String name, File copiedFile, Directory parent)
        {
            this.name = name;
            this.parent = parent;

            //寻找空闲的inode
            for (int i = 0; i < Config.GROUPS; i++)
            {
                if (VFS.BLOCK_GROUPS[i].hasFreeINode())
                {
                    this.inode = VFS.BLOCK_GROUPS[i].getFreeInode();
                    break;
                }
            }

            //复制数据
            this.inode.save(copiedFile.getContent());

            updateCTime();
        }

        //创建文件
        public File(String name, Directory parent)
        {
            this.name = name;
            this.parent = parent;

            //寻找空闲的inode
            for (int i = 0; i < Config.GROUPS; i++ )
            {
                if (VFS.BLOCK_GROUPS[i].hasFreeINode())
                {
                    this.inode = VFS.BLOCK_GROUPS[i].getFreeInode();
                    break;
                }
            }

            updateCTime();
        }

        public override Directory getParent()
        {
            return parent;
        }

        public override String getPath()
        {
            if (parent.isRootDir())
                return parent.getPath() + name;
            else
                return parent.getPath() + "/" + name;
        }

        public override string getName()
        {
            return this.name;
        }

        public override void setName(string name)
        {
            this.name = name;

            updateCTime();
        }
        public override string getModifiedTime()
        {
            DateTime dateTime = Utils.getDateTime(inode.m_time);
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public string getCreatedTime()
        {
            DateTime dateTime = Utils.getDateTime(inode.c_time);
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

            parent.updateCTime();
        }

        //读文件
        public override String getContent()
        {
            return this.inode.getContent();
        }

        //删除硬盘数据
        public override void deleteData()
        {
            //删除block
            inode.clearBlock();
            //删除inode
            inode.delete();

            updateCTime();
        }

        public override ArrayList search(string name)
        {
            ArrayList result = new ArrayList();
            if (this.name.IndexOf(name) >= 0)
                result.Add(this);

            return result;
        }

        public override void updateCTime()
        {
            inode.m_time = Utils.getUnixTimeStamp();
            parent.updateCTime();
        }
    }
}
