using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using System.IO;

using ProtoBuf;
using ProtoBuf.Meta;

using System.Windows.Forms;
using System.Collections;

namespace VirtualFileSystem.Core
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class Directory: Entry
    {
        public String name;
        public long modifiedTime;

        public TreeNode treeNode;

        //记录父目录
        public Directory parent;

        public List<Entry> directory = new List<Entry>();

        //默认构造函数
        public Directory() { }


        //复制构造函数
        public Directory (String name, Directory copiedDir, Directory parent)
        {
            this.name = name;
            this.parent = parent;

            foreach (Entry entry in copiedDir.directory)
                if (entry.getType() == "文本文件")
                {
                    File newFile = new File(entry.getName(), entry as File, this);
                    directory.Add(newFile);
                }
                else
                {
                    Directory newDir = new Directory(entry.getName(), entry as Directory, this);
                    directory.Add(newDir);

                }

            updateCTime();
        }

        public Directory(String name, Directory parent)
        {
            this.name = name;

            this.parent = parent;

            updateCTime();
        }

        public override Directory getParent()
        {
            return parent;
        }

        public override String getPath()
        {
            //自己是根节点
            if (isRootDir())
                return "/";
            else
            {
                if (parent.isRootDir())
                    return parent.getPath() + name;
                else
                    return parent.getPath() + "/" + name;
            }
        }

        public bool isRootDir()
        {
            if (parent == null)
                return true;
            else
                return false;
        }

        public override String getName()
        {
            return name;
        }

        public override void setName(string name)
        {
            this.name = name;

            updateCTime();
        }

        public override String getModifiedTime()
        {
            DateTime dateTime = Utils.getDateTime(this.modifiedTime);
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public override String getType()
        {
            return "文件夹";
        }

        public override long getSizeNum()
        {
            long size = 0;

            foreach (Entry entry in directory)
                size += entry.getSizeNum();

            return size;
        }

        public override String getSize()
        {
            long size = getSizeNum();
            if (size >= 1024 * 1024)
            {
                size /= 1024 * 1024;
                return size.ToString() + "MB";
            }
            else
            {
                size /= 1024;
                return size.ToString() + "KB";
            }
        }

        public override String getContent()
        {
            throw new NotImplementedException();
        }

        public override Entry add(Entry entry)
        {
            this.modifiedTime = Utils.getUnixTimeStamp();

            directory.Add(entry);

            updateCTime();

            return this;
        }

        public override TreeNode getTreeNode()
        {
            TreeNode node = new TreeNode(this.name);
            this.treeNode = node;
            node.Tag = this;

            foreach (Entry entry in directory)
                if (entry.getType() == "文件夹")
                {
                    TreeNode newNode = entry.getTreeNode();
                    node.Nodes.Add(newNode);
                }

            return node;
        }

        public TreeNode getLinkedTreeNode()
        {
            return this.treeNode;
        }

        public override List<Entry> getEntries()
        {
            return directory;
        }

        public override ListViewItem getListViewItem()
        {
            ListViewItem item = new ListViewItem(name, 0);
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

        public Entry findByName(String name)
        {
            foreach (Entry entry in directory)
                if (entry.getName() == name)
                    return entry;

            return null;
        }

        public bool isExist(String name)
        {
            if (findByName(name) != null)
                return true;
            else
                return false;
        }

        public override void deleteData() 
        {
            foreach (Entry entry in directory)
                entry.deleteData();
        }

        public void deleteEntry(Entry entry)
        {
            directory.Remove(entry);

            updateCTime();
        }

        public override ArrayList search(String name)
        {
            ArrayList result = new ArrayList();

            //检查儿子
            foreach (Entry entry in directory)
                result.AddRange(entry.search(name));

            //检查自己
            if (this.name.IndexOf(name) >= 0)
                result.Add(this);

            return result;
        }

        public override void updateCTime()
        {
            this.modifiedTime = Utils.getUnixTimeStamp();
            if (!isRootDir())
                parent.updateCTime();
        }

    }

}
