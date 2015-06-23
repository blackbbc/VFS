using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Collections;

namespace VirtualFileSystem.Core
{
    public class Directory: Entry
    {
        private String name;
        private long modifiedTime;

        private TreeNode treeNode;

        //记录父目录
        public Directory parent;

        private ArrayList directory = new ArrayList();

        public Directory(String name, Directory parent)
        {
            this.name = name;

            this.parent = parent;

            this.modifiedTime = Utils.getUnixTimeStamp();
        }

        public String getPath()
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

        public override String getSize()
        {
            return "";
        }

        public override String getContent()
        {
            throw new NotImplementedException();
        }

        public override Entry add(Entry entry)
        {
            this.modifiedTime = Utils.getUnixTimeStamp();

            directory.Add(entry);
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

        public override ArrayList getEntries()
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

        public override void deleteData() { }

        public void deleteEntry(Entry entry)
        {
            directory.Remove(entry);
        }

    }
}
