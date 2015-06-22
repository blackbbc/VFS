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
            VFS.rootDir.add(libDir);
            VFS.rootDir.add(homeDir);
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
        }

        private void treeView1_NodeMouseClick_1(object sender, TreeNodeMouseClickEventArgs e)
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
            MessageBox.Show("修改成功！");
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

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enterDirectory(currentDir);
        }

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

    }
}
