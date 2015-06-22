using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Shell;
using System.Runtime.InteropServices;
using System.Collections;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{

    public partial class Form1 : Form
    {
        private Directory currentDir;

        //剪贴板
        private Entry sharedEntry;

        private bool isCut = false;

        //发送消息依赖-------------------------------------------------------------
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);
        uint MSG_SHOW = RegisterWindowMessage("TextEditor Closed");
        //发送消息依赖-------------------------------------------------------------

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == MSG_SHOW)
            {
                enterDirectory(currentDir);
                //MessageBox.Show("收到！");
            }
            base.WndProc(ref m);
        }


        private void InitialVFS()
        {
            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

            //初始化目录树
            VFS.rootDir = new Directory("/");

            Directory bootDir = new Directory("boot");
            Directory etcDir = new Directory("etc");
            Directory libDir = new Directory("lib");
            Directory homeDir = new Directory("home");
            Directory rootDir = new Directory("root");
            Directory tempDir = new Directory("temp");
            VFS.rootDir.add(bootDir);
            VFS.rootDir.add(etcDir);
            VFS.rootDir.add(homeDir);
            VFS.rootDir.add(libDir);
            VFS.rootDir.add(rootDir);
            VFS.rootDir.add(tempDir);

            File file1 = new File("bashrc");
            File file2 = new File("shadowsocks");
            etcDir.add(file1);
            etcDir.add(file2);
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode = VFS.rootDir.getTreeNode();

            treeView1.Nodes.Add(rootNode);
        }

        private void enterDirectory(Directory dir)
        {
            currentDir = dir;
            listView1.Items.Clear();

            ArrayList entries = dir.getEntries();

            foreach (Entry entry in entries)
            {
                listView1.Items.Add(entry.getListViewItem());
            }

            //排序功能

            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //自动修改宽度
        }

        public Form1()
        {
            InitializeComponent();

            InitialVFS();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.currentDir = VFS.rootDir;
            PopulateTreeView();

            enterDirectory(currentDir);
        }

        private void treeView1_NodeMouseClick_1(object sendeblock_indexr, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            listView1.Items.Clear();

            Directory selectedDir = (Directory)selectedNode.Tag;

            enterDirectory(selectedDir);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.Show();
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {

            if (e.Label == null)
                return;

            if (e.Label == "")
            {
                e.CancelEdit = true;
                MessageBox.Show("必须键入文件名", "重命名");
                return;
            }

            //检查文件重名
            ArrayList entries = currentDir.getEntries();

            foreach (Entry entry in entries)
                if (entry.getName() == e.Label)
                {
                    e.CancelEdit = true;
                    MessageBox.Show("文件名重复", "重命名");
                    return;
                }

            ListViewItem selectedItem = listView1.Items[e.Item];
            Entry selectedEntry = (Entry)selectedItem.Tag;
            selectedEntry.setName(e.Label);

            //刷新listview
            enterDirectory(currentDir);

            //刷新treeview
            if (selectedEntry.getType() == "文件夹")
            {
                Directory selectedDir = (Directory)selectedEntry;
                selectedDir.getLinkedTreeNode().Text = e.Label;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Entry selectedEntry = (Entry)listView1.SelectedItems[0].Tag;

            if (selectedEntry.getType() == "文本文件")
            {
                //打开编辑器
                File file = (File)selectedEntry;
                TextEditor textEditor = new TextEditor(file);
                textEditor.Show();
            }
            else
            {
                //进入目录
                Directory directory = (Directory)selectedEntry;
                enterDirectory(directory);

            }

        }

        //设置快捷键
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
                enterDirectory(currentDir);
            else if (keyData == Keys.F2)
                OnRename();
            else if (keyData == Keys.Delete)
                OnDelete();
            else if (keyData == (Keys.Control | Keys.X))
                OnCut();
            else if (keyData == (Keys.Control | Keys.C))
                OnCopy();
            else if (keyData == (Keys.Control | Keys.V))
                OnPaste();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //刷新
        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enterDirectory(currentDir);
        }


        //右键菜单
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            bool match = false;

            if (e.Button == MouseButtons.Right)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Bounds.Contains(e.Location))
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                {
                    contextMenuStrip2.Show(Cursor.Position);
                }
                else
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        public void OnNewFolder()
        {
            String newName = Utils.getLegalNewName("新建文件夹", currentDir);
            Directory newDir = new Directory(newName);
            currentDir.add(newDir);

            //刷新listview
            enterDirectory(currentDir);

            //刷新treeview
            currentDir.getLinkedTreeNode().Nodes.Add(newDir.getTreeNode());

            ListViewItem newItem = null;
            foreach (ListViewItem item in listView1.Items)
                if (item.Text == newDir.getName())
                {
                    newItem = item;
                    break;
                }

            if (newItem != null)
                newItem.BeginEdit();
        }

        //新建文件夹
        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewFolder();
        }

        private void OnNewFile()
        {
            String newName = Utils.getLegalNewName("新建文本文件", currentDir);
            File newFile = new File(newName);
            currentDir.add(newFile);

            //刷新listview
            enterDirectory(currentDir);

            ListViewItem newItem = null;
            foreach (ListViewItem item in listView1.Items)
                if (item.Text == newFile.getName())
                {
                    newItem = item;
                    break;
                }

            if (newItem != null)
                newItem.BeginEdit();
        }

        //新建文本文件
        private void 文本文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewFile();
        }

        private void OnRename()
        {
            ListViewItem selectedItem = (ListViewItem)listView1.SelectedItems[0];
            selectedItem.BeginEdit();
        }

        //重命名
        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnRename();
        }

        private void OnSort(int column)
        {
            int index = 0;
            foreach (ToolStripMenuItem item in (contextMenuStrip1.Items[0] as ToolStripDropDownItem).DropDownItems)
            {
                if (index == column)
                    item.Checked = true;
                else
                    item.Checked = false;
                index++;
            }
            listView1.ListViewItemSorter = new ListViewItemComparer(column);
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            OnSort(e.Column);
        }

        //按名称排序
        private void 名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(0);
        }

        //按修改日期排序
        private void 修改日期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(1);
        }

        //按修改日期排序
        private void 类型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(2);
        }

        //按大小排序
        private void 大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(3);
        }

        private void OnDelete()
        {
            Entry deletedEntry = listView1.SelectedItems[0].Tag as Entry;
            var result = MessageBox.Show("确定要删除"+deletedEntry.getName()+"吗", "删除"+deletedEntry.getType(), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                //删除硬盘数据
                deletedEntry.deleteData();

                //删除目录树数据
                currentDir.deleteEntry(deletedEntry);
                if (deletedEntry.getType() == "文件夹")
                {
                    //刷新TreeNode
                    Directory deletedDir = deletedEntry as Directory;
                    TreeNode deletedTreeNode = deletedDir.getLinkedTreeNode();
                    deletedTreeNode.Parent.Nodes.Remove(deletedTreeNode);
                }

                enterDirectory(currentDir);
            }
        }

        //删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnDelete();
        }

        public void OnCopy()
        {
            Entry copiedEntry = listView1.SelectedItems[0].Tag as Entry;
            sharedEntry = copiedEntry;

            粘贴ToolStripMenuItem.Enabled = true;

        }

        //复制
        private void 复制ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnCopy();
        }


        public void OnCut()
        {
            OnCopy();
            isCut = true;
        }

        //剪切
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnCut();
        }

        public void OnPaste()
        {
            if (!粘贴ToolStripMenuItem.Enabled)
                return;

            String newName = Utils.getLegalCopyName(sharedEntry.getName(), currentDir);
            if (sharedEntry.getType() == "文本文件")
            {
                File newFile = new File(newName, sharedEntry as File);
                currentDir.add(newFile);

                enterDirectory(currentDir);
            }
            else
            {
                //复制文件夹，递归复制，Fuck!
            }

            if (isCut)
            {
                isCut = false;
                粘贴ToolStripMenuItem.Enabled = false;
            }

        }

        //粘贴
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnPaste();
        }




    }
}
