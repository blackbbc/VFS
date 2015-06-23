using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Collections;

namespace VirtualFileSystem.Core
{
    public abstract class Entry
    {

        public abstract String getName();

        public abstract void setName(String name);

        public abstract String getModifiedTime();

        public abstract String getType();

        public abstract String getSize();

        public abstract String getContent();

        public abstract Entry add(Entry entry);

        public abstract TreeNode getTreeNode();

        public abstract ArrayList getEntries();

        public abstract ListViewItem getListViewItem();

        public abstract void deleteData();

        public abstract Directory getParent();

        public abstract String getPath();

    }
}
